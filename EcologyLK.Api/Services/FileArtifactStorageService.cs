using Microsoft.AspNetCore.StaticFiles;

namespace EcologyLK.Api.Services;

/// <summary>
/// Реализация сервиса хранения, использующая локальную файловую систему.
/// (Для MVP)
/// </summary>
public class FileArtifactStorageService : IArtifactStorageService
{
    private readonly string _storagePath;
    private readonly IContentTypeProvider _contentTypeProvider;

    public FileArtifactStorageService(IWebHostEnvironment env)
    {
        // Сохраняем файлы в папку "uploads" в корне проекта
        _storagePath = Path.Combine(env.ContentRootPath, "uploads");
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }

        _contentTypeProvider = new FileExtensionContentTypeProvider();
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
    {
        var filePath = Path.Combine(_storagePath, fileName);

        await using (var outputStream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(outputStream);
        }

        // Возвращаем просто имя файла, т.к. путь нам известен
        return fileName;
    }

    public Task<(Stream FileStream, string MimeType)> GetFileStreamAsync(string storedFileName)
    {
        var filePath = Path.Combine(_storagePath, storedFileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл не найден в хранилище.", storedFileName);
        }

        // Определяем MimeType по расширению файла
        if (!_contentTypeProvider.TryGetContentType(storedFileName, out var mimeType))
        {
            mimeType = "application/octet-stream"; // Тип по умолчанию
        }

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult(((Stream)fileStream, mimeType));
    }

    public Task DeleteFileAsync(string storedFileName)
    {
        var filePath = Path.Combine(_storagePath, storedFileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
