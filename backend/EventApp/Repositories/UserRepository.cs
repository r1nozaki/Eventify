using EventApp.Data;
using EventApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EventApp.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => dbContext.Users.AnyAsync(u => u.Username == username, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
