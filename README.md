---

## 🎮 Core Gameplay Systems

### 🚗 ATV Controller (Player + NPC)
- Based on Unity Wheel Collider (placeholder for now)
- Basic arcade physics: jumps, drifts, donuts
- Trigger-based trick evaluation (e.g., jump + midair spin = "360 Whirl")

### 🤖 NPC ATV AI
- Wandering + idle state in overworld
- Battle challenge triggers
- Dialogue system (blocky text UI + voice brr-brrs)

### 🛠️ Battle System
- Either turn-based or mini-arena real-time
- Start with single 1v1: simple attack/dodge/boost moves
- Health, cooldowns, and “moves” via Unity ScriptableObjects

### 🌱 Evolution & Stats
- Every ATV has:
  - Type (Dirt, Nitro, Scrap, etc.)
  - Tier (Starter → Upgraded → Evolved)
  - Stats: Torque, Speed, Handling, Armor
  - Moveset

### 🗺️ World & Regions
- Region 1: **Dustwell Dunes**
  - 3 towns: garages, shopkeepers
  - 1 arena (boss fight)
  - Wild ATV zones with spawns
  - Hidden ramps, collectibles

---

## 🎨 Art & Assets

### Visual Style
- Hard low-poly shapes
- Retro pixel textures
- Chunky UI (early DS/GBA feel)
- Billboards for trees

### Tools
- Kenney asset packs for prototype
- [Synty Studios](https://syntystore.com/) for future polish

---

## 🧪 MVP Milestones

### ✅ Phase 1: Core Systems
- [ ] ATV movement (player + NPC AI)
- [ ] Terrain collisions and ramps
- [ ] Trick recognition
- [ ] Simple dialogue boxes
- [ ] Trigger → arena battle scene

### 🏁 Phase 2: Battle Prototypes
- [ ] Arena mode prefab
- [ ] Health, moves, win condition
- [ ] Move animations (basic)
- [ ] Placeholder UI for battle choices

### 🚧 Phase 3: World Building
- [ ] 1 biome (Dustwell)
- [ ] Garage/town prefab
- [ ] Encounter zones
- [ ] Save/load progress

---

## 🤖 Dev Tools / Agent Prompts for Cursor

```bash
# Generate low-poly ATV model
"Create a low-poly ATV model with 4 large wheels and simple mesh shapes using Unity primitives"

# Create simple ATV movement script
"Write a Unity C# script for basic ATV-style driving with acceleration, turning, and friction"

# Create dialogue system prefab
"Create a basic Unity dialogue box with typing effect and NPC interaction trigger zones"

# Make an ATV evolution system using ScriptableObjects
"Define an ATV species with stats and evolution paths using ScriptableObjects"