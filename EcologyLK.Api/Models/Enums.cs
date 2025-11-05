namespace EcologyLK.Api.Models;

// Категория объекта НВОС (Негативное Воздействие на Окружающую Среду)
public enum NvosCategory
{
    I,
    II,
    III,
    IV,
}

// Тип водопользования
public enum WaterUseType
{
    None, // Нет
    Well, // Скважина
    River, // Река/Озеро
    Other,
}

// Статус выполнения требования
public enum RequirementStatus
{
    NotStarted, // Не выполнено
    InProgress, // В работе
    Completed, // Выполнено
}
