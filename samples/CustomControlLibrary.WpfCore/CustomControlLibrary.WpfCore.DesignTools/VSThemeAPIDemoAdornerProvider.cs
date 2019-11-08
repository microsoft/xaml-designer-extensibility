using Microsoft.VisualStudio.DesignTools.Extensibility.Interaction;
using Microsoft.VisualStudio.DesignTools.Extensibility.Model;
using Microsoft.VisualStudio.PlatformUI;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    // The following class implements an adorner provider for the 
    // adorned control. The adorner is a ractangle which is using
    // VS theme APIs to set its background. Purpose of this FeatureProvider
    // is to demo use of VS theme APIs
    class VSThemeAPIDemoAdornerProvider : PrimarySelectionAdornerProvider
    {
        public VSThemeAPIDemoAdornerProvider()
        {
        }

        // The following method is called when the adorner is activated.
        // It creates the adorner control, sets up the adorner panel,
        // and attaches a ModelItem to the adorned control.
        protected override void Activate(ModelItem item)
        {
            // The adorner is a Rectangle element.
            Rectangle r = new Rectangle();
            r.Width = 23.0;
            r.Height = 23.0;

            // Setting the color for rectangle using VS theme APIs
            r.Fill = this.ToBrush(VSColorTheme.GetThemedColor(EnvironmentColors.EnvironmentBackgroundColorKey));

            // Set the rectangle's placement in the adorner panel.
            AdornerPanel.SetAdornerHorizontalAlignment(r, AdornerHorizontalAlignment.OutsideLeft);
            AdornerPanel.SetAdornerVerticalAlignment(r, AdornerVerticalAlignment.OutsideTop);

            AdornerPanel p = new AdornerPanel();
            p.Children.Add(r);

            Adorners.Add(p);

            base.Activate(item);
        }

        // Method to convert System.Drawing.Color to Brush
        private Brush ToBrush(System.Drawing.Color color)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        // The following method deactivates the adorner.
        protected override void Deactivate()
        {
            base.Deactivate();
        }
    }
}