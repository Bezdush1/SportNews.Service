namespace SportNews.Service.Settings;

/// <summary>
/// Класс, хранящий значения конфигурации БД.
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// Строка подключения.
    /// </summary>
    public string ConnectionStringMongoDb { get; set; } = null!;

    /// <summary>
    /// Имя БД.
    /// </summary>
    public string DatabaseName { get; set; } = null!;
}
