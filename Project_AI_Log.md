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

### Этап 4: Реализация UI (Angular) для "Анкеты" и "Карты требований"

### Действия ИИ:

- Проанализировав лог (Backend API из Этапа 3 готово) и код `EcologyLK.Client` (пустой boilerplate), ИИ определил, что следующим шагом является создание UI для основного MVP-сценария.
- ИИ настроил Angular-проект (`app.config.ts`) для работы с API и формами, добавив `provideHttpClient()` и `provideReactiveForms()`.
- Создал файл `EcologyLK.Client/src/app/models.ts` (TypeScript-интерфейсы и enums), который является точным зеркалом DTO и Enums из .NET API.
- Создал `EcologyLK.Client/src/app/client-site.service.ts` для инкапсуляции HTTP-запросов (`POST` и `GET`) к `ClientSitesController`.
- Создал компонент "Анкеты" (`client-site-form.component.ts/.html/.scss`) с использованием `ReactiveFormsModule` для сбора данных и отправки их на `POST /api/ClientSites`.
- Создал компонент "Карты требований" (`requirement-map.component.ts/.html/.scss`) для получения `id` из URL, вызова `GET /api/ClientSites/{id}` и отображения сгенерированных требований в таблице (согласно ТЗ).
- Настроил `EcologyLK.Client/src/app/app.routes.ts` для навигации между "Анкетой" и "Картой".
- Очистил `app.ts` и `app.html` от boilerplate-кода, добавив базовый layout с `router-outlet`.

### Предложенные изменения/артефакты:

- **Файл (Изменен):** `EcologyLK.Client/src/app/app.config.ts` (Добавлены provideHttpClient, provideReactiveForms)
- **Файл (Изменен):** `EcologyLK.Client/src/app/app.routes.ts` (Настроены маршруты для новых компонентов)
- **Файл (Изменен):** `EcologyLK.Client/src/app/app.ts` (Очищен от boilerplate)
- **Файл (Изменен):** `EcologyLK.Client/src/app/app.html` (Очищен от boilerplate, добавлен layout и router-outlet)
- **Файл (Изменен):** `EcologyLK.Client/src/app/app.scss` (Добавлены базовые стили layout)
- **Файл (Удален):** `EcologyLK.Client/src/app/app.spec.ts` (Удален boilerplate тест)
- **Файл (Новый):** `EcologyLK.Client/src/app/models.ts` (Модели DTO и Enums)
- **Файл (Новый):** `EcologyLK.Client/src/app/client-site.service.ts` (Сервис API)
- **Файл (Новый):** `EcologyLK.Client/src/app/client-site-form/client-site-form.component.ts` (Логика "Анкеты")
- **Файл (Новый):** `EcologyLK.Client/src/app/client-site-form/client-site-form.component.html` (Шаблон "Анкеты")
- **Файл (Новый):** `EcologyLK.Client/src/app/client-site-form/client-site-form.component.scss` (Стили "Анкеты")
- **Файл (Новый):** `EcologyLK.Client/src/app/requirement-map/requirement-map.component.ts` (Логика "Карты требований")
- **Файл (Новый):** `EcologyLK.Client/src/app/requirement-map/requirement-map.component.html` (Шаблон "Карты требований")
- **Файл (Новый):** `EcologyLK.Client/src/app/requirement-map/requirement-map.component.scss` (Стили "Карты требований")

### Предложение ИИ для следующего этага:

- Базовый UI "Карты требований" реализован. Следующий логичный шаг — реализация **раздела "Артефакты/Хранилище" (ТЗ, п. 1)**, так как он тесно связан с требованиями.
- Это потребует создания нового `ArtifactsController` в .NET API для обработки загрузки (upload) и скачивания (download) файлов, привязанных к `ClientSite` и/или `EcologicalRequirement`.
- В Angular нужно будет добавить UI (например, кнопку "Артефакты" в каждой строке таблицы `requirement-map`) для вызова этого API и отображения списка файлов.

### Этап 5: Реализация API "Хранилища Артефактов" (Backend)

### Действия ИИ:

- Проанализировав ТЗ (требуется блок "Артефакты/Хранилище") и лог (Этап 4 завершил UI для "Карты требований"), ИИ определил, что следующим шагом является создание **Backend API** для управления файлами (артефактами).
- **Действие 1 (Модель):** Модифицирована модель `Artifact.cs` для включения `OriginalFileName`, `StoredFileName`, `MimeType`, `FileSize` и опциональной связи `EcologicalRequirementId`.
- **Действие 2 (DTO и Маппинг):** Создан `ArtifactDto.cs` для безопасной передачи данных клиенту. Обновлен `MappingProfile.cs`.
- **Действие 3 (Сервис Хранения):** Созданы интерфейс `IArtifactStorageService.cs` и его реализация `FileArtifactStorageService.cs` для инкапсуляции логики сохранения/чтения/удаления файлов из локальной папки `uploads` (для MVP).
- **Действие 4 (API-Контроллер):** Создан `ArtifactsController.cs`. Он предоставляет CRUD-операции для файлов:
  - `POST /api/Artifacts/Upload`: Загрузка файла (IFormFile) с привязкой к `siteId` и (опционально) `requirementId`.
  - `GET /api/Artifacts?siteId=...`: Получение списка (DTO) артефактов для площадки.
  - `GET /api/Artifacts/Download/{id}`: Скачивание физического файла.
  - `DELETE /api/Artifacts/{id}`: Удаление файла и записи в БД.
