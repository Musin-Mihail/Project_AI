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

// Тип финансового документа
export enum FinancialDocumentType {
  Contract, // Договор
  Invoice, // Счет
  Act, // Акт
}

// Статус финансового документа
export enum FinancialDocumentStatus {
  Draft, // Черновик
  Sent, // Отправлен
  Paid, // Оплачен
  Overdue, // Просрочен
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
  penaltyRisk?: string;
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

/**
 * DTO для отображения Финансового документа
 */
export interface FinancialDocumentDto {
  id: number;
  documentNumber: string;
  documentDate: Date;
  amount: number;
  type: FinancialDocumentType;
  status: FinancialDocumentStatus;
  clientSiteId: number;
}

// --- DTO ДЛЯ АУТЕНТИФИКАЦИИ ---

/**
 * DTO для регистрации нового пользователя
 * (Зеркало EcologyLK.Api/DTOs/AuthDtos.cs)
 */
export interface RegisterUserDto {
  email: string;
  password?: string; // Опционально, т.к. мы его не храним
  fullName: string;
  clientId: number;
}

/**
 * DTO для входа пользователя
 * (Зеркало EcologyLK.Api/DTOs/AuthDtos.cs)
 */
export interface LoginUserDto {
  email: string;
  password?: string;
}

/**
 * DTO ответа после успешной аутентификации
 * (Зеркало EcologyLK.Api/DTOs/AuthDtos.cs)
 */
export interface AuthResponseDto {
  userId: string;
  email: string;
  fullName: string;
  token: string;
  roles: string[];
  clientId?: number;
}

// --- DTO ДЛЯ АДМИН-ПАНЕЛИ (Новое) ---
// (Зеркало EcologyLK.Api/DTOs/AdminDtos.cs)

/**
 * DTO для отображения информации о Клиенте (ЮрЛице)
 */
export interface ClientDto {
  id: number;
  name: string;
  inn: string;
  ogrn: string;
}

/**
 * DTO для создания нового Клиента (ЮрЛица)
 */
export interface CreateClientDto {
  name: string;
  inn: string;
  ogrn: string;
}

/**
 * DTO для отображения информации о Пользователе
 */
export interface UserDto {
  id: string;
  email: string;
  fullName: string;
  clientId?: number;
  roles: string[];
}

/**
 * DTO для создания нового Пользователя (Администратором)
 */
export interface CreateUserDto {
  email: string;
  password?: string;
  fullName: string;
  clientId?: number;
  role: string;
}

/**
 * DTO для обновления данных Пользователя
 */
export interface UpdateUserDto {
  fullName: string;
  clientId?: number;
}

// --- DTO ДЛЯ РЕДАКТИРОВАНИЯ ТРЕБОВАНИЙ (Новое) ---

/**
 * DTO для ручного обновления Экологического Требования
 * (Зеркало EcologyLK.Api/DTOs/UpdateRequirementDto.cs)
 */
export interface UpdateRequirementDto {
  status: RequirementStatus;
  deadline?: Date;
  responsiblePerson?: string;
}

// --- DTO ДЛЯ СПРАВОЧНИКА НПА (НОВОЕ) ---

/**
 * DTO для отображения НПА
 * (Зеркало EcologyLK.Api/DTOs/LegalActDto.cs)
 */
export interface LegalActDto {
  id: number;
  title: string;
  referenceCode: string;
  description?: string;
  externalLink?: string;
}

/**
 * DTO для создания/обновления НПА
 * (Зеркало EcologyLK.Api/DTOs/LegalActDto.cs)
 */
export interface CreateOrUpdateLegalActDto {
  title: string;
  referenceCode: string;
  description?: string;
  externalLink?: string;
}
