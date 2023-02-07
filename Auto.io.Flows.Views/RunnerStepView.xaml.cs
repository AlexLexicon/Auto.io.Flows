using Auto.io.Flows.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;

namespace Auto.io.Flows.Views;
public partial class RunnerStepView : UserControl
{
    public RunnerStepView() => InitializeComponent();

    private void Test()
    {
        BringIntoView();
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is RunnerStepViewModel vm)
        {
            vm.BringIntoViewCommand = new RelayCommand(Test);
        }
    }
}
