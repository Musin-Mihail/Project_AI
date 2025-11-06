namespace EcologyLK.Api.Services;

/// <summary>
/// Интерфейс сервиса для физического управления файлами (сохранение, чтение, удаление)
/// </summary>
public interface IArtifactStorageService
{
    /// <summary>
    /// Сохраняет файл в хранилище.
    /// </summary>
    /// <param name="fileStream">Поток данных файла.</param>
    /// <param name="fileName">Уникальное имя файла для сохранения.</param>
    /// <returns>Путь к сохраненному файлу (или идентификатор).</returns>
    Task<string> SaveFileAsync(Stream fileStream, string fileName);

    /// <summary>
    /// Возвращает поток данных файла из хранилища.
    /// </summary>
    /// <param name="storedFileName">Имя файла в хранилище.</param>
    /// <returns>Поток данных (Stream) и MimeType.</returns>
    Task<(Stream FileStream, string MimeType)> GetFileStreamAsync(string storedFileName);

    /// <summary>
    /// Удаляет файл из хранилища.
    /// </summary>
    /// <param name="storedFileName">Имя файла в хранилище.</param>
    Task DeleteFileAsync(string storedFileName);
}
