using UnityEngine;
using UnityEngine.InputSystem;

public class ATVInputHandler : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private bool enableInput = true;
    
    // Input values (accessible by ATVController)
    public float MotorInput { get; private set; }
    public float SteerInput { get; private set; }
    public float BrakeInput { get; private set; }
    
    // Input smoothing for arcade feel
    [Header("Input Smoothing")]
    [SerializeField] private float inputSmoothTime = 0.1f;
    
    private float motorInputVelocity;
    private float steerInputVelocity;
    private float brakeInputVelocity;

    void Update()
    {
        if (!enableInput) return;
        
        HandleInput();
    }
    
    private void HandleInput()
    {
        // NEW INPUT SYSTEM - Modern approach
        float rawMotorInput = 0f;
        float rawSteerInput = 0f;
        float rawBrakeInput = 0f;
        
        // Check if we have a keyboard
        if (Keyboard.current != null)
        {
            // WASD and Arrow key detection using New Input System
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                rawMotorInput = 1f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                rawMotorInput = -1f;
                
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                rawSteerInput = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                rawSteerInput = 1f;
            
            // Brake input
            rawBrakeInput = Keyboard.current.spaceKey.isPressed ? 1f : 0f;
        }
        else
        {
            Debug.LogWarning("No keyboard detected by New Input System!");
        }
        
        // Debug the raw input (only when keys are pressed)
        if (Mathf.Abs(rawMotorInput) > 0.1f || Mathf.Abs(rawSteerInput) > 0.1f)
        {
            Debug.Log($"🎮 NEW INPUT SYSTEM: Motor={rawMotorInput:F2}, Steer={rawSteerInput:F2}");
        }
        
        // Smooth the input for arcade feel (prevents sudden jerky movements)
        MotorInput = Mathf.SmoothDamp(MotorInput, rawMotorInput, ref motorInputVelocity, inputSmoothTime);
        SteerInput = Mathf.SmoothDamp(SteerInput, rawSteerInput, ref steerInputVelocity, inputSmoothTime);
        BrakeInput = Mathf.SmoothDamp(BrakeInput, rawBrakeInput, ref brakeInputVelocity, inputSmoothTime * 0.5f); // Faster brake response
        
        // Debug final smoothed values
        if (Mathf.Abs(MotorInput) > 0.1f || Mathf.Abs(SteerInput) > 0.1f)
        {
            Debug.Log($"🚗 SMOOTHED INPUT: Motor={MotorInput:F2}, Steer={SteerInput:F2}");
        }
    }
    
    // Public methods for external control (e.g., AI or cutscenes)
    public void SetInputEnabled(bool enabled)
    {
        enableInput = enabled;
        if (!enabled)
        {
            MotorInput = 0f;
            SteerInput = 0f;
            BrakeInput = 0f;
        }
    }
    
    // Override input programmatically (useful for AI or scripted sequences)
    public void SetMotorInput(float value) => MotorInput = Mathf.Clamp(value, -1f, 1f);
    public void SetSteerInput(float value) => SteerInput = Mathf.Clamp(value, -1f, 1f);
    public void SetBrakeInput(float value) => BrakeInput = Mathf.Clamp01(value);
} 