- **Действие 5 (Регистрация):** Новый `IArtifactStorageService` зарегистрирован в `Program.cs`.

### Предложенные изменения/артефакты:

### - Файл (Изменен): `EcologyLK.Api/Models/Artifact.cs` (Улучшена модель данных)

- Файл (Новый): `EcologyLK.Api/DTOs/ArtifactDto.cs` (DTO для артефакта)
- Файл (Изменен): `EcologyLK.Api/Utils/MappingProfile.cs` (Добавлен маппинг Artifact)
- Файл (Изменен): `EcologyLK.Api/Data/AppDbContext.cs` (Настроена доп. связь для Artifact)
- Файл (Новый): `EcologyLK.Api/Services/IArtifactStorageService.cs` (Интерфейс сервиса)
- Файл (Новый): `EcologyLK.Api/Services/FileArtifactStorageService.cs` (Реализация сервиса)
- Файл (Новый): `EcologyLK.Api/Controllers/ArtifactsController.cs` (API-контроллер)
- Файл (Изменен): `EcologyLK.Api/Program.cs` (Регистрация IArtifactStorageService)

### Предложение ИИ для следующего этага:

- Backend API для артефактов полностью готово.
- Следующий логичный шаг — **реализация UI в Angular (`EcologyLK.Client`)** для использования этого API:
  1. Обновить `models.ts` (добавить `ArtifactDto`).
  2. Обновить `client-site.service.ts` (или создать `artifact.service.ts`) для вызова новых эндпоинтов (`Upload`, `GetArtifactsForSite`, `Download`, `Delete`).
  3. Создать новый компонент `artifact-manager.component.ts/.html` для отображения списка файлов, формы загрузки и кнопок управления.
  4. Встроить `artifact-manager` в страницу `requirement-map.component.html`, чтобы "Карта требований" и "Артефакты" отображались вместе для выбранной площадки.

### Этап 6: Реализация UI "Хранилища Артефактов" (Frontend)

### Действия ИИ:

- Проанализировав лог (Backend API для артефактов из Этапа 5 готово) и ТЗ (требуется блок "Артефакты/Хранилище"), ИИ реализовал недостающий **Frontend-интерфейс** в Angular.
- **Действие 1 (Модель):** Обновлен `models.ts` для включения интерфейса `ArtifactDto`.
- **Действие 2 (Сервис):** Создан `artifact.service.ts` для инкапсуляции HTTP-запросов к `ArtifactsController`. Он реализует методы `getArtifactsForSite`, `uploadArtifact` (с `FormData`), `deleteArtifact` и `downloadArtifact` (с обработкой `blob`).
- **Действие 3 (Компонент):** Создан новый компонент `artifact-manager.component.ts/.html/.scss`, который отображает список файлов, форму загрузки и кнопки управления (скачать/удалить). Компонент принимает `siteId` в качестве `@Input()`.
- **Действие 4 (Интеграция):** Модифицирован компонент `requirement-map.component.ts/.html` (Карта требований), чтобы он теперь включал в себя новый компонент `app-artifact-manager`, передавая ему `siteId` текущей площадки.

### Предложенные изменения/артефакты:

- Файл (Изменен): `EcologyLK.Client/src/app/models.ts` (Добавлен `ArtifactDto`)
- Файл (Новый): `EcologyLK.Client/src/app/artifact.service.ts` (Сервис API для артефактов)
- Файл (Новый): `EcologyLK.Client/src/app/artifact-manager/artifact-manager.component.ts` (Логика компонента)
- Файл (Новый): `EcologyLK.Client/src/app/artifact-manager/artifact-manager.component.html` (Шаблон компонента)
- Файл (Новый): `EcologyLK.Client/src/app/artifact-manager/artifact-manager.component.scss` (Стили компонента)
- Файл (Изменен): `EcologyLK.Client/src/app/requirement-map/requirement-map.component.ts` (Импорт и регистрация `ArtifactManagerComponent`)
- Файл (Изменен): `EcologyLK.Client/src/app/requirement-map/requirement-map.component.html` (Встроен `app-artifact-manager`)
- Файл (Изменен): `EcologyLK.Client/src/app/requirement-map/requirement-map.component.scss` (Добавлены стили для заголовка артефактов)

