## Suggested Actions Extensibility

- [Tokens](./xaml-designer-suggested-actions-extensibility-tokens.md)
- [Features](./xaml-designer-suggested-actions-extensibility-features.md)
- [Behaviors](./xaml-designer-suggested-actions-extensibility-behaviors.md)
- [Non-Visual Elements](./xaml-designer-suggested-actions-extensibility-nonvisualelements.md)
- [Customization](./xaml-designer-suggested-actions-extensibility-customization.md)

In Visual Studio 2019 16.7 Preview 3 we added extensibility support for "XAML Suggested Actions". The feature will be release for GA in 16.10.
>
To enable "Xaml Suggested Actions" for any control, you can create a `SuggestedActionProvider` feature provider ([example](#implementation-for-example-above)) and register it in metadata ([example](#metadata-registration)).

>*A Control can have multiple Suggested Actions providers. Each provider will be shown as a separate tab in the Suggested Actions UI.*

Example:
```cs
public class ExampleButton : Button { }
```

![extensibility-migration-architecture](xaml-suggested-actions.png)

### Implementation for Example Above
```CS
public class ExampleButtonSuggestedActionProvider : SuggestedActionProvider
{
    public static ActionToken Token_Property_IsCancel = new ActionToken(0x1001);
    public static ActionToken Token_Property_IsDefault = new ActionToken(0x1002);
    public static ActionToken Token_Last = new ActionToken(0x10FF);
    
    public override string Header => "Actions";
    
    public override void PrepareActions()
    {
        // Whether or not to display the Name textbox at the top of the tab
        this.ShowNameProperty = true;

        base.PrepareActions();
        
        this.AddGroup(new ActionGroup(SuggestedActionProviderTokens.Token_Group_Common,
            new PropertyAction(SuggestedActionProviderTokens.Token_Property_Content, "Content"),
            new PropertyAction(SuggestedActionProviderTokens.Token_Property_Background, "Background"),
            new PropertyAction(SuggestedActionProviderTokens.Token_Property_IsEnabled, "IsEnabled")
            ));
        
        this.AddGroup(new ActionGroup(SuggestedActionProviderTokens.Token_Group_Specific,
            new PropertyAction(ExampleButtonSuggestedActionProvider.Token_Property_IsCancel,"IsCancel"),
            new PropertyAction(ExampleButtonSuggestedActionProvider.Token_Property_IsDefault,"IsDefault")));
        
        this.AddGroup(new BorderBrushActionGroup());
        this.AddGroup(new VisibilityActionGroup());
        this.AddGroup(new FontSettingsActionGroup());
    }
}
```
### Inheriting Existing Providers Example
Suggested Action Providers can also inherit existing providers. In the below example, the `ExampleSimpleButtonSuggestedActionProvider` extends the `ExampleButtonSuggestedActionProvider`, hides some of the unwanted actions, and adds a few additional actions of its own.

```cs
public class ExampleSimpleButton : ExampleButton { }
```

![extensibility-migration-architecture](xaml-suggested-actions-2.png)

```CS
public class ExampleSimpleButtonSuggestedActionProvider : ExampleButtonSuggestedActionProvider
{
    // Compute the next token property using the last value set in  the provider we're inheriting.
    public static ActionToken Token_Property_CustomProp = ExampleButtonSuggestedActionProviderToken_Last + 1;

    public new static ActionToken Token_Last = new ActionToken(0x2FFF);

    public override void PrepareActions()
    {
        base.PrepareActions();

        // Hide Visibility Group
        this.GetGroupByToken(SuggestedActionProviderTokens.Token_Group_VisibilitySettings).IsVisible =false;
        
        // Hide IsDefault Property
        this.GetActionByToken(ExampleSimpleButtonSuggestedActionProvider.Token_Property_IsDefault)IsVisible = false;
        
        // Add new Opacity (without Token) property after IsDefault (even if it was hidden before)
        this.InsertAction(new PropertyAction("Opacity"), after:ExampleSimpleButtonSuggestedActionProvider.Token_Property_IsDefault);
        
        // Add new Link Action after IsDefault - will be inserted after IsDefault, but before Opacity
        this.InsertAction(new LinkAction("New link", () => { }), after:ExampleSimpleButtonSuggestedActionProvider.Token_Property_IsDefault);
        
        // Subscribe to ModelItem Property Changed Event to update view if needed
        this.ModelItemPropertyChanged +=ExampleSimpleButtonSuggestedActionProvider_ModelItemPropertyChanged;
    }
    
    private void ExampleSimpleButtonSuggestedActionProvider_ModelItemPropertyChanged(object sender,PropertyChangedEventArgs e)
    {
        // Do Work...
    }
}
```

### Metadata Registration
```CS
...
// Add one provider
builder.AddCustomAttributes("CustomControlLibrary.WpfCore.ExampleButton", new FeatureAttribute(typeof(ExampleButtonSuggestedActionProvider)));

// Same for another control
builder.AddCustomAttributes("CustomControlLibrary.WpfCore.ExampleSimpleButton", new FeatureAttribute(typeof(ExampleSimpleButtonSuggestedActionProvider)));
...
```

### Documentation link
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