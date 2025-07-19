using Microsoft.AspNetCore.Mvc;

namespace TableNiceAdmin.Controllers
{
    [CheckAccess]

    public class TableController : Controller
    {
        public IActionResult GeneralTable()
        {
            return View();
        }

        public IActionResult Employee()
        {
            return View();
        }

        public IActionResult Department()
        {
            return View();
        }

        public IActionResult Project()
        {
            return View();
        }

        public IActionResult EmployeeProject()
        {
            return View();
        }

        public IActionResult AddEmployee()
        {
            return View();
        }

        public IActionResult AddDepartment()
        {
            return View();
        }

        public IActionResult AddProject()
        {
            return View();
        }

        public IActionResult AddEmployeeProject()
        {
            return View();
        }

    }
}
