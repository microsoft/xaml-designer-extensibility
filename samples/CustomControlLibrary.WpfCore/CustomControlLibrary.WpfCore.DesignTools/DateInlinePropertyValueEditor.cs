using Microsoft.VisualStudio.DesignTools.Extensibility.PropertyEditing;
using System.Windows;
using System.Windows.Markup;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    class DateInlinePropertyValueEditor : PropertyValueEditor
    {
        private const string editorTemplate =
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
            "<DatePicker SelectedDate=\"{Binding Value, Mode=TwoWay}\" />" +
            "</DataTemplate>";

        public DateInlinePropertyValueEditor()
        {
            DataTemplate template = XamlReader.Parse(editorTemplate) as DataTemplate;
            this.InlineEditorTemplate = template;
        }
    }
}
