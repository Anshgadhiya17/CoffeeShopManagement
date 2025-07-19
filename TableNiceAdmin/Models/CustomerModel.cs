using System.ComponentModel.DataAnnotations;

namespace TableNiceAdmin.Models
{
    public class CustomerModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please Enter CustomerName")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Please Enter HomeAddress")]
        public string HomeAddress { get; set; }

        [Required(ErrorMessage = "Please Enter Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter MobileNo")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Please Enter GstNo")]
        public string Gst_No { get; set; }

        [Required(ErrorMessage = "Please Enter CityName")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "Please Enter PinCode")]
        public string PinCode { get; set; }

        [Required(ErrorMessage = "Please Enter NetAmount")]
        public decimal NetAmount { get; set; }

        [Required(ErrorMessage = "Please Enter UserID")]
        public int UserID { get; set; }
    }

    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
