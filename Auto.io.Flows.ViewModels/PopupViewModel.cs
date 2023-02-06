using Auto.io.Flows.ViewModels.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicon.Common.Wpf.DependencyInjection.Mvvm.Abstractions;
using System.Windows.Input;

namespace Auto.io.Flows.ViewModels;
public partial class PopupViewModel : ObservableObject, IDataContextCreate, IDataContextShowDialog, IDataContextClose
{
    private readonly Popup _popup;

    public PopupViewModel(Popup popup)
    {
        _popup = popup;

        Title = _popup.Title;
        Description= _popup.Description;
        IsCancellable = _popup.IsCancellable;
    }

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private bool _isCancellable;

    public bool IsCancelled { get; protected set; }
    public bool IsOk { get; protected set; }

    public ICommand? ShowDialogCommand { get; set; }
    public ICommand? CloseCommand { get; set; }

    public void Create()
    {
        ShowDialogCommand?.Execute(null);
    }

    [RelayCommand]
    private void Cancel()
    {
        IsCancelled = true;

        CloseCommand?.Execute(null);
    }

    private bool _isOk = false;
    [RelayCommand]
    private void Ok()
    {
        IsOk = true;

        CloseCommand?.Execute(null);
    }
}
