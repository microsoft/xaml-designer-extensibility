using Microsoft.VisualStudio.DesignTools.Extensibility.PropertyEditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    class BrushExtendedPropertyValueEditor : ExtendedPropertyValueEditor
    {
        private EditorResources res = new EditorResources();

        public BrushExtendedPropertyValueEditor()
        {
            this.ExtendedEditorTemplate = res["BrushExtendedEditorTemplate"] as DataTemplate;
            this.InlineEditorTemplate = res["InlineEditorTemplate"] as DataTemplate;
        }
    }
}
