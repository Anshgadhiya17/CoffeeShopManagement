using System.ComponentModel.DataAnnotations;

namespace TableNiceAdmin.Models
{
    public class BillsModel
    {
        public int BillID { get; set; }

        [Required(ErrorMessage = "Please Enter BillNumber")]
        public string BillNumber { get; set; }

        [Required(ErrorMessage = "Please Enter BillDate")]
        public DateTime? BillDate { get; set; }

        [Required(ErrorMessage = "Please Enter OrderID")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Please Enter TotalAmount")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Please Enter Discount")]
        public decimal Discount { get; set; }

        [Required(ErrorMessage = "Please Enter NetAmount")]
        public decimal NetAmount { get; set; }

        [Required(ErrorMessage = "Please Enter UserID")]
        public int UserID { get; set; }
    }
}
