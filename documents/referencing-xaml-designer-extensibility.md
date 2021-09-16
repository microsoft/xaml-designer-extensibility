# How should I reference the XAML designer extensibility binaries?

We recommend that you obtain the XAML designer extensibility DLLs by referencing the [*Microsoft.VisualStudio.DesignTools.Extensibility* NuGet package](https://www.nuget.org/packages/Microsoft.VisualStudio.DesignTools.Extensibility).

Alternatively, you can also obtain *Microsoft.VisualStudio.DesignTools.Extensibility.dll* and *Microsoft.VisualStudio.DesignTools.Interaction.dll* from a Visual Studio install.

# Which version should I reference?

The assembly version of *Microsoft.VisualStudio.DesignTools.Extensibility.dll* that a *designtools.dll* references is interpreted as the minimum version of Visual Studio that that DLL supports. So for example, if your *designtools.dll* references *Microsoft.VisualStudio.DesignTools.Extensibility.dll* version 16.6, the designer will know that that DLL supports Visual Studio 16.6 and later. It will never be loaded in earlier versions of Visual Studio.

Therefore it is generally best for you to reference the earliest version of *Microsoft.VisualStudio.DesignTools.Extensibility* which contains the APIs you require, since that will ensure that your *designtools.dll* will be compatible with as many versions of Visual Studio as possible.

## Visual Studio interop APIs breaking changes in VS 17.0

If you are consuming Visual Studio interop APIs (ex. EnvDTE) from your *designtools.dll*, be aware that there were a large number of [interop API breaking changes in VS 17.0](https://docs.microsoft.com/en-us/visualstudio/extensibility/migration/breaking-api-list?view=vs-2022). Attempting to call the old, VS 16.x versions of these APIs will cause the XAML designer in VS 17.0 to crash. If the VS interop APIs you are using were impacted by these changes and you want your *designtools.dll* to work properly in VS 17.0 and earlier versions of Visual Studio, you will need to ship a two versions of your *designtools.dll*:

- For VS 17.0 support: A *designtools.dll* that references the new VS 17.0 interop assemblies and the 17.0 version of *Microsoft.VisualStudio.DesignTools.Extensibility.dll*
- For VS 16.x support: A *designtools.dll* that references the old VS 16.x interop assemblies and a 16.x version of *Microsoft.VisualStudio.DesignTools.Extensibility.dll*

# Shipping multiple versions of a *designtools.dll*

You may find that you need to ship more than one version of your *designtools.dll*, each targeting a different version of Visual Studio. For example, you might have basic design-time support for your controls starting in VS 16.4, but due to the introduction of some new designer extensibility API in 16.9 you might be able to provide a better user experience in 16.9 and later. In that case you would want to ship two *designtools.dlls*, one referencing version 16.4 of *Microsoft.VisualStudio.DesignTools.Extensibility.dll* and the other referencing version 16.9.

When the designer looks for a *designtools.dll* for your control assembly and finds more than one, it will choose the *designtools.dll* that references the highest version of *Microsoft.VisualStudio.DesignTools.Extensibility.dll* which is less than or equal to the version of *Microsoft.VisualStudio.DesignTools.Extensibility.dll* used by the designer (i.e. the version that shipped with the running instance of Visual Studio).

The following table shows how the XAML designer in VS 17.0 would react when presented with three *designtools.dlls* referencing different versions of *Microsoft.VisualStudio.DesignTools.Extensibility.dll*.

| Referenced version of *Microsoft.VisualStudio.DesignTools.Extensibility.dll* | Will the designer load it in VS 17.0?                             |
| -----------------------------------------------------------------------------| ----------------------------------------------------------------- |
| 16.8                                                                         | No. It is compatible with 17.0, but a higher version is available |
| 16.10                                                                        | Yes. Closest to the designer's version                            |
| 17.1                                                                         | No. Built against a higher version than the designer              |