using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcologyLK.Api.Models;

/// <summary>
/// Модель финансового документа (Договор, Счет, Акт)
/// </summary>
public class FinancialDocument
{
    public int Id { get; set; }

    [StringLength(100)]
    public string DocumentNumber { get; set; } = string.Empty;

    public DateTime DocumentDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    public FinancialDocumentType Type { get; set; }

    public FinancialDocumentStatus Status { get; set; }

    // Связь: Принадлежит одной площадке
    // (По ТЗ, этот раздел является частью ЛК площадки)
    public int ClientSiteId { get; set; }
    public ClientSite? ClientSite { get; set; }
}
