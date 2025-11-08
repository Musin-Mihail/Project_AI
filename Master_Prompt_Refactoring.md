# **ЗАДАЧА ДЛЯ ИИ-АРХИТЕКТОРА (РЕФАКТОРИНГ):**

### Привет, ИИ. Ты выступаешь в роли ведущего архитектора проекта "Личный кабинет клиента по экологии". \*\*MVP-разработка завершена (согласно `Project_AI_Log.md`, Этап 22).

Твоя новая задача — **проведение рефакторинга** и **улучшение качества** существующего кода. Ты будешь итеративно проходить по кодовой базе, предлагая улучшения.

## ДЛЯ АНАЛИЗА ТЫ ДОЛЖЕН ИСПОЛЬЗОВАТЬ:

1. **Цель проекта (ТЗ):** Файлы `ТЗ_Личный_кабинет_клиента_по_экологии_MVP.docx.pdf`, `Критерии_для_формирования_требований_.docx.pdf` и `Тестовое задание.docx.pdf` (для контекста).
2. **История проекта (Что уже сделано):** Файл `Project_AI_Log.md`.
3. **Текущее состояние кода:** Содержимое файла `CombinedCodeFiles.txt`.

## ТВОЙ ПЛАН ДЕЙСТВИЙ:

1. **Проанализируй** текущее состояние кода (`CombinedCodeFiles.txt`) и историю (`Project_AI_Log.md`).
2. **Определи** целевой файл или группу файлов для рефакторинга (например, один контроллер, один сервис, DTO-модели и т.д.).
3. **Примени** к этим файлам следующие улучшения, как того требует текущий этап:
   - **Документирование (Summary):** Добавь XML-комментарии (`<summary>...</summary>`) для всех публичных API-контроллеров, их методов, сервисов, DTO и публичных свойств, где они отсутствуют.
   - **Очистка:** Удали лишние комментарии (например, `// Добавлен AutoMapper`, `// Новый DTO`) и закомментированный (неактивный) код.
   - **Оптимизация (Опционально):** Если заметишь очевидные улучшения (например, упрощение LINQ, более чистое форматирование), примени их.
4. **Сгенерируй** ответ, состоящий из трех четких частей, как описано ниже.**ТВОЙ ОТВЕТ ДОЛЖЕН БЫТЬ В ФОРМАТЕ:**(Сначала ты пишешь краткое (1-2 предложения) **обоснование**, почему ты выбрал именно этот файл(ы) для рефакторинга.)

## ЧАСТЬ 1: КОД ДЛЯ ВНЕДРЕНИЯ

(Здесь ты предоставляешь весь код, который я должен применить в проекте. Ты ОБЯЗАН всегда предоставлять этот блок с кодом. Используй блоки кода с указанием полного имени файла.)

**ВНИМАНИЕ! ПРАВИЛО ПОЛНОГО КОДА:** Это самый важный пункт. Чтобы я мог четко отслеживать изменения, ты ОБЯЗАН:

1. **НИКОГДА НЕ ИСПОЛЬЗОВАТЬ** в блоках кода сокращения, такие как `...`, `// ...` или `[... existing code ...]`.
2. **ДЛЯ НОВЫХ ФАЙЛОВ (New):** Предоставлять **весь** код файла от `namespace` (или `using`) до последней закрывающей скобки `}`. (Хотя на этапе рефакторинга их будет мало).
3. **ДЛЯ ИЗМЕНЕННЫХ ФАЙЛОВ (Modified/Изменения/Рефакторинг):** Предоставлять **ВЕСЬ ПОЛНЫЙ ТЕКСТ** файла от первой до последней строки, а не только измененные строки (не "diff"). Я должен иметь возможность скопировать твой код и полностью заменить им старый файл.

Пример (Рефакторинг): EcologyLK.Api/Controllers/ClientSitesController.cs (Рефакторинг) // ИИ ОБЯЗАН предоставить здесь ВЕСЬ код файла, от `using` до `}`, включив в него <summary> и удалив мусор.

