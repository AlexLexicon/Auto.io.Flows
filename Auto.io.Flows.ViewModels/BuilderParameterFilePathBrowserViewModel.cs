using Auto.io.Flows.Application.Models;
using Auto.io.Flows.ViewModels.Abstractions;
using Auto.io.Flows.ViewModels.Options;
using CommunityToolkit.Mvvm.Input;
using Lexicom.Wpf.Amenities.Dialogs;
using Microsoft.Extensions.Options;

namespace Auto.io.Flows.ViewModels;
public partial class BuilderParameterFilePathBrowserViewModel : BuilderParameterViewModel
{
    private readonly IWindowsDialog _windowsDialog;
    private readonly IOptionsMonitor<FileDirectoryOptions> _fileDirectoryOptions;

    public BuilderParameterFilePathBrowserViewModel(
        IParameter parameter,
        IWindowsDialog windowsDialog,
        IOptionsMonitor<FileDirectoryOptions> fileDirectoryOptions) : base(parameter)
    {
        Value = _parameter.InitalValue?.ToString();
        _windowsDialog = windowsDialog;
        _fileDirectoryOptions = fileDirectoryOptions;
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
        Options.FileDirectoryOptions fileConfiguration = _fileDirectoryOptions.CurrentValue;

        string? filePath = _windowsDialog.SaveFile(new SaveFileSettings
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