### Предложение ИИ для следующего этапа:

### - Ключевые блоки MVP "Карта требований" и "Артефакты" теперь реализованы (API + UI).

- Следующим логичным шагом по ТЗ (п.1) является реализация блока **"Календарь событий"**. Это потребует создания API в .NET для выборки событий (в первую очередь, `Deadline` из `EcologicalRequirement`) и создания нового UI-компонента-календаря в Angular (возможно, с использованием сторонней библиотеки) для их визуализации.

### Этап 7: Реализация API и UI для "Календаря событий"

### Действия ИИ:

- Проанализировав ТЗ (п.1, требуется "Календарь событий") и лог (Этапы 1-6 завершены), ИИ определил, что следующим шагом является реализация блока "Календарь событий".
- **Действие 1 (Backend API):**
  - Создан `CalendarEventDto.cs` для передачи данных о событиях.
  - Обновлен `MappingProfile.cs` для преобразования `EcologicalRequirement` (с `Deadline`) в `CalendarEventDto`.
  - Создан `CalendarEventsController.cs` с эндпоинтом `GET /api/CalendarEvents`, который (для MVP) извлекает все `EcologicalRequirement`, у которых задан `Deadline`, и возвращает их как список событий.
- **Действие 2 (Frontend UI):**
  - Обновлен `models.ts` (добавлен `CalendarEventDto`).
  - Создан `calendar.service.ts` (Сервис API для `GET /api/CalendarEvents`).
  - Создан новый компонент `calendar-view.component.ts/.html/.scss` для отображения событий в виде таблицы (MVP-реализация календаря).
  - Обновлен `app.routes.ts` (добавлен маршрут `/calendar`).
  - Обновлен `app.html` (добавлена ссылка "Календарь событий" в главное меню).

### Предложенные изменения/артефакты:

- **Файл (Новый):** `EcologyLK.Api/DTOs/CalendarEventDto.cs`
- **Файл (Новый):** `EcologyLK.Api/Controllers/CalendarEventsController.cs`
- **Файл (Изменен):** `EcologyLK.Api/Utils/MappingProfile.cs` (Добавлен маппинг для Календаря)
- **Файл (Изменен):** `EcologyLK.Client/src/app/models.ts` (Добавлен `CalendarEventDto`)
- **Файл (Новый):** `EcologyLK.Client/src/app/calendar.service.ts`
- **Файл (Новый):** `EcologyLK.Client/src/app/calendar-view/calendar-view.component.ts`
- **Файл (Новый):** `EcologyLK.Client/src/app/calendar-view/calendar-view.component.html`
- **Файл (Новый):** `EcologyLK.Client/src/app/calendar-view/calendar-view.component.scss`
- **Файл (Изменен):** `EcologyLK.Client/src/app/app.routes.ts` (Добавлен маршрут `/calendar`)
- **Файл (Изменен):** `EcologyLK.Client/src/app/app.html` (Добавлена ссылка в меню)

### Предложение ИИ для следующего этапа:

- Реализованы три ключевых блока MVP: "Карта требований", "Артефакты" и "Календарь событий".
- Следующим логичным шагом по ТЗ является реализация блока **"Риски"**. Это потребует обновления `EcologicalRequirement`, чтобы включить информацию о штрафах (например, `PenaltyBasis` (статья КоАП) и `PenaltyAmount` (сумма)). Затем необходимо будет создать API и UI (вероятно, как часть `requirement-map.component`) для отображения этой информации.

### Этап 8: Реализация блока "Риски"

### Действия ИИ:

- Проанализировав ТЗ (п.1, требуется блок "Риски") и лог (Этапы 1-7 завершены), ИИ определил, что следующим шагом является реализация этого блока для MVP.
- **Backend:** Модифицирована модель `EcologyLK.Api/Models/EcologicalRequirement.cs` (добавлено поле `PenaltyRisk`). Обновлен `EcologyLK.Api/Services/RequirementGenerationService.cs` для автоматического заполнения рисков (размера штрафа) при создании требования. Обновлен `EcologyLK.Api/DTOs/EcologicalRequirementDto.cs` для передачи этих данных в API.
- **Frontend:** Обновлен `EcologyLK.Client/src/app/models.ts` (интерфейс `EcologicalRequirementDto`). Модифицирован `EcologyLK.Client/src/app/requirement-map/requirement-map.component.html` для отображения новой колонки "Риски (Штраф)" в "Карте требований".

### Предложенные изменения/артефакты:

