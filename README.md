<p align="center">
  <a href="#">
    
  </a>
  <p align="center">
   <img width="150" height="150" src="./images/logo.png" alt="Logo">
  </p>
  <h1 align="center"><b>Joa</b></h1>
  
</p>

Joa is a cross-platform command-palette for everything. It has similarities to Spotlight, Alfred and Powertoys Run. 
For a detailed comparison on what makes Joa better than the alternatives, see [here](#comparison-to-alternatives). Joa is currently still in development, for a list of tasks that need to be done before Joa can release, see [here](#todo-for-10-release).

### Features
- Cross-platform (Windows, Mac, Linux)
- Extensible via Plugins (C#)
- UI can be customized
- Search Results can act as "directories"
- Completely free of charge and open source (MIT Licence)

### Plugins
Joa offers a bunch of default plugins which cover the most common use cases. But you can also import plugins other people have created or even create your own. The Joa plugin api is very flexible and easy to use.

### ToDo for 1.0 release
Planned release date is the 29. April 2023

- Create documentation
- Create landing page
- Create more default plugins
- Implement a way to specify a GitHub repository as a plugin source and add automatic updates
- Create release process
- Implement settings UI

### Comparison to alternatives
|               | Cross platform | Free of charge        | Plugin support  | Open source | Customizable UI | Nested Search Results |
|---------------|----------------|-----------------------|-----------------|-------------|-----------------|-----------------------|
| Spotlight     | MacOS          | Yes                   | No              | No          |                 |                       |
| Alfred        | MacOS          | Yes (without plugins) | Yes (only paid) | No          |                 |                       |
| Raycast       | MacOS          | Yes                   | Yes             | No          |                 |                       |
| Powertoys     | Windows        | Yes                   | No              | Yes         |                 |                       |
| Fluent Search | Windows        | Yes                   | Yes             | No          |                 |                       |
| Flow-Launcher | Windows        | Yes                   | Yes             | Yes         |                 |                       |
| Cerebro       | Yes            | Yes                   | Yes             | Yes         |                 |                       |
| Wox           | Windows        | Yes                   | Yes             | Yes         |                 |                       |
| Keypirinha    | Windows        | Yes                   | Yes             | No          |                 |                       |
| Ueli          | Windows, MacOS | Yes                   | No              | Yes         | Yes             | No                    |
| Script Kit    |                |                       |                 |             |                 |                       |
| paletro       |                |                       |                 |             |                 |                       |
| Joa           | Yes            | Yes                   | Yes             | Yes         | Yes             | Yes                   |

### Tech
Joa uses a fork of the [Photino](https://www.tryphotino.io/) framework in combination with [Blazor](https://github.com/tryphotino/photino.Blazor).
However, in the future, we intend to switch to a custom UI framework that uses Skia for rendering.

### Development
Joa relies on the NuGet package JoaLauncher.Api, which is hosted on two different platforms:

- The versions that will be used in releases are hosted on [nuget.org](https://www.nuget.org/packages/JoaLauncher.Api). 
- Versions used during development are hosted on [GitHub](https://github.com/Joa-Launcher/Plugin-Api/pkgs/nuget/JoaLauncher.Api). To use these versions, you need to be [authenticated to GitHub packages](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-to-github-packages):
  1. Create a [Personal access token (classic)](https://github.com/settings/tokens). Make sure it has the read:packages scope.
  2. Create a `nuget.config` file in the `./src` folder
  3. Paste the following into the nuget.config file
  ```xml
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
   4. Replace USERNAME with your GitHub username
   5. Replace TOKEN with the token from step 1
