using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RaiseHand.Desktop.Views.Converters
{
    public class BoolToVisibility : DependencyObject, IValueConverter
    {
        public Visibility TrueValue
        {
            get { return (Visibility)GetValue(TrueValueProperty); }
            set { SetValue(TrueValueProperty, value); }
        }
        public static DependencyProperty TrueValueProperty = DependencyProperty.RegisterAttached("TrueValue", typeof(Visibility), typeof(BoolToVisibility));

        public Visibility FalseValue
        {
            get { return (Visibility)GetValue(FalseValueProperty); }
            set { SetValue(FalseValueProperty, value); }
        }
        public static DependencyProperty FalseValueProperty = DependencyProperty.RegisterAttached("FalseValue", typeof(Visibility), typeof(BoolToVisibility));
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || value == null || !(value is bool) || (bool)value == false)
            {
                return FalseValue;
            } else
            {
                return TrueValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
