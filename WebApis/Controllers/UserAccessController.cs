using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApis.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserAccessController : ControllerBase
{
    [HttpGet]
    public List<string> Get()
    {
        return new List<string> { "user data" };
    }
}
