using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using TableNiceAdmin.Models;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace TableNiceAdmin.Controllers
{
    public class CityController : Controller
    {
        private IConfiguration configuration;

        public CityController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult CityDetail()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_LOC_City_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            return View(dataTable);
        }

        public IActionResult CityDelete(string CityID)
        {
            try
            {
                int decryptedCityID = Convert.ToInt32(UrlEncryptor.Decrypt(CityID.ToString()));

                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_LOC_City_Delete";
                command.Parameters.Add("@CityID", SqlDbType.Int).Value = decryptedCityID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("CityDetail");
        }
        public IActionResult AddEditCity(string? CityID)
        {

            int? decryptedCityID = 0;

            // Decrypt only if CityID is not null or empty
            if (!string.IsNullOrEmpty(CityID))
            {
                string decryptedCityIDString = UrlEncryptor.Decrypt(CityID); // Decrypt the encrypted CityID
                decryptedCityID = int.Parse(decryptedCityIDString); // Convert decrypted string to integer
            }

            LoadCountryList();

            // City By ID
            string connectionString1 = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString1);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_LOC_City_SelectByPK";
            command.Parameters.AddWithValue("@CityID", SqlDbType.Int).Value = decryptedCityID;
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            CityModel cityModel = new CityModel();

            foreach (DataRow dataRow in table.Rows)
            {
                cityModel.CityID = Convert.ToInt32(@dataRow["CityID"]);
                cityModel.CityName = @dataRow["CityName"].ToString();
                cityModel.CityCode = @dataRow["CityCode"].ToString();
                cityModel.StateID = Convert.ToInt32(@dataRow["StateID"]);
                cityModel.CountryID = Convert.ToInt32(@dataRow["CountryID"]);
                ViewBag.StateList = GetStateByCountryID(cityModel.CountryID);
            }
            GetStateByCountryID(cityModel.CountryID);

            //Url parameter decrypt
            //string url = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            //Uri uri = new Uri(url);
            //cityModel.CityID = int.Parse(UrlEncryptor.Decrypt($"Query: {uri.Query}"));

            return View("AddEditCity", cityModel);
        }

        public void LoadCountryList()
        {
            string connectionString2 = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection2 = new SqlConnection(connectionString2);
            connection2.Open();
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_LOC_Country_DropDown";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<CountryDropDownModel> countryList = new List<CountryDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                CountryDropDownModel countryDropDownModel = new CountryDropDownModel();
                countryDropDownModel.CountryID = Convert.ToInt32(data["CountryID"]);
                countryDropDownModel.CountryName = data["CountryName"].ToString();
                countryList.Add(countryDropDownModel);
            }
            ViewBag.CountryList = countryList;
        }

        [HttpPost]
        public JsonResult GetStatesByCountry(int CountryID)
        {
            List<StateDropDownModel> loc_State = GetStateByCountryID(CountryID); // Fetch states
            return Json(loc_State); // Return JSON response
        }

        public List<StateDropDownModel> GetStateByCountryID(int CountryID)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_LOC_State_DropDownByCountryID";
            command.Parameters.AddWithValue("@CountryID", CountryID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            List<StateDropDownModel> stateList = new List<StateDropDownModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                StateDropDownModel stateDropDownModel = new StateDropDownModel
                {
                    StateID = Convert.ToInt32(dataRow["StateID"]),
                    StateName = dataRow["StateName"].ToString()
                };
                stateList.Add(stateDropDownModel);
            }
            ViewBag.StateList = stateList;
            connection.Close();
            return stateList;
        }

        [HttpPost]
        public IActionResult CitySave(CityModel cityModel)
        {
            if (cityModel.StateID < 0)
            {
                ModelState.AddModelError("StateID", "A valid State is required.");
            }

            if (cityModel.CountryID < 0)
            {
                ModelState.AddModelError("CountryID", "A valid Country is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (cityModel.CityID == null)
                {
                    command.CommandText = "PR_LOC_City_Insert";
                }
                else
                {
                    command.CommandText = "PR_LOC_City_Update";
                    command.Parameters.Add("@CityID", SqlDbType.Int).Value = cityModel.CityID;
                }
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = cityModel.CountryID;
                command.Parameters.Add("@StateID", SqlDbType.VarChar).Value = cityModel.StateID;
                command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = cityModel.CityName;
                command.Parameters.Add("@CityCode", SqlDbType.VarChar).Value = cityModel.CityCode;
                command.ExecuteNonQuery();
                return RedirectToAction("CityDetail");
            }
            LoadCountryList();
            return View("AddEditCity", cityModel);
        }
    }
}