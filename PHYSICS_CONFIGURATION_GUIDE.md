# ATVeria Physics Configuration Guide

## Overview

This guide explains how to configure the physics system for the ATV driving game. The system uses Unity's Rigidbody physics with custom force-based movement for an arcade-style feel.

## Core Physics Architecture

### Components Overview
- **SimpleArcadeATVController**: Main physics controller using force-based movement
- **ATVInputHandler**: Input processing with smoothing
- **Rigidbody**: Unity's physics body (mass: 300kg, lightweight for responsiveness)
- **Physics Materials**: High-grip materials for tires and ground

### Physics Philosophy
- **Arcade-style**: Responsive, forgiving physics
- **Force-based**: Uses `AddForce()` instead of direct velocity manipulation
- **Momentum preservation**: Minimal drag for better ramp jumping
- **Grounded detection**: Raycast-based ground checking

## Rigidbody Configuration

### Base Settings (Set in ATVSceneSetup.cs)
```csharp
Rigidbody rb = atv.AddComponent<Rigidbody>();
rb.mass = 300f;                    // Lightweight for responsiveness
rb.linearDamping = 0.05f;          // Very low damping (preserves momentum)
rb.angularDamping = 0.3f;          // Low angular damping
rb.constraints = RigidbodyConstraints.FreezeRotationZ; // Can tilt forward/back, can't roll sideways
rb.centerOfMass = new Vector3(0, -0.3f, 0); // Low center of mass for stability
```

### Tuning Guidelines
- **Mass (300kg)**: Lower = more responsive, Higher = more stable
- **Linear Damping (0.05)**: Lower = better momentum, Higher = more drag
- **Angular Damping (0.3)**: Lower = more spinny, Higher = more stable
- **Center of Mass**: Lower Y = more stable, Higher Y = more tippy

## Movement Forces Configuration

### Acceleration Settings (SimpleArcadeATVController.cs)
```csharp
[Header("Arcade Driving Settings")]
[SerializeField] private float accelerationForce = 45000f;  // Forward force
[SerializeField] private float reverseForce = 25000f;       // Reverse force (usually lower)
[SerializeField] private float maxSpeed = 40f;              // Speed limit
[SerializeField] private float steerSpeed = 200f;           // Steering responsiveness
[SerializeField] private float brakeForce = 30000f;         // Braking strength
```

### Force Tuning Guide
- **accelerationForce (45000N)**: 
  - Higher = faster acceleration
  - Lower = more gradual build-up
  - Should be proportional to mass (45000N ÷ 300kg = 150 m/s²)
- **reverseForce (25000N)**: Usually 50-70% of forward force
- **maxSpeed (40 m/s)**: Prevents excessive speed
- **steerSpeed (200°/s)**: How fast the ATV turns
- **brakeForce (30000N)**: How quickly it stops

### Physics Materials Configuration

#### Tire Material (High Grip)
```csharp
PhysicsMaterial tiresMaterial = new PhysicsMaterial("GripTires");
tiresMaterial.dynamicFriction = 1.2f;   // Excellent grip
tiresMaterial.staticFriction = 1.5f;    // Great for starting/stopping
tiresMaterial.bounciness = 0.1f;        // Low bounce for stability
```

#### Ground Material (High Grip)
```csharp
PhysicsMaterial groundMaterial = new PhysicsMaterial("HighGripGround");
groundMaterial.dynamicFriction = 0.9f;  // Maximum grip
groundMaterial.staticFriction = 0.9f;   // Maximum grip
groundMaterial.bounciness = 0.2f;       // Better landing bounce
```

## Advanced Physics Features

### Traction System
```csharp
[Header("Arcade Physics")]
[SerializeField] private float downforce = 200f;           // Downward force
[SerializeField] private float dragCoefficient = 0.99f;    // Momentum preservation
[SerializeField] private float steerHelper = 0.8f;         // Steering assistance
[SerializeField] private float traction = 2f;              // Anti-slide force
```

### Traction Tuning
- **downforce (200N)**: Pushes ATV down for better grip
- **dragCoefficient (0.99)**: Preserves momentum (0.99 = 1% drag per frame)
- **traction (2f)**: Anti-slide force multiplier
  - Higher = less sliding
  - Lower = more drifting

### Grounding System
```csharp
[Header("Grounding")]
[SerializeField] private LayerMask groundLayer = -1;       // What counts as ground
[SerializeField] private float groundRayLength = 2f;       // Raycast distance
```

### Grounding Tuning
- **groundLayer**: Set to specific layers for precise ground detection
- **groundRayLength**: How far down to check for ground
- **isGrounded**: Boolean that affects physics behavior

## Camera Physics Configuration

### Camera Rig Setup
```csharp
// Camera positioning (God of War style)
mainCamera.transform.localPosition = new Vector3(0, 4f, -6f); // Behind and above
mainCamera.transform.localRotation = Quaternion.Euler(12f, 0, 0); // Slight downward angle
mainCamera.fieldOfView = 60f; // Perfect FOV for action
```

### Camera Smoothing (ActionCameraSmooth.cs)
```csharp
[SerializeField] private float rotationSmoothTime = 0.1f;   // Rotation smoothing
[SerializeField] private float positionSmoothTime = 0.05f;  // Position smoothing
```

