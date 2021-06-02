## Quick Actions Tokens

Optionally, you can assign each action group and action a token of type `ActionToken`. Using this token, the corresponding action group or action can be referenced and modified. Example:
```CS
this.GetGroupByToken(MyButtonSuggestedActionProvider.Token_Group_VisibilitySettings).IsVisible = false;

this.GetActionByToken(MyButtonSuggestedActionProvider.Token_Property_IsDefault).IsVisible = false;
```
Predefined tokens:
```CS
/// <summary>
/// Predefined Tokens for Actions and ActionGroups
/// Reserved ranges:
/// 0x0000-0x00FF - Groups<br/>
/// 0x0100-0x03FF - Properties<br/>
/// 0x0400-0x05FF - Other Actions<br/>
/// 0x0600-0x0FFF - Reserved<br/>
/// </summary>
public static class SuggestedActionProviderTokens
{
    public static ActionToken Token_Group_Common    = new ActionToken(0x0001);
    public static ActionToken Token_Group_Specific  = new ActionToken(0x0002);
    public static ActionToken Token_Group_BorderBrushSettings = new ActionToken(0x0010);
    public static ActionToken Token_Group_VisibilitySettings  = new ActionToken(0x0011);
    public static ActionToken Token_Group_FontSettings        = new ActionToken(0x0012);
    public static ActionToken Token_Property_Content    = new ActionToken(0x0100);
    public static ActionToken Token_Property_Background = new ActionToken(0x0101);
    public static ActionToken Token_Property_IsEnabled  = new ActionToken(0x0102);
    public static ActionToken Token_Property_Items      = new ActionToken(0x0103);
    public static ActionToken Token_Property_Text       = new ActionToken(0x0104);
}
```