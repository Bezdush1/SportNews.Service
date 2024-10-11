using MongoDB.Bson;
using SportNews.Service.Models;

namespace SportNews.Service.UnitTests.Data;

/// <summary>
/// Класс, предоставляющий тестовые данные новостей для модульных тестов.
/// </summary>
internal static class NewsDataFactory
{
    /// <summary>
    /// Получение новости.
    /// </summary>
    /// <returns>Новость.</returns>
    public static News GetNews()
    {
        return new News
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "Title",
            Category = "Category",
            Content = "Content",
            PublishedAt = DateTime.Now,
        };
    }

    /// <summary>
    /// Получение списка новостей.
    /// </summary>
    /// <returns>Список новостей.</returns>
    public static IEnumerable<News> GetListNews()
    {
        return new List<News>() { GetNews(), GetNews() };
    }
}
