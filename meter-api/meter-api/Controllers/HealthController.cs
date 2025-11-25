using meter_api.Datatypes.Responses;
using Microsoft.AspNetCore.Mvc;

namespace meter_api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [ProducesResponseType(typeof(HealthStatusResponse), StatusCodes.Status200OK)]
    [HttpGet]
    public ActionResult<HealthStatusResponse> GetHealthStatus()
    {
        var result = new HealthStatusResponse { Status = "Healthy" };

        return Ok(result);
    }
}
