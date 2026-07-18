namespace Core.Repositories;

using Core.Models;

/// <summary>
/// Repository abstraction for the single-row <see cref="Profile"/> settings entry.
/// The backing table always has either 0 or 1 row.
/// </summary>
public interface IProfileRepository
{
    /// <summary>
    /// Gets the profile, or <see langword="null"/> if it hasn't been saved yet.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="Profile"/>, or <see langword="null"/>.</returns>
    Task<Profile?> GetAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Creates the profile if it doesn't exist yet, otherwise updates it.
    /// </summary>
    /// <param name="entity">The profile to save.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The saved <see cref="Profile"/>.</returns>
    Task<Profile> SaveAsync(Profile entity, CancellationToken cancellationToken);
}
