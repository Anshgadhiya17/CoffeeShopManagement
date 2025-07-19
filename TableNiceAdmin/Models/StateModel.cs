using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TableNiceAdmin.Models
{
    public class StateModel
    {
        public int? StateID { get; set; }

        [Required]
        [DisplayName("State Name")]
        public string StateName { get; set; }

        [Required]
        [DisplayName("Country Name")]
        public int CountryID { get; set; }

        [DisplayName("State Code")]
        public string StateCode { get; set; }
    }
    public class StateDropDownModel
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
    }
}
