using JoaPluginsPackage;
using JoaPluginsPackage.Attributes;
using JoaPluginsPackage.Plugin;

namespace Github;

[Plugin("Github", "Interact with Github", "1.0", "Core", "")]
public class Github : IGlobalSearchPlugin
{
    public List<ISearchResult>? GlobalSearchResults { get; set; }
    public void UpdateIndex()
    {
        GlobalSearchResults = new List<ISearchResult>
        {
            new RepositoriesSearchResult()
        };
    }
}