## Suggested Actions Extensibility

### Table of Contents
1. [Overview](#Overview)
2. [Screenshot](#Screenshot)
3. [Suggested Action Providers](#Suggested-Action-Providers)
4. [Actions and Action Groups](#Actions-and-Action-Groups)
5. [Sample Implementation](#Sample-Implementation)
6. [Metadata Registration](#Metadata-Registration)
7. [Documentation Link](#Documentation-Link)
8. [Further Reading](#Further-Reading)

### Overview

In Visual Studio 2019 16.7 Preview 3 we added extensibility support for "XAML Suggested Actions". The feature will be release for GA in 16.10.
>
To enable "Xaml Suggested Actions" for any control, you can create a `SuggestedActionProvider` feature provider ([example](#sample-implementation)) and register it in metadata ([example](#metadata-registration)).

A Control can have multiple Suggested Actions providers. Each provider will be shown as a separate tab in the Suggested Actions UI.

### Screenshot

![Example Suggested Action Popup](xaml-suggested-actions.png)


### Suggested Action Providers

The `SuggestedActionProvider` is at the core of implementing custom Suggested Actions for your own control. Each provider assigned to a control appears as a tab within the Suggested Actions UI. For example, in the image above, there are two providers: "Actions" and  "Microsoft Xaml Behaviors".

### Actions and Action Groups

Each provider contains a set of ActionGroups, which themselves contain Actions. These Actions are what appear as property editors in the tab content. Groups are separated by a horizontal line in the tab content. If you need to refer to an Action or ActionGroup programmatically, you can assign it an [Action Token](./xaml-designer-suggested-actions-extensibility-tokens.md).

For a list of available pre-made actions, see [Features](./xaml-designer-suggested-actions-extensibility-features.md). To learn about creating custom actions, see [Customization](./xaml-designer-suggested-actions-extensibility-customization.md).

If a SuggestedActionProvider does not contain any visible actions after `PrepareActions()` completes, it will not be shown in the popup. However, if the provider _becomes_ empty as a result of some action performed while the popup is open, the tab will remain present and display its `OnEmptyDisplayString`.

### Sample Implementation

Below is the code for a sample provider ("Actions" pictured above) with comments to explain what each aspect of the class does.
```cs
public class ButtonActionProvider : SuggestedActionProvider
{
    /*
    * Action Tokens are optional for your groups and actions, but can be useful if you need to
    * refer to them in other areas of the code.
    */
    public static ActionToken Token_Group_Common        = SuggestedActionProviderTokens.Token_Group_Common;
    public static ActionToken Token_Group_Specific      = SuggestedActionProviderTokens.Token_Group_Specific;

    public static ActionToken Token_Property_Background = SuggestedActionProviderTokens.Token_Property_Background;
    public static ActionToken Token_Property_Content    = SuggestedActionProviderTokens.Token_Property_Content;
    public static ActionToken Token_Property_IsEnabled  = SuggestedActionProviderTokens.Token_Property_IsEnabled;

    public static ActionToken Token_Property_IsCancel   = new ActionToken(0x1001);
    public static ActionToken Token_Property_IsDefault  = new ActionToken(0x1002);

    /// <summary>
    /// The Token_Last field is useful if you wish to extend this suggested action provider.
    /// It can be used by child providers to help avoid conflicting token values.
    /// </summary>
    public static ActionToken Token_Last = new ActionToken(0x10FF);

    /// <summary>
    /// Header defines the text string shown in the TabItem's header.
    /// </summary>
    public override string Header => "Actions";

    /// <summary>
    /// AppearsOnInheritedTypes indicates whether this provider should be added to controls
    /// that inherit its primary target control.For example, if a provider on a WPF ListBox
    /// has AppearsOnInheritedTypes set to true, that same provider will appear on WPF ListViews,
    /// because ListView inherits from ListBox. If AppearsOnInheritedTypes is set to false, it will
    /// only appear on ListBox.
    ///
    /// This value defaults to false.
    /// </summary>
    public override bool AppearsOnInheritedTypes => true;

    /// <summary>
    /// The Position field can be used to order the providers within the Tab View.
    /// Lower numbers appear first.
    /// Note that your provider's position must be above ActionProviderBase.MinimumAllowablePosition or it will be ignored.
    /// 
    /// Note: This field was not added until 16.10 Preview 3.
    /// </summary>
    public override int Position => ActionProviderBase.MinimumAllowablePosition + 1;

    /// <summary>
    /// This string will be displayed if the provider has no actions left to display.
    ///
    /// Note that, in general, an empty provider simply will not be added as a tab.
    /// This string is only used if the provider begins with content, but becomes empty
    /// as a result of RemoveAction(...) calls performed while the popup is open.
    /// </summary>
    public override string OnEmptyDisplayString => "No More Actions";

    /// <summary>
    /// Initialize is run when the provider is first created.
    /// You can override this method to perform any class setup needed before other methods are run.
    ///
    /// Note: Initialize can be run very frequently and should be kept as lightweight as possible.
    /// </remarks>
    public override void Initialize()
    {
        base.Initialize();

        // Perform common setup required on provider creation...
    }

    /// <summary>
    /// PrepareActions is where you should setup all of your tab's action groups and actions.
    /// These define the tab's main content inside of the popup.
    /// </summary>
    public override void PrepareActions()
    {
        // This defines whether or not to display the Name textbox at the top of the tab  (defaults to false)
        this.ShowNameProperty = true;

        base.PrepareActions();

        // Note: You must use AddGroup (or AddAction) when adding actions to your provider.
        // *Do not* add them directly to this.ActionGroups.
        this.AddGroup(new ActionGroup(ButtonActionProvider.Token_Group_Common,
            new PropertyAction(ButtonActionProvider.Token_Property_Content, "Content"),
            new PropertyAction(ButtonActionProvider.Token_Property_Background, "Background"),
            new PropertyAction(ButtonActionProvider.Token_Property_IsEnabled, "IsEnabled")
            )); ;

        this.AddGroup(new ActionGroup(ButtonActionProvider.Token_Group_Specific,
            new PropertyAction(ButtonActionProvider.Token_Property_IsCancel, "IsCancel"),
            new PropertyAction(ButtonActionProvider.Token_Property_IsDefault, "IsDefault")));

        // Groups can be created inline (as above), or as separate classes that extend ActionGroup (as below).
        this.AddGroup(new BorderBrushActionGroup());
        this.AddGroup(new VisibilityActionGroup());
        this.AddGroup(new FontSettingsActionGroup());
    }
}
```
*Note: Not shown here, `SuggestedActionProvider` also contains a` GetPromotedActions()` function that can be overriden. **This is not currently supported and should not be used.***

For more information on Token_Last and inheriting an existing provider, see [Provider Inheritance](./xaml-designer-suggested-actions-provider-inheritance.md).

### Metadata Registration
To ensure the provider appears on your control, you must add it in the metadata attribute table as seen below:

```CS
...
// Add one provider
builder.AddCustomAttributes("CustomControlLibrary.WpfCore.ExampleButton", new FeatureAttribute(typeof(ExampleButtonSuggestedActionProvider)));

// Additional provider for another control
builder.AddCustomAttributes("CustomControlLibrary.WpfCore.ExampleSimpleButton", new FeatureAttribute(typeof(ExampleSimpleButtonSuggestedActionProvider)));
...
```

### Documentation Link
![extensibility-migration-architecture](xaml-suggested-actions-documentation.png)

There is a Type Name label at the top of the Suggested Actions dialog.
If desired, this can be used as a hyperlink to documentation. There are two ways to enable it:

1. Specify `DocumentationAttribute` in Metadata:

   ```csharp
   builder.AddCustomAttributes("System.Windows.Controls.ComboBox", 
                               new DocumentationAttribute(helpUrl: "https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox"));
   ```

2. Create `DocumentationProvider` and register it in Metadata:

   ```csharp
   public class ButtonDocumentationProvider : DocumentationProvider
   {
       public override string GetHelpUrl(ModelItem modelItem)
       {
           return "https://docs.microsoft.com/en-us/dotnet/desktop/wpf/controls/button";
       }
   }
   
   // Metatdata registration:
   builder.AddCustomAttributes("System.Windows.Controls.Button",
                               new FeatureAttribute(typeof(ButtonDocumentationProvider));
   ```

   

>- If both `DocumentationAttribute`  and `DocumentationProvider` exist for the same type, `DocumentationAttribute`  will be used.
>- `DocumentationAttribute`  will not be searched in parent classes.
>- If `DocumentationProvider` has not been added, we will use the help URL from the first parent class that has this provider, otherwise the Type Name label will not be a hyperlink.

### Further Reading
- [Action Tokens](./xaml-designer-suggested-actions-extensibility-tokens.md)
- [Features](./xaml-designer-suggested-actions-extensibility-features.md)
- [Provider Inheritance](./xaml-designer-suggested-actions-provider-inheritance.md)
- [Behaviors](./xaml-designer-suggested-actions-extensibility-behaviors.md)
- [Non-Visual Elements](./xaml-designer-suggested-actions-extensibility-nonvisualelements.md)
- [Customization](./xaml-designer-suggested-actions-extensibility-customization.md)