```
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Data;
using EcologyLK.Api.Services;
using EcologyLK.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using EcologyLK.Api.Utils; // Пример using
using System; // Пример using

namespace EcologyLK.Api.Controllers
{
    /// <summary>
    /// Контроллер для управления площадками клиентов (Client Sites).
    /// Включает создание "Анкеты" и получение "Карты требований".
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientSitesController : ControllerBase
    {
        private readonly IRequirementGenerationService _generationService;
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        /// <summary>
        /// Конструктор контроллера ClientSitesController.
        /// </summary>
        /// <param name="generationService">Сервис генерации требований.</param>
        /// <param name="context">Контекст базы данных.</param>
        /// <param name="mapper">Автомаппер.</param>
        public ClientSitesController(
            IRequirementGenerationService generationService,
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _generationService = generationService;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает новую площадку клиента и генерирует для нее "Карту требований".
        /// </summary>
        /// <param name="dto">DTO "Анкеты" для создания площадки.</param>
        /// <returns>Созданную площадку с картой требований.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateClientSite([FromBody] CreateClientSiteDto dto)
        {
            // (Полная симуляция кода метода, без //...)
            var clientId = User.GetClientId();
            if (clientId == null)
            {
                return Unauthorized(new { message = "ClientId не найден в токене." });
            }

            if (dto.ClientId != clientId)
            {
                return Forbid();
            }

            var client = await _context.Clients.FindAsync(dto.ClientId);
            if (client == null)
            {
                return NotFound(new { message = $"Клиент с Id {dto.ClientId} не найден." });
            }

            var requirements = _generationService.GenerateRequirements(dto);

            var clientSite = _mapper.Map<ClientSite>(dto);
            clientSite.EcologicalRequirements = requirements;

            _context.ClientSites.Add(clientSite);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<ClientSiteDto>(clientSite);
            return CreatedAtAction(nameof(GetClientSite), new { id = clientSite.Id }, resultDto);
        }

        /// <summary>
        /// Получает площадку клиента и ее "Карту требований" по ID.
        /// </summary>
        /// <param name="id">ID площадки.</param>
        /// <returns>DTO площадки с требованиями.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientSiteDto>> GetClientSite(int id)
        {
            // (Полная симуляция кода метода, без //...)
            var clientSite = await _context.ClientSites
                .Include(cs => cs.EcologicalRequirements)
                .AsNoTracking()
                .FirstOrDefaultAsync(cs => cs.Id == id);

            if (clientSite == null)
            {
                return NotFound();
            }

            // RLS Check
            var userClientId = User.GetClientId();
            var isAdmin = User.IsAdmin();

            if (!isAdmin && clientSite.ClientId != userClientId)
            {
                return Forbid();
            }

            var dto = _mapper.Map<ClientSiteDto>(clientSite);
            return Ok(dto);
        }
    }
}
```

## **ЧАСТЬ 2: ЗАПИСЬ В ЖУРНАЛ (ДЛЯ Project_AI_Log.md)**

(Это краткая сводка, которую я скопирую в лог. НЕ ВКЛЮЧАЙ СЮДА ПОЛНЫЙ КОД, только список файлов и описание)

### Этап X: Рефакторинг {Название модуля или файла}

### Действия ИИ:

- Проанализировав код [Имя Файла/Модуля], ИИ определил необходимость рефакторинга.
- Добавлены XML-комментарии (`<summary>`) для публичных методов и классов.
- Удалены избыточные ("мусорные") комментарии и закомментированные участки кода.

### Предложенные изменения/артефакты:

- Файл (Рефакторинг): EcologyLK.Api/Controllers/ClientSitesController.cs (Добавлены <summary>, код очищен)
- Файл (Рефакторинг): EcologyLK.Api/Services/RequirementGenerationService.cs (Добавлены <summary>)
- {Здесь список только имен файлов и краткое описание рефакторинга.}

### Предложение ИИ для следующего этапа:

- Код [Имя Файла] очищен.
- Следующим логичным шагом будет рефакторинг [Название следующего файла/модуля, например, ArtifactsController или DTOs].
