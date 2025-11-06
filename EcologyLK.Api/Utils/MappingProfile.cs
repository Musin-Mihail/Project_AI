using AutoMapper;
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Models;

namespace EcologyLK.Api.Utils;

/// <summary>
/// Настройка правил маппинга DTO <-> Entity
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // "Анкета" DTO -> Модель Площадки
        CreateMap<CreateClientSiteDto, ClientSite>();

        // Модель Площадки -> DTO для отображения
        CreateMap<ClientSite, ClientSiteDto>();

        // Модель Требования -> DTO для отображения
        CreateMap<EcologicalRequirement, EcologicalRequirementDto>();

        // --- Добавлено ИИ (Этап 5) ---
        // Модель Артефакта -> DTO для отображения
        CreateMap<Artifact, ArtifactDto>();
        // --- Конец ---

        // --- Добавлено ИИ (Этап 7) ---
        // Модель Требования -> DTO для Календаря
        CreateMap<EcologicalRequirement, CalendarEventDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Deadline))
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => "Requirement"))
            .ForMember(dest => dest.RelatedSiteId, opt => opt.MapFrom(src => src.ClientSiteId))
            .ForMember(
                dest => dest.RelatedSiteName,
                opt => opt.MapFrom(src => src.ClientSite != null ? src.ClientSite.Name : null)
            ); // <-- Исправлено ИИ: добавлена проверка на null
        // --- Конец ---
    }
}
