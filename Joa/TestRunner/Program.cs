using JoaCore;

var search = new Search();
search.Settings.SaveSettingsToJson();
search.Settings.UpdateSettingsFromJson();
await search.UpdateSearchResults("test");