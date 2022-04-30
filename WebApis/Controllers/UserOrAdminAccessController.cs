using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApis.Controllers;

[Authorize]
[Route("[controller]")]
public class UserOrAdminAccessController : ControllerBase
{
    [HttpGet]
    public List<string> Get()
    {
        return new List<string> { "data for users and admins data" };
    }
}
