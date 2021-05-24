## SuggestedActionProvider Breakdown

### Overview

The SuggestedActionProvider is at the core of implementing custom Suggested Actions for your own control. Each provider assigned to a control appears as a tab within the Suggested Actions UI. For example, in the image below, there are two providers: "Actions" and  "Microsoft Xaml Behaviors".

![Example Suggested Action Popup](xaml-suggested-actions.png)

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