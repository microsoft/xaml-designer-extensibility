
# XAML designer extensibility migration

In Visual Studio 2019, the XAML designer supports two different architectures: the designer isolation architecture and the more recent surface isolation architecture. This architecture transition is required to support target runtimes that can't be hosted in a .NET Framework process. Moving to the surface isolation architecture introduces breaking changes to the third-party extensibility model. This article outlines these changes, which are available in Visual Studio 2019 starting with version 16.3.

**Designer isolation** is used by the WPF designer for projects that target the .NET Framework and supports *.design.dll* extensions. User code, control libraries, and third-party extensions are loaded in an external process (*XDesProc.exe*) along with the actual designer code and designer panels.

**Surface isolation** is used by the UWP designer. It's also used by the WPF designer for projects that target .NET Core. In surface isolation, only user code and control libraries are loaded in a separate process, while the designer and its panels are loaded in the Visual Studio process (*DevEnv.exe*). The runtime used for executing user code and control libraries is different from that used by the .NET Framework for the actual designer and third-party extensibility code.

![extensibility-migration-architecture](xaml-designer-extensibility-migration-architecture.png)

Because of this architecture transition, third-party extensions are no longer loaded into the same process as the third-party control libraries. The extensions can no longer have direct dependencies on control libraries or directly access run-time objects. Extensions that were previously written for the designer isolation architecture using the *Microsoft.Windows.Extensibility.dll* API must be migrated to a new approach to work with the surface isolation architecture. In practice, an existing extension will need to be compiled against new extensibility API assemblies. Access to run-time control types via [typeof](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/typeof) or run-time instances must be replaced or removed because control libraries are now loaded in a different process.

## New extensibility API assemblies

The new extensibility API assemblies are similar to the existing extensibility API assemblies but follow a different naming scheme in order to differentiate them. Similarly, the namespace names have changed to reflect the new assembly names.

| Designer isolation API assembly            | Surface isolation API assembly                       |
|:------------------------------------------ |:---------------------------------------------------- |
| Microsoft.Windows.Design.Extensibility.dll | Microsoft.VisualStudio.DesignTools.Extensibility.dll |
| Microsoft.Windows.Design.Interaction.dll   | Microsoft.VisualStudio.DesignTools.Interaction.dll   |

## New file extension and discovery

Instead of using the *.design.dll* file extension, new surface extensions will be discovered by using the *.designtools.dll* file extension. *.design.dll* and *.designtools.dll* extensions can exist in the same *Design* subfolder.

While third-party control libraries are compiled for the actual target runtime (.NET Core or UWP), the *.designtools.dll* extension should always be compiled as a .NET Framework assembly.

## Decouple attribute tables from run-time types

The surface isolation extensibility model doesn't allow for extensions to depend on actual control libraries, and therefore, extensions can't reference types from the control library. For example, *MyLibrary.designtools.dll* can't have a dependency on *MyLibrary.dll*.

