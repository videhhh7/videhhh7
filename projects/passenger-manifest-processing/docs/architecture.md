# Architecture and operational design

## Processing sequence

1. EventBridge Scheduler invokes the Lambda entry point.
2. The handler validates the scheduled event and derives a stable processing key.
3. The service checks DynamoDB for an already-completed processing record.
4. The data adapter retrieves only the records required for the configured reporting window.
5. The manifest generator creates an output artifact in memory or in a controlled storage location.
6. The delivery adapter sends the message through SES.
7. The workflow records the outcome and emits structured telemetry.
8. CloudWatch alarms can notify an SNS topic when error or duration thresholds are exceeded.

## Idempotency

Use a stable key such as `{flight-number}#{flight-date}#{manifest-type}`. Use a conditional DynamoDB write to claim processing before delivery. Define a clear state model such as `IN_PROGRESS`, `COMPLETED` and `FAILED`, with a TTL for operational cleanup.

A production design must consider the failure window between sending an email and recording completion. Use an outbox/delivery ledger or reconciliation strategy if exactly-once business delivery is required; a simple status flag alone cannot guarantee exactly-once delivery across two independent services.

## Security

- Store secrets in Secrets Manager or Parameter Store SecureString.
- Encrypt data at rest and in transit.
- Use a dedicated execution role with resource-scoped permissions.
- Avoid logging passenger details, document contents, connection strings or tokens.
- Configure SES identities and recipient controls per environment.
- Keep environment-specific values outside source control.

## Observability

Recommended structured fields: `correlationId`, `processingKey`, `flightDate`, `status`, `durationMs` and a non-sensitive error category.

Recommended alarms: Lambda errors, throttles, duration, failed invocations and workflow-specific failure counts.

## Test strategy

- Unit tests for key generation, validation and state transitions.
- Adapter tests using local fakes or mocks.
- Integration tests in a dedicated non-production AWS account.
- Failure tests for timeouts, duplicate invocations, SES rejection and DynamoDB conditional-write conflicts.
