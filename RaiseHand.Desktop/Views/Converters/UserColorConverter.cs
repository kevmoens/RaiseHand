using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RaiseHand.Desktop.Views.Converters
{
    /// <summary>
    /// <MultiBinding Converter="{StaticResource UserColorConverter}">
    ///     <Binding Path = "{Binding ElementName=_this,Path=DataContext.Name}" />
    ///     < Binding Path="{Binding ElementName=_this, Path=DataContext.RaisedNames }"/>
    /// </MultiBinding>
    /// </summary>
    public class UserColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || DependencyProperty.UnsetValue == values[0] || DependencyProperty.UnsetValue == values[1] || values[1] == null || !((ObservableCollection<string>)values[1]).Contains((string)values[0]))
            {
                return Brushes.Transparent;
            }
            return Brushes.Orange;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
