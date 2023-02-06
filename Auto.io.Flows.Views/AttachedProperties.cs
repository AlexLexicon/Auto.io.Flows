using System.Windows;

namespace Auto.io.Flows.Views;
public static class AttachedProperties
{
    public static readonly DependencyProperty IsValidProperty = DependencyProperty.RegisterAttached("IsValid", typeof(bool?), typeof(AttachedProperties));
    public static bool? GetIsValid(DependencyObject obj) => (bool?)obj.GetValue(IsValidProperty);
    public static void SetIsValid(DependencyObject obj, bool? isValid) => obj.SetValue(IsValidProperty, isValid);
}
