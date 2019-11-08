using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;
using Microsoft.VisualStudio.DesignTools.Extensibility.Model;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    // DesignModeValueProvider support for selector control. When user selects combobox
    // CustomComboBoxAdornerProvider call the invalidates which calls TranslatePropertyValue to set value
    // on runtime object for registered properties (in this case IsDropDownOpen).
    class CustomComboBoxDesignModeValueProvider : DesignModeValueProvider
    {
        public CustomComboBoxDesignModeValueProvider()
        {
            Properties.Add(new TypeIdentifier("System.Windows.Controls.ComboBox"), "IsDropDownOpen");
        }

        public override object TranslatePropertyValue(ModelItem item, PropertyIdentifier identifier, object value)
        {
            if (identifier.DeclaringTypeIdentifier.Name.Equals("System.Windows.Controls.ComboBox") &&
                identifier.Name == "IsDropDownOpen")
            {
                return (bool)value | CustomComboBoxAdornerProvider.SelectedItem == item;
            }

            return base.TranslatePropertyValue(item, identifier, value);
        }
    }
}
