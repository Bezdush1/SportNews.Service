namespace SportNews.Service.DatabaseTests.CustomOrder;

/// <summary>
/// Класс, представляющий приоритет для тестов.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestPriorityAttribute : Attribute
{
    /// <summary>
    /// Конструктор класса.
    /// </summary>
    /// <param name="priority">Приоритет.</param>
    public TestPriorityAttribute(int priority)
    {
        Priority = priority;
    }

    /// <summary>
    /// Приоритет.
    /// </summary>
    public int Priority { get; private set; }
}