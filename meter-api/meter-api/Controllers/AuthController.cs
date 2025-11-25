using meter_api.Datatypes.Requests;
using meter_api.Datatypes.Responses;
using meter_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace meter_api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [ProducesResponseType(typeof(AgentLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("agent/login")]
    public async Task<ActionResult<AgentLoginResponse>> AgentLogin([FromBody] AgentLoginRequest request)
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

    [ProducesResponseType(typeof(AgentLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("client/login")]
    public async Task<ActionResult<ClientLoginResponse>> ClientLogin([FromBody] ClientLoginRequest request)
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
