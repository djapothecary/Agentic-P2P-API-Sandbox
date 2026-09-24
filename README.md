# P2P.AgentApi - Agentic Procurement Sandbox

A Minimal API designed as a machine-friendly, tool-calling interface for autonomous Purchase-to-Pay (P2P) agentic workflows.

Built with **C# 13, ASP.NET Core Minimal APIs**, and **EF Core (In-Memory)** for zero-friction local execution.

## 🚀 One-Line Run Instruction

Open a terminal in the project root and execute:

```bash
dotnet run
```
*(Alternatively, use the provided `.vscode/launch.json` and hit F5 to launch the debugger).*

## 🧪 Testing the Agent Interface

This API is self-documenting for agentic ingestion. Once running, navigate to:
* **Swagger UI (Human Testing):** `http://localhost:5256/swagger`
* **OpenAPI Spec (Agent Ingestion):** `http://localhost:5256/swagger/v1/swagger.json`

## 📦 Data Seeding
The EF Core In-Memory database automatically seeds on startup with:
* **4 Vendors** (e.g., MCD, Tamiya) demonstrating `NET30`/`NET60` terms.
* **4 Purchase Orders** with associated `POLineItem` records to immediately test the 3-Way Match endpoints.