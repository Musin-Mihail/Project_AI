# Журнал использования ИИ в проекте "Личный кабинет клиента по экологии"

Этот документ в хронологическом порядке описывает вклад ИИ в разработку проекта.

### Этап 1: Создание структуры проекта (Backend и Frontend)

ИИ проанализировал запрос на создание базовой структуры проекта. В ТЗ (`Тестовое задание.docx.pdf`) изначально предлагался Yii2, но по текущему запросу стек обновлен на **.NET 9 (Backend)** и **Angular 20 (Frontend)**.

**Действия ИИ:**

- Сформировал набор CLI-команд для быстрой инициализации "пустых", но структурно верных проектов для выбранного стека.

**Предложенные изменения/артефакты:**

- **Backend (.NET 9):**

  - `dotnet new sln -n EcologyLK` (Создает "solution" файл, который объединит все проекты .NET)

  - `dotnet new webapi -n EcologyLK.Api -f net9.0` (Создает основной проект Web API)

  - `dotnet sln add EcologyLK.Api` (Добавляет проект API в solution)

- **Frontend (Angular 20):**

  - `npm install -g @angular/cli` (Установка Angular CLI, если отсутствует)

  - `ng new EcologyLK.Client --routing --style=scss` (Создает новое приложение Angular с файлами маршрутизации и SCSS для стилей)

  - **Архитектура:** По умолчанию используется стандартный рендеринг на стороне клиента (CSR). Рендеринг на стороне сервера (SSR) **не используется**, так как приложение является закрытым личным кабинетом, не требующим публичной индексации (SEO).

  - **Change Detection:** Будет использоваться стандартная стратегия `zone.js`. `Zoneless`-архитектура (на чистых signals) **не используется**, так как для MVP-проекта (личный кабинет) стандартный `zone.js` обеспечивает необходимую стабильность и простоту разработки.

**Предложение ИИ для следующего этага:**

1. Настроить базовое взаимодействие между проектами: сконфигурировать **CORS (Cross-Origin Resource Sharing)** в проекте `.NET (EcologyLK.Api)`, чтобы Angular-приложение (`EcologyLK.Client`) могло безопасно отправлять запросы к API.

2. Начать проектирование моделей данных и контекста базы данных (DB Context) в .NET-проекте, основываясь на анализе ТЗ (Пользователи, Клиенты, Объекты, Требования).

### Этап 2: Настройка CORS, Identity и создание базовых моделей данных (Core Data Models) с DbContext

### **Действия ИИ:**

- Проанализировав предложение из Этапа 1, ТЗ и текущий код (`CombinedCodeFiles.txt`), ИИ подтвердил, что в `EcologyLK.Api` отсутствуют:

1. Политика CORS для связи с Angular-клиентом.
2. Какие-либо модели данных.
3. Контекст базы данных (DbContext) и система аутентификации (Identity).

- **Действие 1 (CORS):** ИИ сконфигурировал базовую политику CORS в `Program.cs`, разрешающую запросы от Angular-приложения (работающего на `http://localhost:4200`).
- **Действие 2 (Пакеты EF Core & Identity):** ИИ добавил в `EcologyLK.Api.csproj` необходимые пакеты NuGet для Entity Framework Core (для работы с данными) и ASP.NET Identity (для управления пользователями и ролями, что является требованием ТЗ). Для MVP будет использоваться `InMemoryDatabase`, чтобы ускорить разработку без настройки полноценного SQL Server.
- **Действие 3 (Модели данных):** ИИ спроектировал и создал C#-классы (POCOs) для ключевых сущностей, описанных в ТЗ: `AppUser` (пользователь системы), `Client` (клиент-юрлицо), `ClientSite` (площадка клиента, ключевая сущность с параметрами НВОС), `EcologicalRequirement` (экологическое требование из "Карты требований") и `Artifact` (артефакт/документ). Также созданы `enum` для типизации параметров.
- **Действие 4 (DbContext):** ИИ создал `AppDbContext`, унаследованный от `IdentityDbContext`, который регистрирует все новые модели данных и настраивает их связи (например, "один Client ко многим ClientSites").
- **Действие 5 (Регистрация сервисов):** ИИ зарегистрировал `DbContext` и `Identity` в контейнере сервисов (`Program.cs`).

**Предложенные изменения/артефакты:**

- **Файл (Изменен):** `EcologyLK.Api/EcologyLK.Api.csproj` (добавлены пакеты EF Core и Identity)
- **Файл (Изменен):** `EcologyLK.Api/Program.cs` (Настройка CORS, DbContext и Identity)
- **Файл (Новый):** `EcologyLK.Api/Models/Enums.cs`
- **Файл (Новый):** `EcologyLK.Api/Models/AppUser.cs`
- **Файл (Новый):** `EcologyLK.Api/Models/Client.cs`
- **Файл (Новый):** `EcologyLK.Api/Models/ClientSite.cs`
- **Файл (Новый):** `EcologyLK.Api/Models/EcologicalRequirement.cs`
- **Файл (Новый):** `EcologyLK.Api/Models/Artifact.cs`
- **Файл (Новый):** `EcologyLK.Api/Data/AppDbContext.cs`

