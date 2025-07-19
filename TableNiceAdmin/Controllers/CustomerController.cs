using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using TableNiceAdmin.Models;

namespace TableNiceAdmin.Controllers
{
    [CheckAccess]

    public class CustomerController : Controller
    {

        private IConfiguration configuration;

        public  CustomerController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult CustomerList()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Customer_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            return View(dataTable);
        }

        public IActionResult CustomerDelete(int CustomerID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Customer_Delete";
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = CustomerID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("CustomerList");
        }
        public IActionResult AddCustomer(int CustomerID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            #region CustomerByID

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Customer_SelectByPrimaryKey";
            command.Parameters.AddWithValue("@CustomerID", CustomerID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            CustomerModel customerModel = new CustomerModel();

            foreach (DataRow dataRow in table.Rows)
            {
                customerModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
                customerModel.CustomerId = Convert.ToInt32(@dataRow["CustomerID"]);
                customerModel.HomeAddress = @dataRow["HomeAddress"].ToString();
                customerModel.CustomerName = @dataRow["CustomerName"].ToString();
                customerModel.Email = @dataRow["Email"].ToString();
                customerModel.MobileNo = @dataRow["MobileNo"].ToString();
                customerModel.Gst_No = @dataRow["Gst_No"].ToString();
                customerModel.CityName = @dataRow["CityName"].ToString();
                customerModel.PinCode = @dataRow["PinCode"].ToString();
                customerModel.NetAmount = Convert.ToDecimal(@dataRow["NetAmount"]);

            }
            #endregion
            UserDropDown();
            return View("AddCustomer",customerModel);
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

        [HttpPost]
        public IActionResult CustomerSave(CustomerModel customerModel)
        {
            if (customerModel.UserID <= 0)
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
                if (customerModel.CustomerId == 0)
                {
                    command.CommandText = "PR_Customer_Insert";
                }
                else
                {
                    command.CommandText = "PR_Customer_Update";
                    command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerModel.CustomerId;
                }
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = customerModel.UserID;
                command.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = customerModel.CustomerName;
                command.Parameters.Add("@HomeAddress", SqlDbType.VarChar).Value = customerModel.HomeAddress;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = customerModel.Email;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = customerModel.MobileNo;
                command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = customerModel.CityName;
                command.Parameters.Add("@PinCode", SqlDbType.VarChar).Value = customerModel.PinCode;
                command.Parameters.Add("@Gst_No", SqlDbType.VarChar).Value = customerModel.Gst_No;
                command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = customerModel.NetAmount;
                command.ExecuteNonQuery();
                return RedirectToAction("CustomerList");
            }
            UserDropDown();
            return View("AddCustomer", customerModel);
        }

    }
}
