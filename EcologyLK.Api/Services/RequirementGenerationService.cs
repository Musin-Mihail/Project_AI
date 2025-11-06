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
                "КоАП РФ 8.41/8.41.1"
            );

            AddReq(
                requirements,
                "Отчет по программе производственного экологического контроля (ПЭК)",
                "ФЗ-7, приказы РПН"
            );

            AddReq(
                requirements,
                "Журналы учета стационарных источников выбросов и их характеристик",
                "Закон об атм. воздухе"
            );
        }

        // Эти требования есть у всех (I, II, III, IV)
        AddReq(
            requirements,
            "Журналы учета движения отходов производства и потребления",
            "Приказ Минприроды"
        );

        AddReq(
            requirements,
            "Отчет инвентаризации выбросов вредных (загрязняющих) веществ в атмосферу",
            "ФЗ-7"
        );

        AddReq(requirements, "Отчет инвентаризации отходов производства и потребления", "ФЗ-89");

        AddReq(requirements, "Паспорта на отходы I-IV класса опасности", "ФЗ-89");

        AddReq(requirements, "Инструкции по обращению с отходами и приказы", "ФЗ-89");

        AddReq(
            requirements,
            "Проект санитарно-защитной зоны (СЗЗ) и Решение об установлении СЗЗ",
            "СанПиН"
        );

        AddReq(
            requirements,
            "Статотчетность по форме 2-ТП (воздух) (при выбросе > 5 т/год)",
            "Приказ Росстата"
        );

        AddReq(
            requirements,
            "Статотчетность по форме 2-ТП (отходы) (при образовании > 100 т/год)",
            "Приказ Росстата"
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
                "ФЗ-7"
            );

            AddReq(
                requirements,
                "Программа производственного экологического контроля (ППЭК)",
                "ФЗ-7"
            );
        }

        // Требования для I, II
        if (category == NvosCategory.I || category == NvosCategory.II)
        {
            AddReq(
                requirements,
                "Нормативы образования отходов и лимиты на их размещение (НООЛР)",
                "ФЗ-89"
            );

            AddReq(requirements, "Нормативы допустимых выбросов (НДВ)", "КоАП РФ 8.21 ч.1");
        }

        // Специфичные по категориям
        switch (category)
        {
            case NvosCategory.I:
                AddReq(requirements, "Комплексное экологическое разрешение (КЭР)", "ФЗ-7");
                break;
            case NvosCategory.II:
                AddReq(requirements, "Декларация о воздействии на окружающую среду (ДВОС)", "ФЗ-7");
                break;
            case NvosCategory.III:
                AddReq(
                    requirements,
                    "Нормативы выбросов для веществ І, ІІ класса опасности",
                    "ФЗ-7"
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
                AddReq(requirements, "Лицензия на право пользования недрами", "Закон о недрах");
                break;
            case WaterUseType.River: // Река/Озеро
                AddReq(
                    requirements,
                    "Решение на право пользования водным объектом и/или Договор водопользования",
                    "Водный кодекс РФ"
                );
                break;
        }

        // --- 3. Побочный продукт ---
        if (hasByproducts)
        {
            AddReq(
                requirements,
                "Технические условия 'Удобрения органические на основе побочной продукции животноводства'",
                "ГОСТ/ТУ"
            );
        }

        // --- 4. Общее для всех (не из таблицы) ---
        AddReq(requirements, "Обучение ответственных лиц (Удостоверение)", "ФЗ-7");

        return requirements;
    }

    /// <summary>
    /// Вспомогательный метод для сокращения кода при добавлении требования
    /// </summary>
    private void AddReq(List<EcologicalRequirement> list, string title, string basis)
    {
        list.Add(
            new EcologicalRequirement
            {
                Title = title,
                Basis = basis,
                Status = RequirementStatus.NotStarted, // По умолчанию "Не выполнено"
                Deadline = null, // Срок пока не ставим
                ResponsiblePerson = null,
            }
        );
    }
}
