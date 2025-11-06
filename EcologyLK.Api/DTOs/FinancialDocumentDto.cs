using EcologyLK.Api.Models;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения Финансового документа
/// </summary>
public class FinancialDocumentDto
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public decimal Amount { get; set; }
    public FinancialDocumentType Type { get; set; }
    public FinancialDocumentStatus Status { get; set; }
    public int ClientSiteId { get; set; }
}
