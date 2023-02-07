using Auto.io.Flows.Application;
using Auto.io.Flows.Application.Models;
using Auto.io.Flows.Application.Models.Steps;
using Auto.io.Flows.Application.Services;
using Auto.io.Flows.ViewModels.Configurations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicon.Common.Wpf.DependencyInjection.Abstractions.Services;
using Lexicon.Common.Wpf.DependencyInjection.Amenities.Abstractions.Services;
using Lexicon.Common.Wpf.DependencyInjection.Amenities.Abstractions.Settings;
using Lexicon.Common.Wpf.DependencyInjection.Mvvm.Abstractions.Factories;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Auto.io.Flows.ViewModels;
public partial class BuilderFlowViewModel : ObservableObject, IDisposable
{
    private readonly Guid _keyboardHandlerId;

    private readonly IDataContextFactory _dataContextFactory;
    private readonly IStepService _stepService;
    private readonly IWindowsDialogService _windowsDialogService;
    private readonly IFlowService _flowService;
    private readonly IOptionsMonitor<FileConfiguration> _fileOptions;
    private readonly ISettingsService _settingsService;
    private readonly IMouseService _mouseService;
    private readonly IKeyboardService _keyboardService;

    public BuilderFlowViewModel(
        IDataContextFactory dataContextFactory,
        IStepService stepService,
        IWindowsDialogService windowsDialogService,
        IFlowService flowService,
        IOptionsMonitor<FileConfiguration> fileOptions,
        ISettingsService settingsService,
        IMouseService mouseService,
        IKeyboardService keyboardService)
    {
        _keyboardHandlerId = Guid.NewGuid();

        _dataContextFactory = dataContextFactory;
        _stepService = stepService;
        _windowsDialogService = windowsDialogService;
        _flowService = flowService;
        _fileOptions = fileOptions;
        _settingsService = settingsService;
        _mouseService = mouseService;
        _keyboardService = keyboardService;

        StepIdentifiers = _stepService.GetAllStepIdentifiers();
        SelectedStepIdentifier = StepIdentifiers.First();
        BuilderStepViewModels = new ObservableCollection<BuilderStepViewModel>();
        IsEmpty = true;
        MouseHotKeys = IKeysService.KEYS;
        SelectedMouseHotKey = MouseHotKeys.First();
    }

    [ObservableProperty]
    private bool _isSavable;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private IEnumerable<string> _stepIdentifiers = null!;

    [ObservableProperty]
    private string _selectedStepIdentifier = null!;

    [ObservableProperty]
    private ObservableCollection<BuilderStepViewModel> _builderStepViewModels = null!;

    [ObservableProperty]
    private int _mousePositionX;

    [ObservableProperty]
    private int _mousePositionY;

    [ObservableProperty]
    private IReadOnlyList<string> _mouseHotKeys = null!;

    private string? _selectedMouseHotKey;
    public string? SelectedMouseHotKey
    {
        get => _selectedMouseHotKey;
        set
        {
            _selectedMouseHotKey = value;
            OnSelectedMouseHotKeyChanged();
            OnPropertyChanged();
        }
    }

    public ICommand? ActivateWindowCommand { get; set; }

    private void Update()
    {
        bool isValid = BuilderStepViewModels.All(s => s.IsValid);

        IsEmpty = !BuilderStepViewModels.Any();
        IsSavable = !IsEmpty && isValid;
    }

    private bool _isDisposed;
    public void Dispose()
    {
        _keyboardService.UnRegisterKeyReleased(_keyboardHandlerId);
        _isDisposed = true;
    }

    public void LoadFlow(Flow flow)
    {
        if (flow.Steps is not null)
        {
            foreach (FlowStep flowStep in flow.Steps)
            {
                IStep step = _stepService.GetStepByIdentifier(flowStep.Identifer);

                AddStep(step, builderStepViewModel =>
                {
                    builderStepViewModel.LoadFlowParameters(flowStep.Parameters);
                });
            }
        }
    }

