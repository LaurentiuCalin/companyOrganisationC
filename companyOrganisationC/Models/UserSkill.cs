namespace companyOrganisationC.Models
{
    public class UserSkill
    {
        public int UserSkillId { get; set; }
        public int SkillId { get; set; }
        public string UserId { get; set; }
        public string Stage { get; set; }
        public int ExpYears { get; set; }
    }
}