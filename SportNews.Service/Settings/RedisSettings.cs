namespace SportNews.Service.Settings;

/// <summary>
/// Класс, хранящий значения конфигурации Redis.
/// </summary>
public class RedisSettings
{
    /// <summary>
    /// Строка, хранящая значение для подключения к Redis.
    /// </summary>
    public string Redis { get; set; } = null!;
}
