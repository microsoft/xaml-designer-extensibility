using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;
using Microsoft.VisualStudio.DesignTools.Extensibility.Features;
using Microsoft.VisualStudio.DesignTools.Extensibility.PropertyEditing;

// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
[assembly: ProvideMetadata(typeof(CustomControlLibrary.WpfCore.DesignTools.Metadata))]
namespace CustomControlLibrary.WpfCore.DesignTools
{
    // Container for any general design-time metadata to initialize.
    // Designers look for a type in the design-time assembly that 
    // implements IProvideAttributeTable. If found, designers instantiate 
    // this class and access its AttributeTable property automatically.
    internal class Metadata : IProvideAttributeTable
    {
        // Accessed by the designer to register any design-time metadata.
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                // Add the FeatureProviders to the design-time metadata.
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomButton",
                    new FeatureAttribute(typeof(CustomButtonDefaultInitializer)));
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomButton",
                    new FeatureAttribute(typeof(OpacitySliderAdornerProvider)));
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomButton",
                    new FeatureAttribute(typeof(VSThemeAPIDemoAdornerProvider)));
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomButton",
                    new FeatureAttribute(typeof(CustomContextMenuProvider)));
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomButton",
                    new FeatureAttribute(typeof(CustomButtonParentAdapter)));
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomButton",
                    new FeatureAttribute(typeof(CustomButtonPlacementAdapter)));
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomComboBox",
                    new FeatureAttribute(typeof(CustomComboBoxDesignModeValueProvider)));
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomComboBox",
                    new FeatureAttribute(typeof(CustomComboBoxAdornerProvider)));


                // Add the PropertyValueEditors to the design-time metadata.
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomButton", "Date",
                    PropertyValueEditor.CreateEditorAttribute(
                    typeof(DateInlinePropertyValueEditor)));
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomButton", "Background",
                    PropertyValueEditor.CreateEditorAttribute(
                    typeof(BrushExtendedPropertyValueEditor)));
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomButton", "FileName",
                    PropertyValueEditor.CreateEditorAttribute(
                    typeof(FileBrowserDialogPropertyValueEditor)));
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.CustomButton", "ComplexNumber",
                    PropertyValueEditor.CreateEditorAttribute(
                    typeof(ComplexProxyEditor)));

                // Add the TypeConverters to the design-time metadata
                builder.AddCustomAttributes(
                    "System.Globalization.CultureInfo",
                    new System.ComponentModel.TypeConverterAttribute(typeof(CultureInfoNamesConverter)));
                builder.AddCustomAttributes(
                    "CustomControlLibrary.WpfCore.Complex",
                    new System.ComponentModel.TypeConverterAttribute(typeof(ComplexProxyTypeConverter)));

                return builder.CreateTable();
            }
        }
    }
}