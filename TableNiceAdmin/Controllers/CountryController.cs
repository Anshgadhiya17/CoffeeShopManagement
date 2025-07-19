using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using TableNiceAdmin.Models;

namespace TableNiceAdmin.Controllers
{
    public class CountryController : Controller
    {
        private readonly IConfiguration _configuration;

        #region Configuration
        public CountryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion
        #region Country
        public IActionResult Country()
        {
            string connectionStr = _configuration.GetConnectionString("ConnectionString");
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();

                // Prepare the Command
                SqlCommand objCmd = conn.CreateCommand();
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.CommandText = "PR_LOC_Country_SelectAll";

                // Execute and load data
                SqlDataReader objSDR = objCmd.ExecuteReader();
                dt.Load(objSDR);
            }

            return View("Country", dt);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int CountryID)
        {
            try
            {
                string connectionStr = _configuration.GetConnectionString("ConnectionString");

                using (SqlConnection conn = new SqlConnection(connectionStr))
                {
                    conn.Open();

                    using (SqlCommand objCmd = conn.CreateCommand())
                    {
                        objCmd.CommandType = CommandType.StoredProcedure;
                        objCmd.CommandText = "PR_LOC_Country_Delete";
                        objCmd.Parameters.AddWithValue("@CountryID", CountryID);
                        objCmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Country");
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = $"Error deleting Country: {ex.Message}";
                return RedirectToAction("Country");
            }
        }
        #endregion

        #region CountryAddEdit

        public IActionResult CountryAddEdit(int CountryID)
        {

            #region CountryByID

            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_LOC_Country_SelectByPK";
            command.Parameters.AddWithValue("@CountryID", CountryID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            CountryModel CountryModel = new CountryModel();

            foreach (DataRow dataRow in table.Rows)
            {
                CountryModel.CountryID = Convert.ToInt32(@dataRow["CountryID"]);
                CountryModel.CountryName = @dataRow["CountryName"].ToString();
                CountryModel.CountryCode = @dataRow["CountryCode"].ToString();
            }

            #endregion

            return View("CountryAddEdit", CountryModel);
        }

        #endregion

        #region CountrySave
        [HttpPost]

        public IActionResult CountrySave(CountryModel CountryModel)
        {



            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (CountryModel.CountryID == null)
                {
                    command.CommandText = "PR_LOC_Country_Insert";
                }
                else
                {
                    command.CommandText = "PR_LOC_Country_Update";
                    command.Parameters.Add("@CountryID", SqlDbType.Int).Value = CountryModel.CountryID;
                }

                command.Parameters.Add("@CountryCode", SqlDbType.VarChar).Value = CountryModel.CountryCode;
                command.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = CountryModel.CountryName;
                command.ExecuteNonQuery();

                TempData["successMessage"] = CountryModel.CountryID == 0 ? "Country added successfully!" : "Country updated successfully!";
                return RedirectToAction("Country");
            }

            return View("CountryAddEdit", CountryModel);
        }
        #endregion


        public IActionResult CountryAjax()
        {
            string connectionStr = _configuration.GetConnectionString("ConnectionString");
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();

                // Prepare the Command
                SqlCommand objCmd = conn.CreateCommand();
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.CommandText = "PR_LOC_Country_SelectAll";

                // Execute and load data
                SqlDataReader objSDR = objCmd.ExecuteReader();
                dt.Load(objSDR);
            }

            // Build HTML string directly
            StringBuilder htmlString = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                htmlString.Append("<tr>");
                htmlString.Append($"<td>{row["CountryName"]}</td>");
                htmlString.Append($"<td>{row["CountryCode"]}</td>");
                htmlString.Append("<td>");
                htmlString.Append($"<a class='btn btn-warning btn-sm' href='/Country/CountryAddEdit/{row["CountryID"]}'><i class='bi bi-pencil-square'></i></a> ");
                htmlString.Append($"<a class='btn btn-danger btn-sm' href='/Country/Delete/{row["CountryID"]}' onclick=\"return confirm('Are you sure you want to delete this country?');\"><i class='bi bi-trash'></i></a>");
                htmlString.Append("</td>");
                htmlString.Append("</tr>");
            }

            return Content(htmlString.ToString());
        }



    }
}
