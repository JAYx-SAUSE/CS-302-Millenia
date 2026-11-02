# CS-302-Millenia
Millenia: Academic Special-Permission & Course Override Triage System

Course: CS302 Software Design & Development: AI Engineering (Fall 2026)

Institution: Jacksonville University – School of Engineering & Technology

Project: Milestone Repository & Implementation Guide

📌 Project Overview

Millenia is an AI-powered decision-support system engineered to triage, extract, and evaluate student course-override and prerequisite-waiver requests. Built strictly under Clean Architecture principles, the non-deterministic AI components (LLM transcript parsing and RAG policy lookups) operate as outer infrastructure adapters behind explicit interface boundaries.

Hard prerequisite verification, GPA validation, and status state transitions remain 100% deterministic, executed inside a pure C# core domain.

Human Accountability: RhoTriage never automatically grants overrides or writes directly to the university registrar system. All AI outputs present a pipelined evaluation report for one-click review and sign-off by the Department Lead Advisor.

🏗 Screaming Architecture & Solution Structure

The project structure explicitly highlights the business domain rather than technical framework details.

AcademicOverrideSystem/
├── src/
│   ├── Core.Domain/                    # Pure Domain (Zero External Dependencies)
│   │   ├── Entities/                   # OverrideRequest, Student, Course, AuditRecord
│   │   ├── ValueObjects/               # GPACalculation, CourseGrade, PolicyCitation
│   │   ├── Services/                   # PrerequisiteValidator (Pure Business Rules)
│   │   └── Interfaces/                 # ITranscriptParserGateway, IPolicyRagGateway
│   │
│   ├── Core.Application/               # Application Use Cases & Orchestration
│   │   ├── Commands/                   # IngestRequestCommand, SubmitDecisionCommand
│   │   ├── Queries/                    # GetPendingQueueQuery, GetAuditTrailQuery
│   │   └── Interfaces/                 # IObservabilityLogger
│   │
│   ├── Infrastructure.AI/              # Outer AI Infrastructure Adapters
│   │   ├── Adapters/                   # GeminiTranscriptParserAdapter, PolicyRagAdapter
│   │   ├── CircuitBreaker/             # TokenBudgetManager, CostCircuitBreaker
│   │   └── Prompts/                    # Externalized Prompt Templates
│   │
│   └── Presentation.WebAPI/            # Entry Point / Composition Root
│       ├── Controllers/                # OverrideQueueController
│       └── Program.cs                  # DI Wiring & Governance Registration
│
└── tests/
    ├── Core.Domain.Tests/              # Unit Tests for Rules (No Live AI Calls Needed)
    └── Infrastructure.AI.Tests/        # Gateway Adapter Integration Tests


📑 Governance & Boundary Architecture

Component

Responsibility

Boundary Rule

Core Domain

Prerequisite checks, GPA validation, recommendation rules

Zero AI dependencies; fully testable via pure unit tests.

AI Adapters

Extracts JSON from PDFs, queries RAG policy embeddings

Hidden behind C# gateways (ITranscriptParserGateway).

Human-In-The-Loop

Advisor review dashboard & audit logger

Enforces mandatory human sign-off on 100% of decisions.

Circuit Breaker

Per-request token caps and daily budget safeguards

Hard off-ramp fallback to manual queue on quota trip.

🚀 Quickstart & Setup Guide

Prerequisites

.NET 8.0 SDK

Google AI Studio API Key (Gemini 1.5 Flash Free Tier)

Visual Studio 2022 or VS Code (with C# Dev Kit)

Step 1: Clone & Configure

git clone https://github.com/your-team-repo/AcademicOverrideSystem.git
cd AcademicOverrideSystem


Configure your environment secrets or set your local API key in src/Presentation.WebAPI/appsettings.Development.json:

{
  "GeminiApi": {
    "ApiKey": "YOUR_GEMINI_FREE_TIER_API_KEY",
    "Model": "gemini-1.5-flash"
  }
}


Step 2: Build the Solution

dotnet build AcademicOverrideSystem.sln


Step 3: Run Domain Unit Tests (No Live Model Calls)

dotnet test tests/Core.Domain.Tests/Core.Domain.Tests.csproj


Step 4: Launch Web API & Advisor Queue

dotnet run --project src/Presentation.WebAPI/Presentation.WebAPI.csproj


Navigate to https://localhost:7001/swagger (or the HTTP endpoint) to inspect the API contracts.

🛠 Usage Pipeline

Submit Request: POST a student request form with an attached transcript PDF stream to /api/override/ingest.

AI Extraction Phase: GeminiTranscriptParserAdapter extracts completed courses and numeric GPA into structured JSON.

Deterministic Evaluation: PrerequisiteValidator (Core Domain) runs hard prerequisite rules and assigns a recommendation (RECOMMEND_APPROVE, RECOMMEND_DENY, or FLAG_HUMAN_REVIEW).

Human Decision Gate: The Department Lead Advisor fetches the pending review item via /api/override/pending, verifies policy citations, and submits a final approval decision.

🤖 AI Attribution & Audit

AI Contribution: AI is utilized for structured document parsing (extracting raw course codes/grades from PDFs) and RAG policy lookup.

Human Architecture: Domain entity boundaries, hard eligibility evaluation, token circuit-breaking, and final override decisions are strictly authored and maintained by the engineering team.

Audit Trail: Every processed request logs raw JSON extraction, prompt versions, retrieved policy chunk IDs, and advisor signature timestamps in an immutable record.
