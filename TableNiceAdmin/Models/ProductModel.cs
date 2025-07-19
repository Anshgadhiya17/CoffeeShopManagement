using System.ComponentModel.DataAnnotations;

namespace TableNiceAdmin.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Please Enter ProductName")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Please Enter ProductPrice")]
        public double ProductPrice { get; set; }

        [Required(ErrorMessage = "Please Enter ProductCode")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Please Enter Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please Enter UserID")]
        public int UserID { get; set; }
    }

    public class ProductDropDownModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
    }
}
