# Rashomon Engine Architecture Guide

## Overview
The Rashomon Engine is a Goal-Oriented Action Planning (GOAP) proof of concept built on the **Uniform Entity Container** model. Every element of the simulation—from NPCs and Locations to Body Parts, Items, and Internal Needs—is an **Entity**. 

NPCs act as dynamic knowledge graphs, discovering activities through subjective relationships with these entities.

---

## Core Components

### 1. Entities (The Universal Atom)
The base class for everything in the world. 
- **Affordances**: Entities "offer" Activities (e.g., an Axe affords "Chop", a Stomach affords "Hunger_Goal").
- **Container Logic**: Entities can contain other entities (e.g., an NPC contains Body Parts; a Chest contains Items).
- **State**: Key-value pairs representing the entity's current condition (e.g., `Health: 100`, `Sharpness: 5`).

### 2. Agents (NPCs as Knowledge Graphs)
Specialized entities that manage a "Personal World" of internal and external links.
- **Knowledge Graph**: A dictionary of `Relationships` to other entities.
    - **External**: Friends, Enemies, Known Locations.
    - **Internal**: Body Parts (Hands, Eyes), Needs (Stomach, Mind), Inventory (Items).
- **Discovery-Based Planning**: NPCs do not have a static list of actions. They query their **Knowledge Graph** to find entities that afford the activities required to satisfy their current goals.

### 3. Activities (Scripted Interactions)
The "verbs" that drive the simulation.
- **Director Logic**: Activities orchestrate the interaction between multiple entities (e.g., a "Combat" activity orchestrates the "Sword" entity of the attacker and the "Body" entity of the defender).
- **Latent Execution**: Activities "tick" over time, allowing for duration and phased logic.
- **Relationship Mutation**: Activities can create or destroy relationships (e.g., "Eating" destroys the relationship to the "Apple" entity).

### 4. The Planner (Uniform Discovery)
The planner is implementation-agnostic. It follows a simple loop:
1. **Need**: An internal entity (like `Stomach`) broadcasts a goal state.
2. **Search**: The Agent searches its **Knowledge Graph** for any entity (Internal or External) that provides a matching **Affordance**.
3. **Chain**: The planner builds a sequence of activities to move the agent into position and trigger the affordance.

---

## System Flow (The "Recursive" Logic)

1. **Goal**: The `Stomach` entity (Internal Relationship) reaches `Hunger > 80` and affords a `Get_Food` goal.
2. **Discovery**: 
    - Bob scans his `Inventory` relationship; finds no food.
    - Bob scans his `External` relationships; finds `Market`.
3. **Planning**: 
    - `Market` affords `Trade`. 
    - `Trade` requires `NearTarget`.
    - `WalkTo` affords `NearTarget`.
4. **Execution**: Bob walks to the market and triggers the trade script.
5. **Mutation**: The `Trade` activity removes the `Gold` entity from Bob and adds an `Apple` entity to his `Inventory` relationship.

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
