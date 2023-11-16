using Auto.io.Flows.Application.Models;
using Auto.io.Flows.Application.Models.Steps;
using Auto.io.Flows.Application.Services;
using Auto.io.Flows.ViewModels.Options;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicom.Configuration.Settings;
using Lexicom.Mvvm;
using Lexicom.Wpf.Amenities.Dialogs;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Auto.io.Flows.ViewModels;
public partial class BuilderFlowViewModel : ObservableObject, IDisposable
{
    private readonly Guid _keyboardHandlerId;

    private readonly IViewModelFactory _viewModelFactory;
    private readonly IStepService _stepService;
    private readonly IWindowsDialog _windowsDialog;
    private readonly IFlowService _flowService;
    private readonly IOptionsMonitor<FileDirectoryOptions> _fileDirectoryOptions;
    private readonly ISettingsWriter _settingsWriter;
    private readonly IMouseService _mouseService;
    private readonly IKeyboardService _keyboardService;

    public BuilderFlowViewModel(
        IViewModelFactory viewModelFactory,
        IStepService stepService,
        IWindowsDialog windowsDialog,
        IFlowService flowService,
        IOptionsMonitor<FileDirectoryOptions> fileDirectoryOptions,
        ISettingsWriter settingsWriter,
        IMouseService mouseService,
        IKeyboardService keyboardService)
    {
        _keyboardHandlerId = Guid.NewGuid();

        _viewModelFactory = viewModelFactory;
        _stepService = stepService;
        _windowsDialog = windowsDialog;
        _flowService = flowService;
        _fileDirectoryOptions = fileDirectoryOptions;
        _settingsWriter = settingsWriter;
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

    [RelayCommand]
    private void Append()
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

            LoadFlow(flow);
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
        Options.FileDirectoryOptions fileConfiguration = _fileDirectoryOptions.CurrentValue;

        string? filePath = _windowsDialog.SaveFile(new SaveFileSettings
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

            _settingsWriter.SaveAndBind(fileConfiguration);

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

    [RelayCommand]
    private void AddStepTop()
    {
        IStep step = _stepService.GetStepByIdentifier(SelectedStepIdentifier);

        var builderStepViewModel = AddStep(step, null);

        MoveTop(builderStepViewModel);
    }

    private BuilderStepViewModel AddStep(IStep step, Action<BuilderStepViewModel>? configure)
    {
        var builderStepViewModel = _viewModelFactory.Create<BuilderStepViewModel, IStep>(step);

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
        builderStepViewModel.MoveTop += () => MoveTop(builderStepViewModel);
        builderStepViewModel.MoveBottom += () => MoveBottom(builderStepViewModel);

        configure?.Invoke(builderStepViewModel);

        BuilderStepViewModels.Add(builderStepViewModel);

        Update();

        return builderStepViewModel;
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

    private void MoveTop(BuilderStepViewModel builderStepViewModel)
    {
        BuilderStepViewModels.Remove(builderStepViewModel);
        BuilderStepViewModels.Insert(0, builderStepViewModel);
    }

    private void MoveBottom(BuilderStepViewModel builderStepViewModel)
    {
        BuilderStepViewModels.Remove(builderStepViewModel);
        BuilderStepViewModels.Add(builderStepViewModel);
    }
}
