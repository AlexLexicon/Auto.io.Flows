using Auto.io.Flows.Application.Models;
using Auto.io.Flows.ViewModels.Abstractions;
using Lexicom.Mvvm;

namespace Auto.io.Flows.ViewModels.Factories;
public interface IBuilderParameterFactory
{
    BuilderParameterViewModel CreateBuilderParameter(IParameter parameter);
}
public class BuilderParameterFactory : IBuilderParameterFactory
{
    private readonly IViewModelFactory _viewModelFactory;

    public BuilderParameterFactory(IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public BuilderParameterViewModel CreateBuilderParameter(IParameter parameter)
    {
        return parameter.UserInterface switch
        {
            UserInterfaces.TextBox => _viewModelFactory.Create<BuilderParameterTextBoxViewModel, IParameter>(parameter),
            UserInterfaces.ComboBox => _viewModelFactory.Create<BuilderParameterComboBoxViewModel, IParameter>(parameter),
            UserInterfaces.FilePathBrowser => _viewModelFactory.Create<BuilderParameterFilePathBrowserViewModel, IParameter>(parameter),
            _ => throw new NotImplementedException($"{nameof(parameter.UserInterface)} '{parameter.UserInterface}' is not implemented for '{nameof(CreateBuilderParameter)}'."),
        };
    }
}
