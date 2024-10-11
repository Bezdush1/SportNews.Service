using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SportNews.Service.Controllers;
using SportNews.Service.DatabaseTests.Data;
using SportNews.Service.Repositories;
using SportNews.Service.Settings;

namespace SportNews.Service.DatabaseTests.Factory;

/// <summary>
/// Класс, предоставляющий набор необходимых объектов для проверки функционала БД.
/// </summary>
public class DatabaseTestsFactory
{
    /// <summary>
    /// Конструктор класса.
    /// </summary>
    public DatabaseTestsFactory()
    {
        _options = OptionsDataFactory.GetDatabaseOptions();

        NewsRepository = new(_options);

        // Настраиваем сервисы и создаем IDistributedCache с использованием памяти
        var services = new ServiceCollection();
        services.AddDistributedMemoryCache();

        var serviceProvider = services.BuildServiceProvider();
        _cache = serviceProvider.GetRequiredService<IDistributedCache>();
    }

    /// <summary>
    /// Объект, для ведения записей.
    /// </summary>
    public Mock<ILogger<NewsController>> Logger { get; set; } = new Mock<ILogger<NewsController>>();

    /// <summary>
    /// Класс, предоставляющий функционал для работы с новостями.
    /// </summary>
    public NewsRepository NewsRepository { get; set; }

    /// <summary>
    /// Создание контроллера, управляющего новостями.
    /// </summary>
    /// <returns>Контроллер для работы с новостями.</returns>
    public NewsController Build()
    {
        return new NewsController(NewsRepository, Logger.Object, _cache);
    }

    /// <summary>
    /// Создание контроллера, управляющего новостями.
    /// </summary>
    /// <param name="cache">Объект для сохранения кэша.</param>
    /// <returns>Контроллер для работы с новостями.</returns>
    public NewsController Build(IDistributedCache cache)
    {
        return new NewsController(NewsRepository, Logger.Object, cache);
    }

    private readonly IOptions<DatabaseSettings> _options;
    private readonly IDistributedCache _cache;
}
