## Quick Actions For Non-Visual Elements

In Visual Studio 2019 16.9 Preview 1, we added support for displaying Quick Actions on non-visual elements.

To display Quick Actions for a non-visual element, the designer needs to know where to place the Quick Actions light bulb adorner. You can create a custom `ViewItemProvider` to provide this information and add it to the non-visual element alongside your `SuggestedActionProvider`.

A `ViewItemProvider` implements one method, `GetViewItem`, which receives the non-visual element as a `ModelItem` and returns a `ViewItem`. This `ViewItem` describes the bounds the designer should use for the non-visual element when placing the Quick Actions light bulb adorner.

### Example
This sample `ViewItemProvider` will lookup  the first visual parent for a non-visual control, then return that parent's view as the `ViewItem`. The designer will then show the Quick Actions light bulb adorner at the corner of the parent's bounds.
```CS
public class MyNonVisualControlViewItemProvider : ViewItemProvider
{
    public override ViewItem GetViewItem(ModelItem modelItem)
    {
        // Find the first visual parent of the non-visual view-item.
        ModelItem parent = modelItem.Parent;
        while (!parent.View.IsVisible)
        {
            parent = parent.Parent;
        }

        return parent.View;
    }
}
```
