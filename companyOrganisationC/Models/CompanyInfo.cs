using System.ComponentModel.DataAnnotations;

namespace companyOrganisationC.Models
{
    public class CompanyInfo
    {
        [Key]
        public int CmpId { get; set; }

        [Display(Name ="Company name")]
        public string CmpName { get; set; }

        [Display(Name = "Company description")]
        public string CmpDesc { get; set; }

        [Display(Name = "Company logo")]
        public string CmpLogo { get; set; }

        [Display(Name = "Company address")]
        public string CmpAddress { get; set; }

        [Display(Name = "Company phone no.")]
        public int? CmpPhone { get; set; }
    }
}