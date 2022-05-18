using System.Reflection.Metadata;
using System.Text.Json;
using Interfaces.Logger;
using JoaCore;
using Microsoft.Extensions.Logging;

var search = new Search();
//var result = search.Settings.PluginDefinitions.First().PluginSettings.Where(x => x.ListType != null).ToList();
await search.UpdateSearchResults("test");