    private void OnSelectedMouseHotKeyChanged()
    {
        if (!_isDisposed)
        {
            if (SelectedMouseHotKey is not null)
            {
                _keyboardService.RegisterKeyReleased(_keyboardHandlerId, SelectedMouseHotKey, () =>
                {
                    (int x, int y) = _mouseService.GetPosition();

                    MousePositionX = x;
                    MousePositionY = y;
                });
            }
        }
    }

    [RelayCommand]
    private void OnSave()
    {
        FileConfiguration fileConfiguration = _fileOptions.CurrentValue;

        string? filePath = _windowsDialogService.SaveFile(new SaveFileSettings
        {
            DefaultExtension = "json",
            InitialDirectory = fileConfiguration.SaveFileDirectory,
            FileName = "UntitledFlow",
            EnsureValidNames = true,
            Title = "Save Flow",
        });

        if (filePath is not null)
        {
            var fileInfo = new FileInfo(filePath);

            fileConfiguration.SaveFileDirectory = fileInfo.DirectoryName;

            _settingsService.BindAndSave(fileConfiguration);

            var flowSteps = new List<FlowStep>();
            foreach (BuilderStepViewModel builderStepViewModel in BuilderStepViewModels)
            {
                flowSteps.Add(builderStepViewModel.GetFlowStep());
            }

            _flowService.SaveFlow(filePath, new Flow
            {
                Steps = flowSteps
            });
        }
    }

    [RelayCommand]
    private void AddStep()
    {
        IStep step = _stepService.GetStepByIdentifier(SelectedStepIdentifier);

        AddStep(step, null);
    }

    private void AddStep(IStep step, Action<BuilderStepViewModel>? configure)
    {
        var builderStepViewModel = _dataContextFactory.Create<BuilderStepViewModel, IStep>(step);

        if (step is MouseMoveStep)
        {
            builderStepViewModel.LoadFlowParameters(new List<FlowParameter>
            {
                new FlowParameter
                {
                    Identifier = "",
                    Value = MousePositionX,
                },
                new FlowParameter
                {
                    Identifier = "",
                    Value = MousePositionY,
                }
            });
        }

        builderStepViewModel.Updated += Update;
        builderStepViewModel.Delete += () => DeleteStep(builderStepViewModel);
        builderStepViewModel.MoveUp += () => MoveStepUp(builderStepViewModel);
        builderStepViewModel.MoveDown += () => MoveStepDown(builderStepViewModel);

        configure?.Invoke(builderStepViewModel);

        BuilderStepViewModels.Add(builderStepViewModel);

        Update();
    }

    private void DeleteStep(BuilderStepViewModel builderStepViewModel)
    {
        BuilderStepViewModels.Remove(builderStepViewModel);

        Update();
    }

    private void MoveStepUp(BuilderStepViewModel builderStepViewModel)
    {
        int currentIndex = BuilderStepViewModels.IndexOf(builderStepViewModel);

        BuilderStepViewModels.Remove(builderStepViewModel);

        if (currentIndex < 0)
        {
            throw new UnreachableException("The BuilderStepViewModel was not in the Steps collection.");
        }

        int newIndex = Math.Max(currentIndex - 1, 0);

        BuilderStepViewModels.Insert(newIndex, builderStepViewModel);
    }

    private void MoveStepDown(BuilderStepViewModel builderStepViewModel)
    {
        int currentIndex = BuilderStepViewModels.IndexOf(builderStepViewModel);

        BuilderStepViewModels.Remove(builderStepViewModel);

        if (currentIndex < 0)
        {
            throw new UnreachableException("The BuilderStepViewModel was not in the Steps collection.");
        }

        int newIndex = Math.Min(currentIndex + 1, BuilderStepViewModels.Count);

        BuilderStepViewModels.Insert(newIndex, builderStepViewModel);
    }
}
