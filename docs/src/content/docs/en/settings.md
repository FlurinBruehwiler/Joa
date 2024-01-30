---
title: "Settings"
description: "How plugin settings work"
---

Joa plugins have can easily have settings. All you need to do is have a class that inherits from 'ISetting'
In this class you can define properties, Joa will automatically save and load these settings plugins from
a settings file and display them in the settings UI.

```csharp
public MySettings : ISetting
{
    public string VariableToSave {get; set;}
}
```

### Supported data types
You can use the following primitive data types:
```
bool
float
int
string
```

And you can define lists of classes:
```
List<MyCustomClass>
```
This classes can **only** use the primitive data types mentioned above, they **can't** use Lists.

You can **NOT** define lists of primitive data types or a class directly (without a list).


### UI customization 
You don't have to do anything special for your settings to show up in the settings UI.
All data types have a default appearance in the settings UI, but there is also a way to customize this.

You can change the display name of the property with the `SettingPropertyAttribute`. 
It also allows you to define a description that will be shown when hovering over the name.  

- You can display your string as a color picker with the `ColorAttribute`
- You can display your string as a path file a file picker using the `PathAttribute`
- You can display your float or int as a slider using the `RangeAttribute`

### Initial values

If you don't specify any default value, the following will be used:

```csharp
bool = false
float = 0
int = 0
string = ""
List<T> = new()
```

But you also have the ability to override this:

```csharp
public MySettings : ISetting
{
    public string VariableToSave {get; set;} = "My custom inital value";
}
```

### Save/Load settings

Upon startup the values are read from the settings file.
If Joa detects, that the file has changed, it will reread the file.
When the user edits the settings via the settings UI, the settings will also be saved.


The values are saved to the settings file after the indexes are updated.


If you modify the settings at a time, other than when the indexes are getting updated, your changes won't be saved
to the file or immediately show in the UI. You need to use the `ISettingsManager.Save()` method to update everything
properly.
