using MarcasaFoodService.Api.Models.Auth;
using MarcasaFoodService.Api.Models.Common;
using MarcasaFoodService.Api.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MarcasaFoodService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "Los datos enviados no son válidos."
                });
            }

            ApiResponse<LoginResponseDto> response = await _authService.LoginAsync(request);

            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordRequestDto request)
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _authService.CambiarPasswordAsync(id, User.Identity!.Name!, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
