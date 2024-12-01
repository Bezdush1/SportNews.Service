namespace KafkaConstants;

/// <summary>
/// Структура, описывающая сообщение для потверждения новости.
/// </summary>
public class NewsProcessMessage
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор новости.
    /// </summary>
    public string ObjectId { get; set; } = string.Empty;
}
