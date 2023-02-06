using System.Windows;
using System.Windows.Controls;

namespace Auto.io.Flows.Views.Controls;
public partial class StepState : UserControl
{
    public StepState() => InitializeComponent();

    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(string), typeof(StepState));
    public string? State
    {
        get => (string?)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }
}