### Camera Tuning
- **rotationSmoothTime**: Lower = more responsive, Higher = more cinematic
- **positionSmoothTime**: Lower = tighter follow, Higher = more lag
- **fieldOfView**: Lower = more zoomed, Higher = wider view

## Input System Configuration

### Input Smoothing (ATVInputHandler.cs)
```csharp
[Header("Input Smoothing")]
[SerializeField] private float inputSmoothTime = 0.1f;     // General input smoothing
```

### Input Tuning
- **inputSmoothTime**: Lower = more responsive, Higher = more arcade-like
- **Brake smoothing**: Usually 0.5x of general smoothing for faster response

## Environment Physics

### Global Physics Settings
```csharp
// Set in ATVSceneSetup.cs
Physics.gravity = new Vector3(0, -20f, 0); // Double default gravity for better ramp physics
```

### Gravity Tuning
- **Default Unity**: -9.81 m/s²
- **ATVeria Setting**: -20 m/s² (double gravity)
- **Higher gravity**: Better ramp momentum, faster falling
- **Lower gravity**: More floaty, longer air time

## Ramp Physics Configuration

### Ramp Setup Guidelines
- **Angle**: 15-30 degrees for good launching
- **Material**: Use high-friction physics material
- **Size**: Scale appropriately for ATV size
- **Positioning**: Place with adequate run-up space

### Jump Physics Tuning
- **Mass**: Lower mass = higher jumps
- **Gravity**: Higher gravity = better ramp momentum
- **Drag**: Lower drag = better air distance
- **Downforce**: Lower downforce = more air time

## Performance Optimization

### Physics Performance Settings
- **Fixed Timestep**: 0.02s (50 FPS physics)
- **Solver Iterations**: 6 (default)
- **Solver Velocity Iterations**: 1 (default)
- **Sleep Threshold**: 0.005 (default)

### Optimization Tips
- Keep ATV mass reasonable (200-500kg)
- Use appropriate collision layers
- Limit physics objects in scene
- Use trigger colliders for non-physical interactions

## Debugging Physics

### Built-in Diagnostics (ATVDiagnostics.cs)
The system includes automatic diagnostics that report:
- Input status
- Physics state
- Grounding status
- Performance metrics

### Manual Debug Features
- **OnGUI()**: Real-time display in game view
- **Debug.Log()**: Console output for physics events
- **Debug.DrawRay()**: Visual ground detection rays

## Common Physics Issues & Solutions

### ATV Too Sluggish
- **Solution**: Increase `accelerationForce` or decrease `mass`
- **Check**: Rigidbody constraints, drag settings

### ATV Too Sensitive
- **Solution**: Decrease `steerSpeed` or increase `inputSmoothTime`
- **Check**: Input smoothing settings

### Poor Ramp Performance
- **Solution**: Increase gravity, decrease drag, lower mass
- **Check**: Ramp angle and material

### ATV Tips Over Easily
- **Solution**: Lower center of mass, increase angular damping
- **Check**: Rigidbody constraints

### Poor Ground Detection
- **Solution**: Adjust `groundRayLength` or `groundLayer`
- **Check**: Collider setup on ground objects

## Quick Tuning Presets

### Arcade Feel
```csharp
mass = 300f;
accelerationForce = 45000f;
steerSpeed = 200f;
dragCoefficient = 0.99f;
inputSmoothTime = 0.1f;
```

### Realistic Feel
```csharp
mass = 800f;
accelerationForce = 30000f;
steerSpeed = 100f;
dragCoefficient = 0.95f;
inputSmoothTime = 0.05f;
```

### Drift Feel
```csharp
mass = 400f;
accelerationForce = 50000f;
steerSpeed = 300f;
traction = 0.5f;
dragCoefficient = 0.98f;
```

### Stunt Feel
```csharp
mass = 200f;
accelerationForce = 60000f;
steerSpeed = 250f;
downforce = 100f;
gravity = -15f;
```

## Scene Setup Automation

### Using ATVSceneSetup.cs
The `ATVSceneSetup` editor script automatically configures:
- Physics materials
- Rigidbody settings
- Camera setup
- Environment objects
- Proper component hierarchy

### Manual Setup Steps
1. Create ATV GameObject with Rigidbody
2. Add SimpleArcadeATVController script
3. Add ATVInputHandler script
4. Create visual body and wheels
5. Apply physics materials
6. Setup camera rig
7. Configure environment

## Input System Integration

### New Input System Support
- Uses Unity's new Input System
- WASD and Arrow key support
- Automatic keyboard detection
- Smooth input processing

### Input Mapping
- **W/Up Arrow**: Forward
- **S/Down Arrow**: Reverse
- **A/Left Arrow**: Turn left
- **D/Right Arrow**: Turn right
- **Space**: Brake

## Testing and Validation

### Physics Validation Checklist
- [ ] ATV responds to input
- [ ] Ground detection works
- [ ] Ramp jumping feels good
- [ ] Camera follows smoothly
- [ ] No excessive sliding
- [ ] Appropriate speed limits
- [ ] Good acceleration feel
- [ ] Proper braking response

### Performance Validation
- [ ] 60+ FPS maintained
- [ ] No physics lag
- [ ] Smooth camera movement
- [ ] Responsive input
- [ ] Stable physics simulation

This configuration guide provides all the knobs and levers needed to tune the ATV physics system for different gameplay feels and performance requirements. 