**Предложение ИИ для следующего этага:**

1. Создать первый API-контроллер (`ClientSitesController`) с методами для создания `ClientSite` (DTO "Анкеты") и получения `ClientSite` с его "Картой требований" (`EcologicalRequirements`).
2. Создать "Сервис генерации требований" (`RequirementGenerationService`), который будет вызываться при создании `ClientSite`. Этот сервис реализует бизнес-логику из файла `Критерии_для_формирования_требований_.docx.pdf` (создает нужные `EcologicalRequirement` в зависимости от `NvosCategory`, `WaterUseType` и `HasByproducts`).
3. Начать создание UI в Angular: создать `ClientSiteService` для запросов к API и компонент "Анкеты" (`client-site-form.component.ts`) для создания новой площадки.

### Этап 3: Реализация сервиса генерации требований и API-контроллера "Анкеты"

### Действия ИИ:

- Проанализировав ТЗ (`Критерии_для_формирования_требований_.docx.pdf`) и предложение из Этапа 2, ИИ реализовал **ключевую бизнес-логику MVP**.
- **Действие 1 (Сервис):** Создан `RequirementGenerationService` (и интерфейс `IRequirementGenerationService`), который содержит C#-реализацию бизнес-правил из ТЗ. Сервис принимает параметры площадки (НВОС, вода, побочные продукты) и возвращает готовый `List<EcologicalRequirement>`.
- **Действие 2 (DTOs):** Созданы Data Transfer Objects (DTOs) для "Анкеты" (`CreateClientSiteDto.cs`) и для отображения данных (`ClientSiteDto.cs`, `EcologicalRequirementDto.cs`), чтобы отделить API-модели от моделей базы данных.
- **Действие 3 (AutoMapper):** В проект добавлен `AutoMapper` для автоматического преобразования DTO в Entities и обратно. Создан `MappingProfile.cs`.
- **Действие 4 (API-Контроллер):** Создан `ClientSitesController.cs`. Он предоставляет два метода:
  - `POST /api/ClientSites`: Принимает DTO "Анкеты" (`CreateClientSiteDto`), вызывает `RequirementGenerationService` для создания "Карты требований", сохраняет новую площадку и ее требования в БД и возвращает `201 Created`.
  - `GET /api/ClientSites/{id}`: Получает площадку и ее "Карту требований" по ID.
- **Действие 5 (Регистрация):** Все новые сервисы (`IRequirementGenerationService`, `AutoMapper`, `AddControllers`) зарегистрированы в `Program.cs`. Пример `WeatherForecast` удален.

### Предложенные изменения/артефакты:

- **Файл (Изменен):** `EcologyLK.Api/EcologyLK.Api.csproj` (Добавлен пакет AutoMapper)
- **Файл (Изменен):** `EcologyLK.Api/Program.cs` (Удален WeatherForecast, зарегистрированы Controllers, AutoMapper и Service)
- **Файл (Новый):** `EcologyLK.Api/DTOs/CreateClientSiteDto.cs` (DTO "Анкеты")
- **Файл (Новый):** `EcologyLK.Api/DTOs/ClientSiteDto.cs` (DTO для отображения площадки)
- **Файл (Новый):** `EcologyLK.Api/DTOs/EcologicalRequirementDto.cs` (DTO для отображения требования)
- **Файл (Новый):** `EcologyLK.Api/Utils/MappingProfile.cs` (Настройки AutoMapper)
- **Файл (Новый):** `EcologyLK.Api/Services/IRequirementGenerationService.cs` (Интерфейс сервиса)
- **Файл (Новый):** `EcologyLK.Api/Services/RequirementGenerationService.cs` (Реализация сервиса с бизнес-логикой)
- **Файл (Новый):** `EcologyLK.Api/Controllers/ClientSitesController.cs` (API-контроллер)

### Предложение ИИ для следующего этага:

- Backend API теперь полностью готово к приему "Анкет" и отображению "Карты требований".
- Следующий логичный шаг — **реализация UI в Angular (`EcologyLK.Client`)** для использования этих API:
  1. Создать сервис `HttpClientModule` и зарегистрировать его в `app.config.ts`.
  2. Создать Angular-сервис (`client-site.service.ts`) для инкапсуляции HTTP-запросов к `POST /api/ClientSites` и `GET /api/ClientSites/{id}`.
  3. Создать Angular-компонент "Анкеты" (`client-site-form.component.ts`) с формой (используя `ReactiveFormsModule`), которая будет отправлять данные на `POST` API.
  4. Создать Angular-компонент "Карты требований" (`requirement-map.component.ts`), который будет получать ID площадки, вызывать `GET` API и отображать список требований в виде таблицы (как в ТЗ).
