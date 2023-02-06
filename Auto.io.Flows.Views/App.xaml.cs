using Auto.io.Flows.Application.Extensions;
using Auto.io.Flows.Application.Services;
using Auto.io.Flows.ViewModels;
using Auto.io.Flows.ViewModels.Configurations;
using Auto.io.Flows.ViewModels.Factories;
using Auto.io.Flows.Views.Services;
using Lexicon.Common.DependencyInjection.Extensions;
using Lexicon.Common.Wpf.DependencyInjection;
using Lexicon.Common.Wpf.DependencyInjection.Amenities.Abstractions.Services;
using Lexicon.Common.Wpf.DependencyInjection.Amenities.Extensions;
using Lexicon.Common.Wpf.DependencyInjection.Extensions;
using Lexicon.Common.Wpf.DependencyInjection.Mvvm.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Auto.io.Flows.Views;
public partial class App : System.Windows.Application
{
    public App()
    {
        WpfApplicationBuilder builder = WpfApplication.CreateBuilder(this);
        builder.Configuration.AddSettings(builder.Services, Views.Properties.Settings.Default);

        builder.Services.ConfigureAndBind<FileConfiguration>(builder.Configuration);

        builder.Services.AddLexiconAmenities();

        builder.Services.AddApplication();

        builder.Services.AddSingleton<WindowsHookService>();

        builder.Services.AddSingleton<IFileService, WindowsFileService>();
        builder.Services.AddSingleton<IBuilderParameterFactory, BuilderParameterFactory>();
        builder.Services.AddSingleton<IMouseService, WindowsMouseService>();
        builder.Services.AddSingleton<IKeyboardService, WindowsKeyboardService>();
        builder.Services.AddSingleton<IScreenService, WindowsScreenService>();

        builder.Services.AddDataContext<BuilderFlowViewModel>();
        builder.Services.AddDataContext<BuilderParameterComboBoxViewModel>();
        builder.Services.AddDataContext<BuilderParameterTextBoxViewModel>();
        builder.Services.AddDataContext<BuilderStepViewModel>();
        builder.Services.AddDataContext<MainViewModel>().ForElement<MainView>();
        builder.Services.AddDataContext<PopupViewModel>().ForElement<PopupView>();

        WpfApplication app = builder.Build();

        app.CreateAndShow<MainViewModel>();
    }
}
