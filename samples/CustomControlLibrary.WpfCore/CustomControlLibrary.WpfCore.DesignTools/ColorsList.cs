using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CustomControlLibrary.WpfCore.DesignTools
{
    public class ColorsList : ObservableCollection<Color>
    {
        public ColorsList()
        {
            Type type = typeof(Colors);
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (propertyInfo.PropertyType == typeof(Color))
                {
                    Add((Color)propertyInfo.GetValue(null, null));
                }
            }
        }
    }
}
