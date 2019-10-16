using System.Windows.Controls;
using System.ComponentModel;
using System;
using System.Windows;
using System.Globalization;

namespace CustomControlLibrary.WpfCore
{
    public class CustomButton : Button
    {
        public CustomButton()
        {
            // The GetIsInDesignMode check and the following design-time 
            // code are optional and shown only for demonstration.
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                Content = "Design mode active";
            }
        }

        // Adding a Date property to CustomButton to provide sample code for InlinePropertyValueEditor
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
            "Date",
            typeof(DateTime),
            typeof(CustomButton),
            new PropertyMetadata(DateTime.MinValue, OnDateChanged));

        public DateTime Date
        {
            get
            {
                return (DateTime)this.GetValue(DateProperty);
            }

            set
            {
                this.SetValue(DateProperty, value);
            }
        }

        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomButton presenter = d as CustomButton;
            presenter.Date = (DateTime)e.NewValue;
        }

        // Adding a FileName property to CustomButton to provide sample code for DialogPropertyValueEditor
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
            "FileName",
            typeof(string),
            typeof(CustomButton),
            new PropertyMetadata("File name not set."));

        public string FileName
        {
            get
            {
                return (string)this.GetValue(FileNameProperty);
            }

            set
            {
                this.SetValue(FileNameProperty, value);
            }
        }

        public static readonly DependencyProperty ComplexNumberProperty = DependencyProperty.Register(
            "ComplexNumber",
            typeof(Complex),
            typeof(CustomButton), new PropertyMetadata(new Complex(1, -1)));

        public Complex ComplexNumber
        {
            get
            {
                return (Complex)this.GetValue(ComplexNumberProperty);
            }

            set
            {
                this.SetValue(ComplexNumberProperty, value);
            }
        }

        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
            "Culture",
            typeof(CultureInfo),
            typeof(CustomButton), new PropertyMetadata(null));

        public CultureInfo Culture
        {
            get
            {
                return (CultureInfo)this.GetValue(CultureProperty);
            }

            set
            {
                this.SetValue(CultureProperty, value);
            }
        }
    }
}