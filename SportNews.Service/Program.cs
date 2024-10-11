using SportNews.Service.Repositories.Interfases;
using SportNews.Service.Repositories;
using SportNews.Service.Settings;
using NLog.Web;
using NLog;
using System.Reflection;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
    logger.Info("������ ������� ���������� ��������");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // �������� �������� ������������ ��.
    builder.Services.Configure<DatabaseSettings>(
        builder.Configuration.GetSection("MongoDBSettings"));

    // ������ �������� ����������� � Redis
    builder.Services.Configure<RedisSettings>(
        builder.Configuration.GetSection("Redis"));

    // ���������� Redis ��� ���������� IDistributedCache
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost:6379";
        options.InstanceName = "RedisCacheInstance"; // �����������, ��� ��������
    });


    // ��������� ���������� ��� ������ � ���������.
    builder.Services.AddSingleton<INewsRepository, NewsRepository>();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath); // ���������� XML-������������
    });

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

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.UseStaticFiles();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "�� ����� ������� ������� �������� ����������");
    throw;
}
finally
{
    // �������� ���� ������� �������� � ���������� ������� �������.
    NLog.LogManager.Shutdown();
}