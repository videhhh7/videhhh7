using AiAgentBuilder.Models;
using AiAgentBuilder.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiAgentBuilder.Controllers;

public sealed class AgentsController(
    AgentRunner runner,
    IAgentEndpointRegistry endpointRegistry) : Controller
{
    [HttpGet]
    public IActionResult Index() => View(new AgentDefinition());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Run(
        AgentDefinition model,
        string input,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(input))
        {
            ModelState.AddModelError(nameof(input), "Enter a prompt before running the agent.");
            return View("Index", model);
        }

        try
        {
            var endpoint = endpointRegistry.GetEndpoint(model.EndpointName);
            var result = await runner.RunAsync(model, endpoint, input, cancellationToken);
            return View("Result", result.RootElement.GetRawText());
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "The configured agent service could not be reached.");
            return View("Index", model);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", model);
        }
    }
}
