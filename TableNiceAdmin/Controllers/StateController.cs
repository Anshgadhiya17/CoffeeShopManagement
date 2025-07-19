using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using TableNiceAdmin.Models;

namespace TableNiceAdmin.Controllers
{
    public class StateController : Controller
    {
        private readonly IConfiguration _configuration;

        #region Configuration
        public StateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region State
        public IActionResult State()
        {
            string connectionStr = _configuration.GetConnectionString("ConnectionString");
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();

                // Prepare the Command
                SqlCommand objCmd = conn.CreateCommand();
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.CommandText = "PR_LOC_State_SelectAll";

                // Execute and load data
                SqlDataReader objSDR = objCmd.ExecuteReader();
                dt.Load(objSDR);
            }

            return View("State", dt);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int StateID)
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
                        objCmd.CommandText = "PR_LOC_State_Delete";
                        objCmd.Parameters.AddWithValue("@StateID", StateID);
                        objCmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("State");
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = $"Error deleting State: {ex.Message}";
                return RedirectToAction("State");
            }
        }
        #endregion

        #region StateAddEdit

        public IActionResult StateAddEdit(int StateID)
        {
            CountryDropdown();

            #region StateByID

            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_LOC_State_SelectByPK";
            command.Parameters.AddWithValue("@StateID", StateID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            StateModel StateModel = new StateModel();

            foreach (DataRow dataRow in table.Rows)
            {
                StateModel.StateID = Convert.ToInt32(@dataRow["StateID"]);
                StateModel.StateName = @dataRow["StateName"].ToString();
                StateModel.StateCode = @dataRow["StateCode"].ToString();
                StateModel.CountryID = Convert.ToInt32(@dataRow["CountryID"]);
            }

            #endregion

            return View("StateAddEdit", StateModel);
        }

        #endregion

        #region StateSave
        [HttpPost]

        public IActionResult StateSave(StateModel StateModel)
        {

            if (StateModel.CountryID <= 0)
            {
                ModelState.AddModelError("CountryID", "A valid  is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (StateModel.StateID == null)
                {
                    command.CommandText = "PR_LOC_State_Insert";
                }
                else
                {
                    command.CommandText = "PR_LOC_State_Update";
                    command.Parameters.Add("@StateID", SqlDbType.Int).Value = StateModel.StateID;
                }

                command.Parameters.Add("@StateName", SqlDbType.VarChar).Value = StateModel.StateName;
                command.Parameters.Add("@StateCode", SqlDbType.NVarChar).Value = StateModel.StateCode;
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = StateModel.CountryID;
                command.ExecuteNonQuery();

                TempData["successMessage"] = StateModel.StateID == 0 ? "State added successfully!" : "State updated successfully!";
                return RedirectToAction("State");
            }

            CountryDropdown();
            return View("StateAddEdit", StateModel);
        }
        #endregion


        #region Country Drop-Down
        public void CountryDropdown()
        {
            // Fetch Country dropdown list
            string connectionString = this._configuration.GetConnectionString("ConnectionString");



            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_LOC_Country_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            connection1.Close();

            List<CountryDropDownModel> country = new List<CountryDropDownModel>();

            foreach (DataRow dataRow in dataTable1.Rows)
            {
                CountryDropDownModel countryDropDownModel = new CountryDropDownModel();
                countryDropDownModel.CountryID = Convert.ToInt32(dataRow["CountryID"]);
                countryDropDownModel.CountryName = dataRow["CountryName"].ToString();
                country.Add(countryDropDownModel);
            }

            ViewBag.CountryList = country;
        }
        #endregion
    }
}
