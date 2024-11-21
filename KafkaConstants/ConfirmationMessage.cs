namespace KafkaConstants;

/// <summary>
/// Структура, подтверждающая создание новости.
/// </summary>
public class ConfirmationMessage
{
    /// <summary>
    /// Идентификатор новости.
    /// </summary>
    public string ObjectId { get; set; } = string.Empty;

    /// <summary>
    /// Время подтвеждения.
    /// </summary>
    public string ConfirmationTimestamp { get; set; } = string.Empty;
}
