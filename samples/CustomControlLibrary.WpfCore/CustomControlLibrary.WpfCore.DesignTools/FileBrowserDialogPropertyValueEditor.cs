using Microsoft.VisualStudio.DesignTools.Extensibility.PropertyEditing;
using Microsoft.Win32;
using System.Windows;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    public class FileBrowserDialogPropertyValueEditor : DialogPropertyValueEditor
    {
        private EditorResources res = new EditorResources();

        public FileBrowserDialogPropertyValueEditor()
        {
            this.InlineEditorTemplate = res["InlineEditorTemplate"] as DataTemplate; ;
        }

        public override void ShowDialog(
            PropertyValue propertyValue,
            IInputElement commandSource)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == true)
            {
                propertyValue.StringValue = ofd.FileName;
            }
        }
    }
}
