using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ATVInputHandler))]
public class SimpleArcadeATVController : MonoBehaviour
{
    [Header("Arcade Driving Settings")]
    [SerializeField] private float accelerationForce = 45000f; // Stronger for heavier mass
    [SerializeField] private float reverseForce = 25000f;
    [SerializeField] private float maxSpeed = 40f; // Speed limit for sanity
    [SerializeField] private float steerSpeed = 200f; // Faster steering
    [SerializeField] private float brakeForce = 30000f;
    
    [Header("Arcade Physics")]
    [SerializeField] private float downforce = 200f; // More downforce for better gravity
    [SerializeField] private float dragCoefficient = 0.99f; // Minimal drag for momentum
    [SerializeField] private float steerHelper = 0.8f;
    [SerializeField] private float traction = 1f;
    
    [Header("Grounding")]
    [SerializeField] private LayerMask groundLayer = -1;
    [SerializeField] private float groundRayLength = 2f;
    
    // Components
    private Rigidbody rb;
    private ATVInputHandler inputHandler;
    
    // Runtime
    private bool isGrounded;
    private float currentSteerAngle;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = GetComponent<ATVInputHandler>();
        
        // Set center of mass low for stability
        rb.centerOfMass = new Vector3(0, -0.3f, 0);
    }
    
    void FixedUpdate()
    {
        // Check if grounded
        CheckGrounded();
        
        // Get input
        float motor = inputHandler.MotorInput;
        float steering = inputHandler.SteerInput;
        float brake = inputHandler.BrakeInput;
        
        // DEBUG INPUT - Log input values
        if (Time.fixedTime % 1f < Time.fixedDeltaTime && (Mathf.Abs(motor) > 0.1f || Mathf.Abs(steering) > 0.1f))
        {
            Debug.Log($"INPUT: Motor={motor:F2}, Steering={steering:F2}, Brake={brake:F2}");
        }
        
        // Apply forces when grounded (or always for testing)
        bool canMove = isGrounded || transform.position.y > -10f; // Allow movement unless we've fallen too far
        
        // DEBUG: Physics state every second
        if (Time.fixedTime % 1f < Time.fixedDeltaTime)
        {
            Debug.Log($"🔧 PHYSICS STATE: Grounded={isGrounded} CanMove={canMove} Mass={rb.mass} Drag={rb.linearDamping}");
            Debug.Log($"🔧 RIGIDBODY: Position={transform.position} Velocity={rb.linearVelocity.magnitude:F2} IsKinematic={rb.isKinematic}");
        }
        
        if (canMove)
        {
            // ARCADE MOTOR - Fast but controlled
            if (Mathf.Abs(motor) > 0.1f)
            {
                // Only apply force if under max speed
                if (rb.linearVelocity.magnitude < maxSpeed)
                {
                    float motorPower = motor > 0 ? accelerationForce : reverseForce;
                    Vector3 forceVector = transform.forward * motor * motorPower;
                    
                    // Wake up the rigidbody if it's sleeping
                    if (rb.IsSleeping())
                    {
                        rb.WakeUp();
                        Debug.Log("💤 WAKING UP RIGIDBODY!");
                    }
                    
                    rb.AddForce(forceVector, ForceMode.Force);
                    
                    Debug.Log($"🚗 APPLYING FORCE: {forceVector.magnitude:F0}N | Mass: {rb.mass}kg | Force/Mass: {forceVector.magnitude/rb.mass:F1} | Speed: {rb.linearVelocity.magnitude:F1}/{maxSpeed} m/s");
                    Debug.Log($"🔍 PHYSICS DEBUG: Kinematic={rb.isKinematic} | Sleeping={rb.IsSleeping()} | Constraints={rb.constraints} | Velocity={rb.linearVelocity}");
                }
                else
                {
                    Debug.Log($"🏁 MAX SPEED REACHED: {rb.linearVelocity.magnitude:F1} m/s");
                }
            }
            
            // ARCADE STEERING - CONTROLLED CAR-LIKE TURNING!
            if (Mathf.Abs(steering) > 0.1f && rb.linearVelocity.magnitude > 0.5f)
            {
                // Detect if going forward or backward
                float forwardDot = Vector3.Dot(rb.linearVelocity.normalized, transform.forward);
                bool goingBackward = forwardDot < 0f;
                
                // Car-like steering: rotate the ATV directly based on speed
                float steerAmount = steering * steerSpeed * Time.fixedDeltaTime;
                
                // Invert steering when going backward (like a real car)
                if (goingBackward)
                    steerAmount *= -1f;
                
                float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / 15f); // Smoother speed scaling
                float finalSteer = steerAmount * speedFactor; // Removed the 0.7 reduction for faster steering
                
                transform.Rotate(0, finalSteer, 0);
                
                string direction = goingBackward ? "REVERSE" : "FORWARD";
                Debug.Log($"🎯 {direction} STEERING: {finalSteer:F1}° | Input: {steering:F2} | Speed: {rb.linearVelocity.magnitude:F1}");
            }
            
            // Braking
            if (brake > 0.1f)
            {
                Vector3 brakeVector = -rb.linearVelocity.normalized * brakeForce * brake;
                rb.AddForce(brakeVector);
            }
            
            // Simplified physics - remove competing forces for now
            // (Traction and steering helper disabled to prevent force conflicts)
        }
        
        // MINIMAL DRAG - Preserve momentum for ramps
        rb.linearVelocity *= dragCoefficient;
        
        // Add MUCH lighter downward force (was way too heavy!)
        rb.AddForce(Vector3.down * downforce * 0.1f); // Only 20N instead of 60,000N!
    }
    
    private void CheckGrounded()
    {
        // Cast ray down to check if we're on the ground
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f; // Start slightly above center
        
        // Try raycast without layer mask first (hit everything)
        bool hitSomething = Physics.Raycast(rayStart, -transform.up, out hit, groundRayLength);
        isGrounded = hitSomething;
        
        // Visual debug - you can see this in scene view
        Debug.DrawRay(rayStart, -transform.up * groundRayLength, isGrounded ? Color.green : Color.red);
        
        // ACTIVE DEBUG - always show in console  
        if (Time.fixedTime % 1f < Time.fixedDeltaTime) // Log once per second
        {
            if (isGrounded)
            {
                Debug.Log($"🌍 ATV GROUNDED ✓ Y={transform.position.y:F2}, Hit: {hit.collider.name} at distance {hit.distance:F2}");
            }
            else
            {
                Debug.Log($"🌍 ATV NOT GROUNDED ✗ Y={transform.position.y:F2}, Raycast failed");
            }
        }
    }
    
    // Public getters
    public bool IsGrounded => isGrounded;
    public float GetCurrentSpeed() => rb.linearVelocity.magnitude;
    public Vector3 GetVelocity() => rb.linearVelocity;
    
    // Reset method
    public void ResetATV()
    {
        transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = new Vector3(0, 2, 0);
    }
    
    // Debug info display (visible in inspector)
    void OnGUI()
    {
        if (Application.isPlaying)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"ATV Debug Info:");
            GUILayout.Label($"Grounded: {(isGrounded ? "YES" : "NO")}");
            GUILayout.Label($"Speed: {GetCurrentSpeed():F1} m/s");
            GUILayout.Label($"Position Y: {transform.position.y:F2}");
            GUILayout.Label($"Motor Input: {inputHandler?.MotorInput:F2}");
            GUILayout.Label($"Steer Input: {inputHandler?.SteerInput:F2}");
            GUILayout.Label("Controls: WASD to move");
            GUILayout.EndArea();
        }
    }
} 