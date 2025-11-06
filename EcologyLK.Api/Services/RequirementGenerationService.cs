using System.Collections.Generic;
using EcologyLK.Api.Models;

namespace EcologyLK.Api.Services;

/// <summary>
/// Реализация сервиса генерации требований.
/// Логика основана на файле "Критерии_для_формирования_требований_.docx.pdf"
/// </summary>
public class RequirementGenerationService : IRequirementGenerationService
{
    public List<EcologicalRequirement> GenerateRequirements(
        NvosCategory category,
        WaterUseType waterUse,
        bool hasByproducts
    )
    {
        var requirements = new List<EcologicalRequirement>();

        // --- 1. Требования, зависящие от категории (Таблица из "Критерии...") ---

        // Эти требования есть у I, II, III (кроме IV)
        if (category != NvosCategory.IV)
        {
            AddReq(
                requirements,
                "Декларация о плате за негативное воздействие на окружающую среду",
                "КоАП РФ 8.41/8.41.1",
                "Штраф (КоАП 8.41): 50-100 тыс. руб."
            );

            AddReq(
                requirements,
                "Отчет по программе производственного экологического контроля (ПЭК)",
                "ФЗ-7, приказы РПН",
                "Штраф (КоАП 8.5): 20-80 тыс. руб."
            );

            AddReq(
                requirements,
                "Журналы учета стационарных источников выбросов и их характеристик",
                "Закон об атм. воздухе",
                "Штраф (КоАП 8.1): 20-100 тыс. руб."
            );
        }

        // Эти требования есть у всех (I, II, III, IV)
        AddReq(
            requirements,
            "Журналы учета движения отходов производства и потребления",
            "Приказ Минприроды",
            "Штраф (КоАП 8.2): 100-250 тыс. руб."
        );

        AddReq(
            requirements,
            "Отчет инвентаризации выбросов вредных (загрязняющих) веществ в атмосферу",
            "ФЗ-7",
            "Штраф (КоАП 8.21 ч.1): 100-200 тыс. руб."
        );

        AddReq(
            requirements,
            "Отчет инвентаризации отходов производства и потребления",
            "ФЗ-89",
            "Штраф (КоАП 8.2): 100-250 тыс. руб."
        );

        AddReq(
            requirements,
            "Паспорта на отходы I-IV класса опасности",
            "ФЗ-89",
            "Штраф (КоАП 8.2): 100-250 тыс. руб."
        );

        AddReq(
            requirements,
            "Инструкции по обращению с отходами и приказы",
            "ФЗ-89",
            "Штраф (КоАП 8.2): 100-250 тыс. руб."
        );

        AddReq(
            requirements,
            "Проект санитарно-защитной зоны (СЗЗ) и Решение об установлении СЗЗ",
            "СанПиН",
            "Штраф (КоАП 6.3): 10-20 тыс. руб. / приостановка"
        );

        AddReq(
            requirements,
            "Статотчетность по форме 2-ТП (воздух) (при выбросе > 5 т/год)",
            "Приказ Росстата",
            "Штраф (КоАП 13.19): 20-70 тыс. руб."
        );

        AddReq(
            requirements,
            "Статотчетность по форме 2-ТП (отходы) (при образовании > 100 т/год)",
            "Приказ Росстата",
            "Штраф (КоАП 13.19): 20-70 тыс. руб."
        );

        // Требования для I, II, III
        if (
            category == NvosCategory.I
            || category == NvosCategory.II
            || category == NvosCategory.III
        )
        {
            AddReq(requirements, "Экспертное заключение и СЭЗ на проект НДВ", "Роспотребнадзор");

            AddReq(
                requirements,
                "План мероприятий неблагоприятных метеорологических условий (НМУ)",
                "ФЗ-7",
                "Штраф (КоАП 8.1): 20-100 тыс. руб."
            );

            AddReq(
                requirements,
                "Программа производственного экологического контроля (ППЭК)",
                "ФЗ-7",
                "Штраф (КоАП 8.5): 20-80 тыс. руб."
            );
        }

        // Требования для I, II
        if (category == NvosCategory.I || category == NvosCategory.II)
        {
            AddReq(
                requirements,
                "Нормативы образования отходов и лимиты на их размещение (НООЛР)",
                "ФЗ-89",
                "Штраф (КоАП 8.2): 100-250 тыс. руб."
            );

            AddReq(
                requirements,
                "Нормативы допустимых выбросов (НДВ)",
                "КоАП РФ 8.21 ч.1",
                "Штраф (КоАП 8.21 ч.1): 100-200 тыс. руб."
            );
        }

        // Специфичные по категориям
        switch (category)
        {
            case NvosCategory.I:
                AddReq(
                    requirements,
                    "Комплексное экологическое разрешение (КЭР)",
                    "ФЗ-7",
                    "Штраф (КоАП 8.47): 50-100 тыс. руб."
                );
                break;
            case NvosCategory.II:
                AddReq(
                    requirements,
                    "Декларация о воздействии на окружающую среду (ДВОС)",
                    "ФЗ-7",
                    "Штраф (КоАП 8.5): 20-80 тыс. руб."
                );
                break;
            case NvosCategory.III:
                AddReq(
                    requirements,
                    "Нормативы выбросов для веществ І, ІІ класса опасности",
                    "ФЗ-7",
                    "Штраф (КоАП 8.21 ч.1): 100-200 тыс. руб."
                );
                break;
            case NvosCategory.IV:
                // У категории IV нет доп. требований сверх общих
                break;
        }

        // --- 2. Экологические требования (Водопользование) ---
        switch (waterUse)
        {
            case WaterUseType.Well: // Скважина
                AddReq(
                    requirements,
                    "Лицензия на право пользования недрами",
                    "Закон о недрах",
                    "Штраф (КоАП 7.3 ч.1): 800-1000 тыс. руб."
                );
                break;
            case WaterUseType.River: // Река/Озеро
                AddReq(
                    requirements,
                    "Решение на право пользования водным объектом и/или Договор водопользования",
                    "Водный кодекс РФ",
                    "Штраф (КоАП 7.6): 50-100 тыс. руб."
                );
                break;
        }

        // --- 3. Побочный продукт ---
        if (hasByproducts)
        {
            AddReq(
                requirements,
                "Технические условия 'Удобрения органические на основе побочной продукции животноводства'",
                "ГОСТ/ТУ",
                "Риск (ФЗ-248): 250-350 тыс. руб."
            );
        }

        // --- 4. Общее для всех (не из таблицы) ---
        AddReq(
            requirements,
            "Обучение ответственных лиц (Удостоверение)",
            "ФЗ-7",
            "Штраф (КоАП 8.1): 20-100 тыс. руб."
        );

        return requirements;
    }

    /// <summary>
    /// Вспомогательный метод для сокращения кода при добавлении требования
    /// </summary>
    private void AddReq(
        List<EcologicalRequirement> list,
        string title,
        string basis,
        string? penaltyRisk = null
    )
    {
        list.Add(
            new EcologicalRequirement
            {
                Title = title,
                Basis = basis,
                PenaltyRisk = penaltyRisk,
                Status = RequirementStatus.NotStarted, // По умолчанию "Не выполнено"
                Deadline = null, // Срок пока не ставим
                ResponsiblePerson = null,
            }
        );
    }
}
