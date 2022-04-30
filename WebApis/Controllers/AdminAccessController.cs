using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApis.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AdminAccessController : ControllerBase
{
    [HttpGet]
    public List<string> Get()
    {
        string[] scopeRequiredByApi = new string[] { "access_as_user" };

        return new List<string> { "admin data" };
    }
}
