using Manifest.Function.Models;
using Manifest.Function.Services;

namespace Manifest.Function;

/// <summary>
/// Framework-neutral entry point. An AWS Lambda adapter can deserialize the EventBridge
/// event and call HandleAsync; cloud SDK concerns remain outside the domain workflow.
/// </summary>
public sealed class Function(ManifestWorkflow workflow)
{
    public Task<ManifestResult> HandleAsync(
        ManifestRequest request, CancellationToken cancellationToken = default) =>
        workflow.ExecuteAsync(request, cancellationToken);
}
