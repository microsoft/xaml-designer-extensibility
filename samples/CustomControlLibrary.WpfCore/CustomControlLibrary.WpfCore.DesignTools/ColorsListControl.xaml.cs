using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    /// <summary>
    /// Interaction logic for ColorsListControl.xaml
    /// </summary>
    public partial class ColorsListControl : System.Windows.Controls.UserControl
    {
        public ColorsListControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorsListControl), new FrameworkPropertyMetadata(null));
        public Color SelectedColor
        {
            get { return (Color)base.GetValue(SelectedColorProperty); }
            set { base.SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register("SelectedBrush", typeof(SolidColorBrush), typeof(ColorsListControl), new FrameworkPropertyMetadata(null));
        public SolidColorBrush SelectedBrush
        {
            get { return (SolidColorBrush)base.GetValue(SelectedBrushProperty); }
            set { base.SetValue(SelectedBrushProperty, value); }
        }

        private void ItemsControl_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = (Color)((Button)sender).Tag;
            SelectedBrush = new SolidColorBrush(SelectedColor);
        }
    }
}
