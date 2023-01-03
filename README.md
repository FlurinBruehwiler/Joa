# Joa

Joa is a cross-platform command-palette for everything. It has similarities to Spotlight, Alfred and Powertoys Run. 
For a detailed comparison on what makes Joa better than the alternatives, see [here](#comparison-to-alternatives). Joa is currently still in development, for a list of tasks that need to be done bevore Joa can release, see [here](#todo-for-10-release).

### Features
- Cross-plattform (Windows, Mac, Linux)
- Extensible via Plugins (C#)
- UI can be replaced (The UI is also a plugin)
- Search Results can act as "directories"
- Completly free of charge and open source (MIT Licence)

### Plugins
Joa offers a bunch a default plugins which cover the most common use cases. But you can also import plugins other people have created or even create your own. The Joa plugin api is very flexible and easy to use.

The main purpose of plugins is to provide search result, but it is important to know that the ui itself is also a plugin. So if you dont like the default UI, you could theoretically create your own.

### ToDo for 1.0 release
Planned release date is the 29. April

- Create documentation
- Create landing page
- Create more default plugins
- Imlement a way to specify a Github repository as a plugin source and add automatic updates
- Create release process
- Implement settings ui

### Comparison to alternatives
todo

### Development
Joa relies on the nuget package JoaLauncher.Api, which is hosted on two different platforms:

- The versions that will be used in releases are hosted on [nuget.org](https://www.nuget.org/packages/JoaLauncher.Api). 

- Versions used during development are hosted on [github](https://github.com/Joa-Launcher/Plugin-Api/pkgs/nuget/JoaLauncher.Api). To use these versions you need do be [authenticated to github packages](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-to-github-packages):
    1. Create a [Personal access tokens (classic)](https://github.com/settings/tokens), it has to have the read:packages scope
    2. Create a nuget.config file in the ./src folder
    3. Paste the following into the nuget.config file
    ```XML
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
        <packageSources>
            <add key="github" value="https://nuget.pkg.github.com/Joa-Launcher/index.json" />
        </packageSources>
        <packageSourceCredentials>
            <github>
                <add key="Username" value="USERNAME" />
                <add key="ClearTextPassword" value="TOKEN" />
            </github>
        </packageSourceCredentials>
        <packageSourceMapping>
            <packageSources key="github">
                <package pattern="*" />
            </packageSources>
        </packageSourceMapping>
    </configuration>
    ```
    4. Replace USERNAME with your username
    5. Replace TOKEN with the token from step 1
