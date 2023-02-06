using Auto.io.Flows.Application.Models;
using Auto.io.Flows.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Auto.io.Flows.Application.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IFlowService, FlowService>();
        services.AddSingleton<IStepService, StepService>();
        services.AddSingleton<IParameterService, ParameterService>();

        List<TypeInfo> stepTypes = typeof(ServiceCollectionExtensions)
            .Assembly
            .DefinedTypes.Where(t => !t.IsAbstract && !t.IsInterface && t.IsAssignableTo(typeof(IStep)))
            .ToList();

        foreach (TypeInfo stepType in stepTypes)
        {
            services.AddSingleton(typeof(IStep), stepType);
        }
    }
}
