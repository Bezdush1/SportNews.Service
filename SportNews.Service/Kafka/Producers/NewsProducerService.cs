using Confluent.Kafka;
using KafkaConstants;
using System.Text.Json;

namespace SportNews.Service.Kafka.Producers;

/// <summary>
/// Класс, представляющий kafka-продюсера,
/// для отправления запроса на подтверждения новости.
/// </summary>
public class NewsProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<NewsProducerService> _logger;

    /// <summary>
    /// Конструктор класса.
    /// </summary>
    /// <param name="producer">Объект kafka-продюсер.</param>
    /// <param name="logger">Объект для ведения записей.</param>
    public NewsProducerService(IProducer<Null, string> producer, ILogger<NewsProducerService> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    /// <summary>
    /// Отправка запроса в сервис пользователей 
    /// для потверждения создания новости.
    /// </summary>
    /// <param name="objectId">Идентификатор новости.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    public async Task SendRegistrationRequestAsync(string objectId, string userId)
    {
        var message = new { ObjectId = objectId, UserId = userId };
        var serializedMessage = JsonSerializer.Serialize(message);

        try
        {
            var result = await _producer.ProduceAsync(KafkaTopicsConstants.ObjectServiceTopic,
                new Message<Null, string> { Value = serializedMessage });
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex.Error.Reason);
        }
    }
}