- Файл (Изменен): `EcologyLK.Api/Models/EcologicalRequirement.cs` (Добавлено поле `PenaltyRisk`)
- Файл (Изменен): `EcologyLK.Api/DTOs/EcologicalRequirementDto.cs` (Добавлено поле `PenaltyRisk`)
- Файл (Изменен): `EcologyLK.Api/Services/RequirementGenerationService.cs` (Добавлена логика заполнения рисков)
- Файл (Изменен): `EcologyLK.Client/src/app/models.ts` (Обновлен `EcologicalRequirementDto`)
- Файл (Изменен): `EcologyLK.Client/src/app/requirement-map/requirement-map.component.html` (Добавлена колонка "Риски (Штраф)")
- Файл (Изменен): `EcologyLK.Client/src/app/requirement-map/requirement-map.component.scss` (Добавлены стили для колонки рисков)

### Предложение ИИ для следующего этапа:

- Реализованы четыре из пяти основных блоков MVP: "Карта требований", "Артефакты", "Календарь событий" и "Риски".
- Последним нереализованным блоком MVP из ТЗ (п.1) является **"Договора/счета/акты"**. Следующим шагом будет создание API в .NET и UI в Angular для управления этими финансовыми документами.

### Этап 9: Реализация блока "Договора/счета/акты" (API + UI)

### Действия ИИ:

- Проанализировав ТЗ и лог проекта, ИИ определил, что последним нереализованным блоком MVP (ТЗ, п.1) является **"Договора/счета/акты"**.
- **Backend (.NET):** ИИ создал новую модель `FinancialDocument.cs` (и связанные Enums, DTO, маппинги), привязав ее к `ClientSite`. Создан `FinancialDocumentsController.cs` с эндпоинтом `GET /api/FinancialDocuments?siteId=...` для получения списка документов.
- **Frontend (Angular):** ИИ создал `financial-document.service.ts` для вызова API. Создан новый компонент `financial-document-list.component` для отображения документов в виде таблицы. Этот компонент был интегрирован в существующую страницу `requirement-map.component.html` (страница "Площадки") для отображения всех блоков MVP в одном месте.

### Предложенные изменения/артефакты:

- Файл (Изменен): `EcologyLK.Api/Models/Enums.cs` (Добавлены `FinancialDocumentType` и `FinancialDocumentStatus`)
- Файл (Новый): `EcologyLK.Api/Models/FinancialDocument.cs` (Модель данных)
- Файл (Изменен): `EcologyLK.Api/Data/AppDbContext.cs` (Добавлен `DbSet` и связь)
- Файл (Изменен): `EcologyLK.Api/Models/ClientSite.cs` (Добавлена навигационная коллекция)
- Файл (Новый): `EcologyLK.Api/DTOs/FinancialDocumentDto.cs`
- Файл (Изменен): `EcologyLK.Api/Utils/MappingProfile.cs` (Добавлен маппинг)
- Файл (Новый): `EcologyLK.Api/Controllers/FinancialDocumentsController.cs` (API-контроллер)
- Файл (Изменен): `EcologyLK.Client/src/app/models.ts` (Добавлены DTO и Enums)
- Файл (Новый): `EcologyLK.Client/src/app/financial-document.service.ts` (Сервис API)
- Файл (Новый): `EcologyLK.Client/src/app/financial-document-list/financial-document-list.component.ts`
- Файл (Новый): `EcologyLK.Client/src/app/financial-document-list/financial-document-list.component.html`
- Файл (Новый): `EcologyLK.Client/src/app/financial-document-list/financial-document-list.component.scss`
- Файл (Изменен): `EcologyLK.Client/src/app/requirement-map/requirement-map.component.ts` (Интеграция нового компонента)
- Файл (Изменен): `EcologyLK.Client/src/app/requirement-map/requirement-map.component.html` (Интеграция нового компонента)
- Файл (Изменен): `EcologyLK.Client/src/app/requirement-map/requirement-map.component.scss` (Стили для заголовка)

### Предложение ИИ для следующего этапа:

- Все **5 основных блоков MVP** ("Карта требований", "Артефакты", "Календарь", "Риски", "Договора/счета/акты") теперь реализованы.
- Следующим логичным шагом является реализация **фундаментальной системы безопасности и ролей**, как того требует ТЗ (п.4 "Роли и доступы" и п.6 "Безопасность"). Это потребует настройки `ASP.NET Identity` для регистрации и входа, создания API-эндпоинтов для аутентификации, защиты контроллеров .NET атрибутом `[Authorize]` и создания UI в Angular (Login/Register components).

### Этап 10: Инициализация (Seed) базы данных для тестирования

### Действия ИИ:

- Проанализировав обратную связь (ошибка `Client with Id 1 not found` при отправке "Анкеты"), ИИ определил, что `InMemoryDatabase` пуста, и API не может найти "Клиента" с `Id = 1`, который жестко задан в Angular-форме.
- **Действие 1:** ИИ создал новый класс `EcologyLK.Api/Data/DataSeeder.cs`.
- **Действие 2:** Этот класс при старте API проверяет, пуста ли таблица `Clients`, и если да — добавляет одного "Тестового Клиента". В `InMemoryDatabase` этот клиент автоматически получает `Id = 1`.
- **Действие 3:** ИИ вызвал `DataSeeder.SeedDatabase(app)` в `Program.cs` сразу после `app.Build()`, чтобы гарантировать наличие данных до обработки первого запроса.

