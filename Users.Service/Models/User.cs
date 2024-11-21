using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Users.Service.Models;

/// <summary>
/// Пользователь.
/// </summary>
public class User
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Количество зарегестрированных пользователей.
    /// </summary>
    public int RegisteredObjects { get; set; } = 0;
}