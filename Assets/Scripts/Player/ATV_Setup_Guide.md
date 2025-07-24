# ATV Setup Guide

## Quick Setup Instructions

### 1. Create the ATV GameObject Structure
```
ATV_GameObject (with Rigidbody, ATVInputHandler, ATVController)
├── Body (Cube - scaled to 2, 1, 3)
├── WheelColliders/
│   ├── FrontLeft_WheelCollider
│   ├── FrontRight_WheelCollider
│   ├── RearLeft_WheelCollider
│   └── RearRight_WheelCollider
└── WheelModels/ (Optional - for visual rotation)
    ├── FrontLeft_Wheel (Cylinder)
    ├── FrontRight_Wheel (Cylinder)
    ├── RearLeft_Wheel (Cylinder)
    └── RearRight_Wheel (Cylinder)
```

### 2. Component Setup

**On the main ATV GameObject, add:**
- Rigidbody (Mass: 1500, Drag: 0.3, Angular Drag: 3)
- ATVInputHandler script
- ATVController script

### 3. WheelCollider Setup

**Create 4 empty GameObjects as children, each with a WheelCollider:**
- Position them at the four corners of your ATV
- Recommended WheelCollider settings:
  - Mass: 20
  - Radius: 0.5
  - Wheel Damping Rate: 0.25
  - Suspension Distance: 0.3
  - Force App Point Distance: 0
  - Spring: 35000
  - Damper: 4500
  - Target Position: 0.5

### 4. ATVController Settings

**Recommended starting values:**
- Max Motor Torque: 2000
- Max Steer Angle: 35
- Max Brake Torque: 3000
- Center of Mass Offset: (0, -0.5, 0)
- Downforce: 100
- Anti Roll Force: 5000
- All Wheel Drive: ✓

### 5. Physics Materials (Optional but Recommended)

Create Physics Materials for different surfaces:
- **Ground_PhysMat**: Dynamic Friction: 0.6, Static Friction: 0.6
- **Ice_PhysMat**: Dynamic Friction: 0.1, Static Friction: 0.1
- **Mud_PhysMat**: Dynamic Friction: 1.0, Static Friction: 1.0

### 6. Controls

- **W/S or ↑/↓**: Accelerate/Reverse
- **A/D or ←/→**: Steer left/right
- **Space**: Brake

### 7. Tips for Arcade Feel

- Increase Max Motor Torque for faster acceleration
- Increase Max Steer Angle for sharper turns
- Lower center of mass for better stability
- Adjust suspension spring/damper for jumpiness
- Use the Animation Curves in the controller for custom input response

### 8. Ramp Setup for Physics-Based Jumping

Create ramps using:
- Cube primitives scaled and rotated as ramps
- No special scripting needed - physics handles jumping automatically
- Place ramps with ~15-30 degree angles for good launching

## Common Issues

- **ATV feels sluggish**: Increase maxMotorTorque
- **ATV tips over easily**: Lower centerOfMassOffset.y further
- **Steering too sensitive**: Decrease maxSteerAngle or adjust steerCurve
- **ATV doesn't jump well**: Check Rigidbody mass and suspension settings

## Advanced Customization

The scripts include Animation Curves for `motorCurve` and `steerCurve` that let you customize the input response. For example:
- Linear curve = consistent response
- Ease-in curve = slow start, fast end
- Ease-out curve = fast start, slow end 