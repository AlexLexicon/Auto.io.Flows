using Auto.io.Flows.Application.Models;
using Auto.io.Flows.Application.Services;
using Auto.io.Flows.ViewModels.Configurations;
using Auto.io.Flows.ViewModels.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicon.Common.Wpf.DependencyInjection.Abstractions.Services;
using Lexicon.Common.Wpf.DependencyInjection.Amenities.Abstractions.Services;
using Lexicon.Common.Wpf.DependencyInjection.Amenities.Abstractions.Settings;
using Lexicon.Common.Wpf.DependencyInjection.Mvvm.Abstractions.Factories;
using Microsoft.Extensions.Options;

namespace Auto.io.Flows.ViewModels;
public partial class MainViewModel : ObservableObject
{
    private readonly IDataContextFactory _dataContextFactory;
    private readonly IWindowsDialogService _windowsDialogService;
    private readonly IOptionsMonitor<FileConfiguration> _fileOptions;
    private readonly ISettingsService _settingsService;
    private readonly IFlowService _flowService;

    public MainViewModel(
        IDataContextFactory dataContextFactory,
        IWindowsDialogService windowsDialogService,
        IOptionsMonitor<FileConfiguration> fileOptions,
        ISettingsService settingsService,
        IFlowService flowService)
    {
        _dataContextFactory = dataContextFactory;
        _windowsDialogService = windowsDialogService;
        _fileOptions = fileOptions;
        _settingsService = settingsService;
        _flowService = flowService;
    }

    [ObservableProperty]
    public ObservableObject? _contentViewModel;

    [RelayCommand]
    private void CreateFlow()
    {
        if (ContentViewModel is null || PopupIsOk("Create new Flow?", "Are you sure you want to create a new flow? Any unsaved changes you have will be lost."))
        {
            ContentViewModel = _dataContextFactory.Create<BuilderFlowViewModel>();
        }
    }

    [RelayCommand]
    private void LoadFlow()
    {
        if (ContentViewModel is null || PopupIsOk("Load Flow?", "Are you sure you want to load a flow? Any unsaved changes you have will be lost."))
        {
            FileConfiguration fileConfiguration = _fileOptions.CurrentValue;

            string? filePath = _windowsDialogService.OpenFile(new OpenFileSettings
            {
                InitialDirectory = fileConfiguration.SaveFileDirectory,
            });

            if (filePath is not null)
            {
                var fileInfo = new FileInfo(filePath);

                fileConfiguration.SaveFileDirectory = fileInfo.DirectoryName;

                _settingsService.BindAndSave(fileConfiguration);

                Flow flow = _flowService.LoadFlow(filePath);

                var builderFlowViewModel = _dataContextFactory.Create<BuilderFlowViewModel>();

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
            FileConfiguration fileConfiguration = _fileOptions.CurrentValue;

            string? filePath = _windowsDialogService.OpenFile(new OpenFileSettings
            {
                InitialDirectory = fileConfiguration.SaveFileDirectory,
            });

            if (filePath is not null)
            {
                var fileInfo = new FileInfo(filePath);

                fileConfiguration.SaveFileDirectory = fileInfo.DirectoryName;

                _settingsService.BindAndSave(fileConfiguration);

                Flow flow = _flowService.LoadFlow(filePath);

                ContentViewModel = _dataContextFactory.Create<RunnerFlowViewModel, Flow>(flow);
            }
        }
    }

    private bool PopupIsOk(string title, string message)
    {
        var popup = new Popup(title, message, IsCancellable: true);

        PopupViewModel popupViewModel = _dataContextFactory.Create<PopupViewModel, Popup>(popup);

        return popupViewModel.IsOk;
    }
}
