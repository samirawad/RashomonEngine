# Rashomon Engine Architecture Guide

## Overview
The Rashomon Engine is a Goal-Oriented Action Planning (GOAP) proof of concept built on the **Uniform Entity Container** model and the **Capability-Affordance Bridge**. 

In this model, NPCs do not possess "skills." Instead, they possess **Capabilities** (Tags) and consume **Affordances** (Activities) provided by other entities in the world.

---

## Perception and Belief (Dynamic Reality)
To prevent the "Sticky State" bug (where an NPC believes they are somewhere they are not), the engine distinguishes between two types of information in an NPC's mind:

1.  **Status States (Internal)**: These represent the NPC's condition (e.g., `IsHungry`, `IsTired`). These are modified by **Activities** (e.g., Eating clears hunger).
2.  **Perceptual States (External)**: These represent the NPC's spatial awareness (e.g., `AtHome`, `Near(Alice)`). These are modified by the **Perception Heartbeat**.

### The Perception Heartbeat
Every world tick, before planning or acting, an NPC performs a **Perception Scan**:
- **Reset**: All perceptual states (all "Near" keys and "AtHome") are cleared.
- **Calculate**: The NPC compares its physical `Position` to the positions of all entities in its **Knowledge Graph**.
- **Update**: If the NPC is at the same coordinates as an entity, the corresponding `Near` state is set to `true` in its mind.

This ensures the **Planner** always operates on the physical truth of the world, and the **Activity Contracts** are never "surprised" by a lying NPC.

---

## The Capability-Affordance Bridge
Every interaction in the world is a contract between a **Subject** (the NPC) and an **Object** (an Item, Location, or other NPC).

1.  **Affordances live on the Object**: An entity only affords activities to *others*. (e.g., An Apple affords "Eat", a Destination affords "WalkTo", a Merchant affords "Trade").
2.  **Capabilities live on the Subject**: An NPC provides the physical means to interact via **Tags** on their internal parts (e.g., a "Mouth" part tagged `Mouth`, "Feet" parts tagged `Feet`).
3.  **The Interaction Rule**: An NPC can only plan an activity if they possess the **Required Capability** defined by that activity's template.

---

## The Rule of Discovery
NPCs "find" their way through the world by matching their internal Capabilities against external Affordances:
1.  **Scan**: NPC scans their **Knowledge Graph** (Relationships and Children) for entities.
2.  **Filter**: NPC identifies which entities "offer" affordances.
3.  **Match**: The Planner verifies if the NPC has the **Tag** required to "unlock" that affordance.

---

## Activities as Scripted Scenes
Activities are **Centralized Orchestration Units** (Scenes) with empty **Roles**.
1.  **Late Binding**: An activity binds participants to roles (Initiator, Target, etc.) during planning.
2.  **Verifiable Contracts**: Before completion, an activity verifies that the Initiator still has the Capability and the Object still provides the Affordance.
3.  **Atomic Truth**: Social activities are shared instances; one object orchestrates all participants simultaneously.

---

## The Behavioral Marketplace (Social Coordination)
NPCs are **Social Opportunists**. They do not just follow static plans; they negotiate their participation in the world.

1.  **The Invitation (Proposal)**: When an NPC (the Initiator) plans a social activity, they send an **Invitation** to potential participants.
2.  **The Evaluation**: NPCs evaluate incoming invitations against their current highest-priority goal. If the role's payoff provides a faster or better path to a goal, they accept.
3.  **Handshake & Commitment**: Upon acceptance, the NPC "Subscribes" to the shared scene, pausing their own autonomous plans to participate.

---

## Spatio-Temporal Affordances (The Party Model)
Complex, multi-NPC activities (like Parties, Markets, or Rituals) are modeled as **Temporary Entities** anchored to a location.

1.  **Anchor**: An activity entity (e.g., `PartyScene`) is added as a **Child** of a physical location (e.g., `Tavern`).
2.  **Temporal Gating**: The activity entity uses its **State** to define availability (e.g., `IsActive = true`).
3.  **Discovery**: NPCs find the activity by scanning the hierarchy of locations they know.
4.  **Collective Participation**: Any number of NPCs can discover and join the same anchored scene simultaneously.

---

## Knowledge Propagation (Rumors & Information)
Knowledge in the Rashomon Engine is a social commodity that flows through the Knowledge Graph.

1.  **Relationship Sharing**: Activities (like `ChatActivity`) can have effects that modify the Knowledge Graph of the participants.
2.  **Discovery via Interaction**: An NPC can "learn" about an entity (like a secret party or a distant resource) by participating in a scene with another NPC who already has that relationship.
3.  **Subjective Reality**: This creates a world where different NPCs have radically different maps of the available affordances.

---

## Core Components

### 1. Entities (The Universal Atom)
The base class for everything in the world. 
- **Hierarchy**: Entities contain other entities (e.g., Bob contains a Stomach and an Inventory).
- **Affordances**: Entities "offer" Activities.
- **State & Tags**: Intrinsic (Tags) and Transient (States) properties define an entity.

### 2. Agents (NPCs as Time-Sliced Containers)
Specialized entities that manage a "Personal World" and own a **Brain Update Cycle**.
- **Non-Blocking Logic**: NPCs perform exactly one "slice" of an activity per world tick.
- **Queue-Based Execution**: NPCs maintain a queue of activities generated by the planner.

### 3. Activities (Scripted Contracts)
Activities are the "verbs" of the world, designed as **Verifiable Contracts** for multi-agent coordination.
- **Targeted States**: Activities use specific, targeted keys for preconditions (e.g., `Near(Alice)` instead of a generic `NearTarget`).
- **Verification Guards**: Before applying effects, an activity MUST verify that its physical and logical conditions are still met.
- **Atomic Physical Transfer**: Securely moves entities between hierarchies (e.g., `TradeActivity` swaps items).

### 4. The Planner (Discovery-Based)
The planner recursively searches the Knowledge Graph for entities that afford the activities needed to reach a targeted goal state.

---

## Simultaneous System Flow

The simulation operates on a **Parallel Heartbeat**:

1.  **World Tick**: The Global Clock increments.
2.  **Entity Tick**: All internal entities update their states.
3.  **NPC Thought Cycle**:
    - **Perception**: NPC checks if their goal is still valid.
    - **Verification**: Current activity checks if its contract is still valid.
    - **Execution Slice**: NPC performs exactly **one tick** of the current activity.
4.  **Contract Fulfillment**: Upon completion, guards are checked. If valid, effects are applied atomically.

---

## Project Structure

```text
RashomonTest/
├── Core/               
│   ├── Entity.cs       # Base Universal Atom
│   ├── Activity.cs     # Verifiable Contract
│   ├── NPC.cs          # Time-Sliced Agent
│   └── ...             
├── Activities/         # The Script Library
└── Program.cs          # Main Parallel Loop
```
