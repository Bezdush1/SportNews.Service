namespace SportNews.Service.Interaction.In;

/// <summary>
/// Данные о новости, приходящие из вне.
/// </summary>
public class NewsInfo
{
    /// <summary>
    /// Заголовок.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Контент.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Категория.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Дата публикации.
    /// </summary>
    public DateTime? PublishedAt { get; set; }
}
