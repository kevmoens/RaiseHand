using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RaiseHand.Desktop.Views.Converters
{
    public class UserEnabledConverter : IMultiValueConverter
    {
        /// <MultiBinding Converter = "{StaticResource UserEnabledConverter}" >
        ///     <Binding Path="{Binding ElementName=_this,Path=DataContext.Name}"/>
        ///     <Binding Path = "{Binding }" />
        /// </ MultiBinding >
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || DependencyProperty.UnsetValue == values[0] || values[0] == null || values[1] == null || values[0].ToString() != values[1].ToString())
            {
                return false;
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
