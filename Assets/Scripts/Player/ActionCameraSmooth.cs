using UnityEngine;

public class ActionCameraSmooth : MonoBehaviour
{
    [Header("Camera Smoothing")]
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float positionSmoothTime = 0.05f;
    
    private Vector3 rotationVelocity;
    private Vector3 positionVelocity;
    private Vector3 targetRotation;
    private Vector3 targetPosition;
    
    void Start()
    {
        // Initialize target values
        targetRotation = transform.eulerAngles;
        targetPosition = transform.position;
    }
    
    void LateUpdate()
    {
        // Get the parent's (ATV's) current transform
        if (transform.parent != null)
        {
            Transform atvTransform = transform.parent;
            
            // Target position follows ATV exactly
            targetPosition = atvTransform.position;
            
            // Target rotation follows ATV but smoothed for cinematic feel
            targetRotation = atvTransform.eulerAngles;
            
            // Apply smoothing
            Vector3 smoothRotation = Vector3.SmoothDamp(transform.eulerAngles, targetRotation, ref rotationVelocity, rotationSmoothTime);
            Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref positionVelocity, positionSmoothTime);
            
            // Apply the smoothed values
            transform.rotation = Quaternion.Euler(smoothRotation);
            transform.position = smoothPosition;
        }
    }
} 