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

    /// <summary>
    /// Конструктор класса.
    /// </summary>
    /// <param name="producer">Объект kafka-продюсер.</param>
    public NewsProducerService(IProducer<Null, string> producer)
    {
        _producer = producer;
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

        await _producer.ProduceAsync(KafkaTopicsConstants.ObjectServiceTopic,
            new Message<Null, string> { Value = serializedMessage });
    }
}
