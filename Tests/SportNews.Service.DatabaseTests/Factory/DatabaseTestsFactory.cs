using Confluent.Kafka;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SportNews.Service.Controllers;
using SportNews.Service.DatabaseTests.Data;
using SportNews.Service.Kafka.Consumers;
using SportNews.Service.Kafka.Producers;
using SportNews.Service.Repositories;
using SportNews.Service.Settings;

namespace SportNews.Service.DatabaseTests.Factory;

/// <summary>
/// Класс, предоставляющий набор необходимых объектов для проверки функционала БД.
/// </summary>
public class DatabaseTestsFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Конструктор класса.
    /// </summary>
    public DatabaseTestsFactory()
    {
        // Настраиваем параметры для репозитория
        _options = OptionsDataFactory.GetDatabaseOptions();
        NewsRepository = new(_options);

        // Создаём коллекцию сервисов
        var services = new ServiceCollection();

        // Настраиваем Redis в памяти
        services.AddDistributedMemoryCache();

        // Настраиваем Kafka
        var consumerConfig = new ConsumerConfig
        {
            GroupId = "news-service-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        services.AddSingleton<IConsumer<Ignore, string>>(sp =>
            new ConsumerBuilder<Ignore, string>(consumerConfig).Build());
        services.AddSingleton<IProducer<Null, string>>(sp =>
            new ProducerBuilder<Null, string>(producerConfig).Build());

        // Настройка логирования
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        // Регистрация Kafka-продюсера и консюмера
        services.AddSingleton<NewsProducerService>();
        services.AddHostedService<NewsConsumerService>();

        // Регистрируем репозиторий и другие зависимости
        services.AddSingleton<NewsRepository>();
        services.AddSingleton(_options);

        // Сохраняем провайдер сервисов
        _serviceProvider = services.BuildServiceProvider();

        // Получаем экземпляр Kafka-продюсера
        NewsProducerService = _serviceProvider.GetRequiredService<NewsProducerService>();
        _cache = _serviceProvider.GetRequiredService<IDistributedCache>();
    }

    /// <summary>
    /// Объект для ведения записей.
    /// </summary>
    public Mock<ILogger<NewsController>> Logger { get; set; } = new Mock<ILogger<NewsController>>();

    /// <summary>
    /// Класс, предоставляющий функционал для работы с новостями.
    /// </summary>
    public NewsRepository NewsRepository { get; set; }

    /// <summary>
    /// Класс, представляющий Kafka-продюсера,
    /// для отправления запроса на подтверждения новости.
    /// </summary>
    public NewsProducerService NewsProducerService { get; set; }

    /// <summary>
    /// Создание контроллера, управляющего новостями.
    /// </summary>
    /// <returns>Контроллер для работы с новостями.</returns>
    public NewsController Build()
    {
        return new NewsController(NewsRepository, Logger.Object, _cache, NewsProducerService);
    }

    /// <summary>
    /// Создание контроллера, управляющего новостями.
    /// </summary>
    /// <param name="cache">Объект для сохранения кэша.</param>
    /// <returns>Контроллер для работы с новостями.</returns>
    public NewsController Build(IDistributedCache cache)
    {
        return new NewsController(NewsRepository, Logger.Object, cache, NewsProducerService);
    }

    private readonly IOptions<DatabaseSettings> _options;
    private readonly IDistributedCache _cache;
}
