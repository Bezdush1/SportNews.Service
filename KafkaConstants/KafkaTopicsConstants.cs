namespace KafkaConstants;

/// <summary>
/// Класс, предоставляющий набор названия топиков при работе с кафкой.
/// </summary>
public static class KafkaTopicsConstants
{
    /// <summary>
    /// Топик для получения сообщений.
    /// </summary>
    public const string ObjectServiceTopic = "object_service_topic";

    /// <summary>
    /// Топик для отправки подтверждения.
    /// </summary>
    public const string ConfirmationTopic = "confirmation_topic";
}