### Предложенные изменения/артефакты:

- Файл (Новый): `EcologyLK.Api/Data/DataSeeder.cs` (Логика инициализации БД)
- Файл (Изменен): `EcologyLK.Api/Program.cs` (Вызов `DataSeeder.SeedDatabase(app)`)

### Предложение ИИ для следующего этапа:

- После этого исправления основной MVP-сценарий (Анкета -> Карта -> Артефакты -> Календарь -> Риски -> Документы) должен работать.
- Следующим логичным шагом, как и планировалось, является реализация **фундаментальной системы безопасности и ролей** (ТЗ, п.4 и п.6). Это потребует настройки `ASP.NET Identity` для регистрации и входа, защиты API и создания UI для аутентификации в Angular.

### Этап 11: Реализация аутентификации (JWT) и авторизации (Backend)

### Действия ИИ:

- Проанализировав ТЗ (п.4 "Роли" и п.6 "Безопасность") и лог (Этапы 1-10 завершили функционал MVP), ИИ определил, что система не защищена и не имеет механизма входа.
- **Действие 1 (Настройка JWT):** ИИ добавил конфигурацию для JSON Web Token в `appsettings.json` и настроил сервисы `AddAuthentication` (с `JwtBearer`) и `AddAuthorization` в `Program.cs`.
- **Действие 2 (Сервис токенов):** Создан `AuthTokenService` (и интерфейс) для генерации JWT при успешном входе.
- **Действие 3 (API Аутентификации):** Созданы DTO (`AuthDtos.cs`) и новый `AuthController.cs` с публичными эндпоинтами `POST /api/Auth/Register` (для регистрации) и `POST /api/Auth/Login` (для входа).
- **Действие 4 (Защита API):** Все существующие контроллеры (`ClientSitesController`, `ArtifactsController`, `CalendarEventsController`, `FinancialDocumentsController`) были защищены атрибутом `[Authorize]`.
- **Действие 5 (Seeding Ролей):** `DataSeeder.cs` был обновлен для асинхронного создания ролей ("Admin", "Client") и пользователя "Admin" при первом запуске API.

### Предложенные изменения/артефакты:

- Файл (Изменен): `EcologyLK.Api/appsettings.json` (Добавлена секция `Jwt`)
- Файл (Новый): `EcologyLK.Api/DTOs/AuthDtos.cs` (DTO для Register, Login, AuthResponse)
- Файл (Новый): `EcologyLK.Api/Services/IAuthTokenService.cs` (Интерфейс)
- Файл (Новый): `EcologyLK.Api/Services/AuthTokenService.cs` (Реализация)
- Файл (Новый): `EcologyLK.Api/Controllers/AuthController.cs` (API для Register/Login)
- Файл (Изменен): `EcologyLK.Api/Data/DataSeeder.cs` (Добавлен асинхронный метод `SeedRolesAndAdminAsync`)
- Файл (Изменен): `EcologyLK.Api/Program.cs` (Настроены Authentication/Authorization, вызван Seeder ролей)
- Файл (Изменен): `EcologyLK.Api/Controllers/ClientSitesController.cs` (Добавлен `[Authorize]`)
- Файл (Изменен): `EcologyLK.Api/Controllers/ArtifactsController.cs` (Добавлен `[Authorize]`)
- Файл (Изменен): `EcologyLK.Api/Controllers/CalendarEventsController.cs` (Добавлен `[Authorize]`)
- Файл (Изменен): `EcologyLK.Api/Controllers/FinancialDocumentsController.cs` (Добавлен `[Authorize]`)

### Предложение ИИ для следующего этапа:

- Backend API теперь защищено и готово к аутентификации.
- Следующий логичный шаг — **реализация UI в Angular (`EcologyLK.Client`)** для аутентификации:
  1. Создать `auth.service.ts` (для вызова `POST /api/Auth/Login` и `Register`).
  2. Создать компоненты `login.component.ts/.html` и (опционально) `register.component.ts/.html`.
  3. Создать `AuthGuard` для защиты маршрутов Angular.
  4. Реализовать `HttpInterceptor` для автоматического добавления JWT-токена в заголовки всех API-запросов к защищенным эндпоинтам.

### Этап 12: Реализация UI аутентификации (Angular)

### Действия ИИ:

