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

// --- НАЧАЛО: Добавлено ИИ (Этап 9) ---

// Тип финансового документа
public enum FinancialDocumentType
{
    Contract, // Договор
    Invoice, // Счет
    Act, // Акт
}

// Статус финансового документа
public enum FinancialDocumentStatus
{
    Draft, // Черновик
    Sent, // Отправлен
    Paid, // Оплачен
    Overdue, // Просрочен
}
// --- КОНЕЦ: Добавлено ИИ (Этап 9) ---
