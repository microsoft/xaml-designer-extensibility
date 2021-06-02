## Inheriting Existing Providers Example
Suggested Action Providers can inherit existing providers. In the example below, the `ExampleSimpleButtonSuggestedActionProvider` extends the `ExampleButtonSuggestedActionProvider`, hides some of the unwanted actions, and adds a few additional actions of its own.

Note the use of the base class' `Token_Last` property to ensure there are no conflicting `ActionToken` values between the inherited provider and the base provider.

## Sample Image
![extensibility-migration-architecture](xaml-suggested-actions-2.png)

## Sample Code
```cs
public class ExampleSimpleButton : ExampleButton { }
```

```CS
public class ExampleSimpleButtonSuggestedActionProvider : ExampleButtonSuggestedActionProvider
{
    // Compute the next token property using the last value set in  the provider we're inheriting.
    public static ActionToken Token_Property_CustomProp = ExampleButtonSuggestedActionProvider.Token_Last + 1;

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