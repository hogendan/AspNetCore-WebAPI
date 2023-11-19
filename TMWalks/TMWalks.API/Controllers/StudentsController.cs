using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMWalks.API;

// https://localhost:portnumber/api/students
[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    // GET: https://locahost:portnumber/api/studentes
    [HttpGet]
    public IActionResult GetAllStudents() {
        string[] students = new string[] { "John", "Jane", "Mark", "Emily", "David" };

        return Ok(students);
    }

}
