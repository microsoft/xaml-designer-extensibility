using Microsoft.VisualStudio.DesignTools.Extensibility.PropertyEditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    class DateInlinePropertyValueEditor : PropertyValueEditor
    {
        private const string editorTemplate =
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
            "<DatePicker SelectedDate=\"{Binding StringValue, Mode=TwoWay}\" />" +
            "</DataTemplate>";

        public DateInlinePropertyValueEditor()
        {
            DataTemplate template = XamlReader.Parse(editorTemplate) as DataTemplate;
            this.InlineEditorTemplate = template;
        }
    }
}
