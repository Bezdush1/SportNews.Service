using Confluent.Kafka;
using KafkaConstants;
using System.Text.Json;
using Users.Service.Kafka.Produsers;
using Users.Service.Repositories.Interfases;

namespace Users.Service.Kafka.Consumers;

/// <summary>
/// Сервис Kafka-консюмера для обработки подтверждений от сервера пользователей.
/// </summary>
public class UserConsumerService : IHostedService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IUserRepository _userRepository;
    private readonly UserProducerService _producerService;
    private readonly ILogger<UserConsumerService> _logger;

    /// <summary>
    /// Конструктор класса.
    /// </summary>
    /// <param name="consumer">Объект kafka-консьюмер.</param>
    /// <param name="userRepository">Объект для взаимодействия с БД.</param>
    /// <param name="producerService">Объект kafka-продюсер.</param>
    /// <param name="logger">Объект для ведения записей.</param>
    public UserConsumerService(IConsumer<Ignore, string> consumer,
        IUserRepository userRepository, UserProducerService producerService,
        ILogger<UserConsumerService> logger)
    {
        _consumer = consumer;
        _userRepository = userRepository;
        _producerService = producerService;
        _logger = logger;
    }


    /// <summary>
    /// Запуск сервиса консюмера.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => ConsumeMessages(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ConsumeMessages(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(KafkaTopicsConstants.ObjectServiceTopic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(cancellationToken);
                _logger.LogInformation($"Получено сообщение: {result.Message.Value}");

                var message = JsonSerializer.Deserialize<NewsProcessMessage>(result.Message.Value);

                if (message == null)
                {

                    _logger.LogWarning("Сообщение не удалось десериализовать.");
                    continue;
                }

                var user = await _userRepository.GetUserByIdAsync(message.UserId);

                if (user == null)
                {
                    _logger.LogWarning($"пользователь с идентификатором {message.UserId} не найдена.");
                    continue;
                }

                user.RegisteredObjects++;
                await _userRepository.UpdateUserAsync(message.UserId, user);

                await _producerService.SendConfirmation(message.ObjectId, DateTime.UtcNow.ToString());

                _logger.LogInformation($"Пользователь с идентификатором {message.UserId}, подтвердил новость " +
                    $"с идентификатором {message.ObjectId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке сообщения из Kafka.");
            }
        }
    }

    /// <summary>
    /// Остановка сервиса консюмера.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Close();
        return Task.CompletedTask;
    }
}
