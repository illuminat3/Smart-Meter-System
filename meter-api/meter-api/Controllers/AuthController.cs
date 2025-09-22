using meter_api.Datatypes.Requests;
using meter_api.Datatypes.Responses;
using meter_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace meter_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("agent/login")]
        public async Task<IActionResult> AgentLogin([FromBody] AgentLoginRequest request)
        {
            var response = await authService.AgentLogin(request);

            return Ok(response);
        }
    }
}
