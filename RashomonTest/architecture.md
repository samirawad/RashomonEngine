# Rashomon Engine Architecture Guide

## Overview
The Rashomon Engine is a Goal-Oriented Action Planning (GOAP) proof of concept designed for RPG environments. It enables NPCs to autonomously achieve goals by dynamically chaining modular activities based on world state and preconditions.

---

## Core Components

### 1. Agents (NPCs)
Located in `GoapRpgPoC.Core.NPC`.
- **State Management**: NPCs maintain a `Dictionary<string, bool>` representing their internal and world-view state (e.g., `HasGold`, `NearTarget`).
- **Memory**: Every NPC has a personal list of `Memory` objects, recording every activity they participated in.

### 2. Activities
Located in `GoapRpgPoC.Core.Activity` and `GoapRpgPoC.Activities`.
Activities are the "verbs" of the world, designed as **Shared Nodes** for multi-agent coordination.

- **Role-Based Grouping**: `Preconditions` and `Effects` are indexed by `ActivityRole` (e.g., `Initiator`, `Target`). 
- **Activities as "Scripts"**: An activity acts as a **Choreographer** or **Director** for a scene. Because it holds references to all participants, the `OnTick` method can orchestrate complex, synchronized behaviors (like a dance, a card game, or a battle) from a single source of logic.
- **Internal Scene State**: Activities can maintain internal state (e.g., `currentPhase`, `score`) that exists only for the duration of the interaction.
- **Latent Execution**: Activities are not instant. They "tick" over time, allowing for movement, animations, or multi-stage logic.
- **Atomic Transactions**: Once the "script" finishes, the final effects are applied to all NPCs simultaneously, ensuring world-state consistency.

### 3. The Planner
Located in `GoapRpgPoC.Core.SimplePlanner`.
The engine uses a **Recursive Backward-Chaining Planner**:
1. It identifies an activity that provides the desired **Goal State**.
2. It checks if the `Initiator` meets that activity's **Preconditions**.
3. If not, it recursively searches for activities that satisfy those preconditions, building a stack of actions.

### 4. Logging & Memory
- **Global History Log**: A static repository (`GlobalHistoryLog`) that tracks every activity occurring in the world.
- **Flyweight Pattern**: Activities are logged as objects, and NPCs store references to these shared objects in their personal memories to save space while maintaining rich data.

---

## System Flow

1. **Goal Setting**: A goal is defined (e.g., `HasApple = true`).
2. **Planning**: The `SimplePlanner` generates a sequence (e.g., `WalkTo` -> `Trade`).
3. **Execution**:
    - The plan is iterated.
    - `CompleteActivity` is called for each step.
    - World states are updated.
    - Events are recorded globally and in individual NPC memories.
4. **Validation**: The final state is checked against the original goal.

---

## Project Structure

```text
RashomonTest/
├── Core/               # Engine foundation
│   ├── Activity.cs     # Base activity logic
│   ├── NPC.cs          # Agent logic
│   ├── SimplePlanner.cs # GOAP algorithm
│   └── ...             # Supporting structures
├── Activities/         # Domain-specific implementations
│   ├── WalkToActivity.cs
│   └── TradeActivity.cs
└── Program.cs          # Entry point and simulation loop
```
