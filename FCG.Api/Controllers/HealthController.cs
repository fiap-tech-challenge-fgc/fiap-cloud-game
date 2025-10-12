using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

public class HealthController : ControllerBase
{
    [HttpGet()]
    public IActionResult Get() => Ok("Healthy");
}