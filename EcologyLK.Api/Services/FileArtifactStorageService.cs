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

    /// <summary>
    /// Конструктор FileArtifactStorageService
    /// </summary>
    /// <param name="env">Web Host Environment (для определения ContentRootPath)</param>
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

    /// <summary>
    /// Сохраняет файл в хранилище (локальная папка 'uploads').
    /// </summary>
    /// <param name="fileStream">Поток данных файла.</param>
    /// <param name="fileName">Уникальное имя файла для сохранения.</param>
    /// <returns>Имя сохраненного файла.</returns>
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

    /// <summary>
    /// Возвращает поток данных файла из хранилища (папка 'uploads').
    /// </summary>
    /// <param name="storedFileName">Имя файла в хранилище.</param>
    /// <returns>Кортеж: Поток данных (Stream) и MimeType.</returns>
    /// <exception cref="FileNotFoundException">Если файл не найден.</exception>
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

    /// <summary>
    /// Удаляет файл из хранилища (папка 'uploads').
    /// </summary>
    /// <param name="storedFileName">Имя файла в хранилище.</param>
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
