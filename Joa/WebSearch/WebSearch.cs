using PluginBase;
using System;

namespace HelloPlugin
{
    public class WebSearch : IPlugin
    {
        public string Name { get => "hello"; }
        public string Description { get => "Displays hello message."; }
        
        public bool AcceptNonMatchingSearchString { get => false; }
        
        public List<Func<string, bool>> Matchers { get => new(); }

        public async Task<List<ISearchResult>> GetResults(string searchString)
        {
            throw new NotImplementedException();
        }

        public void Execute(ISearchResult searchResult)
        {
            throw new NotImplementedException();
        }
    }
}