# Rashomon Engine Architecture Guide

## Overview
The Rashomon Engine is a Goal-Oriented Action Planning (GOAP) proof of concept designed for RPG environments. It enables NPCs to autonomously achieve goals by dynamically chaining modular activities discovered through **Subjective Affordances** and a **Knowledge Graph** of relationships.

---

## Core Components

### 1. Entities
The base class for all interactive objects in the world (NPCs, Locations, Items).
- **Affordances**: Every entity "offers" a set of Activities it can participate in (e.g., a Bed offers "Sleep", a Shopkeeper offers "Trade").
- **Identity**: Entities have a unique name and physical position on the grid.

### 2. Agents (NPCs)
Specialized entities that maintain internal state and a personal knowledge graph.
- **Relationships**: A dynamic data store (Knowledge Graph) linking the NPC to other entities (e.g., `Relationships["Home"]` points to a specific Location).
- **Subjective Discovery**: NPCs don't have a global list of activities. Instead, they scan their relationships to find entities that "afford" the activities needed to reach their goals.
- **Memory**: A personal history of all activities the NPC participated in, stored as a timeline of events.

### 3. Activities (The "Scripted" Affordances)
Activities are the "verbs" of the world, designed as **Shared Nodes** for multi-agent coordination.
- **Role-Based Grouping**: `Preconditions` and `Effects` are indexed by `ActivityRole` (e.g., `Initiator`, `Target`).
- **Director Logic**: Activities orchestrate complex, synchronized behaviors (like a dance, card game, or battle) using an `OnTick` heartbeat.
- **Dynamic Modification**: Activities can create, remove, or update relationships (e.g., a "Marriage" activity adds a "Spouse" relationship to both participants).

### 4. The Planner (Discovery-Based)
The engine uses a **Recursive Backward-Chaining Planner** that operates on discovery:
1. **Need**: NPC identifies a goal state.
2. **Scan**: NPC scans their known **Relationships** for entities.
3. **Query**: NPC queries those entities for **Affordances** (Activities) that satisfy the goal.
4. **Plan**: If a match is found, the planner builds a chain of activities to satisfy the requirements.

### 5. Logging & Memory (The "Rashomon" Files)
- **Global History Log**: A static repository tracking every activity occurring in the world.
- **Personal Truth**: NPCs store references to shared activities in their personal memories.
- **Memory Utility**: A tool to export each NPC's personal memories to disk as text files for inspection.

---

## System Flow

1. **Goal Generation**: An NPC identifies a need (e.g., `Rest = true`).
2. **Discovery**: NPC looks at `Relationships["Home"]` and finds it affords a `SleepActivity`.
3. **Planning**: Planner builds the chain: `WalkTo(Home)` -> `SleepActivity`.
4. **Execution**: The `World` heartbeat drives the activity "scripts" until finished.
5. **Memory Injection**: Participants record the event, potentially updating their relationships based on the outcome.

---

## Project Structure

```text
RashomonTest/
├── Core/               # Engine foundation
│   ├── Entity.cs       # Base world object
│   ├── Activity.cs     # Base scripted interaction
│   ├── NPC.cs          # Agent with Knowledge Graph
│   ├── SimplePlanner.cs # Discovery-based GOAP
│   └── ...             # Supporting structures
├── Activities/         # Scripted Affordances
│   ├── WalkToActivity.cs
│   ├── TradeActivity.cs
│   └── ChatActivity.cs
└── Program.cs          # Simulation orchestrator
```
