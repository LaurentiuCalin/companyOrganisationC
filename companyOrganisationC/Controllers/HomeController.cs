﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using companyOrganisationC.Models;
using Microsoft.AspNet.Identity;

namespace companyOrganisationC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult SkillList()
        {
            var currentSkills = db.Skills.ToList();
            var thisUserId = User.Identity.GetUserId();
            var admins =
                db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains("800c32e0-42b9-4817-ac67-dc06700edadd"))
                    .ToList();

            var thisUserSkills = db.UserSkills.Where(user => user.UserId == thisUserId).ToList();
            var thisUserFocuses = db.UserFocus.Where(user => user.UserId == thisUserId).ToList();
            var companyUsers = db.Users.ToList();

            var normalUser = companyUsers.Except(admins).ToList();

            ViewBag.companyUsers = normalUser;
            ViewBag.userSkills = thisUserSkills;
            ViewBag.userFocuses = thisUserFocuses;
            return View(currentSkills);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateSkill()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateSkill(Skill skill)
        {
            skill.SkillCreationDate = DateTime.Now;

            db.Skills.Add(skill);
            db.SaveChanges();

            return RedirectToAction("SkillList");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteSkill(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var skill = db.Skills.Find(id);
            var userFocuses = db.UserFocus.Where(focus => focus.SkillId == id).ToList();
            var userSkills = db.UserSkills.Where(x => x.SkillId == id).ToList();

            foreach (var item in userSkills)
                db.UserSkills.Remove(item);

            foreach (var item in userFocuses)
                db.UserFocus.Remove(item);

            if (skill != null) db.Skills.Remove(skill);
            db.SaveChanges();
            return RedirectToAction("SkillList");
        }

        public ActionResult ChooseFocus(int id, UserFocus userfocus)
        {
            var userId = User.Identity.GetUserId();
            var newSkillId = id;

            var thisUserFocuses = db.UserFocus.Where(user => user.UserId == userId).ToList();

            if (thisUserFocuses.Count >= 3) return RedirectToAction("SkillList");

            userfocus.SkillId = newSkillId;
            userfocus.UserId = userId;
            userfocus.Year = DateTime.Now.AddYears(1);

            db.UserFocus.Add(userfocus);
            db.SaveChanges();

            return RedirectToAction("SkillList");
        }

        public ActionResult AchievedSkill()
        {
            var skillStage = new List<string> {"Beginner", "Intermediate", "Advanced"};

            var skillStageList = new SelectList(skillStage);
            ViewBag.SkillStage = skillStageList;
            return View();
        }

        [HttpPost]
        public ActionResult AchievedSkill(int id, UserSkill userskill)
        {
            var userId = User.Identity.GetUserId();
            var skillId = id;

            userskill.SkillId = skillId;
            userskill.UserId = userId;

            db.UserSkills.Add(userskill);
            db.SaveChanges();
            return RedirectToAction("SkillList");
        }

        public ActionResult RemoveSkill(int id)
        {
            var userskill = db.UserSkills.Find(id);
            if (userskill != null) db.UserSkills.Remove(userskill);
            db.SaveChanges();
            return RedirectToAction("SkillList");
        }

        public ActionResult RemoveFocus(int id)
        {
            var userfocus = db.UserFocus.Find(id);
            if (userfocus != null) db.UserFocus.Remove(userfocus);
            db.SaveChanges();
            return RedirectToAction("SkillList");
        }

        public ActionResult SkillDetails(int id)
        {
            var thisSkill = db.Skills.Find(id);
            var userSkills = db.UserSkills.Where(x => x.SkillId == id).ToList();
            var userFocuses = db.UserFocus.Where(x => x.SkillId == id).ToList();
            var users = db.Users.ToList();

            ViewBag.UserSkills = userSkills;
            ViewBag.UserInfo = users;
            ViewBag.SkillInfo = thisSkill;
            ViewBag.userFocuses = userFocuses;
            return View();
        }

        public ActionResult UserDetails(string id)
        {
            var userfocus = db.UserFocus.Where(x => x.UserId == id).ToList();
            var userskill = db.UserSkills.Where(x => x.UserId == id).ToList();
            var skill = db.Skills.ToList();

            ViewBag.UserFocus = userfocus;
            ViewBag.UserSkill = userskill;
            ViewBag.Skills = skill;

            var user = db.Users.Find(id);
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddCompany()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddCompany(CompanyInfo company, HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    if (fileName != null)
                    {
                        var path = Path.Combine(Server.MapPath("~/images/"), fileName);
                        company.CmpLogo = "images/" + fileName;
                        file.SaveAs(path);
                    }
                    db.CompanyInfo.Add(company);
                    db.SaveChanges();
                    return RedirectToAction("Contact");
                }
                ViewBag.Message = "File Uploaded Successfully!!";
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditCmp(int id)
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditCmp(CompanyInfo newCompany, int id)
        {
            var company = db.CompanyInfo.Find(id);
            if (company != null)
            {
                if (newCompany.CmpAddress != null)
                    company.CmpAddress = newCompany.CmpAddress;
                if (newCompany.CmpDesc != null)
                    company.CmpDesc = newCompany.CmpDesc;
                if (newCompany.CmpName != null)
                    company.CmpName = newCompany.CmpName;
                if (newCompany.CmpPhone != null)
                    company.CmpPhone = newCompany.CmpPhone;
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Contact");
        }

        public ActionResult UserEdit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserEdit(ApplicationUser newUser, HttpPostedFileBase file)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (newUser.FirstName != null)
                user.FirstName = newUser.FirstName;
            if (newUser.LastName != null)
                user.LastName = newUser.LastName;
            if (file != null)
                try
                {
                    if (file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        if (fileName != null)
                        {
                            var path = Path.Combine(Server.MapPath("~/images/"), fileName);
                            user.Photo = "images/" + fileName;
                            file.SaveAs(path);
                        }
                    }
                }
                catch
                {
                    ViewBag.Message = "File upload failed!!";
                    return View();
                }

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SkillList");
        }

        public ActionResult Contact()
        {
            if (db.CompanyInfo.Any())
            {
                var companyInfo = db.CompanyInfo.First();
                ViewBag.CompanyInfo = companyInfo;
            }

            if (Request.IsAuthenticated)
            {
                var email = User.Identity.GetUserName();

                IEnumerable<ApplicationUser> usr = db.Users.Where(u => u.Email == email);
                if (usr.First() != null)
                {
                    ViewBag.first_name = usr.First().FirstName;
                    ViewBag.last_name = usr.First().LastName;
                }
                else
                {
                    ViewBag.first_name = "User ";
                    ViewBag.last_name = "not found";
                }
            }

            return View();
        }
    }
}