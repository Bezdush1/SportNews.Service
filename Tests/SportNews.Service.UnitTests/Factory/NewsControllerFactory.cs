using Microsoft.Extensions.Logging;
using Moq;
using SportNews.Service.Controllers;
using SportNews.Service.Repositories.Interfases;

namespace SportNews.Service.UnitTests.Factory;

/// <summary>
/// Класс, предоствляющий необходимые данные для проверки функционала контроллера <see cref="NewsController"/>.
/// </summary>
public class NewsControllerFactory
{
    /// <summary>
    /// Объект, для ведения записей.
    /// </summary>
    public Mock<ILogger<NewsController>> Logger { get; set; } = new Mock<ILogger<NewsController>>();

    /// <summary>
    /// Объект, реализующий функционал взаимодействия с БД.
    /// </summary>
    public Mock<INewsRepository> NewsRepository { get; set; } = new Mock<INewsRepository>();


    /// <summary>
    /// Создание объекта контроллера.
    /// </summary>
    /// <returns></returns>
    public NewsController Build()
    {
        return new NewsController(NewsRepository.Object, Logger.Object);
    }
}
