# How should I reference the XAML designer extensibility binaries?

We recommend that you obtain the XAML designer extensibility DLLs by referencing the [Microsoft.VisualStudio.DesignTools.Extensibility NuGet package](https://www.nuget.org/packages/Microsoft.VisualStudio.DesignTools.Extensibility).

Alternatively, you can also obtain Microsoft.VisualStudio.DesignTools.Extensibility.dll and Microsoft.VisualStudio.DesignTools.Interaction.dll from a Visual Studio install.

# Which version should I reference?

The assembly version of Microsoft.VisualStudio.DesignTools.Extensibility.dll that a designtools DLL references is interpreted as the minimum version of Visual Studio that that DLL supports. So for example, if your designtools DLL references Microsoft.VisualStudio.DesignTools.Extensibility.dll version 16.6, the designer will know that that DLL supports Visual Studio 16.6 and later. It will never be loaded in earlier versions of Visual Studio.

Therefore it is generally best for you to reference the earliest version of Microsoft.VisualStudio.DesignTools.Extensibility which contains the APIs you require, since that will ensure that your designtools DLL will be compatible with as many versions of Visual Studio as possible.

# Shipping multiple versions of a designtools DLL

You may find that you need to ship more than one version of your designtools DLL, each targeting a different version of Visual Studio. For example, you might have basic design-time support for your controls starting in VS 16.4, but due to the introduction of some new designer extensibility API in 16.9 you might be able to provide a better user experience in 16.9 and later. In that case you would want to ship two designtools DLLs, one referencing version 16.4 of Microsoft.VisualStudio.DesignTools.Extensibility.dll and the other referencing version 16.9.

When the designer looks for a designtools DLL for your control assembly and finds more than one, it will choose the DLL that references the highest version of Microsoft.VisualStudio.DesignTools.Extensibility.dll which is less than or equal to the version of Microsoft.VisualStudio.DesignTools.Extensibility.dll used by the designer (i.e. the version that shipped with the running instance of Visual Studio).

The following table shows how the XAML designer in VS 17.0 would react when presented with three designtools DLLs referencing different versions of Microsoft.VisualStudio.DesignTools.Extensibility.dll.

| Referenced version of Microsoft.VisualStudio.DesignTools.Extensibility.dll | Will the designer load it in VS 17.0?                             |
| ---------------------------------------------------------------------------| ----------------------------------------------------------------- |
| 16.8                                                                       | No. It is compatible with 17.0, but a higher version is available |
| 16.10                                                                      | Yes. Closest to the designer's version                            |
| 17.1                                                                       | No. Built against a higher version than the designer              |