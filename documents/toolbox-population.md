# Toolbox population

## Toolbox items from unreferenced assemblies (all supported versions of Visual Studio)

This approach requires registering with the Toolbox Controls Installer (TCI) in the Windows registry. The TCI spec is available as a Word document [here](https://www.microsoft.com/en-us/download/details.aspx?id=35536) and explains how to register your assemblies. Once registered, your controls will be shown in the Toolbox for any compatible project, even projects that do not reference your assembly.

### Limitations

* This approach only works reliably for WPF .NET Framework. The controls in registered assemblies are enumerated via .NET Reflection. Reflecting over .NET Core assemblies within a .NET Framework process such as Visual Studio (devenv.exe) is unreliable.
* Since registration requires modifying the registry, you need to provide an installer of some sort.

## Toolbox items from referenced NuGet packages (Visual Studio 15.0 and later)

If a NuGet package contains a [VisualStudioToolsManifest.xml file](https://docs.microsoft.com/en-us/nuget/guides/create-ui-controls) in its tools directory, the Toolbox items listed in the manifest will be shown in the Toolbox for any project that references that package. More details on the manifest can be found .

In Visual Studio 16.6 and later, Toolbox population supports multiple VisualStudioToolsManifest.xml files per package -- the maniest in the tools root plus additional manifests for specific Target Framework Monikers (TFMs) in subdirectories of tools. The Toolbox will show the items from the manifest that best matches the target framework of the current project, falling back to the manifest in the tools root if there is not better match. The manifest in the tools root is also required for compatibility with older versions of Visual Studio.

Here's an example:

| File path                                         | Will be used for projects targetting...                     |
| :------------------------------------------------ | :---------------------------------------------------------- |
| \tools\net47\VisualStudioToolsManifest.xml        | .NET Framework >= 4.7                                       |
| \tools\netcoreapp31\VisualStudioToolsManifest.xml | .NET Core >= 3.1                                            |
| \tools\VisualStudioToolsManifest.xml              | .NET Framework < 4.7, .NET Core < 3.1, and other frameworks |

## Toolbox items from unreferenced NuGet packages (Visual Studio 16.7 Preview 2 and later)

If a NuGet package in a NuGet fallback folder contains a VisualStudioToolsManifest.xml in the root of its tools directory, the Toolbox items listed in the manifest will be shown in the Toolbox for any project that is compatible with the package, even projects that do not reference that package.

### Limitations of the fallback folder approach

* It currently only supports WPF .NET Core packages and projects.
* It only supports one manifest per package, in the root of the tools directory. Manifests for specific Target Framework Monikers described above are not supported.

### Known issues in 16.7 Preview 2

* Custom icons are not supported yet.
* After a package is referenced, Toolbox shows two copies of the Toolbox items.

### Getting started

1. Create a NuGet package containing WPF .NET Core controls and a [VisualStudioToolsManifest.xml file](https://docs.microsoft.com/en-us/nuget/guides/create-ui-controls) in the tools directory.

   * You can either create your own package, or...
   * Build our sample package by doing the following:

     1. Clone this repo.
     2. Open samples\CustomControlLibrary.WpfCore\CustomControlLibrary.WpfCore.sln.
     3. Build solution.
     4. Generate the package (bin\Debug\CustomControlLibrary.WpfCore.1.0.0.nupkg) by right-clicking on the CustomControlLibrary.WpfCore project in Solution Explorer and selecting Pack, or alternatively run the following command from the directory containing CustomControlLibrary.WpfCore.csproj: msbuild /t:Pack

2. Create a directory somewhere on disk to serve as your NuGet fallback folder.
3. Create a new text file with the extension .config (ex. MyFallbackFolder.config) in %ProgramFiles(x86)%\NuGet\Config.
4. Add the following XML to the .config file, replacing c:\MyFallbackFolder with the actual path to your fallback folder.

    ```xml
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <fallbackPackageFolders>
        <add key="FallbackFolderTest" value="c:\MyFallbackFolder" />
      </fallbackPackageFolders>
    </configuration>
    ```

5. Download nuget.exe as described [here](https://docs.microsoft.com/en-us/nuget/reference/nuget-exe-cli-reference).
6. Expand the NuGet package into your fallback folder using the following command:

    ```bat
    nuget.exe add <path to your nupkg> -Source <path to your fallback folder> -Expand
    ```

7. Launch Visual Studio 16.7 Preview 2 or later.
8. Create a new WPF .NET Core project.
9. Open the Toolbox pane
10. Wait for both the Toolbox and XAML designer to finish initializing.
11. The Toolbox items defined in tools\VisualStudioToolsManifest.xml should appear in the Toolbox. If you're using our sample project, you should see a CustomControlLibrary.WpfCore tab in Toolbox containing a Toolbox item for the CustomButton control.
12. Double-click one of the Toolbox items to add a control of that type to the XAML file.
