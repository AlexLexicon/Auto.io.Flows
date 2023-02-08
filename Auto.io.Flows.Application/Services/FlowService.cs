using Auto.io.Flows.Application.Models;
using System.Diagnostics;
using System.Text.Json;

namespace Auto.io.Flows.Application.Services;
public interface IFlowService
{
    void SaveFlow(string filePath, Flow stepDefinitions);
    Flow LoadFlow(string filePath);
    FlowStep CreateStep(IStep step, IEnumerable<FlowParameter?> parameters);
    //Task Excute(Flow flow);
}
public class FlowService : IFlowService
{
    private readonly IFileService _fileService;
    private readonly IStepService _stepService;

    public FlowService(
        IFileService fileService, 
        IStepService stepService)
    {
        _fileService = fileService;
        _stepService = stepService;
    }

    public void SaveFlow(string filePath, Flow flow)
    {
        string json = JsonSerializer.Serialize(flow);

        _fileService.WriteAllText(filePath, json);
    }

    public Flow LoadFlow(string filePath)
    {
        string json = _fileService.ReadAllText(filePath);

        Flow? flow = JsonSerializer.Deserialize<Flow>(json);

        if (flow is null)
        {
            throw new UnreachableException("Flow was null after being Deserialized.");
        }

        return flow;
    }

    public FlowStep CreateStep(IStep step, IEnumerable<FlowParameter?> parameters)
    {
        return new FlowStep
        {
            Identifer = step.Identifier,
            Parameters = parameters,
        };
    }

    //public async Task Excute(Flow flow)
    //{
    //    if (flow.Steps is not null)
    //    {
    //        foreach (FlowStep flowStep in flow.Steps)
    //        {
    //            string identifier = flowStep.Identifer;

    //            IStep step = _stepService.GetStepByIdentifier(identifier);

    //            var parameters = new List<object?>();
    //            foreach (FlowParameter? flowParameter in flowStep.Parameters)
    //            {
    //                if (flowParameter is not null)
    //                {
    //                    parameters.Add(flowParameter.Value);
    //                }
    //            }

    //            await step.ExecuteAsync(parameters);
    //        }
    //    }
    //}
}
