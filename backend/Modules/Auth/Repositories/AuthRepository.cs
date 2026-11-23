using BackEnd.Data;
using BackEnd.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Modules.Auth.Repositories;

// Interactúa con la base de datos
public class AuthRepository(AppDbContext db) {
  private readonly AppDbContext _db = db;

  public async Task<User?> FindByEmailAsync(string email) {
    return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
  }

  public async Task<bool> EmailExistsAsync(string email) {
    return await _db.Users.AnyAsync(u => u.Email == email);
  }

  public async Task<User> CreateAsync(User user) {
    user.Uuid = Guid.NewGuid();
    _db.Users.Add(user);
    await _db.SaveChangesAsync();
    return user;
  }
}
