using System.ComponentModel.DataAnnotations;

namespace TableNiceAdmin.Models
{
    public class OrderModel
    {
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Please Enter OrderDate")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Please Enter CustomerName")]
        public int CustomerID { get; set; }    

        [Required(ErrorMessage = "Please Enter PaymentMode")]
        public string PaymentMode { get; set; }

        [Required(ErrorMessage = "Please Enter TotalAmount")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Please Enter ShippingAddress")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Please Enter UserID")]
        public int UserID { get; set; }
    }

    public class OrderDropDownModel
    {
        public int OrderID { get; set; }
    }
}
