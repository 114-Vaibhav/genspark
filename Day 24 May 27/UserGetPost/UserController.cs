using Microsoft.AspNetCore.Mvc;
using UserGetPost.Context;
using UserGetPost.Models;
using ClosedXML.Excel;

namespace UserGetPost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        UserRepository _repository;

        public UsersController(UserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public List<User> Get()
        {
            return _repository.GetAllUsers();
        }

        [HttpPost]
        public void Post(User user)
        {
            _repository.AddUser(user);
        } 
        [HttpGet("export")]
        public IActionResult ExportUsers()
        {
            var users = _repository.GetAllUsers();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");

                // Headers
                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Phone";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "Age";

                // Data
                for (int i = 0; i < users.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = users[i].Name;
                    worksheet.Cell(i + 2, 2).Value = users[i].Phone;
                    worksheet.Cell(i + 2, 3).Value = users[i].Email;
                    worksheet.Cell(i + 2, 4).Value = users[i].Age;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "users.xlsx");
                }
            }
        }
    }
}