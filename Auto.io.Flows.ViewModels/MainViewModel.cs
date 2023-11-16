using Auto.io.Flows.Application.Models;
using Auto.io.Flows.Application.Services;
using Auto.io.Flows.ViewModels.Options;
using Auto.io.Flows.ViewModels.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicom.Configuration.Settings;
using Lexicom.Mvvm;
using Lexicom.Wpf.Amenities.Dialogs;
using Microsoft.Extensions.Options;

namespace Auto.io.Flows.ViewModels;
public partial class MainViewModel : ObservableObject
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IWindowsDialog _windowsDialog;
    private readonly IOptionsMonitor<FileDirectoryOptions> _fileDirectoryOptions;
    private readonly ISettingsWriter _settingsWriter;
    private readonly IFlowService _flowService;

    public MainViewModel(
        IViewModelFactory viewModelFactory,
        IWindowsDialog windowsDialog,
        IOptionsMonitor<FileDirectoryOptions> fileDirectoryOptions,
        ISettingsWriter settingsWriter,
        IFlowService flowService)
    {
        _viewModelFactory = viewModelFactory;
        _windowsDialog = windowsDialog;
        _fileDirectoryOptions = fileDirectoryOptions;
        _settingsWriter = settingsWriter;
        _flowService = flowService;
    }

    [ObservableProperty]
    public IDisposable? _contentViewModel;

    [RelayCommand]
    private void CreateFlow()
    {
        if (ContentViewModel is null || PopupIsOk("Create new Flow?", "Are you sure you want to create a new flow? Any unsaved changes you have will be lost."))
        {
            ContentViewModel?.Dispose();

            ContentViewModel = _viewModelFactory.Create<BuilderFlowViewModel>();
        }
    }

    [RelayCommand]
    private void LoadFlow()
    {
        if (ContentViewModel is null || PopupIsOk("Load Flow?", "Are you sure you want to load a flow? Any unsaved changes you have will be lost."))
        {
            Options.FileDirectoryOptions fileConfiguration = _fileDirectoryOptions.CurrentValue;

            string? filePath = _windowsDialog.OpenFile(new OpenFileSettings
            {
                InitialDirectory = fileConfiguration.SaveFileDirectory,
            });

            if (filePath is not null)
            {
                var fileInfo = new FileInfo(filePath);

                fileConfiguration.SaveFileDirectory = fileInfo.DirectoryName;

                _settingsWriter.SaveAndBind(fileConfiguration);

                Flow flow = _flowService.LoadFlow(filePath);

                ContentViewModel?.Dispose();

                var builderFlowViewModel = _viewModelFactory.Create<BuilderFlowViewModel>();

                builderFlowViewModel.LoadFlow(flow);

                ContentViewModel = builderFlowViewModel;
            }
        }
    }

    [RelayCommand]
    private void RunFlow()
    {
        if (ContentViewModel is null || PopupIsOk("Run Flow?", "Are you sure you want to run a flow? Any unsaved changes you have will be lost."))
        {
            Options.FileDirectoryOptions fileConfiguration = _fileDirectoryOptions.CurrentValue;

            string? filePath = _windowsDialog.OpenFile(new OpenFileSettings
            {
                InitialDirectory = fileConfiguration.SaveFileDirectory,
            });

            if (filePath is not null)
            {
                var fileInfo = new FileInfo(filePath);

                fileConfiguration.SaveFileDirectory = fileInfo.DirectoryName;

                _settingsWriter.SaveAndBind(fileConfiguration);

                Flow flow = _flowService.LoadFlow(filePath);

                ContentViewModel?.Dispose();

                ContentViewModel = _viewModelFactory.Create<RunnerFlowViewModel, Flow>(flow);
            }
        }
    }

    private bool PopupIsOk(string title, string message)
    {
        var popup = new Popup(title, message, IsCancellable: true);

        PopupViewModel popupViewModel = _viewModelFactory.Create<PopupViewModel, Popup>(popup);

        popupViewModel.Create();

        return popupViewModel.IsOk;
    }
}
