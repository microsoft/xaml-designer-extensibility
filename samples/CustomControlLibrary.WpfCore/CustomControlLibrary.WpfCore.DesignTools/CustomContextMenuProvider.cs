using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Interaction;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;
using Microsoft.VisualStudio.DesignTools.Extensibility.Model;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    class CustomContextMenuProvider : PrimarySelectionContextMenuProvider
    {
        private MenuAction setBackgroundToBlueMenuAction;
        private MenuAction clearBackgroundMenuAction;
        private MenuAction addReferenceAction;

        // The provider's constructor sets up the MenuAction objects 
        // and the the MenuGroup which holds them.
        public CustomContextMenuProvider()
        {
            // Set up the MenuAction which sets the control's 
            // background to Blue.
            setBackgroundToBlueMenuAction = new MenuAction("Blue");
            setBackgroundToBlueMenuAction.Checkable = true;
            setBackgroundToBlueMenuAction.Execute +=
                new EventHandler<MenuActionEventArgs>(SetBackgroundToBlue_Execute);

            // Set up the MenuAction which sets the control's 
            // background to its default value.
            clearBackgroundMenuAction = new MenuAction("Cleared");
            clearBackgroundMenuAction.Checkable = true;
            clearBackgroundMenuAction.Execute +=
                new EventHandler<MenuActionEventArgs>(ClearBackground_Execute);

            // Set up the MenuGroup which holds the MenuAction items.
            MenuGroup backgroundFlyoutGroup =
                new MenuGroup("SetBackgroundsGroup", "Set Background");

            // Set up the MenuAction which adds the reference. 
            addReferenceAction = new MenuAction("Add Reference");
            addReferenceAction.Checkable = true;
            addReferenceAction.Execute +=
                new EventHandler<MenuActionEventArgs>(AddReference_Execute);

            // If HasDropDown is false, the group appears inline, 
            // instead of as a flyout. Set to true.
            backgroundFlyoutGroup.HasDropDown = true;
            backgroundFlyoutGroup.Items.Add(setBackgroundToBlueMenuAction);
            backgroundFlyoutGroup.Items.Add(clearBackgroundMenuAction);
            this.Items.Add(backgroundFlyoutGroup);
            this.Items.Add(addReferenceAction);

            // The UpdateItemStatus event is raised immediately before 
            // this provider shows its tabs, which provides the opportunity 
            // to set states.
            UpdateItemStatus +=
                new EventHandler<MenuActionEventArgs>(
                    CustomContextMenuProvider_UpdateItemStatus);
           
        }

        // The following method handles the UpdateItemStatus event.
        // It sets the MenuAction states according to the state
        // of the control's Background property. This method is
        // called before the context menu is shown.
        void CustomContextMenuProvider_UpdateItemStatus(
            object sender,
            MenuActionEventArgs e)
        {
            // Turn everything on, and then based on the value 
            // of the BackgroundProperty, selectively turn some off.
            clearBackgroundMenuAction.Checked = false;
            clearBackgroundMenuAction.Enabled = true;
            setBackgroundToBlueMenuAction.Checked = false;
            setBackgroundToBlueMenuAction.Enabled = true;

            // Get a ModelItem which represents the selected control. 
            ModelItem selectedControl = e.Selection.PrimarySelection;

            // Get the value of the Background property from the ModelItem.
            ModelProperty backgroundProperty =
                selectedControl.Properties["Background"];

            // Set the MenuAction items appropriately.
            if (!backgroundProperty.IsSet)
            {
                clearBackgroundMenuAction.Checked = true;
                clearBackgroundMenuAction.Enabled = false;
            }
            else //if (backgroundProperty.ComputedValue == Brushes.Blue)
            {
                setBackgroundToBlueMenuAction.Checked = true;
                setBackgroundToBlueMenuAction.Enabled = false;
            }
        }

        // The following method handles the Execute event. 
        // It sets the Background property to its default value.
        void ClearBackground_Execute(
            object sender,
            MenuActionEventArgs e)
        {
            ModelItem selectedControl = e.Selection.PrimarySelection;
            selectedControl.Properties["Background"].ClearValue();
        }

        // The following method handles the Execute event. 
        // It sets the Background property to Brushes.Blue.
        void SetBackgroundToBlue_Execute(
            object sender,
            MenuActionEventArgs e)
        {
            ModelItem selectedControl = e.Selection.PrimarySelection;
            selectedControl.Properties["Background"].SetValue(Brushes.Blue);
        }

        // The following method handles the Execute event. 
        // It adds reference using modified AssemblyReferences APIs.
        void AddReference_Execute(
            object sender,
            MenuActionEventArgs e)
        {
            ModelItem selectedControl = e.Selection.PrimarySelection;

            List<AssemblyIdentifier> assemblies = new List<AssemblyIdentifier>();
            assemblies.Add(new AssemblyIdentifier("AddReferenceDemo", "..\\CustomControlLibrary.WpfCore\\bin\\Debug\\netcoreapp3.0\\AddReferenceDemo.dll"));

            AssemblyReferences assemblyReferences = new AssemblyReferences(assemblies);
            selectedControl.Context.Items.SetValue(assemblyReferences);
        }
    }
}