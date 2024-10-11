using Microsoft.AspNetCore.Mvc;

namespace SportNews.Service.Utils;

/// <summary>
/// Вспомогательный класс, для предоставления статуса ошибки
/// с пояснительным сообщением.
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Возвращает http-статус 404 с описанием <paramref name="details"/>.
    /// </summary>
    /// <param name="controller">Контроллер.</param>
    /// <param name="details">Подробное описание проблемы.</param>
    /// <returns>Результат.</returns>
    public static IActionResult NotFoundDetails(this ControllerBase controller, string details)
    {
        return controller.Problem(details, statusCode: StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Возвращает http-статус 422 с описанием <paramref name="details"/>.
    /// </summary>
    /// <param name="controller">Контроллер.</param>
    /// <param name="details">Подробное описание проблемы.</param>
    /// <returns>Результат.</returns>
    public static IActionResult UnprocessableEntityDetails(this ControllerBase controller, string details)
    {
        return controller.Problem(details, statusCode: StatusCodes.Status422UnprocessableEntity);
    }

    /// <summary>
    /// Возвращает http-статус 400 с описанием <paramref name="details"/>.
    /// </summary>
    /// <param name="controller">Контроллер.</param>
    /// <param name="details">Подробное описание проблемы.</param>
    /// <returns>Результат.</returns>
    public static IActionResult BadRequestDetails(this ControllerBase controller, string details)
    {
        return controller.Problem(details, statusCode: StatusCodes.Status400BadRequest);
    }
}
