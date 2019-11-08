using Microsoft.VisualStudio.DesignTools.Extensibility.Interaction;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;
using Microsoft.VisualStudio.DesignTools.Extensibility.Model;
using System;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    // The following class implements an ParentAdapter for the 
    // custom button. ParentAdapter is invoked when the 
    // controls are dragged from toolbox to custom button.
    internal class CustomButtonParentAdapter : ParentAdapter
    {
        // The following method is called when any control is dropped over custom button
        // from toolbox. It checks if custombutton can take this child or not.
        // It might be called at other times too.
        public override bool CanParent(ModelItem parent, TypeIdentifier childType)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (childType == null) throw new ArgumentNullException("childType");

            return (childType.Equals(new TypeIdentifier("System.Windows.Controls.TextBox")));
        }

        // The following method is called when allowed control is dropped over custom button
        // from toolbox. It adds the child and changes the text.
        public override void Parent(ModelItem newParent, ModelItem child)
        {
            if (newParent == null) throw new ArgumentNullException("newParent");
            if (child == null) throw new ArgumentNullException("child");

            using (ModelEditingScope scope = newParent.BeginEdit())
            {
                child.Properties["Text"].SetValue("Button Child");
                newParent.Content.Collection.Add(child);

                scope.Complete();
            }

        }

        // This method can redirect from one parent to another.
        public override ModelItem RedirectParent(ModelItem parent, TypeIdentifier childType)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (childType == null) throw new ArgumentNullException("childType");

            return base.RedirectParent(parent, childType);
        }

        // The following method is called when child control is dragged away to different parent.
        // Here we are removing the text.
        public override void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child)
        {
            if (currentParent == null) throw new ArgumentNullException("currentParent");
            if (newParent == null) throw new ArgumentNullException("newParent");
            if (child == null) throw new ArgumentNullException("child");

            using (ModelEditingScope scope = child.BeginEdit())
            {
                child.Properties["Text"].SetValue("");
                newParent.Content.Collection.Remove(child);
                scope.Complete();
            }
        }
    }
}