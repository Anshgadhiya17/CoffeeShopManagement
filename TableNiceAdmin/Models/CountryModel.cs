using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TableNiceAdmin.Models
{
    public class CountryModel
    {
        public int? CountryID { get; set; }

        [Required]
        [DisplayName("Country Name")]
        public string CountryName { get; set; }

        [Required]
        [DisplayName("Country Code")]
        public string CountryCode { get; set; }
    }

    public class CountryDropDownModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
    }

}
