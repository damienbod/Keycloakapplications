using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApis.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AngularAccessController : ControllerBase
{
    [HttpGet]
    public List<string> Get()
    {
        return new List<string> { "data for Angular public client PKCE" };
    }
}