- Проанализировав лог (Этап 11), ИИ определил, что Backend API теперь полностью защищено (`[Authorize]`), но Frontend (`EcologyLK.Client`) не отправляет JWT-токен, что приводит к ошибкам `401 Unauthorized` и делает приложение неработоспособным.
- **Действие 1 (Сервис):** Создан `EcologyLK.Client/src/app/auth.service.ts` для инкапсуляции логики входа (`login`), выхода (`logout`), а также хранения/загрузки сессии (JWT и данных пользователя) из `localStorage` с использованием `signal`.
- **Действие 2 (Компонент):** Создан `EcologyLK.Client/src/app/login/...` (компонент) с `ReactiveFormsModule` для входа пользователя.
- **Действие 3 (Interceptor):** Создан `EcologyLK.Client/src/app/auth.interceptor.ts`. Этот перехватчик автоматически добавляет заголовок `Authorization: Bearer <token>` ко всем исходящим API-запросам.
- **Действие 4 (Guard):** Создан `EcologyLK.Client/src/app/auth.guard.ts` (тип `CanActivateFn`) для защиты функциональных маршрутов (`/new-site`, `/site/:id`, `/calendar`).
- **Действие 5 (Интеграция):** Обновлены `app.config.ts` (для регистрации Interceptor), `app.routes.ts` (для добавления `/login` и `authGuard`), `app.html` и `app.ts` (для отображения информации о пользователе и кнопки "Выйти").
- **Действие 6 (Модели):** Обновлен `EcologyLK.Client/src/app/models.ts` для включения DTO `AuthResponseDto`, `LoginUserDto` и `RegisterUserDto`.

### Предложенные изменения/артефакты:

- Файл (Изменен): `EcologyLK.Client/src/app/models.ts` (Добавлены DTO для аутентификации)
- Файл (Новый): `EcologyLK.Client/src/app/auth.service.ts` (Сервис управления сессией и API)
- Файл (Новый): `EcologyLK.Client/src/app/auth.interceptor.ts` (Перехватчик для добавления JWT)
- Файл (Новый): `EcologyLK.Client/src/app/auth.guard.ts` (Защитник маршрутов)
- Файл (Новый): `EcologyLK.Client/src/app/login/login.component.ts` (Логика компонента)
- Файл (Новый): `EcologyLK.Client/src/app/login/login.component.html` (Шаблон компонента)
- Файл (Новый): `EcologyLK.Client/src/app/login/login.component.scss` (Стили компонента)
- Файл (Изменен): `EcologyLK.Client/src/app/app.config.ts` (Зарегистрирован `authInterceptor`)
- Файл (Изменен): `EcologyLK.Client/src/app/app.routes.ts` (Добавлен маршрут `/login` и `authGuard`)
- Файл (Изменен): `EcologyLK.Client/src/app/app.ts` (Добавлена логика `currentUser` и `logout`)
- Файл (Изменен): `EcologyLK.Client/src/app/app.html` (Добавлен `currentUser` и кнопка "Выйти")
- Файл (Изменен): `EcologyLK.Client/src/app/app.scss` (Стили для `logout-btn`)

### Предложение ИИ для следующего этапа:

- Теперь система полностью защищена и функциональна (API + UI). Пользователь может войти, и его токен используется для всех API-запросов.
- Следующим шагом (согласно ТЗ, п.4 'Роли и доступы') является реализация **логики, зависящей от ролей**.
- Например:
  1. **Backend (API):** Улучшить `[Authorize]` до `[Authorize(Roles = "Admin")]` на критических методах (например, `DELETE /api/Artifacts/{id}`), чтобы 'Клиент' не мог удалять файлы (если это требуется).
  2. **Frontend (UI):** В `AuthService` сохранить роли (`user.roles`). В компонентах (например, `artifact-manager.component.ts`) проверять эти роли и скрывать кнопки (например, "Удалить") с помощью `@if` для пользователей с ролью 'Client'.

### Этап 13: Реализация авторизации на основе ролей (Admin vs. Client)

### Действия ИИ:

- Проанализировав ТЗ (п.4 "Роли") и предложение из Этапа 12, ИИ реализовал базовую авторизацию на основе ролей.
- **Backend:** Метод `DELETE /api/Artifacts/{id}` в `ArtifactsController.cs` защищен атрибутом `[Authorize(Roles = "Admin")]`, чтобы только администраторы могли удалять файлы.
- **Frontend:** В `AuthService` добавлен метод `hasRole('Admin')`. Компонент `artifact-manager` теперь использует этот метод, чтобы скрыть кнопку "Удалить" (`@if`) для пользователей, не являющихся администраторами.

### Предложенные изменения/артефакты:

- Файл (Изменен): `EcologyLK.Api/Controllers/ArtifactsController.cs` (Добавлен `[Authorize(Roles = "Admin")]` на метод `DeleteArtifact`)
- Файл (Изменен): `EcologyLK.Client/src/app/auth.service.ts` (Добавлен метод `hasRole`)
- Файл (Изменен): `EcologyLK.Client/src/app/artifact-manager/artifact-manager.component.ts` (Интеграция `AuthService` для проверки роли)
- Файл (Изменен): `EcologyLK.Client/src/app/artifact-manager/artifact-manager.component.html` (Кнопка "Удалить" скрыта через `@if`)

