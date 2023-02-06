using Auto.io.Flows.Application.Models;
using Auto.io.Flows.ViewModels.Abstractions;
using Auto.io.Flows.ViewModels.Configurations;
using CommunityToolkit.Mvvm.Input;
using Lexicon.Common.Wpf.DependencyInjection.Amenities.Abstractions.Services;
using Lexicon.Common.Wpf.DependencyInjection.Amenities.Abstractions.Settings;
using Microsoft.Extensions.Options;

namespace Auto.io.Flows.ViewModels;
public partial class BuilderParameterFilePathBrowserViewModel : BuilderParameterViewModel
{
    private readonly IWindowsDialogService _windowsDialogService;
    private readonly IOptionsMonitor<FileConfiguration> _fileOptions;

    public BuilderParameterFilePathBrowserViewModel(
        IParameter parameter,
        IWindowsDialogService windowsDialogService,
        IOptionsMonitor<FileConfiguration> fileOptions) : base(parameter)
    {
        Value = _parameter.InitalValue?.ToString();
        _windowsDialogService = windowsDialogService;
        _fileOptions = fileOptions;
    }

    private bool _isValueChanging = false;
    private string? _value;
    public string? Value
    {
        get => _value;
        set
        {
            _value = value;

            if (!_isValueChanging)
            {
                _isValueChanging = true;

                OnTextValueChanged();
                OnPropertyChanged();

                _isValueChanging = false;
            }
        }
    }

    public override FlowParameter? GetFlowParameter()
    {
        return new FlowParameter
        {
            Identifier = _parameter.Identifier,
            Value = Value,
        };
    }

    public override void LoadFlowParameter(FlowParameter flowParameter)
    {
        Value = flowParameter.Value?.ToString();
    }

    [RelayCommand]
    private void Browse()
    {
        FileConfiguration fileConfiguration = _fileOptions.CurrentValue;

        string? filePath = _windowsDialogService.SaveFile(new SaveFileSettings
        {
            InitialDirectory = fileConfiguration.SaveFileDirectory,
            DefaultExtension = _parameter.Argument?.ToString() ?? "txt",
            FileName = _parameter.InitalValue?.ToString(),
            EnsureValidNames = true,
        });

        if (filePath is not null)
        {
            Value = filePath;
        }
    }

    private void OnTextValueChanged()
    {
        Update(Value);
    }
}
