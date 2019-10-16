using Microsoft.VisualStudio.DesignTools.Extensibility.Interaction;
using Microsoft.VisualStudio.DesignTools.Extensibility.Model;
using System;
using System.Windows;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    // The following class implements an PlacementAdapter for the 
    // custom buttom. PlacementAdapter is invoked when the 
    // controls are dragged from toolbox to custom button.
    public class CustomButtonPlacementAdapter : PlacementAdapter
    {
        // Returns true if the given coordinate can be set
        public override bool CanSetPosition(PlacementIntent intent, RelativePosition position)
        {
            return false;
        }

        // Returns collection of positions that describe this placement of the item.
        public override RelativeValueCollection GetPlacement(ModelItem item, params RelativePosition[] positions)
        {
            RelativeValueCollection bounds = new RelativeValueCollection();
            return bounds;
        }

        // Retrieves the boundary to the parent edge for the given item
        public override Rect GetPlacementBoundary(ModelItem item, PlacementIntent intent, params RelativeValue[] positions)
        {
            if (item == null) throw new ArgumentNullException("item");

            return new Rect();
        }

        // Retrieves the boundary to the parent edge for the given item
        public override Rect GetPlacementBoundary(ModelItem item)
        {
            if (item == null) throw new ArgumentNullException("item");

            return new Rect();
        }

        // Sets the given collection of positions into the item.
        public override void SetPlacements(ModelItem item, PlacementIntent intent, RelativeValueCollection placement)
        {
            if (item == null) throw new ArgumentNullException("item");

            item.Properties["Width"].SetValue("50");
        }

        // Sets the given set of positions into the item.
        public override void SetPlacements(ModelItem item, PlacementIntent intent, params RelativeValue[] positions)
        {
        }
    }
}