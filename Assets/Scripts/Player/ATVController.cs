using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ATVInputHandler))]
public class ATVController : MonoBehaviour
{
    [Header("Wheel References")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;
    
    [Header("Optional Wheel Visuals (for rotation)")]
    public Transform frontLeftWheelModel;
    public Transform frontRightWheelModel;
    public Transform rearLeftWheelModel;
    public Transform rearRightWheelModel;
    
    [Header("Arcade Driving Settings")]
    [SerializeField] private float maxMotorTorque = 2000f;
    [SerializeField] private float maxSteerAngle = 35f;
    [SerializeField] private float maxBrakeTorque = 3000f;
    
    [Header("Physics Tuning")]
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0, -0.5f, 0);
    [SerializeField] private float downforce = 100f;
    [SerializeField] private bool allWheelDrive = true; // If false, only rear wheels get power
    
    [Header("Arcade Feel Adjustments")]
    [SerializeField] private AnimationCurve steerCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve motorCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float antiRollForce = 5000f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    // Components
    private Rigidbody rb;
    private ATVInputHandler inputHandler;
    
    // Runtime variables
    private float currentSteerAngle;
    private float currentMotorTorque;
    private float currentBrakeTorque;
    
    void Start()
    {
        // Get required components
        rb = GetComponent<Rigidbody>();
        inputHandler = GetComponent<ATVInputHandler>();
        
        // Set center of mass for better handling and jumping
        rb.centerOfMass = centerOfMassOffset;
        
        // Validate wheel colliders
        ValidateWheelSetup();
    }
    
    void FixedUpdate()
    {
        // Get input from the input handler
        float motorInput = inputHandler.MotorInput;
        float steerInput = inputHandler.SteerInput;
        float brakeInput = inputHandler.BrakeInput;
        
        // Apply input through curves for arcade feel
        float adjustedMotorInput = motorCurve.Evaluate(Mathf.Abs(motorInput)) * Mathf.Sign(motorInput);
        float adjustedSteerInput = steerCurve.Evaluate(Mathf.Abs(steerInput)) * Mathf.Sign(steerInput);
        
        // Calculate forces
        currentMotorTorque = adjustedMotorInput * maxMotorTorque;
        currentSteerAngle = adjustedSteerInput * maxSteerAngle;
        currentBrakeTorque = brakeInput * maxBrakeTorque;
        
        // Apply steering to front wheels
        ApplySteering();
        
        // Apply motor torque
        ApplyMotorTorque();
        
        // Apply braking
        ApplyBraking();
        
        // Apply downforce for better ground contact at speed
        ApplyDownforce();
        
        // Apply anti-roll bars for arcade stability
        ApplyAntiRoll();
        
        // Update wheel visuals if assigned
        UpdateWheelVisuals();
        
        // Debug info
        if (showDebugInfo)
            DisplayDebugInfo();
    }
    
    private void ApplySteering()
    {
        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;
    }
    
    private void ApplyMotorTorque()
    {
        if (allWheelDrive)
        {
            // Apply torque to all wheels for better acceleration and climbing
            frontLeftWheel.motorTorque = currentMotorTorque * 0.7f;  // Slightly less to front
            frontRightWheel.motorTorque = currentMotorTorque * 0.7f;
            rearLeftWheel.motorTorque = currentMotorTorque;
            rearRightWheel.motorTorque = currentMotorTorque;
        }
        else
        {
            // Rear wheel drive only
            frontLeftWheel.motorTorque = 0;
            frontRightWheel.motorTorque = 0;
            rearLeftWheel.motorTorque = currentMotorTorque;
            rearRightWheel.motorTorque = currentMotorTorque;
        }
    }
    
    private void ApplyBraking()
    {
        // Apply braking to all wheels
        float brakeForce = currentBrakeTorque;
        
        frontLeftWheel.brakeTorque = brakeForce;
        frontRightWheel.brakeTorque = brakeForce;
        rearLeftWheel.brakeTorque = brakeForce;
        rearRightWheel.brakeTorque = brakeForce;
    }
    
    private void ApplyDownforce()
    {
        // Add downward force based on speed for better contact with ground
        float speed = rb.linearVelocity.magnitude;
        float downForce = downforce * speed * rb.mass;
        rb.AddForce(-transform.up * downForce);
    }
    
    private void ApplyAntiRoll()
    {
        // Anti-roll bars help prevent tipping and improve arcade feel
        ApplyAntiRollBar(frontLeftWheel, frontRightWheel);
        ApplyAntiRollBar(rearLeftWheel, rearRightWheel);
    }
    
    private void ApplyAntiRollBar(WheelCollider leftWheel, WheelCollider rightWheel)
    {
        WheelHit leftHit;
        WheelHit rightHit;
        bool leftGrounded = leftWheel.GetGroundHit(out leftHit);
        bool rightGrounded = rightWheel.GetGroundHit(out rightHit);
        
        if (leftGrounded || rightGrounded)
        {
            float leftTravel = 1.0f;
            float rightTravel = 1.0f;
            
            if (leftGrounded)
                leftTravel = (-leftWheel.transform.InverseTransformPoint(leftHit.point).y - leftWheel.radius) / leftWheel.suspensionDistance;
            
            if (rightGrounded)
                rightTravel = (-rightWheel.transform.InverseTransformPoint(rightHit.point).y - rightWheel.radius) / rightWheel.suspensionDistance;
            
            float antiRollTorque = (leftTravel - rightTravel) * antiRollForce;
            
            if (leftGrounded)
                rb.AddForceAtPosition(leftWheel.transform.up * -antiRollTorque, leftWheel.transform.position);
            if (rightGrounded)
                rb.AddForceAtPosition(rightWheel.transform.up * antiRollTorque, rightWheel.transform.position);
        }
    }
    
    private void UpdateWheelVisuals()
    {
        // Update wheel model positions and rotations if assigned
        UpdateWheelModel(frontLeftWheel, frontLeftWheelModel);
        UpdateWheelModel(frontRightWheel, frontRightWheelModel);
        UpdateWheelModel(rearLeftWheel, rearLeftWheelModel);
        UpdateWheelModel(rearRightWheel, rearRightWheelModel);
    }
    
    private void UpdateWheelModel(WheelCollider wheelCollider, Transform wheelModel)
    {
        if (wheelModel == null) return;
        
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        
        wheelModel.position = position;
        wheelModel.rotation = rotation;
    }
    
    private void ValidateWheelSetup()
    {
        if (frontLeftWheel == null || frontRightWheel == null || rearLeftWheel == null || rearRightWheel == null)
        {
            Debug.LogError("ATVController: One or more WheelColliders are not assigned! Please assign all four wheel colliders.", this);
        }
    }
    
    private void DisplayDebugInfo()
    {
        Debug.Log($"ATV Debug - Speed: {rb.linearVelocity.magnitude:F1} m/s | Motor: {currentMotorTorque:F0} | Steer: {currentSteerAngle:F1}° | Brake: {currentBrakeTorque:F0}");
    }
    
    // Public methods for external systems
    public float GetCurrentSpeed() => rb.linearVelocity.magnitude;
    public bool IsGrounded() => 
        frontLeftWheel.isGrounded || frontRightWheel.isGrounded || 
        rearLeftWheel.isGrounded || rearRightWheel.isGrounded;
    public Vector3 GetVelocity() => rb.linearVelocity;
    
    // Utility method to reset the ATV if it gets stuck or flipped
    public void ResetATV()
    {
        transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
} 