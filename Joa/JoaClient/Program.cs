namespace AppWithPlugin
{
    internal class Program
    {
        private static async Task Main()
        {
            var search = new Search();
            await search.UpdateSearchResults("test");
            var searchResults = search.SearchResults;
            foreach (var searchResult in searchResults)
            {
                Console.WriteLine(searchResult.Title);
            }
        }
    }
}