## Suggested Actions Extensibility

- [Tokens](./xaml-designer-suggested-actions-extensibility-tokens.md)
- [Features](./xaml-designer-suggested-actions-extensibility-features.md)
- [Behaviors](./xaml-designer-suggested-actions-extensibility-behaviors.md)
- [Customization](./xaml-designer-suggested-actions-extensibility-customization.md)

Starting from Visual Studio 2019 16.7 Preview 3 we added extensibility support for "XAML Suggested Actions"
>*Note: this API will be available only in Preview Channel until feature is finalized and completed*

To enable "Xaml Suggested Actions" for any control, `SuggestedActionProvider` feature provider should be created and registered in metadata.

>*Control could have multiple Suggested Actions providers. Each separate provider will be shown as a separate tab in UI.*

Example for Button:

![extensibility-migration-architecture](xaml-suggested-actions.png)

### Implementation for example above:
```CS
public class ButtonActionProvider : SuggestedActionProvider
{
    public static ActionToken Token_Group_Common        = SuggestedActionProviderTokens.Token_Group_Common;
    public static ActionToken Token_Group_Specific      = SuggestedActionProviderTokens.Token_Group_Specific;
    public static ActionToken Token_Property_Background = SuggestedActionProviderTokens.Token_Property_Background;
    public static ActionToken Token_Property_Content    = SuggestedActionProviderTokens.Token_Property_Content;
    public static ActionToken Token_Property_IsEnabled  = SuggestedActionProviderTokens.Token_Property_IsEnabled;
    public static ActionToken Token_Property_IsCancel   = new ActionToken(0x1001);
    public static ActionToken Token_Property_IsDefault  = new ActionToken(0x1002);
    public static ActionToken Token_Last = new ActionToken(0x10FF);
    
    public override string Header { get => "Actions"; }
    public override string Type { get => "System.Windows.Controls.Button"; }
    
    public override void Initialize()
    {
        this.ShowNameProperty = true;
        base.Initialize();
        this.AddGroup(new ActionGroup(ButtonActionProvider.Token_Group_Common,
            new PropertyAction(ButtonActionProvider.Token_Property_Content, "Content"),
            new PropertyAction(ButtonActionProvider.Token_Property_Background, "Background"),
            new PropertyAction(ButtonActionProvider.Token_Property_IsEnabled, "IsEnabled")
            )); ;
        this.AddGroup(new ActionGroup(ButtonActionProvider.Token_Group_Specific,
            new PropertyAction(ButtonActionProvider.Token_Property_IsCancel, "IsCancel"),
            new PropertyAction(ButtonActionProvider.Token_Property_IsDefault, "IsDefault")));
        this.AddGroup(new BorderBrushActionGroup());
        this.AddGroup(new VisibilityActionGroup());
        this.AddGroup(new FontSettingsActionGroup());
    }
}
```
### Example for `MyButtonSuggestedActionProvider : ButtonActionProvider`
```CS
public class MyButtonSuggestedActionProvider : ButtonActionProvider
{
    public static ActionToken Token_Property_CustomProp = ButtonActionProvider.Token_Last + 1;
    public new static ActionToken Token_Last = new ActionToken(0x2FFF);
    
    public override string Type { get => "CustomControlLibrary.MyButton"; }

    public override void Initialize()
    {
        base.Initialize();

        //Hide Visibility Group
        this.GetGroupByToken(SuggestedActionProviderTokens.Token_Group_VisibilitySettings).IsVisible = false;

        //Hide IsDefault Property
        this.GetActionByToken(MyButtonSuggestedActionProvider.Token_Property_IsDefault).IsVisible = false;

        //Add new Opacity (without Token) property after IsDefault (even if it was hidden before)
        this.InsertAction(new PropertyAction("Opacity"), after: MyButtonSuggestedActionProvider.Token_Property_IsDefault);

        //Add new Link Action after IsDefault - will be inserted after IsDefault, but before Opacity
        this.InsertAction(new LinkAction("New link", () => { }), after: MyButtonSuggestedActionProviderToken_Property_IsDefault);
        
        //Subscribe to ModelItem Property Changed Event to update view if needed
        this.ModelItemPropertyChanged += MyButtonSuggestedActionProvider_ModelItemPropertyChanged;
    }
    
    private void MyButtonSuggestedActionProvider_ModelItemPropertyChanged(object sender, System.ComponentModelPropertyChangedEventArgs e)
    {
    }
}
```

### Metadata registration
```CS
...
//Add SuggestionsAttribute to enabled "XAML Suggested Actions for control"
builder.AddCustomAttributes("System.Windows.Controls.Button", new SuggestionsAttribute());

//Add one provider
builder.AddCustomAttributes("System.Windows.Controls.Button", new FeatureAttribute(typeof(ButtonActionProvider)));

//Same for another control
builder.AddCustomAttributes("CustomControlLibrary.MyButton", new SuggestionsAttribute());
builder.AddCustomAttributes("CustomControlLibrary.MyButton", new FeatureAttribute(typeof(MyButtonSuggestedActionProvider)));
...
```

### Documentation link
At the top of 'XAML Suggested Actions' there is a Type Name label.
It could be used as a hyperlink to documentation. To enabled it, specify `DocumentationAttribute` in Metadata:
```cs
builder.AddCustomAttributes("System.Windows.Controls.Button", 
                            new DocumentationAttribute(helpLink: "https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.button"));
```
>*If `DocumentationAttribute` has not been added, Type Name label will be static*