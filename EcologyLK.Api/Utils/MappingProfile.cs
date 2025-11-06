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

        // --- Добавлено ИИ ---
        // Модель Артефакта -> DTO для отображения
        CreateMap<Artifact, ArtifactDto>();
        // --- Конец ---
    }
}
