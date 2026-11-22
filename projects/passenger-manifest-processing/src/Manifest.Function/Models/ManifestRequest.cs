namespace Manifest.Function.Models;

public sealed record ManifestRequest(
    string FlightNumber,
    DateOnly FlightDate,
    string Destination,
    string CorrelationId);

public sealed record ManifestDocument(
    string FileName,
    byte[] Content,
    string ContentType);

public sealed record ManifestResult(
    string ProcessingKey,
    string Status,
    string? Message = null);