### Предложение ИИ для следующего этапа:

- Базовая авторизация по ролям реализована. Следующий критически важный шаг — реализация **"Row-Level Security" (RLS)**, как того требует ТЗ (п.6 "Безопасность").
- Это потребует: 1) Связать `AppUser` с `ClientId` при регистрации. 2) Модифицировать API-контроллеры (напр., `ClientSitesController`, `ArtifactsController`) для проверки того, что `ClientId` текущего пользователя совпадает с `ClientId` запрашиваемого ресурса, или что пользователь является 'Admin'.

### Этап 14: Внедрение Row-Level Security (RLS) и привязка к ClientId

### Действия ИИ:

- Проанализировав ТЗ (п.6 "Безопасность") и лог (Этап 13), ИИ определил наличие **критической уязвимости**: отсутствие "Row-Level Security" (RLS). Любой аутентифицированный клиент мог получить доступ к данным (площадкам, артефактам, документам) другого клиента, перебирая ID в URL.
- **Действие 1 (Backend - JWT):** ИИ модифицировал `AuthTokenService` для встраивания `client_id` пользователя в JWT, а также `AuthController` и `AuthResponseDto` для передачи `ClientId` при входе.
- **Действие 2 (Backend - Утилита):** ИИ создал `ClaimsPrincipalExtensions` для удобного получения `User.GetClientId()` и `User.IsAdmin()` в контроллерах.
- **Действие 3 (Backend - RLS):** ИИ модифицировал **все** контроллеры (`ClientSites`, `Artifacts`, `CalendarEvents`, `FinancialDocuments`), добавив в них логику проверки. Теперь при запросе данных API проверяет, что `ClientId` запрашиваемого ресурса совпадает с `ClientId` пользователя в JWT, или что пользователь имеет роль "Admin".
- **Действие 4 (Frontend - UI):** ИИ исправил `client-site-form.component.ts`, убрав жестко заданный `clientId: 1` и заменив его на `clientId` из `AuthService.currentUser()`, что устранило ошибку привязки данных.

### Предложенные изменения/артефакты:

- Файл (Новый): `EcologyLK.Api/Utils/ClaimsPrincipalExtensions.cs` (Служебный класс для работы с Claims)
- Файл (Изменен): `EcologyLK.Api/DTOs/AuthDtos.cs` (Добавлен ClientId в Register и Response)
- Файл (Изменен): `EcologyLK.Api/Services/AuthTokenService.cs` (Добавлена `client_id` claim в JWT)
- Файл (Изменен): `EcologyLK.Api/Controllers/AuthController.cs` (Обработка ClientId при Register/Login)
- Файл (Изменен): `EcologyLK.Api/Controllers/ClientSitesController.cs` (Добавлены проверки RLS)
- Файл (Изменен): `EcologyLK.Api/Controllers/ArtifactsController.cs` (Добавлены проверки RLS)
- Файл (Изменен): `EcologyLK.Api/Controllers/CalendarEventsController.cs` (Добавлены проверки RLS)
- Файл (Изменен): `EcologyLK.Api/Controllers/FinancialDocumentsController.cs` (Добавлены проверки RLS)
- Файл (Изменен): `EcologyLK.Client/src/app/models.ts` (Обновлен `AuthResponseDto`)
- Файл (Изменен): `EcologyLK.Client/src/app/client-site-form/client-site-form.component.ts` (Исправлен hardcode `clientId: 1`)
- Файл (Изменен): `EcologyLK.Client/src/app/client-site-form/client-site-form.component.html` (Удалено поле ClientId из HTML-формы)

### Предложение ИИ для следующего этапа:

- MVP-проект теперь функционально завершен и защищен (реализованы аутентификация, авторизация по ролям и RLS).
- Следующим логичным шагом (согласно ТЗ, п.7 "Админ-панель") является **создание панели администратора** для управления пользователями и справочниками. Это потребует создания `AdminController` в API (для CRUD операций над `Client` и `AppUser`) и нового набора компонентов в Angular, защищенных `authGuard` и проверкой на роль "Admin".

### Этап 15: Реализация API-эндпоинтов для "Админ-панели" (Управление Клиентами и Пользователями)

### Действия ИИ:

- Проанализировав ТЗ (п.7 "Админ-панель") и предложение из Этапа 14, ИИ определил, что проект функционально завершен для _Клиента_, но отсутствует API для _Администратора_.
- **Действие 1:** ИИ создал новый `AdminController.cs`, полностью защищенный `[Authorize(Roles = "Admin")]`.
- **Действие 2:** ИИ создал набор `AdminDtos.cs` для управления `Client` (ЮрЛицами) и `AppUser` (Пользователями).
- **Действие 3:** `AdminController` реализует эндпоинты `GET /api/Admin/Clients`, `POST /api/Admin/Clients`, `GET /api/Admin/Users`, `POST /api/Admin/Users` (создание пользователя Админом) и `PUT /api/Admin/Users/{id}` (обновление пользователя, смена `ClientId`).
- **Действие 4:** ИИ модифицировал `RegisterUserDto` (сделав `ClientId` nullable), чтобы разрешить "публичную" регистрацию пользователей, не привязанных к ЮрЛицу.

