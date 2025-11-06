// --- Enums (зеркало EcologyLK.Api/Models/Enums.cs) ---

// Категория объекта НВОС
export enum NvosCategory {
  I,
  II,
  III,
  IV,
}

// Тип водопользования
export enum WaterUseType {
  None, // Нет
  Well, // Скважина
  River, // Река/Озеро
  Other,
}

// Статус выполнения требования
export enum RequirementStatus {
  NotStarted, // Не выполнено
  InProgress, // В работе
  Completed, // Выполнено
}

// --- DTOs (зеркало EcologyLK.Api/DTOs) ---

/**
 * DTO для "Анкеты".
 * Данные, которые мы отправляем в API
 * при создании новой площадки.
 */
export interface CreateClientSiteDto {
  clientId: number;
  name: string;
  address: string;
  nvosCategory: NvosCategory;
  waterUseType: WaterUseType;
  hasByproducts: boolean;
}

/**
 * DTO для отображения Экологического Требования
 */
export interface EcologicalRequirementDto {
  id: number;
  title: string;
  basis: string;

  // --- Добавлено ИИ (Этап 8) ---
  penaltyRisk?: string;
  // --- Конец ---

  responsiblePerson?: string;
  deadline?: Date;
  status: RequirementStatus;
}

/**
 * DTO для отображения полной информации о площадке
 * (включая ее "Карту требований")
 */
export interface ClientSiteDto {
  id: number;
  name: string;
  address: string;
  requirements: EcologicalRequirementDto[];
}

// --- Добавлено ИИ (Этап 6): DTO для Артефакта ---
/**
 * DTO для отображения информации об Артефакте (файле)
 */
export interface ArtifactDto {
  id: number;
  originalFileName: string;
  mimeType: string;
  fileSize: number;
  uploadDate: Date;
  clientSiteId: number;
  ecologicalRequirementId?: number;
}
// --- Конец ---

// --- Добавлено ИИ (Этап 7): DTO для Календаря ---
/**
 * DTO для отображения события в календаре
 */
export interface CalendarEventDto {
  id: number;
  title: string;
  startDate: Date;
  eventType: string;
  relatedSiteId?: number;
  relatedSiteName?: string;
}
// --- Конец ---
