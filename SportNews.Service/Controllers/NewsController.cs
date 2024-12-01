using KafkaConstants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using SportNews.Service.Interaction.In;
using SportNews.Service.Kafka.Producers;
using SportNews.Service.Models;
using SportNews.Service.Repositories.Interfases;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Utils;

namespace SportNews.Service.Controllers;

/// <summary>
/// Класс, предоставляющий функционал для работы со спортивными новостями.
/// </summary>
[ApiController]
[Route("api/news")]
public class NewsController : ControllerBase
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="newsRepository">Объект для работы с новостями.</param>
    /// <param name="logger">Объект для ведения записей.</param>
    /// <param name="cache">Объект для работы с кэшем.</param>
    /// <param name="newsProducerService">Объект для взаимодействия с сервисом пользователей.</param>
    public NewsController(INewsRepository newsRepository, ILogger<NewsController> logger,
        IDistributedCache cache, NewsProducerService newsProducerService)
    {
        _newsRepository = newsRepository;
        _logger = logger;
        _cache = cache;
        _newsProducerService = newsProducerService;
    }

    /// <summary>
    /// Получение всех новостей из БД.
    /// </summary>
    /// <response code="200">Успешное получение новостей.</response>
    /// <response code="404">В БД отсутствуют новости.</response>
    /// <response code="422">Во время выполнения метода возникло исключение.</response>
    /// <returns>Список новостей.</returns>
    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<News>),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            _logger.LogInformation("Получен запрос на получение всех новостей");

            var news = await _newsRepository.GetAllAsync();

            if(news.Count() == 0)
            {
                const string Msg = "Новости отстутствуют";
                _logger.LogTrace(Msg);
                return this.NotFoundDetails(Msg);
            }

            _logger.LogInformation("Все новости успешно получены");
            return Ok(news);
        }
        catch (Exception ex)
        {
            const string Msg = "Во время получения всех новостей, произошла ошибка";
            _logger.LogError(ex, Msg);
            return this.UnprocessableEntityDetails(Msg);
        }
    }

    /// <summary>
    /// Получение новости по указанному идентификатору.
    /// </summary>
    /// <param name="id">Идеентификатор.</param>
    /// <response code="200">Успешное получение новостей.</response>
    /// <response code="404">В БД отсутствуют новости.</response>
    /// <response code="422">Во время выполнения метода возникло исключение.</response>
    /// <returns>Новость.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(News), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetById([Required] string id)
    {
        try
        {
            _logger.LogInformation($"Получен запрос на получение новости по идентификатору: {id}");

            var cacheKey = $"news_{id}";
            News? news;

            // Получаем данные из кэша
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Если данные найдены в кэше, десериализуем их
                news = JsonSerializer.Deserialize<News>(cachedData!);
            }
            else
            {
                // Если данные не найдены в кэше, получаем их из базы данных
                news = await _newsRepository.GetByIdAsync(id);

                if (news == null)
                {
                    string msg = $"Новость с идентификатором {id} отсутствует";
                    _logger.LogTrace(msg);
                    return this.NotFoundDetails(msg);
                }

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(news));
            }

            _logger.LogInformation($"Новость с идентификатором: {id} успешно получена");
            return Ok(news);
        }
        catch (Exception ex)
        {
            string msg = $"Во время получения новости с идентификатором {id}, произошла ошибка";
            _logger.LogError(ex, msg);
            return this.UnprocessableEntityDetails(msg);
        }
    }


    /// <summary>
    /// Добавление новости.
    /// </summary>
    /// <param name="news">Новость.</param>
    /// <response code="200">Успешное добавление новости.</response>
    /// <response code="422">Во время выполнения метода возникло исключение.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddNews([Required] NewsInfo news)
    {
        try
        {
            _logger.LogInformation("Получен запрос на добавление новости");

            var newNews = new News
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = news.Title,
                Content = news.Content,
                Category = news.Category,
                PublishedAt = news.PublishedAt,
            };

            await _newsRepository.AddAsync(newNews);

            await _cache.SetStringAsync($"news_{newNews.Id}", JsonSerializer.Serialize(newNews));

            // Отправляем сообщение в микросервис пользователей
            await _newsProducerService.SendRegistrationRequestAsync(newNews.Id, "67372df1077cd2c1072a883b");

            _logger.LogInformation("Новость успешно добавлена");
            return Ok();
        }
        catch(Exception ex)
        {
            string msg = $"Во время добавления новости, произошла ошибка";
            _logger.LogError(ex, msg);
            return this.UnprocessableEntityDetails(msg);
        }
        
    }

    /// <summary>
    /// Обновление новости, по указанному идентификатору.
    /// </summary>
    /// <param name="news">Данные новости.</param>
    /// <response code="200">Успешное обновление новости, с указанным идентификатором.</response>
    /// <response code="404">В БД отсутствует новость, с указанным идентификатором.</response>
    /// <response code="422">Во время выполнения метода возникло исключение.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([Required] News news)
    {
        try
        {
            _logger.LogInformation($"Получен запрос на обновление новости с идентификатором: {news.Id}");

            var existingNews = await _newsRepository.GetByIdAsync(news.Id);

            if (existingNews == null)
            {
                string msg = "Новость с идентификатором {id} отстутствует";
                _logger.LogTrace(msg);
                return this.NotFoundDetails(msg);
            }

            var newNews = new News
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                Category = news.Category,
                PublishedAt = news.PublishedAt
            };

            await _newsRepository.UpdateAsync(news.Id, newNews);

            // Удаление только кэша для данной новости
            await _cache.RemoveAsync($"news_{news.Id}");
            await _cache.SetStringAsync($"news_{news.Id}", JsonSerializer.Serialize(news));

            _logger.LogInformation($"Новость с идентификатором: {news.Id} успешно обновлена");
            return Ok();
        }
        catch(Exception ex)
        {
            string msg = $"Во время обновления новости с идентификатором {news.Id}, произошла ошибка";
            _logger.LogError(ex, msg);
            return this.UnprocessableEntityDetails(msg);
        }        
    }

    /// <summary>
    /// Удаление новости, по указанному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор.</param>
    /// <response code="200">Успешное удаление новости, с указанным идентификатором.</response>
    /// <response code="404">В БД отсутствует новость, с указанным идентификатором.</response>
    /// <response code="422">Во время выполнения метода возникло исключение.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Delete([Required] string id)
    {
        try
        {
            _logger.LogInformation($"Получен запрос на удаление новости с идентификатором: {id}");

            var existingNews = await _newsRepository.GetByIdAsync(id);

            if (existingNews == null)
            {
                string msg = "Новость с идентификатором {id} отстутствует";
                _logger.LogTrace(msg);
                return this.NotFoundDetails(msg);
            }

            await _newsRepository.DeleteAsync(id);

            // Удаление кэша для данной новости
            await _cache.RemoveAsync($"news_{id}");

            _logger.LogInformation($"Новость с идентификатором: {id} успешно удалена");
            return Ok();
        }
        catch(Exception ex)
        {
            string msg = $"Во время удаления новости с идентификатором {id}, произошла ошибка";
            _logger.LogError(ex, msg);
            return this.UnprocessableEntityDetails(msg);
        }        
    }

    /// <summary>
    /// Удаление всех новостей.
    /// </summary>
    /// <response code="200">Успешное удаление новости, с указанным идентификатором.</response>
    /// <response code="422">Во время выполнения метода возникло исключение.</response>
    [HttpDelete("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteAll()
    {
        try
        {
            _logger.LogInformation("Получен запрос на удаление всех записей");

            await _newsRepository.DeleteAllAsync();

            // Очистка кэша после удаления всех новостей
            await _cache.RemoveAsync("all_news");

            _logger.LogInformation("Все записи успешно удалены");
            return Ok();
        }
        catch (Exception ex)
        {
            string msg = $"Во время удаления всвех новостей, произошла ошибка";
            _logger.LogError(ex, msg);
            return this.UnprocessableEntityDetails(msg);
        }
    }

    /// <summary>
    /// Обновление метки подтверждения новости.
    /// </summary>
    /// <param name="request">Структура, подтверждающая создание новости.</param>
    /// <response code="200">Успешное обновление новости, с указанным идентификатором.</response>
    /// <response code="404">В БД отсутствует новость, с указанным идентификатором.</response>
    /// <response code="422">Во время выполнения метода возникло исключение.</response>
    [HttpPost("update-timestamp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateTimestamp([FromBody] ConfirmationMessage request)
    {
        try
        {
            _logger.LogInformation($"Получен запрос на обновление времени для новости: {request.ObjectId}");

            var news = await _newsRepository.GetByIdAsync(request.ObjectId);

            if (news == null)
            {
                string msg = $"Новость с идентификатором {request.ObjectId} отсутствует";
                _logger.LogTrace(msg);
                return this.NotFoundDetails(msg);
            }

            news.PublishedAt = DateTime.Parse(request.ConfirmationTimestamp);

            await _newsRepository.UpdateAsync(news.Id, news);

            _logger.LogInformation($"Новость {request.ObjectId} успешно обновлена с новым временем: {request.ConfirmationTimestamp}");
            return Ok();
        }
        catch (Exception ex)
        {
            string msg = $"Во время обновления времени новости {request.ObjectId}, произошла ошибка";
            _logger.LogError(ex, msg);
            return this.UnprocessableEntityDetails(msg);
        }
    }


    private readonly INewsRepository _newsRepository;

    private readonly ILogger<NewsController> _logger;

    private readonly IDistributedCache _cache;

    private readonly NewsProducerService _newsProducerService;
}