Such dependencies were most common when registering metadata for types via attribute tables. Extension code that references control library types directly via [typeof](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/typeof) or [GetType](https://docs.microsoft.com/en-us/dotnet/visual-basic/language-reference/operators/gettype-operator) is substituted in the new APIs by using string-based type names:

```csharp
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;
using Microsoft.VisualStudio.DesignTools.Extensibility.Features;
using Microsoft.VisualStudio.DesignTools.Extensibility.Model;

[assembly: ProvideMetadata(typeof(AttributeTableProvider))]

public class AttributeTableProvider : IProvideAttributeTable
{
  public AttributeTable AttributeTable
  {
    get
    {
      var builder = new AttributeTableBuilder();
      builder.AddCustomAttributes("MyLibrary.MyControl", new DescriptionAttribute(Strings.MyControlDescription);
      builder.AddCustomAttributes("MyLibrary.MyControl", new FeatureAttribute(typeof(MyControlDefaultInitializer));
      return builder.CreateTable();
    }
  }
}
```

```vb
Imports Microsoft.VisualStudio.DesignTools.Extensibility.Metadata
Imports Microsoft.VisualStudio.DesignTools.Extensibility.Features
Imports Microsoft.VisualStudio.DesignTools.Extensibility.Model

<Assembly: ProvideMetadata(GetType(AttributeTableProvider))>

Public Class AttributeTableProvider
    Implements IProvideAttributeTable

    Public ReadOnly Property AttributeTable As AttributeTable Implements IProvideAttributeTable.AttributeTable
        Get
            Dim builder As New AttributeTableBuilder
            builder.AddCustomAttributes("MyLibrary.MyControl", New DescriptionAttribute(Strings.MyControlDescription))
            builder.AddCustomAttributes("MyLibrary.MyControl", New FeatureAttribute(GetType(MyControlDefaultInitializer)))
            Return builder.CreateTable()
        End Get
    End Property
End Class
```

Starting in Visual Studio 2019 16.4, the `AttachedPropertyBrowsableForTypeAttribute` is supported when added directly to a runtime control library. In a *.designtools.dll* extension `AttachedPropertyBrowsableForTypeIdentifierAttribute` should be used instead to reference the type using a string name.

## Feature providers and Model API

Feature providers are implemented in extension assemblies and loaded in the Visual Studio process. `FeatureAttribute` will continue to reference feature provider types directly using [typeof](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/typeof).

Currently, the following feature providers are supported:

* `DefaultInitializer`
* `AdornerProvider`
* `ContextMenuProvider`
* `ParentAdapter`
* `PlacementAdapter`
* `DesignModeValueProvider` is supported with the limitation that `TranslatePropertyValue` will be called via `InvalidateProperty` or when modified in the designer. It will not be called when modified in run-time code.

Because feature providers are now loaded in a process different from the actual run-time code and control libraries, they are no longer able to access run-time objects directly. Instead, all such interactions must be converted to use the corresponding Model-based APIs. The Model API has been updated, and access to [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type) or [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) is either no longer available or has been replaced with `TypeIdentifier` and `TypeDefinition`.

`TypeIdentifier` represents a string without an assembly name identifying a type. A `TypeIdenfifier` can be resolved to a `TypeDefinition` to query additional information about the type. `TypeDefinition` instances can't be cached in extension code.

```csharp
TypeDefinition type = ModelFactory.ResolveType(
    item.Context, new TypeIdentifier("MyLibrary.MyControl"));
TypeDefinition buttonType = ModelFactory.ResolveType(
    item.Context, new TypeIdentifier("System.Windows.Controls.Button"));
if (type?.IsSubclassOf(buttonType) == true)
{
}
```

```vb
Dim type As TypeDefinition = ModelFactory.ResolveType(
    item.Context, New TypeIdentifier("MyLibrary.MyControl"))
Dim buttonType As TypeDefinition = ModelFactory.ResolveType(
    item.Context, New TypeIdentifier("System.Windows.Controls.Button"))
If type?.IsSubclassOf(buttonType) Then

End If
```

APIs removed from the surface isolation extensibility API set:

* `ModelFactory.CreateItem(EditingContext context, object item)`
* `ViewItem.PlatformObject`
* `ModelProperty.DefaultValue`
* `AssemblyReferences.GetTypes(Type baseType)`

APIs that use `TypeIdentifier` instead of [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type):

* `ModelFactory.CreateItem(EditingContext context, Type itemType, params object[] arguments)`
* `ModelFactory.CreateItem(EditingContext context, Type itemType, CreateOptions options, params object[] arguments)`
* `ModelFactory.CreateStaticMemberItem(EditingContext context, Type type, string memberName)`
* `ModelFactory.ResolveType(EditingContext context, Type)` changed to `MetadataFactory.ResolveType(EditingContext context, TypeIdentifier typeIdentifier)`
* `ModelService.ResolveType(TypeIdentifier typeIdentifier)` changed to `MetadataService.ResolveType(TypeIdentifier typeIdentifier)`
* `ViewItem.ItemType`
* `ModelEvent.EventType`
* `ModelEvent.IsEventOfType(Type type)`
* `ModeItem.IsItemOfType(Type type)`
* `ModelParent.CanParent(EditingContext context, ModelItem parent, Type childType)`
* `ModelParent.FindParent(EditingContext context, Type childType, ModelItem startingItem)`
* `ModelParent.FindParent(Type childType, GestureData gestureData)`
* `ModelProperty.IsPropertyOfType(Type type)`
* `ParentAdpater.CanParent(ModelItem parent, Type childType)`
* `ParentAdapter.RedirectParent(ModelItem parent, Type childType)`

APIs that use `TypeIdentifier` instead of [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type) and no longer support constructor arguments:

* `ModelFactory.CreateItem(EditingContext context, TypeIdentifier typeIdentifier, params object[] arguments)`
* `ModelFactory.CreateItem(EditingContext context, TypeIdentifier typeIdentifier, CreateOptions options, params object[] arguments)`

APIs that use `TypeDefinition` instead of [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type):

* `ValueTranslationService.GetProperties(Type itemType)`
* `ValueTranslationService.HasValueTranslation(Type itemType, PropertyIdentifier identifier)`
* `ValueTranslationService.TranslatePropertyValue(Type itemType, ModelItem item, PropertyIdentifier identifier, object value)`
* `ModelService.Find(ModelItem startingItem, Type type)`
* `ModelService.Find(ModelItem startingItem, Predicate<Type> match)`
* `ModelItem.ItemType`
* `ModelProperty.AttachedOwnerType`
* `ModelProperty.PropertyType`
* `FeatureManager.CreateFeatureProviders(Type featureProviderType, Type type)`
* `FeatureManager.CreateFeatureProviders(Type featureProviderType, Type type, Predicate<Type> match)`
* `FeatureManager.InitializeFeatures(Type type)`
* `FeatureManager.GetCustomAttributes(Type type, Type attributeType)`
* `AdapterService.GetAdapter<TAdapterType>(Type itemType)`
* `AdapterService.GetAdapter(Type adapterType, Type itemType)`
* `PropertyEntry.PropertyType`

APIs that use `AssemblyIdentifier` instead of `<xref:System.Reflection.AssemblyName?displayProperty=fullName>`:

* `AssemblyReferences.ReferencedAssemblies`
* `AssemblyReferences.LocalAssemblyName` changed to `AssemblyReferences.LocalAssemblyIdentifier`

Furthermore, `ModelItem` APIs like `SetValue` will only support instances of primitive types or built-in .NET Framework types which can be converted for the target runtime. Currently these types are supported:

* Primitive .NET Framework types: `Boolean`, `Byte`, `Char`, `DateTime`, `Double`, `Enum`, `Guid`, `Int16`, `Int32`, `Int64`, `Nullable`, `SByte`, `Single`, `String`, `Type`, `UInt16`, `UInt32`, `UInt64`, `Uri`
* Known WPF .NET Framework types (and derived types): `Brush`, `Color`, `CompositeTransform`, `CornerRadius`, `Duration`, `EasingFunctionBase`, `EasingMode`, `EllipseGeometry`, `FontFamily`, `GeneralTransform`, `Geometry`, `GradientStopCollection`, `GradientStop`, `GridLength`, `ImageSource`, `InlineCollection`, `Inline`, `KeySpline`, `Material`, `Matrix`, `PathFigureCollection`, `PathFigure`, `PathSegmentCollection`, `PathSegment`, `Path`, `PointCollection`, `Point`, `PropertyPath`, `Rect`, `RepeatBehavior`, `Setter`, `Size`, `StaticResource`, `TextAlignment`, `TextDecorationCollection`, `ThemeResourceExtension`, `Thickness`, `TimeSpan`, `Transform3D`, `TransformCollection`
* Starting in Visual Studio 2019 16.4, [Proxy Objects](#proxy-objects) can be defined to map custom types.

For example:

```csharp
using Microsoft.VisualStudio.DesignTools.Extensibility.Features;
using Microsoft.VisualStudio.DesignTools.Extensibility.Model;

public class MyControlDefaultInitializer : DefaultInitializer
{
  public override void InitializeDefaults(ModelItem item)
  {
    item.Properties["Width"].SetValue(800d);
    base.InitializeDefaults(item);
  }
}
```

```vb
Imports Microsoft.VisualStudio.DesignTools.Extensibility.Features
Imports Microsoft.VisualStudio.DesignTools.Extensibility.Model

Public Class MyControlDefaultInitializer
    Inherits DefaultInitializer

    Public Overrides Sub InitializeDefaults(item As ModelItem)
        item.Properties!Width.SetValue(800.0)
        MyBase.InitializeDefaults(item)
    End Sub
End Class
```

More code samples are available in the [samples](../samples) folder.

## Proxy Objects

Starting in Visual Studio 2019 16.4, a design-time proxy type can be defined as part of an extension along with a `TypeConverter` to map to and from a serialized represenation. When using `PropertyValueEditor`, `ModelItem.GetCurrentValue` or `ModelProperty.ComputedValue`, the designer will serialize the object in the runtime process and deserialize into a proxy instance in the process the extension is running in.

## Limited support for .design.dll extensions

If any *.designtools.dll* extension is discovered for a control library, it is loaded first and discovery for *.design.dll* extensions is skipped.

If no *.designtools.dll* extensions are present but a *.design.dll* extension is found, the XAML Language Service attempts to load this assembly to extract the attribute table information to support basic editor and property inspector scenarios. This mechanism is limited in scope. It doesn't allow loading of designer isolation extensions to execute feature providers but might provide basic support for existing WPF control libraries.
