using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SportNews.Service.Models;
using SportNews.Service.Repositories.Interfases;
using SportNews.Service.Settings;

namespace SportNews.Service.Repositories;

/// <summary>
/// Класс, предоставляющий функционал для работы с новостями.
/// </summary>
public class NewsRepository : INewsRepository
{
    private readonly IMongoCollection<News> _newsCollection;

    /// <summary>
    /// Конструктор класса.
    /// </summary>
    /// <param name="mongoSettings">Объект, хранящий значения конфигурации БД.</param>
    public NewsRepository(IOptions<DatabaseSettings> mongoSettings)
    {
        var client = new MongoClient(mongoSettings.Value.ConnectionStringMongoDb);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        _newsCollection = database.GetCollection<News>("News");
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<News>> GetAllAsync()
    {
        return await _newsCollection.Find(news => true).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<News?> GetByIdAsync(string id)
    {
        return await _newsCollection.Find(news => news.Id == id).FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task AddAsync(News news)
    {
        await _newsCollection.InsertOneAsync(news);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(string id, News news)
    {
        await _newsCollection.ReplaceOneAsync(n => n.Id == id, news);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string id)
    {
        await _newsCollection.DeleteOneAsync(n => n.Id == id);
    }

    /// <inheritdoc/>
    public async Task DeleteAllAsync()
    {
        var filter = Builders<News>.Filter.Empty;
        await _newsCollection.DeleteManyAsync(filter);
    }
}
