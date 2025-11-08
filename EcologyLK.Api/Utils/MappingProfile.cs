using AutoMapper;
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Models;

namespace EcologyLK.Api.Utils;

/// <summary>
/// Настройка правил маппинга (AutoMapper) DTO <-> Entity
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Конструктор, в котором определяются все правила маппинга.
    /// </summary>
    public MappingProfile()
    {
        // "Анкета" (DTO) -> Модель Площадки (Entity)
        CreateMap<CreateClientSiteDto, ClientSite>();

        // Модель Площадки (Entity) -> DTO для отображения
        CreateMap<ClientSite, ClientSiteDto>();

        // Модель Требования (Entity) -> DTO для отображения
        CreateMap<EcologicalRequirement, EcologicalRequirementDto>();

        // Модель Артефакта (Entity) -> DTO для отображения
        CreateMap<Artifact, ArtifactDto>();

        // Модель Требования (Entity) -> DTO для Календаря
        CreateMap<EcologicalRequirement, CalendarEventDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Deadline))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => "Requirement"))
            .ForMember(dest => dest.RelatedSiteId, opt => opt.MapFrom(src => src.ClientSiteId))
            .ForMember(
                dest => dest.RelatedSiteName,
                opt => opt.MapFrom(src => src.ClientSite != null ? src.ClientSite.Name : null)
            )
            .ForMember(
                dest => dest.Color,
                opt =>
                    opt.MapFrom(src =>
                        src.Status == RequirementStatus.Completed
                            ? "#28a745" // Зеленый (Выполнено)
                            : (
                                src.Deadline.HasValue && src.Deadline.Value < DateTime.UtcNow
                                    ? "#dc3545" // Красный (Просрочено)
                                    : "#007bff" // Синий (В работе / Не начато)
                            )
                    )
            );

        // Модель Фин. Документа (Entity) -> DTO для отображения
        CreateMap<FinancialDocument, FinancialDocumentDto>();

        // Маппинги для Админ-панели (Клиенты)
        CreateMap<Client, ClientDto>();
        CreateMap<CreateClientDto, Client>();

        // Маппинги для Справочника НПА
        CreateMap<LegalAct, LegalActDto>();
        CreateMap<CreateOrUpdateLegalActDto, LegalAct>();

        // Маппинги для Справочника Правил
        CreateMap<RequirementRule, RequirementRuleDto>();
        CreateMap<CreateOrUpdateRuleDto, RequirementRule>();
    }
}
