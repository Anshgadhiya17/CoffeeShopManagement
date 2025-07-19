using System.ComponentModel.DataAnnotations;

namespace TableNiceAdmin.Models
{
    public class OrderDetailModel
    {
        public int OrderDetailID { get; set; }

        [Required(ErrorMessage = "Please Enter OrderID")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Please Enter ProductID")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Please Enter Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Please Enter Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please Enter TotalAmount")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Please Enter UserID")]
        public int UserID { get; set; }
    }
}
