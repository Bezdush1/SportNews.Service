using Moq;
using SportNews.Service.Controllers;
using SportNews.Service.Models;
using SportNews.Service.UnitTests.Data;
using SportNews.Service.UnitTests.Factory;
using SportNews.Service.UnitTests.Utils;

namespace SportNews.Service.UnitTests;

/// <summary>
/// �����, ����������� ��������� ����� ��� ����������� <see cref="NewsController"/> �� 
/// ������ �����.
/// </summary>
public class NewsControllerUnitTests
{
    /// <summary>
    /// �������� ������ ��������� ���� ��������.
    /// ���������� ���������: � �� ������������ �������.
    /// ��������� ���������: ��������� �������� ������� ���������.
    /// ����������� ���������: ������� ��������, ������ 200.
    /// </summary>
    [Fact]
    public async void GetAll_DataInStorage_Success()
    {
        // Arrange
        _factory.NewsRepository.Setup(nr => nr.GetAllAsync())
            .Returns(Task.FromResult(NewsDataFactory.GetListNews()));

        var controller = _factory.Build();

        // Act
        var answer = await controller.GetAll();
        
        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetAllAsync(), Times.Once());

        ControllerAnswerExtentions.OkObjectResult<List<News>>(answer);
    }

    /// <summary>
    /// �������� ������ ��������� ���� ��������.
    /// ���������� ���������: � �� ����������� �������.
    /// ��������� ���������: ��������� �������� �� ���������.
    /// ����������� ���������: ������� �� ��������, ������ 404.
    /// </summary>
    [Fact]
    public async void GetAll_EmptyStorage_NotFound()
    {
        // Arrange
        IEnumerable<News> emptyList = new List<News>();

        _factory.NewsRepository.Setup(nr => nr.GetAllAsync())
            .Returns(Task.FromResult(emptyList));

        var controller = _factory.Build();

        // Act
        var answer = await controller.GetAll();

        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetAllAsync(), Times.Once());

        ControllerAnswerExtentions.NotFoundDetails(answer);
    }

    /// <summary>
    /// �������� ������ ��������� ���� ��������.
    /// ���������� ���������: �� ����� ���������� ������ �������� ����������.
    /// ��������� ���������: ��������� �������� �� ���������.
    /// ����������� ���������: ������� �� ��������, ������ 422.
    /// </summary>
    [Fact]
    public async void GetAll_Exception_Unprocessable()
    {
        // Arrange
        _factory.NewsRepository.Setup(nr => nr.GetAllAsync())
            .Throws(new Exception("error"));

        var controller = _factory.Build();

        // Act
        var answer = await controller.GetAll();

        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetAllAsync(), Times.Once());

        ControllerAnswerExtentions.UnprocessableEntityDetails(answer);
    }

    /// <summary>
    /// �������� ������ ��������� ������� �� ��������������.
    /// ���������� ���������: � �� ������������ �������.
    /// ��������� ���������: ��������� ������� ���������.
    /// ����������� ���������: ������� ��������, ������ 200.
    /// </summary>
    [Fact]
    public async void GetById_NewsInStorage_Success()
    {
        // Arrange
        var news = NewsDataFactory.GetNews();

        var id = news.Id;

        _factory.NewsRepository.Setup(nr=>nr.GetByIdAsync(id))
            .Returns(Task.FromResult(news));

        var controller = _factory.Build();

        // Act
        var answer = await controller.GetById(id);

        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetByIdAsync(id), Times.Once);

        ControllerAnswerExtentions.OkObjectResult<News>(answer);
    }

    /// <summary>
    /// �������� ������ ��������� ������� �� ��������������.
    /// ���������� ���������: � �� ����������� �������.
    /// ��������� ���������: ��������� ������� �� ���������.
    /// ����������� ���������: ������� �� ��������, ������ 404.
    /// </summary>
    [Fact]
    public async void GetById_EmptyStorage_NotFound()
    {
        // Arrange
        var id = "111";
        News? news = null;

        _factory.NewsRepository.Setup(nr => nr.GetByIdAsync(id))
            .Returns(Task.FromResult(news));

        var controller = _factory.Build();

        // Act
        var answer = await controller.GetById(id);

        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetByIdAsync(id), Times.Once);

        ControllerAnswerExtentions.NotFoundDetails(answer);
    }

    /// <summary>
    /// �������� ������ ��������� ������� �� ��������������.
    /// ���������� ���������: �� ����� ���������� ������ �������� ����������.
    /// ��������� ���������: ��������� ������� �� ���������.
    /// ����������� ���������: ������� �� ��������, ������ 422.
    /// </summary>
    [Fact]
    public async void GetById_Exception_Unprocessable()
    {
        // Arrange
        var id = "111";

        _factory.NewsRepository.Setup(nr => nr.GetByIdAsync(id))
            .Throws(new Exception("error"));

        var controller = _factory.Build();

        // Act
        var answer = await controller.GetById(id);

        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetByIdAsync(id), Times.Once);

        ControllerAnswerExtentions.UnprocessableEntityDetails(answer);
    }

    /// <summary>
    /// �������� ������ ���������� ������� � ��.
    /// ���������� ���������: ����� ������� �����������.
    /// ��������� ���������: ���������� ������� ���������.
    /// ����������� ���������: ������� ���������, ������ 200.
    /// </summary>
    [Fact]
    public async void AddNews_NoException_Correct()
    {
        // Arrange
        var info = NewsInfoDataFactory.GetNewsInfo();

        _factory.NewsRepository.Setup(nr => nr.AddAsync(It.IsAny<News>()));

        var controller = _factory.Build();

        // Act
        var answer = await controller.AddNews(info);

        // Assert
        _factory.NewsRepository.Verify(nr => nr.AddAsync(It.IsAny<News>()),
            Times.Once);

        ControllerAnswerExtentions.Ok(answer);
    }

    /// <summary>
    /// �������� ������ ���������� ������� � ��.
    /// ���������� ���������: �� ����� ���������� ������ �������� ����������.
    /// ��������� ���������: ���������� ������� �� ���������.
    /// ����������� ���������: ������� �� ���������, ������ 422.
    [Fact]
    public async void AddNews_Exception_Unprocessable()
    {
        // Arrange
        var info = NewsInfoDataFactory.GetNewsInfo();

        _factory.NewsRepository.Setup(nr => nr.AddAsync(It.IsAny<News>()))
            .Throws(new Exception("error"));

        var controller = _factory.Build();

        // Act
        var answer = await controller.AddNews(info);

        // Assert
        _factory.NewsRepository.Verify(nr => nr.AddAsync(It.IsAny<News>()),
            Times.Once);

        ControllerAnswerExtentions.UnprocessableEntityDetails(answer);
    }

    /// <summary>
    /// �������� ������ ���������� ������� � ��.
    /// ���������� ���������: �������, � ��������� ���������������, ������������ � ��.
    /// ��������� ���������: ���������� ������� ���������.
    /// ����������� ���������: ������� ���������, ������ 200.
    [Fact]
    public async void Update_NewsInStorage_Success()
    {
        // Arrange
        var news = NewsDataFactory.GetNews();

        _factory.NewsRepository.Setup(nr => nr.GetByIdAsync(news.Id))
            .Returns(Task.FromResult(news));

        _factory.NewsRepository.Setup(nr => nr.UpdateAsync(news.Id, It.IsAny<News>()));

        var controller = _factory.Build();

        // Act
        var answer = await controller.Update(news);

        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetByIdAsync(news.Id), Times.Once);

        _factory.NewsRepository.Verify(nr => nr.UpdateAsync(news.Id, It.IsAny<News>()), Times.Once);

        ControllerAnswerExtentions.Ok(answer);
    }

    /// <summary>
    /// �������� ������ ���������� ������� � ��.
    /// ���������� ���������: �������, � ��������� ���������������, ����������� � ��.
    /// ��������� ���������: ���������� ������� �� ���������.
    /// ����������� ���������: ������� �� ���������, ������ 404.
    [Fact]
    public async void Update_EmptyStorage_NotFound()
    {
        // Arrange
        News news = NewsDataFactory.GetNews();
        News? foundNews = null;

        _factory.NewsRepository.Setup(nr => nr.GetByIdAsync(news.Id))
            .Returns(Task.FromResult(foundNews));

        _factory.NewsRepository.Setup(nr => nr.UpdateAsync(news.Id, It.IsAny<News>()));

        var controller = _factory.Build();

        // Act
        var answer = await controller.Update(news);

        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetByIdAsync(news.Id), Times.Once);

        _factory.NewsRepository.Verify(nr => nr.UpdateAsync(news.Id, It.IsAny<News>()), Times.Never);

        ControllerAnswerExtentions.NotFoundDetails(answer);
    }

    /// <summary>
    /// �������� ������ ���������� ������� � ��.
    /// ���������� ���������: �� ����� ���������� ������ �������� ����������.
    /// ��������� ���������: ���������� ������� �� ���������.
    /// ����������� ���������: ������� �� ���������, ������ 422.
    [Fact]
    public async void Update_Exception_Unprocessable()
    {
        // Arrange
        var news = NewsDataFactory.GetNews();

        _factory.NewsRepository.Setup(nr => nr.GetByIdAsync(news.Id))
            .Throws(new Exception("error"));

        _factory.NewsRepository.Setup(nr => nr.UpdateAsync(news.Id, It.IsAny<News>()));

        var controller = _factory.Build();

        // Act
        var answer = await controller.Update(news);

        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetByIdAsync(news.Id), Times.Once);

        _factory.NewsRepository.Verify(nr => nr.UpdateAsync(news.Id, It.IsAny<News>()), Times.Never);

        ControllerAnswerExtentions.UnprocessableEntityDetails(answer);
    }

    /// <summary>
    /// �������� ������ �������� ������� �� ��.
    /// ���������� ���������: �������, � ��������� ���������������, ������������ � ��.
    /// ��������� ���������: �������� ������� ���������.
    /// ����������� ���������: ������� �������, ������ 200.
    [Fact]
    public async void Delete_NewsInStorage_Success()
    {
        // Arrange
        var news = NewsDataFactory.GetNews();

        var id = news.Id;

        _factory.NewsRepository.Setup(nr => nr.GetByIdAsync(id))
            .Returns(Task.FromResult(news));

        _factory.NewsRepository.Setup(nr => nr.DeleteAsync(id));

        var controller = _factory.Build();

        // Act
        var answer = await controller.Delete(id);

        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetByIdAsync(id), Times.Once);

        _factory.NewsRepository.Verify(nr => nr.DeleteAsync(id), Times.Once);

        ControllerAnswerExtentions.Ok(answer);
    }

    /// <summary>
    /// �������� ������ �������� ������� �� ��.
    /// ���������� ���������: �������, � ��������� ���������������, ����������� � ��.
    /// ��������� ���������: �������� ������� �� ���������.
    /// ����������� ���������: ������� �� �������, ������ 404.
    [Fact]
    public async void Delete_EmptyStorage_NotFound()
    {
        // Arrange
        News? news = null;

        var id = "111";

        _factory.NewsRepository.Setup(nr => nr.GetByIdAsync(id))
            .Returns(Task.FromResult(news));

        _factory.NewsRepository.Setup(nr => nr.DeleteAsync(id));

        var controller = _factory.Build();

        // Act
        var answer = await controller.Delete(id);

        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetByIdAsync(id), Times.Once);

        _factory.NewsRepository.Verify(nr => nr.DeleteAsync(id), Times.Never);

        ControllerAnswerExtentions.NotFoundDetails(answer);
    }

    /// <summary>
    /// �������� ������ �������� ������� �� ��.
    /// ���������� ���������: �� ����� ���������� ������ �������� ����������.
    /// ��������� ���������: �������� ������� �� ���������.
    /// ����������� ���������: ������� �� �������, ������ 422.
    [Fact]
    public async void Delete_Exception_Unprocessable()
    {
        // Arrange
        var id = "111";

        _factory.NewsRepository.Setup(nr => nr.GetByIdAsync(id))
            .Throws(new Exception("error"));

        _factory.NewsRepository.Setup(nr => nr.DeleteAsync(id));

        var controller = _factory.Build();

        // Act
        var answer = await controller.Delete(id);

        // Assert
        _factory.NewsRepository.Verify(nr => nr.GetByIdAsync(id), Times.Once);

        _factory.NewsRepository.Verify(nr => nr.DeleteAsync(id), Times.Never);

        ControllerAnswerExtentions.UnprocessableEntityDetails(answer);
    }

    /// <summary>
    /// �������� ������ �������� ���� �������� �� ��.
    /// ���������� ���������: ����� ����������� ��� ������.
    /// ��������� ���������: �������� �������� ���������.
    /// ����������� ���������: ������� �������, ������ 200.
    [Fact]
    public async void DeleteAll_NoException_Success()
    {
        // Arrange
        _factory.NewsRepository.Setup(nr => nr.DeleteAllAsync());

        var controller = _factory.Build();

        // Act
        var answer = await controller.DeleteAll();

        // Assert
        _factory.NewsRepository.Verify(nr => nr.DeleteAllAsync(), Times.Once);

        ControllerAnswerExtentions.Ok(answer);
    }

    /// <summary>
    /// �������� ������ �������� ���� �������� �� ��.
    /// ���������� ���������: �� ����� ���������� ������ �������� ����������.
    /// ��������� ���������: �������� �������� �� ���������.
    /// ����������� ���������: ������� �� �������, ������ 422.
    [Fact]
    public async void DeleteAll_Exception_Unprocessable()
    {
        // Arrange
        _factory.NewsRepository.Setup(nr => nr.DeleteAllAsync())
            .Throws(new Exception("error"));

        var controller = _factory.Build();

        // Act
        var answer = await controller.DeleteAll();

        // Assert
        _factory.NewsRepository.Verify(nr => nr.DeleteAllAsync(), Times.Once);

        ControllerAnswerExtentions.UnprocessableEntityDetails(answer);
    }


    private NewsControllerFactory _factory = new NewsControllerFactory();
}