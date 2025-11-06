using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcologyLK.Api.Data;
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Models;
using EcologyLK.Api.Services;
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
        // TODO: Добавить RLS - проверку, что у пользователя есть доступ к siteId
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
        // TODO: Добавить RLS - проверку, что у пользователя есть доступ к siteId
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
    [HttpGet("Download/{id}")]
    public async Task<IActionResult> DownloadArtifact(int id)
    {
        // TODO: Добавить RLS - проверку, что у пользователя есть доступ к этому артефакту
        var artifact = await _context.Artifacts.FindAsync(id);
        if (artifact == null)
        {
            return NotFound("Артефакт не найден.");
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
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
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
