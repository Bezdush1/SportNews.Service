using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SportNews.Service.UnitTests.Utils;

/// <summary>
/// Класс, реализующий проверку возвращаемых статусов контроллеров, 
/// в рамках модульного тестирования.
/// </summary>
public static class ControllerAnswerExtentions
{
    /// <summary>
    /// Проверка статуса 200, с указанным типом возвращаемых значением.
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
    /// <param name="answer">Статус ответа.</param>
    public static void OkObjectResult<T>(IActionResult answer)
    {
        var result = Assert.IsType<OkObjectResult>(answer);
        Assert.NotNull(answer);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        var returnValue = Assert.IsType<T>(result.Value);
        Assert.NotNull(returnValue);
    }

    /// <summary>
    /// Проверка статуса 200, без возвращаемого значения.
    /// </summary>
    /// <param name="answer">Статус ответа.</param>
    public static void Ok(IActionResult answer)
    {
        Assert.NotNull(answer);
        Assert.IsType<OkResult>(answer);
    }

    /// <summary>
    /// Проверка возвращаемого статуса 404, с сообщением об ошибке.
    /// </summary>
    /// <param name="answer">Статус ответа.</param>
    public static void NotFoundDetails(IActionResult answer)
    {
        ProblemDetails(answer, StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Проверка возвращаемого статуса 422, с сообщением об ошибке.
    /// </summary>
    /// <param name="answer">Статус ответа.</param>
    public static void UnprocessableEntityDetails(IActionResult answer)
    {
        ProblemDetails(answer, StatusCodes.Status422UnprocessableEntity);
    }

    private static void ProblemDetails(IActionResult answer, int statusCode)
    {
        var result = Assert.IsType<ObjectResult>(answer);
        Assert.NotNull(answer);
        Assert.Equal(statusCode, result.StatusCode);
    }
}
