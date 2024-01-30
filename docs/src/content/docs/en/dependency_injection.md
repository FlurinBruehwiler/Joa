---
title: "Dependency Injection"
description: "Dependency injection withing Joa Plugins"
---

With Joa plugins the main way to make use of Dependency Injection is the constructor.

You can make use of Dependency Injection in classes that implement the following interfaces.

```csharp
IProvider
IProvider<T>
IPlugin
ISetting
ICache
IAsyncCache
```

You can for example inject the ISettings in an IProvider
 
```csharp
public class MyProvider : IProvider
{
    //MySettings implements ISetting
    private readonly MySettings _settings;

    public MyProvider(MySettings settings)
    {
        _settings = settings;
    }

    public List<SearchResult> GetSearchResults(string searchString){}
}
```


You can inject the builtin Injectables:
```csharp
IBrowserHelper
IIconHelper
IJoaLogger
LogLevel
```

