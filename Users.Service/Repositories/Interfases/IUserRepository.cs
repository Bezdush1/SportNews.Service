using MongoDB.Bson;
using Users.Service.Models;

namespace Users.Service.Repositories.Interfases;

/// <summary>
/// Интерфейс, предоставляющий функционал для работы с пользователями.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Получение пользователя по указанному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор.</param>
    /// <returns>Пользователь.</returns>
    Task<User?> GetUserByIdAsync(string id);

    /// <summary>
    /// Добавить пользователя в БД.
    /// </summary>
    /// <param name="user">Данные пользователя.</param>
    Task AddUserAsync(User user);

    /// <summary>
    /// Обновление данных о пользователе, по указанному идентификатору.
    /// </summary>
    /// <param name="user">Данные пользователя.</param>
    /// <param name="id">Идентификатор.</param>
    Task UpdateUserAsync(string id, User user);

    /// <summary>
    /// Удаление пользователя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор.</param>
    Task DeleteUserAsync(string id);
}
