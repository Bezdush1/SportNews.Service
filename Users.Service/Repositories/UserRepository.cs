using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Users.Service.Models;
using Users.Service.Repositories.Interfases;
using Users.Service.Settings;

namespace Users.Service.Repositories;

/// <summary>
/// Класс, предоставляющий функционал для работы с пользователями.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IOptions<DatabaseSettings> mongoSettings)
    {
        var client = new MongoClient(mongoSettings.Value.ConnectionStringMongoDb);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        _users = database.GetCollection<User>("News");
    }

    /// <inheritdoc/>
    public async Task AddUserAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    /// <inheritdoc/>
    public async Task DeleteUserAsync(string id)
    {
        await _users.DeleteOneAsync(n => n.Id == id);
    }

    /// <inheritdoc/>
    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await _users.Find(news => news.Id == id).FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateUserAsync(string id, User user)
    {
        await _users.ReplaceOneAsync(n => n.Id == id, user);
    }
}
