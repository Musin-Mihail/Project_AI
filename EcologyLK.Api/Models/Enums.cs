namespace EcologyLK.Api.Models;

/// <summary>
/// Категория объекта НВОС (Негативное Воздействие на Окружающую Среду).
/// </summary>
public enum NvosCategory
{
    I,
    II,
    III,
    IV,
}

/// <summary>
/// Тип водопользования.
/// </summary>
public enum WaterUseType
{
    /// <summary>
    /// Нет водопользования.
    /// </summary>
    None,

    /// <summary>
    /// Скважина.
    /// </summary>
    Well,

    /// <summary>
    /// Река/Озеро.
    /// </summary>
    River,

    /// <summary>
    /// Другое.
    /// </summary>
    Other,
}

/// <summary>
/// Статус выполнения требования.
/// </summary>
public enum RequirementStatus
{
    /// <summary>
    /// Не выполнено (в ТЗ).
    /// </summary>
    NotStarted,

    /// <summary>
    /// В работе (в ТЗ).
    /// </summary>
    InProgress,

    /// <summary>
    /// Выполнено (в ТЗ).
    /// </summary>
    Completed,
}

/// <summary>
/// Тип финансового документа.
/// </summary>
public enum FinancialDocumentType
{
    /// <summary>
    /// Договор.
    /// </summary>
    Contract,

    /// <summary>
    /// Счет.
    /// </summary>
    Invoice,

    /// <summary>
    /// Акт.
    /// </summary>
    Act,
}

/// <summary>
/// Статус финансового документа.
/// </summary>
public enum FinancialDocumentStatus
{
    /// <summary>
    /// Черновик.
    /// </summary>
    Draft,

    /// <summary>
    /// Отправлен.
    /// </summary>
    Sent,

    /// <summary>
    /// Оплачен.
    /// </summary>
    Paid,

    /// <summary>
    /// Просрочен.
    /// </summary>
    Overdue,
}
