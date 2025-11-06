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
