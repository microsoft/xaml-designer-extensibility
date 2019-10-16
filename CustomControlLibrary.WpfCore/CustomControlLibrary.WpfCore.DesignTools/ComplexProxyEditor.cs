namespace CustomControlLibrary.WpfCore.DesignTools
{
    using Microsoft.VisualStudio.DesignTools.Extensibility.PropertyEditing;
    using System.Windows;
    public class ComplexProxyEditor : PropertyValueEditor
    {
        private ComplexProxyEditorResources res = new ComplexProxyEditorResources();

        public ComplexProxyEditor()
        {
            this.InlineEditorTemplate = res["InlineEditorTemplate"] as DataTemplate;
        }
    }
}
