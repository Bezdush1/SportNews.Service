using SportNews.Service.Models;

namespace SportNews.Service.Repositories.Interfases;

/// <summary>
/// Интерфейс, предоставляющий функционал для работы с новостями.
/// </summary>
public interface INewsRepository
{
    /// <summary>
    /// Получения всех новостей.
    /// </summary>
    /// <returns>Список новостей.</returns>
    Task<IEnumerable<News>> GetAllAsync();

    /// <summary>
    /// Получение новости по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор.</param>
    /// <returns>Новость.</returns>
    Task<News?> GetByIdAsync(string id);

    /// <summary>
    /// Добавление новости.
    /// </summary>
    /// <param name="news">Новость.</param>
    Task AddAsync(News news);

    /// <summary>
    /// Обновление полей новости, с указанным идентификатором.
    /// </summary>
    /// <param name="id">Идентификатор.</param>
    /// <param name="news">Новая новость.</param>
    Task UpdateAsync(string id, News news);

    /// <summary>
    /// Удаление новости по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор.</param>
    Task DeleteAsync(string id);

    /// <summary>
    /// Удаление всех новостей.
    /// </summary>
    Task DeleteAllAsync();
}
