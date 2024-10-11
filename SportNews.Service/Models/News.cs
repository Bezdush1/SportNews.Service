using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SportNews.Service.Models;

/// <summary>
/// Новость.
/// </summary>
public class News
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

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
    public DateTime PublishedAt { get; set; }
}
