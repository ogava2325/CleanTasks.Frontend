using Domain.Dtos.Shared;
using Domain.Dtos.User;
using Refit;

namespace Services.External;

public interface IUserService
{
    [Post("/api/users/login")]
    Task<ResultDto<string>> LoginAsync(LoginUserDto userDto);
    
    [Post("/api/users/register")]
    Task<ResultDto<string>> RegisterAsync(RegisterUserDto userDto);
    
    [Get("/api/users/{userId}")]
    Task<UserDto> GetById(Guid userId);
}