using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Users.Service.Models;
using Users.Service.Repositories.Interfases;
using Users.Service.Kafka.Produsers;
using KafkaConstants;
using Utils;

namespace Users.Service.Controllers;

/// <summary>
/// Контроллер для управления пользователями.
/// </summary>
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly UserProducerService _producerService;
    private readonly ILogger<UserController> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="repository">Репозиторий для работы с пользователями.</param>
    /// <param name="producerService">Kafka-продюсер для отправки сообщений.</param>
    /// <param name="logger">Объект для ведения записей.</param>
    public UserController(IUserRepository repository, UserProducerService producerService,
        ILogger<UserController> logger)
    {
        _repository = repository;
        _producerService = producerService;
        _logger = logger;
    }

    /// <summary>
    /// Создание нового пользователя.
    /// </summary>
    /// <param name="user">Данные пользователя.</param>
    /// <response code="200">Успешное добавление пользователя.</response>
    /// <response code="422">Во время выполнения метода возникло исключение.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        try
        {
            _logger.LogInformation("Получен запрос на создания пользователя");

            user.Id = ObjectId.GenerateNewId().ToString();
            await _repository.AddUserAsync(user);

            // Отправка подтверждения в Kafka
            await _producerService.SendConfirmation(user.Id, DateTime.Now.ToString());

            _logger.LogInformation("Пользователь успешно создан");
            return Ok();
        }
        catch (Exception ex)
        {
            string msg = $"Во время добавления пользователя, произошла ошибка";
            _logger.LogError(ex, msg);
            return this.UnprocessableEntityDetails(msg);
        }
    }

    /// <summary>
    /// Получение пользователя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <response code="200">Успешное получение пользователя.</response>
    /// <response code="404">В БД отсутствует пользователь.</response>
    /// <response code="422">Во время выполнения метода возникло исключение.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetUser(string id)
    {
        try
        {
            _logger.LogInformation($"Получен запрос на получение пользователя с идентификатором {id}");

            var user = await _repository.GetUserByIdAsync(id);

            if (user == null)
            {
                string msg = $"Пользователь с идентификатором {id} отсутствует";
                _logger.LogTrace(msg);
                return this.NotFoundDetails(msg);
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            string msg = $"Во время получения пользователя с идентификатором {id}, произошла ошибка";
            _logger.LogError(ex, msg);
            return this.UnprocessableEntityDetails(msg);
        }
    }

    /// <summary>
    /// Удаление пользователя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <response code="200">Успешное удаление пользователя, с указанным идентификатором.</response>
    /// <response code="404">В БД отсутствует пользователь, с указанным идентификатором.</response>
    /// <response code="422">Во время выполнения метода возникло исключение.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            _logger.LogInformation($"Получен запрос на удаление пользователя с идентификатором {id}");

            var user = await _repository.GetUserByIdAsync(id);

            if (user == null)
            {
                string msg = $"Пользователь с идентификатором {id} отсутствует";
                _logger.LogTrace(msg);
                return this.NotFoundDetails(msg);
            }

            await _repository.DeleteUserAsync(id);

            _logger.LogInformation($"Пользователь с идентификатором {id} успешно удален");
            return Ok();
        }
        catch (Exception ex)
        {
            string msg = $"Во время удаления пользователя с идентификатором {id}, произошла ошибка";
            _logger.LogError(ex, msg);
            return this.UnprocessableEntityDetails(msg);
        }
    }

    /// <summary>
    /// Подтверждение новости.
    /// </summary>
    /// <param name="request">Структура с данными, для подтверждения новости.</param>
    /// <response code="200">Успешное подтверждение новости.</response>
    /// <response code="404">В БД отсутствует пользователь, необходимый для потверждения.</response>
    /// <response code="422">Во время выполнения метода возникло исключение.</response>
    [HttpPost("process-news")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ProcessNews([FromBody] NewsProcessMessage request)
    {
        try
        {
            _logger.LogInformation("Получен запрос на подтверждение новости");

            var user = await _repository.GetUserByIdAsync(request.UserId);

            if (user == null)
            {
                string msg = $"Пользователь с идентификатором {request.UserId} отсутствует";
                _logger.LogTrace(msg);
                return this.NotFoundDetails(msg);
            }

            user.RegisteredObjects++;
            await _repository.UpdateUserAsync(user.Id, user);

            // Отправка в сервис новостей
            await _producerService.SendConfirmation(request.NewsId, DateTime.UtcNow.ToString());

            _logger.LogInformation("Новость успешно подтверждена");
            return Ok();
        }
        catch (Exception ex)
        {
            string msg = $"Во время подтверждения новости, произошла ошибка";
            _logger.LogError(ex, msg);
            return this.UnprocessableEntityDetails(msg);
        }
    }
}
