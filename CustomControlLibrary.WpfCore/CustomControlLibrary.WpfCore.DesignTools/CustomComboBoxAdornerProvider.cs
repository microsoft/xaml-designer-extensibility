using Microsoft.VisualStudio.DesignTools.Extensibility.Interaction;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;
using Microsoft.VisualStudio.DesignTools.Extensibility.Model;
using Microsoft.VisualStudio.DesignTools.Extensibility.Services;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    // AdornerProvider for ComboBox DesignModeValueProvider. When user selects combobox
    // it calls invalides to set registered property on runtime object.
    class CustomComboBoxAdornerProvider : PrimarySelectionAdornerProvider
    {
        public static ModelItem SelectedItem;
        public CustomComboBoxAdornerProvider()
        {
        }

        protected override void Activate(ModelItem item)
        {
            CustomComboBoxAdornerProvider.SelectedItem = item;

            PropertyIdentifier propertyIdentifier = new PropertyIdentifier(new TypeIdentifier("CustomControlLibrary.WpfCore.CustomComboBox"), "IsDropDownOpen");
            item.Context.Services.GetRequiredService<ValueTranslationService>().InvalidateProperty(item, propertyIdentifier);

            base.Activate(item);
        }

        protected override void Deactivate()
        {
            ModelItem cachedItem = CustomComboBoxAdornerProvider.SelectedItem;
            CustomComboBoxAdornerProvider.SelectedItem = null;

            PropertyIdentifier propertyIdentifier = new PropertyIdentifier(new TypeIdentifier("CustomControlLibrary.WpfCore.CustomComboBox"), "IsDropDownOpen");
            cachedItem.Context.Services.GetRequiredService<ValueTranslationService>().InvalidateProperty(cachedItem, propertyIdentifier);

            base.Deactivate();
        }
    }
}
