using Auto.io.Flows.Application.Models;
using Auto.io.Flows.ViewModels.Abstractions;
using Lexicon.Common.Wpf.DependencyInjection.Mvvm.Abstractions.Factories;

namespace Auto.io.Flows.ViewModels.Factories;
public interface IBuilderParameterFactory
{
    BuilderParameterViewModel CreateBuilderParameter(IParameter parameter);
}
public class BuilderParameterFactory : IBuilderParameterFactory
{
    private readonly IDataContextFactory _dataContextFactory;

    public BuilderParameterFactory(IDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    public BuilderParameterViewModel CreateBuilderParameter(IParameter parameter)
    {
        switch (parameter.UserInterface)
        {
            case UserInterfaces.TextBox:
                return _dataContextFactory.Create<BuilderParameterTextBoxViewModel, IParameter>(parameter);
            case UserInterfaces.ComboBox:
                return _dataContextFactory.Create<BuilderParameterComboBoxViewModel, IParameter>(parameter);
            case UserInterfaces.FilePathBrowser:
                return _dataContextFactory.Create<BuilderParameterFilePathBrowserViewModel, IParameter>(parameter);
        }

        throw new NotImplementedException($"{nameof(parameter.UserInterface)} '{parameter.UserInterface}' is not implemented for '{nameof(CreateBuilderParameter)}'.");
    }
}
