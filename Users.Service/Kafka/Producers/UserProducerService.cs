using Confluent.Kafka;
using KafkaConstants;
using System.Text.Json;

namespace Users.Service.Kafka.Produsers;

/// <summary>
/// Класс, представляющий kafka-продюсера,
/// для отправления запроса на подтверждения новости.
/// </summary>
public class UserProducerService
{
    private readonly IProducer<Null, string> _producer;

    /// <summary>
    /// Конструктор класса.
    /// </summary>
    /// <param name="producer">Объект kafka-продюсер.</param>
    public UserProducerService(IProducer<Null, string> producer)
    {
        _producer = producer;
    }

    /// <summary>
    /// Отправка запроса в сервис новостей 
    /// для потверждения создания новости.
    /// </summary>
    /// <param name="newsId">Идентификатор новости.</param>
    /// <param name="timestamp">Время подтверждения.</param>
    public async Task SendConfirmation(string newsId, string timestamp)
    {
        var message = new { ObjectId = newsId, ConfirmationTimestamp = timestamp };
        var serializedMessage = JsonSerializer.Serialize(message);

        await _producer.ProduceAsync(KafkaTopicsConstants.ConfirmationTopic,
            new Message<Null, string> { Value = serializedMessage });
    }
}