namespace CustomControlLibrary.WpfCore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    public class ComplexTypeConverter : TypeConverter
    {
        private static List<Complex> defaultValues = new List<Complex>();

        static ComplexTypeConverter()
        {
            defaultValues.Add(new Complex(0, 0));
            defaultValues.Add(new Complex(1, 1));
            defaultValues.Add(new Complex(-1, 1));
            defaultValues.Add(new Complex(-1, -1));
            defaultValues.Add(new Complex(1, -1));
        }

        // Override CanConvertFrom to return true for String-to-Complex conversions.
        public override bool CanConvertFrom(
            ITypeDescriptorContext context,
            Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        // Override CanConvertTo to return true for Complex-to-String conversions.
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        // Override ConvertFrom to convert from a string to an instance of Complex.
        public override object ConvertFrom(
            ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value)
        {
            string text = value as string;

            if (text != null)
            {
                try
                {
                    return Complex.Parse(text);
                }
                catch (Exception e)
                {
                    throw new Exception(
                        String.Format("Cannot convert '{0}' ({1}) because {2}",
                                        value,
                                        value.GetType(),
                                        e.Message), e);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        // Override ConvertTo to convert from an instance of Complex to string.
        public override object ConvertTo(
            ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value,
            Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            //Convert Complex to a string in a standard format.
            Complex c = value as Complex;

            if (c != null && this.CanConvertTo(context, destinationType))
            {
                return c.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(
            ITypeDescriptorContext context)
        {
            StandardValuesCollection svc = new StandardValuesCollection(defaultValues);
            return svc;
        }
    }
}
