using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using TableNiceAdmin.Models;

namespace TableNiceAdmin.Controllers
{
    [CheckAccess]
    public class BillsController : Controller
    {
        private IConfiguration configuration;

        public BillsController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult BillsDetail()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Bill_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            return View(dataTable);
        }

        public IActionResult BillDelete(int BillID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Bill_Delete";
                command.Parameters.Add("@BillID", SqlDbType.Int).Value = BillID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("BillsDetail");
        }
        public IActionResult AddBill(int BillID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            #region BillByID

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Bill_SelectByPrimaryKey";
            command.Parameters.AddWithValue("@BillID", BillID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            BillsModel billsModel = new BillsModel();

            foreach (DataRow dataRow in table.Rows)
            {
                billsModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
                billsModel.BillID = Convert.ToInt32(@dataRow["BillID"]);
                billsModel.OrderID = Convert.ToInt32(@dataRow["OrderID"]);
                billsModel.BillNumber = @dataRow["BillNumber"].ToString();
                billsModel.BillDate = Convert.ToDateTime(@dataRow["BillDate"]);
                billsModel.TotalAmount = Convert.ToDecimal(@dataRow["TotalAmount"]);
                billsModel.NetAmount = Convert.ToDecimal(@dataRow["NetAmount"]);
                billsModel.Discount = Convert.ToDecimal(@dataRow["Discount"]);
            }

            #endregion
           
            UserDropDown();
            OrderDropDown();
            return View("AddBill", billsModel);
        }

        public void UserDropDown()
        {
            string connectionString2 = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection2 = new SqlConnection(connectionString2);
            connection2.Open();
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_User_DropDown";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<UserDropDownModel> userList = new List<UserDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                UserDropDownModel userDropDownModel = new UserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = userList;
        }


        public void OrderDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Order_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<OrderDropDownModel> orderList = new List<OrderDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                OrderDropDownModel orderDropDownModel = new OrderDropDownModel();
                orderDropDownModel.OrderID = Convert.ToInt32(data["OrderID"]);
                orderList.Add(orderDropDownModel);
            }
            ViewBag.OrderList = orderList;
        }

        [HttpPost]
        public IActionResult BillSave(BillsModel billsModel)
        {
            if (billsModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (billsModel.BillID == 0)
                {
                    command.CommandText = "PR_Bill_Insert";
                }
                else
                {
                    command.CommandText = "PR_Bill_Update";
                    command.Parameters.Add("@BillID", SqlDbType.Int).Value = billsModel.BillID;
                }
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = billsModel.UserID;
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = billsModel.OrderID;
                command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = billsModel.BillNumber;
                command.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = billsModel.BillDate;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = billsModel.TotalAmount;
                command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = billsModel.Discount;
                command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = billsModel.NetAmount;
                command.ExecuteNonQuery();
                return RedirectToAction("BillsDetail");
            }
            OrderDropDown();
            UserDropDown();
            return View("AddBill", billsModel);
        }
    }
}
