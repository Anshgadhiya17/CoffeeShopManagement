using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using TableNiceAdmin.Models;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace TableNiceAdmin.Controllers
{
    [CheckAccess]

    public class OrderController : Controller
    {
        private IConfiguration configuration;

        public OrderController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult OrderList()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Order_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            return View(dataTable);
        }

        public IActionResult AddOrder(int OrderID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            #region OrderByID

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Order_SelectByPrimaryKey";
            command.Parameters.AddWithValue("@OrderID", OrderID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            OrderModel orderModel = new OrderModel();

            foreach (DataRow dataRow in table.Rows)
            {
                orderModel.OrderID = Convert.ToInt32(@dataRow["OrderID"]);
                orderModel.OrderDate = Convert.ToDateTime(@dataRow["OrderDate"]);
                orderModel.CustomerID = Convert.ToInt32(@dataRow["CustomerID"].ToString());
                orderModel.PaymentMode = @dataRow["PaymentMode"].ToString();
                orderModel.TotalAmount = Convert.ToInt32(@dataRow["TotalAmount"]);
                orderModel.ShippingAddress = @dataRow["ShippingAddress"].ToString();
                orderModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }

            #endregion

            UserDropDown();
            CustomerDropdown();
            return View("AddOrder", orderModel);
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

        public void CustomerDropdown()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Customer_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<CustomerDropDownModel> customerList = new List<CustomerDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                CustomerDropDownModel customerDropDownModel = new CustomerDropDownModel();
                customerDropDownModel.CustomerID = Convert.ToInt32(data["CustomerID"]);
                customerDropDownModel.CustomerName = data["CustomerName"].ToString();
                customerList.Add(customerDropDownModel);
            }
            ViewBag.CustomerList = customerList;
        }
        public IActionResult OrderDelete(int @OrderID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Order_Delete";
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = @OrderID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("OrderList");
        }

        [HttpPost]
        public IActionResult OrderSave(OrderModel orderModel)
        {
            if (orderModel.UserID <= 0)
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
                if (orderModel.OrderID == 0)
                {
                    command.CommandText = "PR_Order_Insert";
                }
                else
                {
                    command.CommandText = "PR_Order_Update";
                    command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderModel.OrderID;
                }
                command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = orderModel.OrderDate;
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = orderModel.CustomerID;
                command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = orderModel.PaymentMode;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderModel.TotalAmount;
                command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = orderModel.ShippingAddress;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("OrderList");
            }
            UserDropDown();
            CustomerDropdown();
            return View("AddOrder", orderModel);
        }

        public IActionResult ExportToExcelOrderTable()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "PR_Order_SelectAll";
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(sqlDataReader);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Load data into the worksheet
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, true, TableStyles.Medium9);

                    // Generate the Excel file
                    var stream = new MemoryStream(package.GetAsByteArray());

                    // Return the file as a downloadable response
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrdersExport.xlsx");
                }
            }
        }

        public IActionResult SendOrderTableAsEmail()
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_Order_SelectAll";
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);

                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Orders");

                        // Load data into the worksheet
                        worksheet.Cells["A1"].LoadFromDataTable(dataTable, true, TableStyles.Medium9);

                        // Convert the package to a byte array
                        byte[] fileBytes = package.GetAsByteArray();

                        // Prepare the email
                        var apiKey = "your-sendgrid-api-key"; // Replace with your SendGrid API key
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress("22010101068@darshan.ac.in", "Renish Hirani");
                        var subject = "Orders Export";
                        var to = new EmailAddress("22010101068@darshan.ac.in", "Renish Hirani");
                        var plainTextContent = "Please find the attached Orders Export Excel file.";
                        var htmlContent = "<strong>Please find the attached Orders Export Excel file.</strong>";
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                        // Attach the Excel file
                        var fileName = "OrdersExport.xlsx";
                        msg.AddAttachment(fileName, Convert.ToBase64String(fileBytes), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                        // Send the email
                        var response = client.SendEmailAsync(msg).Result;

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return Content("Email sent successfully.");
                        }
                        else
                        {
                            return Content("There was an issue sending the email. Please try again later.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                ILogger logger = HttpContext.RequestServices.GetService(typeof(ILogger<OrderController>)) as ILogger;
                logger?.LogError(ex, "An unexpected error occurred.");

                // Return a general error message
                return Content("An unexpected error occurred. Please try again later.");
            }
        }
    }
}
