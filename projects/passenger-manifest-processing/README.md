# Passenger Manifest Processing System

A sanitized portfolio reference architecture for an event-driven passenger-manifest workflow.

## Resume-aligned experience

The resume describes work with AWS infrastructure and Lambda-based workflows for automated passenger-manifest generation and secure email delivery, including Lambda, CloudWatch, SNS, SES, EventBridge Scheduler, DynamoDB, Secrets Manager and IAM.

This repository documents those engineering concepts without reproducing employer code, real passenger data, production identifiers or credentials.

## Architecture

```text
EventBridge Scheduler
        |
        v
AWS Lambda (orchestrator)
        |
        +----> Secrets Manager / Parameter Store
        |
        +----> Data source adapter
        |
        +----> Manifest generation adapter
        |
        +----> DynamoDB (idempotency / processing record)
        |
        +----> Amazon SES (secure email delivery)
        |
        +----> CloudWatch Logs / Metrics / Alarms
                         |
                         +----> SNS notifications
```

## Key engineering concerns

- **Scheduling:** invoke the workflow on a configurable schedule.
- **Idempotency:** persist a deterministic flight/manifest key so retries do not send duplicate messages.
- **Secrets:** retrieve connection details at runtime; never commit credentials.
- **Least privilege:** scope Lambda permissions to only the required resources and actions.
- **Observability:** use structured logs, metrics, alarms and SNS notifications.
- **Failure handling:** distinguish transient failures from invalid input; retry only safe operations.
- **Delivery:** use SES with an explicit sender/recipient configuration and avoid logging sensitive manifest content.

## Suggested implementation layout

```text
src/
  Manifest.Function/
    Function.cs
    Models/
    Services/
    Infrastructure/
tests/
  Manifest.Function.Tests/
infra/
  terraform/
docs/
  architecture.md
```

## Local development

This documentation is cloud-provider oriented. Before deploying a real implementation, add an isolated development AWS account, test identities, least-privilege IAM policies, infrastructure state configuration and automated tests. Do not point sample code at employer or production resources.

## Status

Architecture and implementation plan are documented. This is a portfolio reference project, not a claim that this repository is the original enterprise system.
