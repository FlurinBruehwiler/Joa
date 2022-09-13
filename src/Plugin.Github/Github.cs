using JoaPluginsPackage;
using JoaPluginsPackage.Plugin;

namespace Github;

public class Github : IGlobalSearchPlugin
{
    public List<ISearchResult>? GlobalSearchResults { get; set; }
    public void UpdateIndex()
    {
        throw new NotImplementedException();
    }
}