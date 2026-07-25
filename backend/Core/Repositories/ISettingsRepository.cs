namespace Core.Repositories;

using Core.Models;

/// <summary>
/// Repository abstraction for the key-value <see cref="Setting"/> store.
/// Settings are addressed by <see cref="Setting.Key"/>, not by Id.
/// </summary>
public interface ISettingsRepository
{
    /// <summary>
    /// Gets all settings.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of <see cref="Setting"/>.</returns>
    Task<IEnumerable<Setting>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Inserts or updates each key/value pair, then returns the full current set of settings.
    /// </summary>
    /// <param name="values">The key/value pairs to upsert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The full, updated list of <see cref="Setting"/>.</returns>
    Task<IEnumerable<Setting>> UpsertManyAsync(IDictionary<string, string> values, CancellationToken cancellationToken);
}
