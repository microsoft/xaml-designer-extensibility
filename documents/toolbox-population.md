# Toolbox population

There are several ways to get your XAML controls to appear in Visual Studio's Toolbox pane. Some of the approaches are outlined below.

| Population mechanism                                                           | Supported Frameworks | Supported Visual Studio Versions       |
| :----------------------------------------------------------------------------- | :------------------- | :------------------------------------- |
| [Unreferenced assemblies](#toolbox-items-from-unreferenced-assemblies)         | WPF .NET Framework   | All supported versions                 |
| [Referenced NuGet packages](#toolbox-items-from-referenced-nuget-packages)     | All XAML frameworks  | Visual Studio 15.0 and later           |
| [Unreferenced NuGet packages](#toolbox-items-from-unreferenced-nuget-packages) | WPF .NET Core        | Visual Studio 16.7 Preview 2 and later |
| [UWP Extension SDKs](#toolbox-items-from-uwp-extension-sdks)                   | UWP                  | All supported versions                 |

## Toolbox items from unreferenced assemblies

This approach requires registering with the Toolbox Controls Installer (TCI) in the Windows registry. [The TCI specification](https://www.microsoft.com/en-us/download/details.aspx?id=35536) explains how to register your assemblies. Once registered, your controls will be shown in the Toolbox for any compatible project, even projects that do not reference your assembly.

### Limitations

* This approach is only supported for WPF .NET Framework assemblies.
* Since registration requires modifying the registry, you need to provide an installer of some sort.

## Toolbox items from referenced NuGet packages

If a NuGet package referenced by a XAML project contains a [tools\VisualStudioToolsManifest.xml file](https://docs.microsoft.com/en-us/nuget/guides/create-ui-controls), Toolbox will show the Toolbox items listed in the manifest.

You can try this by adding a reference to your own package or to [our WPF .NET Core sample package](#how-to-build-our-wpf-net-core-sample-package).

## Toolbox items from unreferenced NuGet packages

If a NuGet package in a NuGet fallback folder (see [Getting Started](#getting-started) section below) contains a [tools\VisualStudioToolsManifest.xml file](https://docs.microsoft.com/en-us/nuget/guides/create-ui-controls), Toolbox will show the Toolbox items listed in the manifest for any project that is compatible with the package, even projects that do not reference that package.

### Limitations of the fallback folder approach

* It currently only supports WPF .NET Core packages and projects.

### Getting started

1. Create a NuGet package containing WPF .NET Core controls and a [tools\VisualStudioToolsManifest.xml file](https://docs.microsoft.com/en-us/nuget/guides/create-ui-controls). Note that a UIFramework="WPF" attribute is required on ToolboxItems nodes on Visual Studio 16.7 Preview 3 or above. You could create your own package, or [build our sample package](#how-to-build-our-wpf-net-core-sample-package).
1. Create a new directory somewhere on disk to serve as your NuGet fallback folder.
1. Create a new text file with the extension .config (ex. MyFallbackFolder.config) in %ProgramFiles(x86)%\NuGet\Config.
1. Add the following XML to the .config file:

    ```xml
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <fallbackPackageFolders>
        <add key="My Unique Key" value="c:\MyFallbackFolder" />
      </fallbackPackageFolders>
    </configuration>
    ```

1. Replace "c:\MyFallbackFolder" with the actual path to your fallback folder.
1. Replace "My Unique Key" with some value that is unlikely to be used by other SDKs. Note that if two NuGet .config files declare fallbackPackageFolders with the same key attribute value, one of them will win and the other will be ignored. To avoid unexpected interactions between .config files, be they from different control vendors or different SDKs from the same vendor, you should pick key values that are unlikely to be used by others. For example, consider combining your company and SDK names.
1. Download nuget.exe as described [here](https://docs.microsoft.com/en-us/nuget/reference/nuget-exe-cli-reference).
1. Expand your NuGet package into your fallback folder using the following command. It's important to use nuget.exe rather than simply unzipping your nupkg because nuget.exe will create a .nupkg.metadata file which is required for your package to be found during NuGet restore. When creating an installer for your SDK we recommend that you run nuget.exe on your build/dev machine and include the resulting expanded package layout in your installer, rather than running nuget.exe on the customer's machine.

    ```bat
    nuget.exe add <path to your nupkg> -Source <path to your fallback folder> -Expand
    ```

1. Launch Visual Studio 16.7 Preview 2 or later.
1. Create a new WPF .NET Core project.
1. Open the Toolbox pane
1. Wait for both the Toolbox and XAML designer to finish initializing.
1. The Toolbox items defined in tools\VisualStudioToolsManifest.xml should appear in the Toolbox. If you're using our sample project, you should see a CustomControlLibrary.WpfCore tab in Toolbox containing a Toolbox item for the CustomButton control.
1. Double-click one of the Toolbox items to add a control of that type to the XAML file and add a package reference for the NuGet package to the project.

## TargetFramework-specific manifests

Toolbox population supports multiple VisualStudioToolsManifest.xml files per package -- the manifest in the tools root plus additional manifests for specific Target Framework Monikers (TFMs) in subdirectories of tools. The Toolbox will show the items from the manifest that best matches the target framework of the current project, falling back to the manifest in the tools root if there is no better match. The manifest in the tools root is also required for compatibility with older versions of Visual Studio.

For referenced NuGet packages, this feature requires Visual Studio 16.6 or later. For unreferenced NuGet packages, this feature requires Visual Studio 16.8 Preview 3 or later.  

Here's an example:

| File path                                        | Will be used for projects targeting...                      |
| :----------------------------------------------- | :---------------------------------------------------------- |
| tools\net47\VisualStudioToolsManifest.xml        | .NET Framework >= 4.7                                       |
| tools\netcoreapp31\VisualStudioToolsManifest.xml | .NET Core >= 3.1                                            |
| tools\VisualStudioToolsManifest.xml              | .NET Framework < 4.7, .NET Core < 3.1, and other frameworks |

## How to build our WPF .NET Core sample package

1. Clone [this repo](https://github.com/microsoft/xaml-designer-extensibility).
1. Open [...\samples\CustomControlLibrary.WpfCore\CustomControlLibrary.WpfCore.sln](../samples/CustomControlLibrary.WpfCore/CustomControlLibrary.WpfCore.sln) in Visual Studio.
1. Build the solution.
1. Generate the package (bin\Debug\CustomControlLibrary.WpfCore.1.0.0.nupkg) by right-clicking on the CustomControlLibrary.WpfCore project in Solution Explorer and selecting Pack, or by running the following command from the directory containing CustomControlLibrary.WpfCore.csproj: msbuild /t:Pack

## Toolbox items from UWP Extension SDKs

Visual Studio has supported populating Toolbox with controls from [UWP Extension SDKs](https://docs.microsoft.com/en-us/visualstudio/extensibility/creating-a-software-development-kit?view=vs-2019#ToolboxItems) since Visual Studio 11. Through Visual Studio 16, the list of Toolbox controls for an Extension SDK was determined dynamically by enumerating all of the public control types exposed by the SDK's assemblies. The only control the SDK has over Toolbox population in these older versions of Visual Studio is specifying which assemblies contain controls (via File elements in the manifest) and specifying which tab the Toolbox items should appear in (via the VSCategory attribute on ToolboxItems elements).

Starting in Visual Studio 17.0, Extension SDKs are required to explicitly declare the types that they wish to appear in the Toolbox by listing them in their [SdkManifest.xml file](https://docs.microsoft.com/en-us/visualstudio/extensibility/creating-a-software-development-kit?view=vs-2022#ToolboxItems) as children of the manifest's ToolboxItems elements. Toolbox will show the Toolbox items listed in the manifest for any project that is compatible with the package, even projects that do not reference that package. The behavior of older versions of Visual Studio is unchanged; they will ignore the list of controls in the manifest.

Here is an example:

```xml
<FileList>
  <File Reference="AssemblyName.dll">
    <ToolboxItems VSCategory="ToolboxTabName">
      <Item Type="Namespace.ControlName" />
    </ToolboxItems>
  </File>
</FileList>
```
