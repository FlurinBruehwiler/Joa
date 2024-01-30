---
title: "Getting started"
description: "Docs intro"
---

**Welcome to the Joa Plugin API Documentation!**


## Intro
Joa plugins are written in `C#`, but you should be able to follow this tutorial with a basic 
understanding of any programming language.

## Setup environment
- Download and install the [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- Install the Joa plugin template with: 
```
dotnet new install THE NUGET PACKAGE
```
- Create a new project based on the template with: 
```
dotnet new joaplugin -o "MyFirstPlugin"
```
- Build the project with
```
cd ./MyFirstPlugin
dotnet build
```
- You can see, that this created a new Folder: `PluginBuild`. You need to configure Joa, so that it knows where it 
should look for your plugin. For this, go to the Joa settings and add the path.

- You should now be able to test out your plugin. Use Joa to search for `MyFirstPlugin`. You should see a search result
that was provided by your plugin.


## Understanding the template
Open the project in your favourite Editor 
([Visual Studio Code](https://code.visuaxlstudio.com/), 
[Rider](https://www.jetbrains.com/rider/), 
[Visual Studio](https://visualstudio.microsoft.com/))

The template generated a C# file which contains a 
class that implement the `IPlugin` interface. Every plugin must contain **exactly** one of these.

You can also see that the class has a constructor which take an `IJoaLogger` as an argument.
Plugins can use the IJoaLogger to log stuff to the log file. The log file can be opened by 
searching for `Joa log file` in Joa (Developer mode needs to be enabled). Plugins can make use of 
Dependency Injection (see here for more detail). 

The method `ConfigurePlugin(IPluginBuilder builder)` comes from the IPlugin interface and is 
the entry point for plugins. You can use the `IPluginBuilder` to configure your plugin.

In the template we add a global search result which just prints something to the log when executed.




