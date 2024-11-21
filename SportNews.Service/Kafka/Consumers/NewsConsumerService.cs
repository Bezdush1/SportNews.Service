using Confluent.Kafka;
using KafkaConstants;
using SportNews.Service.Repositories.Interfases;
using System.Text.Json;

namespace SportNews.Service.Kafka.Consumers;

/// <summary>
/// Сервис Kafka-консюмера для обработки подтверждений от сервера пользователей.
/// </summary>
public class NewsConsumerService : IHostedService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly INewsRepository _newsRepository;
    private readonly ILogger<NewsConsumerService> _logger;

    /// <summary>
    /// Конструктор класса.
    /// </summary>
    /// <param name="consumer">Объект kafka-консьюмер.</param>
    /// <param name="newsRepository">Объект для взаимодействия с БД.</param>
    /// <param name="logger">Объект для ведения записей.</param>
    public NewsConsumerService(
        IConsumer<Ignore, string> consumer,
        INewsRepository newsRepository,
        ILogger<NewsConsumerService> logger)
    {
        _consumer = consumer;
        _newsRepository = newsRepository;
        _logger = logger;
    }

    /// <summary>
    /// Запуск сервиса консюмера.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => ConsumeMessagesAsync(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Обработка сообщений из Kafka.
    /// </summary>
    private async Task ConsumeMessagesAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(KafkaTopicsConstants.ConfirmationTopic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(cancellationToken);
                _logger.LogInformation($"Получено сообщение: {result.Message.Value}");

                var confirmationMessage = JsonSerializer.Deserialize<ConfirmationMessage>(result.Message.Value);

                if (confirmationMessage == null)
                {
                    _logger.LogWarning("Сообщение не удалось десериализовать.");
                    continue;
                }

                // Получение новости из базы данных
                var news = await _newsRepository.GetByIdAsync(confirmationMessage.ObjectId);
                if (news == null)
                {
                    _logger.LogWarning($"Новость с идентификатором {confirmationMessage.ObjectId} не найдена.");
                    continue;
                }

                // Обновление времени новости
                news.PublishedAt = DateTime.Parse(confirmationMessage.ConfirmationTimestamp);

                // Сохранение изменений
                await _newsRepository.UpdateAsync(news.Id, news);

                _logger.LogInformation($"Новость с ID {news.Id} обновлена с новой датой: {news.PublishedAt}");
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

