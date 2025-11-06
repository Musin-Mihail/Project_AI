using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcologyLK.Api.Data;
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Models;
using EcologyLK.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Controllers;

/// <summary>
/// API контроллер для управления Артефактами (файлами)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ArtifactsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IArtifactStorageService _storageService;

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
    /// GET: api/Artifacts?siteId=5
    /// Получает список артефактов (метаданные) для указанной площадки.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArtifactDto>>> GetArtifactsForSite(
        [FromQuery] int siteId
    )
    {
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
    [HttpPost("Upload")]
    public async Task<ActionResult<ArtifactDto>> UploadArtifact(
        [FromQuery] int siteId,
        [FromQuery] int? requirementId,
        IFormFile file
    )
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Файл не был загружен.");
        }

        // 1. Проверяем, существует ли площадка
        var site = await _context.ClientSites.FindAsync(siteId);
        if (site == null)
        {
            return NotFound($"Площадка с Id {siteId} не найдена.");
        }

        // 2. Генерируем уникальное имя файла для хранения
        var fileExtension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid()}{fileExtension}";

        // 3. Сохраняем файл физически (используя сервис)
        await using (var stream = file.OpenReadStream())
        {
            await _storageService.SaveFileAsync(stream, storedFileName);
        }

        // 4. Создаем запись в БД
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

        // 5. Возвращаем DTO
        var artifactDto = _mapper.Map<ArtifactDto>(artifact);
        return CreatedAtAction(nameof(DownloadArtifact), new { id = artifact.Id }, artifactDto);
    }

    /// <summary>
    /// GET: api/Artifacts/Download/15
    /// Скачивает физический файл артефакта по его ID.
    /// </summary>
    [HttpGet("Download/{id}")]
    public async Task<IActionResult> DownloadArtifact(int id)
    {
        // 1. Находим метаданные файла в БД
        var artifact = await _context.Artifacts.FindAsync(id);
        if (artifact == null)
        {
            return NotFound("Артефакт не найден.");
        }

        // 2. Получаем поток файла из хранилища
        try
        {
            var (fileStream, mimeType) = await _storageService.GetFileStreamAsync(
                artifact.StoredFileName
            );

            // 3. Возвращаем файл
            // fileDownloadName (3й параметр) указывает браузеру оригинальное имя файла
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
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArtifact(int id)
    {
        // 1. Находим метаданные
        var artifact = await _context.Artifacts.FindAsync(id);
        if (artifact == null)
        {
            return NotFound();
        }

        // 2. Удаляем файл из хранилища
        try
        {
            await _storageService.DeleteFileAsync(artifact.StoredFileName);
        }
        catch (FileNotFoundException)
        {
            // Файл уже удален, это нормально
        }

        // 3. Удаляем запись из БД
        _context.Artifacts.Remove(artifact);
        await _context.SaveChangesAsync();

        return NoContent(); // Успешное удаление
    }
}
