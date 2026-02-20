# Rashomon Engine Architecture Guide

## Overview
The Rashomon Engine is a Goal-Oriented Action Planning (GOAP) proof of concept built on the **Uniform Entity Container** model. Every element of the simulation—from NPCs and Locations to Body Parts, Items, and Internal Needs—is an **Entity**. 

NPCs act as dynamic knowledge graphs, discovering activities through subjective relationships with these entities.

---

## Core Components

### 1. Entities (The Universal Atom)
The base class for everything in the world. 
- **Hierarchy**: Entities contain other entities (e.g., Bob contains a Stomach and an Inventory; the Inventory contains an Apple).
- **Affordances**: Entities "offer" Activities (e.g., an Axe affords "Chop").
- **State (Transient)**: Mutable conditions (e.g., `Hunger: 80`, `IsBroken: true`).
- **Tags (Intrinsic)**: Immutable properties used for classification (e.g., `Edible`, `Weapon`, `Flammable`).

### 2. Agents (NPCs as Knowledge Graphs)
Specialized entities that manage a "Personal World" of internal and external links.
- **Knowledge Graph**: A dictionary of `Relationships` to other entities.
    - **External**: Friends, Enemies, Known Locations.
    - **Internal**: Body Parts (Hands, Eyes), Needs (Stomach, Mind), Inventory (Items).
- **Discovery-Based Planning**: NPCs scan their graph to find entities that afford the activities required to satisfy their goals.

### 3. Activities (Scripted Interactions)
The "verbs" that drive the simulation.
- **Preconditions**: Can be state-based (`HasGold = true`) or tag-based (`HasTag("Edible")`).
- **Director Logic**: Activities orchestrate interactions between entities over time.
- **Physical Transfer**: Activities can move entities between hierarchies (e.g., `TradeActivity` moves an Apple entity from Seller to Buyer).

### 4. The Planner (Uniform Discovery)
The planner is implementation-agnostic. It follows a simple loop:
1. **Need**: An internal entity (like `Stomach`) broadcasts a goal state.
2. **Search**: The Agent searches its **Knowledge Graph** for any entity (Internal or External) that provides a matching **Affordance**.
3. **Chain**: The planner builds a sequence of activities to move the agent into position and trigger the affordance.

### 5. Dual-Logging (The "Rashomon" Files)
The engine produces two distinct logs for every NPC:
- **Memory Log (`_Memories.txt`)**: A subjective, narrative history of events from the NPC's perspective ("I remember trading with Alice").
- **Debug Log (`_Debug.txt`)**: A technical, step-by-step audit of the planner's decision tree and execution logic ("Planner selected TradeActivity; Precondition 'NearTarget' failed; Sub-planning WalkTo...").

---

## System Flow (The "Recursive" Logic)

1. **Goal**: The `Stomach` entity reaches `IsHungry = true`.
2. **Discovery**: 
    - Bob needs `IsHungry = false`.
    - `EatActivity` affords this but requires an item with the `Edible` **Tag**.
3. **Planning**: 
    - Bob scans his inventory; finds nothing tagged `Edible`.
    - Bob scans external relationships; finds `Market` (Alice) affords `Trade`.
    - `Trade` provides an item with the `Edible` tag.
4. **Execution**: 
    - `WalkTo` (moves Bob).
    - `Trade` (swaps Gold entity for Apple entity).
    - `Eat` (consumes Apple entity).

---

## Project Structure

```text
RashomonTest/
├── Core/               
│   ├── Entity.cs       # Base Universal Atom
│   ├── NPC.cs          # The Entity Container
│   ├── Activity.cs     # The Scripted Interaction
│   ├── SimplePlanner.cs # Discovery Planner
│   └── ...             
├── Activities/         # The Script Library
└── Program.cs          # World Heartbeat
```
