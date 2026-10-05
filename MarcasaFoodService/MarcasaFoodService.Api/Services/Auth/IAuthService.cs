using MarcasaFoodService.Api.Models.Auth;
using MarcasaFoodService.Api.Models.Common;

namespace MarcasaFoodService.Api.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    }
}