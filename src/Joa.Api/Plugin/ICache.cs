namespace JoaLauncher.Api.Plugin;

/// <summary>
/// An ICache can be used to cache information.
/// </summary>
public interface ICache
{
    /// <summary>
    /// Is getting called at the start of the application and after that periodically
    /// How often this method gets called can be adjusted by the user in the settings
    /// The user can also manually call this method via a global search result 
    /// </summary>
    public void UpdateIndexes();
}

/// <summary>
/// An ICache can be used to cache information.
/// </summary>
public interface IAsyncCache
{
    /// <summary>
    /// Is getting called at the start of the application and after that periodically
    /// How often this method gets called can be adjusted by the user in the settings
    /// The user can also manually call this method via a global search result 
    /// </summary>
    public Task UpdateIndexesAsync();
}