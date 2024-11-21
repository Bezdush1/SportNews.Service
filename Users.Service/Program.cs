using Confluent.Kafka;
using System.Reflection;
using Users.Service.Kafka.Consumers;
using Users.Service.Kafka.Produsers;
using Users.Service.Repositories;
using Users.Service.Repositories.Interfases;
using Users.Service.Settings;
using NLog.Web;
using NLog;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
    logger.Info("Запуск сервиса пользователей");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // Получаем значения конфигурации БД.
    builder.Services.Configure<DatabaseSettings>(
        builder.Configuration.GetSection("MongoDBSettings"));

    // Добавляем функционал для работы с пользователями.
    builder.Services.AddSingleton<IUserRepository, UserRepository>();

    var config = new ConsumerConfig
    {
        GroupId = "user-service-group",
        BootstrapServers = "localhost:9092",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    var producerConfig = new ProducerConfig { BootstrapServers = "localhost:9092" };

    builder.Services.AddSingleton<IConsumer<Ignore, string>>(sp => new ConsumerBuilder<Ignore, string>(config).Build());
    builder.Services.AddSingleton<IProducer<Null, string>>(sp => new ProducerBuilder<Null, string>(producerConfig).Build());

    builder.Services.AddHostedService<UserConsumerService>();
    builder.Services.AddSingleton<UserProducerService>();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath); // Подключаем XML-документацию
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SportsNewsService API V1");
        });
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

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
