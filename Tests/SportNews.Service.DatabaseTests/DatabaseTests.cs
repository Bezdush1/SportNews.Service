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
/// �����, ����������� ��������� �����, ��������������� ��� �������� ����������� ��.
/// </summary>
[TestCaseOrderer("FullNameOfOrderStrategyHere", "OrderStrategyAssemblyName")]
public class DatabaseTests
{
    /// <summary>
    /// ����������� ������.
    /// </summary>
    /// <param name="output">������ ��� ������ ��������� � �������.</param>
    public DatabaseTests(ITestOutputHelper output)
    {
        // ��������� ���� (DistributedMemoryCache)
        var services = new ServiceCollection();
        services.AddDistributedMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        _cache = serviceProvider.GetRequiredService<IDistributedCache>();

        _output = output;
    }

    /// <summary>
    /// ���������� 100 ������� � ��.
    /// </summary>
    /// <remarks>�����, ����������� � ������� �����������.</remarks>
    //[Fact, TestPriority(1)]
    public async Task AddNews_Iteration_100_Success()
    {
        await AddNews_Iteration_Success(100);
    }

    /// <summary>
    /// �������� ������� �� ��.
    /// </summary>
    /// <remarks>�����, ����������� � ������� �����������.</remarks>
    //[Fact, TestPriority(2)]
    public async Task DeleteAll_After_100()
    {
        await DeleteAll_Should_Remove_All_News();
    }

    /// <summary>
    /// ���������� 100000 ������� � ��.
    /// </summary>
    /// <remarks>�����, ����������� � ������� �����������.</remarks>
    [Fact, TestPriority(3)]
    public async Task AddNews_Iteration_100000_Success()
    {
        await AddNews_Iteration_Success(100000);
    }

    /// <summary>
    /// �������� ������� �� ��.
    /// </summary>
    /// <remarks>�����, ����������� � ������� �����������.</remarks>
    [Fact, TestPriority(4)]
    public async Task DeleteAll_After_100000()
    {
        await DeleteAll_Should_Remove_All_News();
    }

    /// <summary>
    /// ������� ���� ����� ���������� ������ �� ��.
    /// </summary>
    /// <remarks>�����, ����������� � ������� �����������.</remarks>
    //[Fact, TestPriority(5)]
    public async Task ClearCache()
    {
        await Clear_Cache();
    }

    /// <summary>
    /// ��������� �������� �� ��.
    /// </summary>
    /// <remarks>
    /// �����, ����������� � ������� �����������.
    /// ����� ������ ����� �������. ������� �� ��, ����� �� ����.
    /// </remarks>
    [Fact, TestPriority(6)]
    public async Task GetById_TimeIt()
    {
        await GetById_News();
    }


    /// <summary>
    /// ������������ �� �� ���������� � ��� <paramref name="iteration"/> ��������.
    /// </summary>
    /// <param name="iteration">���������� ��������.</param>
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
    /// �������� ���� ������� �� ��.
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
    /// ��������� ���� ��������.
    /// </summary>
    private async Task GetById_News()
    {
        // Arrange
        var controller = _factory.Build(_cache);

        // Act
        // ������ ������ - ������ ����� ������ �� �� � ������� � ���
        Stopwatch sw = new Stopwatch();
        sw.Start();
        await controller.GetById("6701194d673eddfef930eaa8");
        sw.Stop();
        var firstCallDuration = sw.ElapsedMilliseconds;
        // ������� ����� ���������� ������� ������� (�� ����)
        _output.WriteLine($"������ ��������� ������ {firstCallDuration} �� (�� ��)");

        // ������ ������ - ������ ������ ���� ��� � ����
        sw.Restart();
        await controller.GetById("6701194d673eddfef930eaa8");
        sw.Stop();
        var secondCallDuration = sw.ElapsedMilliseconds;
        // ������� ����� ���������� ������� ������� (�� ����)
        _output.WriteLine($"������ ��������� ������ {secondCallDuration} �� (�� ����)");

        // Assert
        Assert.True(secondCallDuration < firstCallDuration,
            $"������ ��������� ������ ���� ���� �������, ��� ������. ������ ������ {secondCallDuration} ��," +
            $" � ������ {firstCallDuration} ��.");

    }

    /// <summary>
    /// ������� ���� ����� ���������� ������ �� ��.
    /// </summary>
    private async Task Clear_Cache()
    {
        await _cache.RemoveAsync("all_news");
    }


    private DatabaseTestsFactory _factory = new DatabaseTestsFactory();

    private readonly IDistributedCache _cache;
    private readonly ITestOutputHelper _output;
}