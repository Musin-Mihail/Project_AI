using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcologyLK.Api.Data;
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Models;
using EcologyLK.Api.Services;
using EcologyLK.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Controllers;

/// <summary>
/// API контроллер для управления Артефактами (файлами)
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class ArtifactsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IArtifactStorageService _storageService;

    /// <summary>
    /// Конструктор ArtifactsController
    /// </summary>
    public ArtifactsController(
        AppDbContext context,
        IMapper mapper,
        IArtifactStorageService storageService
    )
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }

    /// <summary>
    /// (Приватный) Проверка, имеет ли пользователь (Клиент или Админ)
    /// доступ к указанной площадке (RLS).
    /// </summary>
    private async Task<bool> CheckSiteAccessAsync(int siteId)
    {
        if (User.IsAdmin())
            return true;

        var userClientId = User.GetClientId();
        if (!userClientId.HasValue)
            return false; // Пользователь не привязан к клиенту

        var site = await _context.ClientSites.FindAsync(siteId);
        return site != null && site.ClientId == userClientId.Value;
    }

    /// <summary>
    /// GET: api/Artifacts?siteId=5
    /// Получает список артефактов (метаданные) для указанной площадки.
    /// </summary>
    /// <param name="siteId">ID площадки, для которой запрашиваются артефакты</param>
    /// <returns>Список DTO Артефактов</returns>
    /// <response code="200">Возвращает список артефактов</response>
    /// <response code="400">Не указан siteId</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Доступ к данной площадке запрещен (RLS)</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ArtifactDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<ArtifactDto>>> GetArtifactsForSite(
        [FromQuery] int siteId
    )
    {
        if (!await CheckSiteAccessAsync(siteId))
        {
            return Forbid("Доступ к данной площадке запрещен.");
        }
        if (siteId <= 0)
        {
            return BadRequest("Необходимо указать siteId.");
        }

        var artifacts = await _context
            .Artifacts.Where(a => a.ClientSiteId == siteId)
            .OrderByDescending(a => a.UploadDate)
            .ProjectTo<ArtifactDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Ok(artifacts);
    }

    /// <summary>
    /// POST: api/Artifacts/Upload?siteId=5&requirementId=10
    /// Загружает новый файл (артефакт) для площадки.
    /// </summary>
    /// <param name="siteId">ID площадки, к которой привязывается файл</param>
    /// <param name="requirementId">(Опционально) ID требования, к которому привязывается файл</param>
    /// <param name="file">Загружаемый файл (IFormFile)</param>
    /// <returns>Созданный DTO Артефакта</returns>
    /// <response code="201">Возвращает созданный DTO артефакта</response>
    /// <response code="400">Файл не был загружен</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Доступ к данной площадке запрещен (RLS)</response>
    /// <response code="404">Площадка не найдена</response>
    [HttpPost("Upload")]
    [ProducesResponseType(typeof(ArtifactDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ArtifactDto>> UploadArtifact(
        [FromQuery] int siteId,
        [FromQuery] int? requirementId,
        IFormFile file
    )
    {
        if (!await CheckSiteAccessAsync(siteId))
        {
            return Forbid("Доступ к данной площадке запрещен.");
        }
        if (file == null || file.Length == 0)
        {
            return BadRequest("Файл не был загружен.");
        }

        var site = await _context.ClientSites.FindAsync(siteId);
        if (site == null)
        {
            return NotFound($"Площадка с Id {siteId} не найдена.");
        }

        var fileExtension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid()}{fileExtension}";

        await using (var stream = file.OpenReadStream())
        {
            await _storageService.SaveFileAsync(stream, storedFileName);
        }

        var artifact = new Artifact
        {
            ClientSiteId = siteId,
            EcologicalRequirementId = requirementId,
            OriginalFileName = file.FileName,
            StoredFileName = storedFileName,
            MimeType = file.ContentType,
            FileSize = file.Length,
            UploadDate = DateTime.UtcNow,
        };

        await _context.Artifacts.AddAsync(artifact);
        await _context.SaveChangesAsync();

        var artifactDto = _mapper.Map<ArtifactDto>(artifact);
        return CreatedAtAction(nameof(DownloadArtifact), new { id = artifact.Id }, artifactDto);
    }

    /// <summary>
    /// GET: api/Artifacts/Download/15
    /// Скачивает физический файл артефакта по его ID.
    /// </summary>
    /// <param name="id">ID артефакта для скачивания</param>
    /// <returns>Файл (FileStreamResult)</returns>
    /// <response code="200">Возвращает файл</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Доступ к данному файлу запрещен (RLS)</response>
    /// <response code="404">Артефакт не найден</response>
    [HttpGet("Download/{id}")]
    [ProducesResponseType(typeof(FileStreamResult), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DownloadArtifact(int id)
    {
        // Устаревший TODO удален (Этап 14 реализовал RLS).
        var artifact = await _context.Artifacts.FindAsync(id);
        if (artifact == null)
        {
            return NotFound("Артефакт не найден.");
        }
        if (!await CheckSiteAccessAsync(artifact.ClientSiteId))
        {
            return Forbid("Доступ к данному файлу запрещен.");
        }
        try
        {
            var (fileStream, mimeType) = await _storageService.GetFileStreamAsync(
                artifact.StoredFileName
            );

            return File(fileStream, mimeType, artifact.OriginalFileName);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// DELETE: api/Artifacts/15
    /// Удаляет артефакт (запись из БД и файл из хранилища).
    /// Доступно только Администраторам.
    /// </summary>
    /// <param name="id">ID артефакта для удаления</param>
    /// <returns>204 No Content</returns>
    /// <response code="204">Артефакт успешно удален</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    /// <response code="404">Артефакт не найден</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteArtifact(int id)
    {
        var artifact = await _context.Artifacts.FindAsync(id);
        if (artifact == null)
        {
            return NotFound();
        }

        try
        {
            await _storageService.DeleteFileAsync(artifact.StoredFileName);
        }
        catch (FileNotFoundException)
        {
            // Файл уже удален, это нормально
        }

        _context.Artifacts.Remove(artifact);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
