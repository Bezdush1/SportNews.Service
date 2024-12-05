using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using SportNews.Service.DatabaseTests.CustomOrder;
using SportNews.Service.DatabaseTests.Data;
using SportNews.Service.DatabaseTests.Factory;
using SportNews.Service.Interaction.In;
using System.Diagnostics;
using Xunit.Abstractions;

namespace SportNews.Service.DatabaseTests;

/// <summary>
/// Класс, реализующий модульные тесты, предназначенные для проверки функционала БД.
/// </summary>
[TestCaseOrderer("FullNameOfOrderStrategyHere", "OrderStrategyAssemblyName")]
public class DatabaseTests
{
    /// <summary>
    /// Конструктор класса.
    /// </summary>
    /// <param name="output">Объект для вывода сообщений в консоль.</param>
    public DatabaseTests(ITestOutputHelper output)
    {
        // Настройка кэша (DistributedMemoryCache)
        var services = new ServiceCollection();
        services.AddDistributedMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        _cache = serviceProvider.GetRequiredService<IDistributedCache>();

        _output = output;
    }

    /// <summary>
    /// Добавление 100 записей в БД.
    /// </summary>
    /// <remarks>Тесты, выполняются в порядке приоритетов.</remarks>
    //[Fact, TestPriority(1)]
    public async Task AddNews_Iteration_100_Success()
    {
        await AddNews_Iteration_Success(100);
    }

    /// <summary>
    /// Удаление записей из БД.
    /// </summary>
    /// <remarks>Тесты, выполняются в порядке приоритетов.</remarks>
    //[Fact, TestPriority(2)]
    public async Task DeleteAll_After_100()
    {
        await DeleteAll_Should_Remove_All_News();
    }

    /// <summary>
    /// Добавление 100000 записей в БД.
    /// </summary>
    /// <remarks>Тесты, выполняются в порядке приоритетов.</remarks>
    [Fact, TestPriority(3)]
    public async Task AddNews_Iteration_100000_Success()
    {
        await AddNews_Iteration_Success(100000);
    }

    /// <summary>
    /// Удаление записей из БД.
    /// </summary>
    /// <remarks>Тесты, выполняются в порядке приоритетов.</remarks>
    [Fact, TestPriority(4)]
    public async Task DeleteAll_After_100000()
    {
        await DeleteAll_Should_Remove_All_News();
    }

    /// <summary>
    /// Очистка кэша перед получением данных из БД.
    /// </summary>
    /// <remarks>Тесты, выполняются в порядке приоритетов.</remarks>
    //[Fact, TestPriority(5)]
    public async Task ClearCache()
    {
        await Clear_Cache();
    }

    /// <summary>
    /// Получение новостей из БД.
    /// </summary>
    /// <remarks>
    /// Тесты, выполняются в порядке приоритетов.
    /// Метод дважды берет новости. Сначала из БД, потом из кэша.
    /// </remarks>
    [Fact, TestPriority(6)]
    public async Task GetById_TimeIt()
    {
        await GetById_News();
    }


    /// <summary>
    /// Тестирование БД на добавление в нее <paramref name="iteration"/> новостей.
    /// </summary>
    /// <param name="iteration">Количество итераций.</param>
    private async Task AddNews_Iteration_Success(int iteration)
    {
        // Arrange
        var newsList = new List<NewsInfo>();
        for (int i = 0; i < iteration; i++)
        {
            newsList.Add(NewsInfoDataFactory.GetNews(i));
        }

        var controller = _factory.Build();

        // Act
        foreach (var news in newsList)
        {
            await controller.AddNews(news);
        }

        // Assert
        var addedNews = await _factory.NewsRepository.GetAllAsync();
        Assert.Equal(iteration, addedNews.Count());
    }

    /// <summary>
    /// Удаление всех записей из БД.
    /// </summary>
    private async Task DeleteAll_Should_Remove_All_News()
    {
        // Arrange
        var controller = _factory.Build();

        // Act
        await controller.DeleteAll();

        // Assert
        var allNews = await _factory.NewsRepository.GetAllAsync();
        Assert.Empty(allNews);
    }

    /// <summary>
    /// Получение всех новостей.
    /// </summary>
    private async Task GetById_News()
    {
        // Arrange
        var controller = _factory.Build(_cache);

        // Act
        // Первый запрос - должен взять данные из БД и занести в кэш
        Stopwatch sw = new Stopwatch();
        sw.Start();
        await controller.GetById("6701194d673eddfef930eaa8");
        sw.Stop();
        var firstCallDuration = sw.ElapsedMilliseconds;
        // Выводим время выполнения второго запроса (из кэша)
        _output.WriteLine($"Первое обращение заняло {firstCallDuration} мс (из БД)");

        // Второй запрос - данные должны быть уже в кэше
        sw.Restart();
        await controller.GetById("6701194d673eddfef930eaa8");
        sw.Stop();
        var secondCallDuration = sw.ElapsedMilliseconds;
        // Выводим время выполнения второго запроса (из кэша)
        _output.WriteLine($"Второе обращение заняло {secondCallDuration} мс (из кэша)");

        // Assert
        Assert.True(secondCallDuration < firstCallDuration,
            $"Второе обращение должно было быть быстрее, чем первое. Второе заняло {secondCallDuration} мс," +
            $" а первое {firstCallDuration} мс.");

    }

    /// <summary>
    /// Очистка кэша перед получением данных из БД.
    /// </summary>
    private async Task Clear_Cache()
    {
        await _cache.RemoveAsync("all_news");
    }


    private DatabaseTestsFactory _factory = new DatabaseTestsFactory();

    private readonly IDistributedCache _cache;
    private readonly ITestOutputHelper _output;
}