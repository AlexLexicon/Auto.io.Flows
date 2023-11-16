using Auto.io.Flows.Application.Extensions;
using Auto.io.Flows.Application.Services;
using Auto.io.Flows.ViewModels;
using Auto.io.Flows.ViewModels.Factories;
using Auto.io.Flows.ViewModels.Options;
using Auto.io.Flows.Views.Services;
using Lexicom.Concentrate.Supports.Wpf.Extensions;
using Lexicom.Concentrate.Wpf.Amenities.Extensions;
using Lexicom.Configuration.Settings.Extensions;
using Lexicom.Configuration.Settings.For.Wpf.Extensions;
using Lexicom.DependencyInjection.Amenities.Extensions;
using Lexicom.Mvvm.Extensions;
using Lexicom.Mvvm.For.Wpf.Extensions;
using Lexicom.Supports.Wpf.Extensions;
using Lexicom.Validation.For.Wpf.Extensions;
using Lexicom.Wpf.Amenities.Extensions;
using Lexicom.Wpf.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Auto.io.Flows.Views;
public partial class App : System.Windows.Application
{
    public App()
    {
        WpfApplicationBuilder builder = WpfApplication.CreateBuilder(this);

        builder.Services
            .AddOptions<FileDirectoryOptions>()
            .BindConfiguration();

        builder.Lexicom(options =>
        {
            options.AddAmenities();
            options.AddSettings(Views.Properties.Settings.Default);
            options.AddMvvm(options =>
            {
                options.AddViewModel<BuilderFlowViewModel>();
                options.AddViewModel<BuilderParameterComboBoxViewModel>();
                options.AddViewModel<BuilderParameterTextBoxViewModel>();
                options.AddViewModel<BuilderParameterFilePathBrowserViewModel>();
                options.AddViewModel<BuilderStepViewModel>();
                options.AddViewModel<RunnerStepViewModel>();
                options.AddViewModel<RunnerFlowViewModel>();
                options.AddViewModel<RunnerParameterViewModel>();
                options.AddViewModel<MainViewModel>(options =>
                {
                    options.ForWindow<MainView>();
                });
                options.AddViewModel<PopupViewModel>(options =>
                {
                    options.ForWindow<PopupView>();
                });
            });
            options.AddValidation();
            options.Concentrate(options =>
            {
                options.AddAmenities();
            });
        });

        builder.Services.AddApplication();

        builder.Services.AddSingleton<WindowsHookService>();
        builder.Services.AddSingleton<WindowsInputService>();

        builder.Services.AddSingleton<IFileService, WindowsFileService>();
        builder.Services.AddSingleton<IBuilderParameterFactory, BuilderParameterFactory>();
        builder.Services.AddSingleton<IMouseService, WindowsMouseService>();
        builder.Services.AddSingleton<IKeyboardService, WindowsKeyboardService>();
        builder.Services.AddSingleton<IScreenService, WindowsScreenService>();

        WpfApplication app = builder.Build();

        app.StartupWindow<MainView>();
    }
}
