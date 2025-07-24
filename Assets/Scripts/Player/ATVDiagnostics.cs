using UnityEngine;
using UnityEngine.InputSystem;

public class ATVDiagnostics : MonoBehaviour
{
    [Header("Auto-Diagnostics")]
    public bool runDiagnostics = true;
    
    private ATVInputHandler inputHandler;
    private SimpleArcadeATVController controller;
    private Rigidbody rb;
    
    private float diagnosticTimer;
    private int frameCount;
    
    void Start()
    {
        inputHandler = GetComponent<ATVInputHandler>();
        controller = GetComponent<SimpleArcadeATVController>();
        rb = GetComponent<Rigidbody>();
        
        if (runDiagnostics)
        {
            Debug.Log("🚗 ATV DIAGNOSTICS STARTED - Will report status every 2 seconds");
            InvokeRepeating(nameof(RunFullDiagnostic), 1f, 2f);
        }
    }
    
    void Update()
    {
        frameCount++;
        diagnosticTimer += Time.deltaTime;
        
        // Show real-time input status on screen
        if (runDiagnostics && Keyboard.current != null)
        {
            // Check for any input and immediately log it using New Input System
            bool wPressed = Keyboard.current.wKey.isPressed;
            bool sPressed = Keyboard.current.sKey.isPressed;
            bool aPressed = Keyboard.current.aKey.isPressed;
            bool dPressed = Keyboard.current.dKey.isPressed;
            
            if (wPressed || sPressed || aPressed || dPressed)
            {
                Debug.Log($"🎮 NEW INPUT KEY DETECTED: W={wPressed} S={sPressed} A={aPressed} D={dPressed}");
            }
        }
    }
    
    void RunFullDiagnostic()
    {
        Debug.Log("================== ATV DIAGNOSTIC REPORT ==================");
        
        // 1. Component Status
        Debug.Log($"📦 COMPONENTS: InputHandler={inputHandler != null} Controller={controller != null} Rigidbody={rb != null}");
        
        // 2. Input Status
        if (inputHandler != null)
        {
            Debug.Log($"🎮 INPUT VALUES: Motor={inputHandler.MotorInput:F2} Steer={inputHandler.SteerInput:F2} Brake={inputHandler.BrakeInput:F2}");
        }
        
        // 3. Physics Status
        if (rb != null)
        {
            Debug.Log($"🏃 PHYSICS: Speed={rb.linearVelocity.magnitude:F2} Position=({transform.position.x:F1}, {transform.position.y:F1}, {transform.position.z:F1})");
        }
        
        // 4. Grounding Status
        if (controller != null)
        {
            Debug.Log($"🌍 GROUNDING: IsGrounded={controller.IsGrounded} Speed={controller.GetCurrentSpeed():F2}");
        }
        
        // 5. Direct Input Test (New Input System)
        bool w = false, s = false, a = false, d = false;
        if (Keyboard.current != null)
        {
            w = Keyboard.current.wKey.isPressed;
            s = Keyboard.current.sKey.isPressed;
            a = Keyboard.current.aKey.isPressed;
            d = Keyboard.current.dKey.isPressed;
        }
        Debug.Log($"🔑 NEW INPUT KEYS: W={w} S={s} A={a} D={d}");
        
        // 6. New Input System Status
        if (Keyboard.current != null)
        {
            Debug.Log($"📐 NEW INPUT SYSTEM: Keyboard detected and ready!");
        }
        else
        {
            Debug.Log($"❌ NEW INPUT SYSTEM: No keyboard detected!");
        }
        
        // 7. Frame Rate
        float fps = frameCount / diagnosticTimer;
        Debug.Log($"📊 PERFORMANCE: FPS={fps:F0} Frame={Time.frameCount}");
        
        // 8. Final Status
        if (inputHandler != null && Mathf.Abs(inputHandler.MotorInput) > 0.1f)
        {
            Debug.Log("✅ ATV STATUS: INPUT WORKING - Should be moving!");
        }
        else if (w || s)
        {
            Debug.Log("⚠️ ATV STATUS: Keys detected but not reaching MotorInput - Input system issue");
        }
        else
        {
            Debug.Log("🔍 ATV STATUS: No input detected - Try pressing WASD now");
        }
        
        Debug.Log("=================== END DIAGNOSTIC =======================");
    }
    
    // Completely removed OnGUI to eliminate debug clutter
    // void OnGUI() { }
} 