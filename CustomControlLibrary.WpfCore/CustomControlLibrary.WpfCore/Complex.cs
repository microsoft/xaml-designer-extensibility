namespace CustomControlLibrary.WpfCore
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    [TypeConverter(typeof(ComplexTypeConverter))]
    public class Complex
    {
        private double realValue;
        private double imaginaryValue;

        public Complex()
        {
        }

        public Complex(double real, double imaginary)
        {
            this.realValue = real;
            this.imaginaryValue = imaginary;
        }

        public double Real
        {
            get
            {
                return realValue;
            }

            set
            {
                realValue = value;
            }
        }

        public double Imaginary
        {
            get
            {
                return imaginaryValue;
            }

            set
            {
                imaginaryValue = value;
            }
        }

        public override string ToString()
        {
            return String.Format(
                CultureInfo.CurrentCulture,
                "{0}{2}{1}",
                this.realValue,
                this.imaginaryValue,
                CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator);
        }

        public static Complex Parse(string complexNumber)
        {
            if (String.IsNullOrEmpty(complexNumber))
            {
                return new Complex();
            }

            // The parts array holds the real and 
            // imaginary parts of the object.
            string[] parts = complexNumber.Split(',');

            if (2 != parts.Length)
            {
                throw new FormatException(
                    String.Format(
                    "Cannot parse '{0}' into a Complex object because " +
                    "it is not in the \"<real>, <imaginary>\" format.",
                    complexNumber));
            }

            return new Complex(double.Parse(parts[0].Trim()), double.Parse(parts[1].Trim()));
        }
    }
}
