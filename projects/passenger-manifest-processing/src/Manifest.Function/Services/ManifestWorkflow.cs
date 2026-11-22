using Manifest.Function.Models;

namespace Manifest.Function.Services;

public interface IManifestDataSource
{
    Task<IReadOnlyList<PassengerRecord>> GetPassengersAsync(
        ManifestRequest request, CancellationToken cancellationToken);
}

public interface IManifestGenerator
{
    Task<ManifestDocument> GenerateAsync(
        ManifestRequest request,
        IReadOnlyList<PassengerRecord> passengers,
        CancellationToken cancellationToken);
}

public interface IManifestDelivery
{
    Task SendAsync(ManifestRequest request, ManifestDocument document,
        CancellationToken cancellationToken);
}

public interface IProcessingLedger
{
    Task<bool> TryStartAsync(string key, CancellationToken cancellationToken);
    Task MarkCompletedAsync(string key, CancellationToken cancellationToken);
    Task MarkFailedAsync(string key, string reason, CancellationToken cancellationToken);
}

public sealed record PassengerRecord(string PassengerReference, string DisplayName);

public sealed class ManifestWorkflow(
    IManifestDataSource dataSource,
    IManifestGenerator generator,
    IManifestDelivery delivery,
    IProcessingLedger ledger)
{
    public async Task<ManifestResult> ExecuteAsync(
        ManifestRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.FlightNumber))
            throw new ArgumentException("Flight number is required.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.CorrelationId))
            throw new ArgumentException("Correlation ID is required.", nameof(request));

        var key = $"{request.FlightNumber.Trim().ToUpperInvariant()}#{request.FlightDate:yyyy-MM-dd}";

        if (!await ledger.TryStartAsync(key, cancellationToken))
            return new ManifestResult(key, "Skipped", "Processing already started or completed.");

        try
        {
            var passengers = await dataSource.GetPassengersAsync(request, cancellationToken);
            var document = await generator.GenerateAsync(request, passengers, cancellationToken);
            await delivery.SendAsync(request, document, cancellationToken);
            await ledger.MarkCompletedAsync(key, cancellationToken);
            return new ManifestResult(key, "Completed");
        }
        catch (Exception ex)
        {
            // Store a safe error category/message only; never log passenger data or secrets.
            await ledger.MarkFailedAsync(key, ex.GetType().Name, CancellationToken.None);
            throw;
        }
    }
}
