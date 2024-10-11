using Microsoft.Extensions.Options;
using SportNews.Service.Settings;

namespace SportNews.Service.DatabaseTests.Data;

/// <summary>
/// Класс, возвращающий значения конфигураций для тестов.
/// </summary>
public static class OptionsDataFactory
{
    /// <summary>
    /// Получение значения конфигураций тестовой БД.
    /// </summary>
    /// <returns>Конфигурационные значения тестовой БД.</returns>
    public static IOptions<DatabaseSettings> GetDatabaseOptions()
    {
        var databaseSettings = new DatabaseSettings()
        {
            DatabaseName = "SportsNewsDB",
            ConnectionStringMongoDb = "mongodb://localhost:27017",
        };

        return Options.Create(databaseSettings);
    }
}
