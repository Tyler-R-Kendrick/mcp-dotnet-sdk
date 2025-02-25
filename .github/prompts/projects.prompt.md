# Projects

This file describes standards for writing msbuild project types.

## Directory files
Prefer the use of directory files for defining common configurations like:
* TargetFramework
* LangVersion
* etc.

Override these configurations in subdirectories with new directory files, unless you are in the specific project directory; only then should overrides be provided in the project file itself.

Subdirectory directory files should reference the parent directory file like so:
```
    <Import Project="..\Directory.Build.props" />
```

### Directory.Build.props

Define all common properties here.
Also prefer the following:
```
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <CentralPackageTransitivePinningEnabled>true</CentralPackageTransitivePinningEnabled>
    <RootNamespace>$(AssemblyName)</RootNamespace>
```

### Directory.Build.targets

Define all custom build tasks here.

### Directory.Packages.props

Define all packages here. Also define globally referenced packages here.
Prefer the following:
```
  <ItemGroup Label="Global Packages">
    <GlobalPackageReference Include="Nerdbank.GitVersioning" Version="3.5.109" />
  </ItemGroup>
```

## Project Files

Project files should rely on directory files for defining common properties and build tasks. The main things defined in these files should be:
* The Sdk attribute on the root Project node. This should be specific to the project type.
  * Use "MSTest.Sdk/3.6.3" for test projects.
  * Use "Microsoft.NET.Sdk" for class libs.
  * Microsoft.NET.Sdk.Web: Designed for ASP.NET Core web applications, providing necessary web-specific tools and features.
  * Microsoft.NET.Sdk.Razor: Supports Razor components and MVC views, typically used in web applications that utilize Razor syntax.
  * Microsoft.NET.Sdk.Worker: Intended for background services and worker applications, facilitating the creation of long-running services.
  * Microsoft.NET.Sdk.WindowsDesktop: Tailored for Windows desktop applications, including support for Windows Presentation Foundation (WPF) and Windows Forms.
  * <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" /> should be included for ".AppHost" aspire projects.

### Project References

Project references should opt for the shortest valid path to describe the project being built. For example, instead of writing "~/src/Project/Abstractions/Project.Abstractions.csproj", opt for "../Abstractions/*.csproj". Assume one csproj per directory.

### Package References

Assume central package management is enabled unless otherwise stated in the project file.