### Предложенные изменения/артефакты:

- Файл (Новый): `EcologyLK.Api/Controllers/AdminController.cs` (API-контроллер для Админ-панели)
- Файл (Новый): `EcologyLK.Api/DTOs/AdminDtos.cs` (DTO для управления Клиентами и Пользователями)
- Файл (Изменен): `EcologyLK.Api/Utils/MappingProfile.cs` (Добавлены маппинги для Client/ClientDto)
- Файл (Изменен): `EcologyLK.Api/Models/Client.cs` (Добавлены атрибуты валидации `StringLength`)
- Файл (Изменен): `EcologyLK.Api/DTOs/AuthDtos.cs` (Сделан `ClientId` nullable в `RegisterUserDto`)
- Файл (Изменен): `EcologyLK.Api/Controllers/AuthController.cs` (Обработка nullable `ClientId` при регистрации)

### Предложение ИИ для следующего этапа:

- Backend API для "Админ-панели" полностью готово.
- Следующий логичный шаг — **реализация UI в Angular (`EcologyLK.Client`)** для использования этого API. Это потребует создания нового модуля (или набора компонентов), защищенного `authGuard` и дополнительной проверкой на роль 'Admin'. Необходимо создать страницы `/admin/users` и `/admin/clients` для отображения и управления этими сущностями.

### Этап 16: Реализация UI "Админ-панели" (Frontend)

### Действия ИИ:

- Проанализировав ТЗ (п.7 "Админ-панель") и лог (Этап 15 завершил API), ИИ определил, что следующим шагом является реализация **Frontend-интерфейса** для администрирования.
- **Действие 1 (Сервис и Модели):** Создан `admin.service.ts` для вызова эндпоинтов `api/Admin`. Обновлен `models.ts`, включив в него DTO `ClientDto`, `UserDto` и др.
- **Действие 2 (Охрана маршрутов):** Создан `admin.guard.ts` (CanActivateFn), который проверяет `authService.hasRole('Admin')`, блокируя доступ для обычных клиентов.
- **Действие 3 (Компоненты):** Созданы два новых компонента (`admin-user-list` и `admin-client-list`) для отображения списков сущностей и форм для их создания.
- **Действие 4 (Интеграция):** Обновлен `app.routes.ts` для добавления маршрутов `/admin/users` и `/admin/clients`, защищенных `authGuard` и `adminGuard`. Обновлены `app.html` и `app.ts` для отображения ссылок на "Админ-панель" в главном меню (только для роли "Admin").

### Предложенные изменения/артефакты:

- Файл (Изменен): `EcologyLK.Client/src/app/models.ts` (Добавлены DTO для Админ-панели)
- Файл (Новый): `EcologyLK.Client/src/app/admin.guard.ts` (Guard для проверки роли "Admin")
- Файл (Новый): `EcologyLK.Client/src/app/admin.service.ts` (Сервис API для `AdminController`)
- Файл (Новый): `EcologyLK.Client/src/app/admin-client-list/admin-client-list.component.ts`
- Файл (Новый): `EcologyLK.Client/src/app/admin-client-list/admin-client-list.component.html`
- Файл (Новый): `EcologyLK.Client/src/app/admin-client-list/admin-client-list.component.scss`
- Файл (Новый): `EcologyLK.Client/src/app/admin-user-list/admin-user-list.component.ts`
- Файл (Новый): `EcologyLK.Client/src/app/admin-user-list/admin-user-list.component.html`
- Файл (Новый): `EcologyLK.Client/src/app/admin-user-list/admin-user-list.component.scss`
- Файл (Изменен): `EcologyLK.Client/src/app/app.routes.ts` (Добавлены маршруты `/admin/*`)
- Файл (Изменен): `EcologyLK.Client/src/app/app.ts` (Сделан `authService` public для шаблона)
- Файл (Изменен): `EcologyLK.Client/src/app/app.html` (Добавлена ссылка "Админ-панель" в меню)

### Предложение ИИ для следующего этапа:

- Проект MVP теперь функционально завершен (API + UI, включая Админ-панель).
- Следующим шагом (согласно ТЗ, п.7) может быть реализация **"Ручной правки требований"** (например, добавление API и UI для изменения `Deadline` или `Status` в `EcologicalRequirement`).
- Либо, улучшение UI/UX, например, замена MVP-таблицы "Календаря" на полноценный графический календарь (с использованием `fullcalendar` или аналога).
