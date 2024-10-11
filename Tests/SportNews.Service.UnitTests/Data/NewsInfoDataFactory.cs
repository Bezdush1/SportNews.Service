using SportNews.Service.Interaction.In;

namespace SportNews.Service.UnitTests.Data;

/// <summary>
/// Класс, предоставляющий тестовые данные новостей, прихлдящих из вне,
/// для модульных тестов.
/// </summary>
public static class NewsInfoDataFactory
{
    /// <summary>
    /// Получение данных о новости из вне.
    /// </summary>
    /// <returns>Данные о новости из вне.</returns>
    public static NewsInfo GetNewsInfo()
    {
        return new NewsInfo
        {
            Title = "Title",
            Category = "Category",
            Content = "Content",
            PublishedAt = DateTime.Now,
        };
    }
}
