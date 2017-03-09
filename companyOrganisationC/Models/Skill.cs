using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace companyOrganisationC.Models
{
    public class Skill
    {
        public int SkillId { get; set; }

        [Required]
        [Display(Name = "Skill name")]
        public string SkillName { get; set; }

        [Required]
        [Display(Name = "Skill description")]
        public string SkillDescription { get; set; }

        [HiddenInput]
        [Display(Name = "Skill creation date")]
        public DateTime SkillCreationDate { get; set; }
    }
}