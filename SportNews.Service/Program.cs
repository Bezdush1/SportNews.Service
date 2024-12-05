using SportNews.Service.Repositories.Interfases;
using SportNews.Service.Repositories;
using SportNews.Service.Settings;
using NLog.Web;
using NLog;
using System.Reflection;
using Confluent.Kafka;
using SportNews.Service.Kafka.Consumers;
using SportNews.Service.Kafka.Producers;
using System.Net;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
    logger.Info("Запуск сервиса спортивных новостей");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // Получаем значения конфигурации БД.
    builder.Services.Configure<DatabaseSettings>(
        builder.Configuration.GetSection("MongoDBSettings"));

    // Чтение настроек подключения к Redis
    builder.Services.Configure<RedisSettings>(
        builder.Configuration.GetSection("Redis"));

    // Добавление Redis как реализацию IDistributedCache
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost:6379";
        options.InstanceName = "RedisCacheInstance"; // Опционально, имя инстанса
    });


    // Добавляем функционал для работы с новостями
    builder.Services.AddSingleton<INewsRepository, NewsRepository>();

    // Конфигурация Kafka для сервиса новостей
    var config = new ConsumerConfig
    {
        GroupId = "news-service-group",
        BootstrapServers = "localhost:9092", 
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    var producerConfig = new ProducerConfig { BootstrapServers = "localhost:9092" };

    var producer = new ProducerBuilder<Null, string>(config).Build();



    // Регистрация консюмера и продюсера для сервиса новостей
    builder.Services.AddSingleton<IConsumer<Ignore, string>>(sp => new ConsumerBuilder<Ignore, string>(config).Build());
    builder.Services.AddSingleton<IProducer<Null, string>>(sp => new ProducerBuilder<Null, string>(producerConfig).Build());

    // Регистрация продюсера и консьюмера
    builder.Services.AddHostedService<NewsConsumerService>();
    builder.Services.AddSingleton<NewsProducerService>();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath); // Подключаем XML-документацию
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SportNewsService API V1");
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.UseStaticFiles();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Во время запуска сервиса возникло исключение");
    throw;
}
finally
{
    // Удаление всех целевых объектов и завершение ведения журнала.
    NLog.LogManager.Shutdown();
}