using EcologyLK.Api.Models;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения Финансового документа
/// </summary>
public class FinancialDocumentDto
{
    /// <summary>
    /// Уникальный ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Номер (напр. "А-123").
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Дата документа.
    /// </summary>
    public DateTime DocumentDate { get; set; }

    /// <summary>
    /// Сумма.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Тип (Договор, Счет, Акт).
    /// </summary>
    public FinancialDocumentType Type { get; set; }

    /// <summary>
    /// Статус (Оплачен, Отправлен...).
    /// </summary>
    public FinancialDocumentStatus Status { get; set; }

    /// <summary>
    /// ID связанной площадки.
    /// </summary>
    public int ClientSiteId { get; set; }
}
