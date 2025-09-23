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
            try
            {
                var response = await authService.AgentLogin(request);

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("client/login")]
        public async Task<IActionResult> ClientLogin([FromBody] ClientLoginRequest request)
        {
            try
            {
                var response = await authService.ClientLogin(request);

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
