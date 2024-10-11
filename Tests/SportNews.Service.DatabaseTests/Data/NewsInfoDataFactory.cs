using SportNews.Service.Interaction.In;

namespace SportNews.Service.DatabaseTests.Data;

/// <summary>
/// Класс, предоставляющий данные новостей для тестирования.
/// </summary>
public static class NewsInfoDataFactory
{
    /// <summary>
    /// Получение данных о новости с отличительным знаком.
    /// </summary>
    /// <param name="i">Отличительный знак, представленный в виде итерации цикла.</param>
    /// <returns>Новость, с отличительным знаком.</returns>
    public static NewsInfo GetNews(int i)
    {
        return new NewsInfo()
        {
            Title = $"Title {i}",
            Content = $"Content {i}",
            Category = $"Category {i}",
            PublishedAt = DateTime.UtcNow
        };
    }
}
