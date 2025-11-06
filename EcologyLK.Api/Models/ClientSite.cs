namespace EcologyLK.Api.Models;

// Площадка/Объект клиента
public class ClientSite
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // --- Ключевые параметры для генерации требований ---
    public NvosCategory NvosCategory { get; set; }
    public WaterUseType WaterUseType { get; set; }
    public bool HasByproducts { get; set; } // Есть побочный продукт (навоз/помет)

    // Связь: Принадлежит одному клиенту
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    // Связь: Имеет много требований
    public List<EcologicalRequirement> Requirements { get; set; } = new();

    // Связь: Имеет много артефактов
    public List<Artifact> Artifacts { get; set; } = new();

    // Связь: Имеет много финансовых документов
    public List<FinancialDocument> FinancialDocuments { get; set; } = new();
}
