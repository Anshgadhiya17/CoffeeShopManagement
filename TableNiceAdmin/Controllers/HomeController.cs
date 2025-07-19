using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using TableNiceAdmin.Models;

namespace TableNiceAdmin.Controllers
{
    [CheckAccess]

    public class HomeController : Controller
    {

        private IConfiguration configuration;

        public HomeController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public async Task<IActionResult> Index()
        {
            var dashboardData = new Dashboard
            {
                Counts = new List<DashboardCounts>(),
                RecentOrders = new List<RecentOrder>(),
                RecentProducts = new List<RecentProduct>(),
                TopCustomers = new List<TopCustomer>(),
                TopSellingProducts = new List<TopSellingProduct>(),
                NavigationLinks = new List<QuickLinks>()
            };

            using (var connection = new SqlConnection(this.configuration.GetConnectionString("ConnectionString")))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("usp_GetDashboardData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            // Fetch counts
                            while (await reader.ReadAsync())
                            {
                                dashboardData.Counts.Add(new DashboardCounts
                                {
                                    Metric = reader["Metric"].ToString(),
                                    Value = Convert.ToInt32(reader["Value"])
                                });
                            }

                            // Fetch recent orders
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    dashboardData.RecentOrders.Add(new RecentOrder
                                    {
                                        OrderID = Convert.ToInt32(reader["OrderID"]),
                                        CustomerName = reader["CustomerName"].ToString(),
                                        OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                        Status = reader["Status"].ToString()
                                    });
                                }
                            }

                            // Fetch recent products
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    dashboardData.RecentProducts.Add(new RecentProduct
                                    {
                                        ProductID = Convert.ToInt32(reader["ProductID"]),
                                        ProductName = reader["ProductName"].ToString(),
                                        Category = reader["Category"].ToString(),
                                        AddedDate = Convert.ToDateTime(reader["AddedDate"]),
                                        StockQuantity = Convert.ToInt32(reader["StockQuantity"])
                                    });
                                }
                            }

                            // Fetch top customers
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    dashboardData.TopCustomers.Add(new TopCustomer
                                    {
                                        CustomerName = reader["CustomerName"].ToString(),
                                        TotalOrders = Convert.ToInt32(reader["TotalOrders"]),
                                        Email = reader["Email"].ToString()
                                    });
                                }
                            }

                            // Fetch top selling products   
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    dashboardData.TopSellingProducts.Add(new TopSellingProduct
                                    {
                                        ProductName = reader["ProductName"].ToString(),
                                        TotalSoldQuantity = Convert.ToInt32(reader["TotalSoldQuantity"]),
                                        Category = reader["Category"].ToString()
                                    });
                                }
                            }
                        }
                    }
                }
            }

            dashboardData.NavigationLinks = new List<QuickLinks> {
        new QuickLinks {ActionMethodName = "Index", ControllerName="Home", LinkName="Dashboard" },
        new QuickLinks {ActionMethodName = "Privacy", ControllerName="Home", LinkName="Privacy" },
        new QuickLinks {ActionMethodName = "Country", ControllerName="Country", LinkName="Country" },
        new QuickLinks {ActionMethodName = "State", ControllerName="State", LinkName="State" },
        new QuickLinks {ActionMethodName = "City", ControllerName="City", LinkName="City" },
        new QuickLinks {ActionMethodName = "ProductList", ControllerName="Product", LinkName="Product" },
        new QuickLinks {ActionMethodName = "CustomerList", ControllerName="Customer", LinkName="Customer" },
        new QuickLinks {ActionMethodName = "OrderList", ControllerName="Order", LinkName="Order" },
        new QuickLinks {ActionMethodName = "OrderDetailList", ControllerName="OrderDetail", LinkName="OrderDetail" },
        new QuickLinks {ActionMethodName = "BillsDetail", ControllerName="Bills", LinkName="Bills" },
        new QuickLinks {ActionMethodName = "UserList", ControllerName="User", LinkName="User" }
    };

            var model = new Dashboard
            {
                Counts = dashboardData.Counts,
                RecentOrders = dashboardData.RecentOrders,
                RecentProducts = dashboardData.RecentProducts,
                TopCustomers = dashboardData.TopCustomers,
                TopSellingProducts = dashboardData.TopSellingProducts,
                NavigationLinks = dashboardData.NavigationLinks
            };

            return View("Dashboard", model);
        }
        public IActionResult CreateCookie()
        {
            string key = "ansh";
            string value = "hello from ansh";
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };
            Response.Cookies.Append(key, value, options);
            return View("Index");
        }
        public IActionResult ReadCookie()
        {
            string key = "ansh";
            var CookieValue= Request.Cookies[key];
            return View("Index");
        }
        public IActionResult RemoveCookie()
        {
            string key = "ansh";
            string value = string.Empty;
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            Response.Cookies.Append(key, value, options);
            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
