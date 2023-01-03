# Joa

A platform keystroke launcher with plugins

### Development

Joa relies on the nuget package JoaLauncher.Api, which is hosted on two different platforms:

- [nuget.org](https://www.nuget.org/packages/JoaLauncher.Api)
  The versions that will be used in releases are hosted on nuget.org.

- [github](https://github.com/Joa-Launcher/Plugin-Api/pkgs/nuget/JoaLauncher.Api)
  Versions used during development are hosted on github. To use this versions you need do be [authenticated to github packages](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-to-github-packages):\
   1. Create a [Personal access tokens (classic)](https://github.com/settings/tokens), it has to have the read:packages scope 2. Create a nuget.config file in the ./src folder 3. Paste the following into the nuget.config file
  `XML
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
    ` 4. Replace USERNAME with your username 5. Replace TOKEN with the token from step 1 6.
