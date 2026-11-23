using BackEnd.Modules.Auth.Dtos;
using BackEnd.Modules.Auth.Entities;
using BackEnd.Modules.Auth.Repositories;

namespace BackEnd.Modules.Auth.Services;

public class AuthService(AuthRepository authRepository, JwtService jwtService) {
  private readonly AuthRepository _authRepository = authRepository;
  private readonly JwtService _jwtService = jwtService;

  public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto) {
    var user = await _authRepository.FindByEmailAsync(dto.Email);
    if (user is null) return null;

    bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
    if (!valid) return null;

    var token = _jwtService.GenerateToken(user);
    return new LoginResponseDto { Token = token, Name = user.Name, Email = user.Email };
  }

  public async Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto dto) {
    if (dto.Password != dto.ConfirmPassword) return null;
    if (await _authRepository.EmailExistsAsync(dto.Email)) return null;

    var user = new User {
      Name = dto.Name,
      Email = dto.Email,
      Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
      Uuid = Guid.NewGuid(),
    };

    var created = await _authRepository.CreateAsync(user);
    var token = _jwtService.GenerateToken(created);
    return new LoginResponseDto { Token = token, Name = created.Name, Email = created.Email };
  }
}
