namespace BackEnd.Modules.Auth.Dtos;

public class LoginResponseDto {
  public required string Token { get; set; }
  public required string Name { get; set; }
  public required string Email { get; set; }
}
