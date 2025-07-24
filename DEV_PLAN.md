# 🛠️ ATVeria Dev Plan (ATV Physics Prototype Phase)

## 🧩 Objective

Create a playable ATV driving prototype in Unity with:
- Arcade-style acceleration, steering, and jumping
- Simple third-person camera follow
- Minimal terrain setup with ramps

---

## 🎮 Core Prototype Systems

### 🚗 ATV Movement
- Rigidbody + WheelCollider-based driving
- Four wheels: front steer, rear drive
- Inputs: W/S = accelerate/reverse, A/D = turn, Space = brake

### 🌄 Terrain & Physics
- Use Unity cubes/slopes or ProBuilder for ramps
- Low-friction Physics Material for drifting
- Camera follows ATV smoothly

### 🧠 Input & Control
- `ATVInputHandler.cs`: Reads Unity Input (WASD)
- `ATVController.cs`: Applies torque/brake/steering

---

## 📁 Folder Structure

```
Assets/
├── Scripts/
│   └── Player/
├── Prefabs/
├── Scenes/
├── Art/
```

---

## ✅ Milestone Goals

### Phase 1 – Core Feel
- [ ] ATV physics: drive, brake, reverse, steer
- [ ] ATV can jump from ramps
- [ ] Camera follow system
- [ ] Playable test scene with terrain and 1–2 ramps

---

## 🧠 Cursor Agent Prompt

```
Create a Unity C# script for ATV-style movement using WheelColliders and Rigidbody physics.

It should:
- Support acceleration, braking, reversing, and steering
- Use motorTorque and brakeTorque for force
- Be arcade-style, not realistic sim
- Work with four wheels (WheelColliders)
- Be attached to the root ATV GameObject with a Rigidbody

Also create a separate script to handle input (WASD/Arrow keys) and pass movement commands cleanly to the controller.

Use placeholder visuals (cubes/cylinders). Assume this is part of a low-poly prototype.
```
