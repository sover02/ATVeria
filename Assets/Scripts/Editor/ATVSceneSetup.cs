using UnityEngine;
using UnityEditor;
using System.Linq;

public class ATVSceneSetup : EditorWindow
{
    [MenuItem("ATVeria/Setup Complete ATV Scene")]
    public static void SetupATVScene()
    {
        // Clear existing objects (optional)
        if (EditorUtility.DisplayDialog("Setup ATV Scene", 
            "This will create a complete ATV scene. Continue?", "Yes", "Cancel"))
        {
            CreateATVScene();
        }
    }
    
    private static void CreateATVScene()
    {
        Debug.Log("Setting up ATV Scene...");
        
        // Increase gravity for better ramp physics
        Physics.gravity = new Vector3(0, -20f, 0); // Double default gravity!
        Debug.Log("✅ Increased gravity to -20 for better ramp momentum!");
        
        // THOROUGH CLEANUP - Remove all old ATV-related objects
        
        // Clear old ATVControllers (the broken ones)
        var oldATVs = Object.FindObjectsByType<ATVController>(FindObjectsSortMode.None);
        for (int i = 0; i < oldATVs.Length; i++)
        {
            Debug.Log("Removing old ATV: " + oldATVs[i].name);
            DestroyImmediate(oldATVs[i].gameObject);
        }
        
        // Clear SimpleArcadeATVControllers
        var existingATVs = Object.FindObjectsByType<SimpleArcadeATVController>(FindObjectsSortMode.None);
        for (int i = 0; i < existingATVs.Length; i++)
        {
            Debug.Log("Removing existing arcade ATV: " + existingATVs[i].name);
            DestroyImmediate(existingATVs[i].gameObject);
        }
        
        // Clear any objects named "ATV"
        var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var atvObjects = allObjects.Where(go => go.name.Contains("ATV")).ToArray();
        for (int i = 0; i < atvObjects.Length; i++)
        {
            if (atvObjects[i] != null)
            {
                Debug.Log("Removing ATV object: " + atvObjects[i].name);
                DestroyImmediate(atvObjects[i]);
            }
        }
        
        // Clear all old camera systems
        var existingCameraFollowers = Object.FindObjectsByType<SimpleATVCameraFollow>(FindObjectsSortMode.None);
        for (int i = 0; i < existingCameraFollowers.Length; i++)
        {
            Debug.Log("Removing old camera follower: " + existingCameraFollowers[i].name);
            DestroyImmediate(existingCameraFollowers[i].gameObject);
        }
        
        var existingCameraRigs = GameObject.FindObjectsOfType<GameObject>().Where(go => go.name == "CameraRig").ToArray();
        for (int i = 0; i < existingCameraRigs.Length; i++)
        {
            Debug.Log("Removing old camera rig: " + existingCameraRigs[i].name);
            DestroyImmediate(existingCameraRigs[i]);
        }
        
        // Reset main camera parent
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.parent = null;
            Debug.Log("Reset main camera parent");
        }
        
        // Clear any WheelCollider objects that might be floating around
        var wheelColliders = Object.FindObjectsByType<WheelCollider>(FindObjectsSortMode.None);
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            Debug.Log("Removing orphaned wheel collider: " + wheelColliders[i].name);
            DestroyImmediate(wheelColliders[i].gameObject);
        }
        
        // Clear ALL old ramps (both old and new naming)
        var allSceneObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var rampsToRemove = allSceneObjects.Where(go => 
            go.name.Contains("Ramp") || 
            go.name.Contains("TestRamp") ||
            go.name.Contains("EasyRamp") ||
            go.name.Contains("MediumRamp") ||
            go.name.Contains("BigRamp")
        ).ToArray();
        
        Debug.Log($"Found {rampsToRemove.Length} old ramps to remove");
        for (int i = 0; i < rampsToRemove.Length; i++)
        {
            if (rampsToRemove[i] != null)
            {
                Debug.Log("Removing old ramp: " + rampsToRemove[i].name);
                DestroyImmediate(rampsToRemove[i]);
            }
        }
        
        // AGGRESSIVE TREE CLEANUP - Remove ALL existing trees
        var treesToRemove = allSceneObjects.Where(go => 
            go.name.Contains("Tree") || 
            go.name.Contains("tree") ||
            (go.GetComponent<Renderer>() != null && go.GetComponent<Renderer>().material != null && go.GetComponent<Renderer>().material.color == Color.green && go.transform.localScale.y > 1.5f)
        ).ToArray();
        
        Debug.Log($"Found {treesToRemove.Length} trees to remove");
        for (int i = 0; i < treesToRemove.Length; i++)
        {
            if (treesToRemove[i] != null)
            {
                Debug.Log("Removing old tree: " + treesToRemove[i].name);
                DestroyImmediate(treesToRemove[i]);
            }
        }
        
        // 1. Create Ground
        GameObject ground = CreateGround();
        
        // 2. Create Ramps
        CreateRamps();
        
        // 3. Create ATV
        GameObject atv = CreateATV();
        
        // 4. Setup Camera
        SetupCamera(atv);
        
        // 5. Create Environment
        CreateEnvironment();
        
        Debug.Log("ATV Scene Setup Complete! Press Play to drive!");
        
        // Focus on the ATV in scene view
        Selection.activeGameObject = atv;
    }
    
    private static GameObject CreateGround()
    {
        // Create large ground plane
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(20f, 1f, 20f); // 200x200 units
        ground.transform.position = Vector3.zero;
        
        // Use default physics material for ground to avoid errors
        Debug.Log("✅ Ground created with default physics material");
        
        // Make it gray
        var renderer = ground.GetComponent<Renderer>();
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = new Color(0.5f, 0.5f, 0.5f);
        renderer.material = material;
        
        return ground;
    }
    
    private static void CreateRamps()
    {
        // Create BETTER ramps - properly spaced and positioned, FACING THE RIGHT WAY
        // Ramp 1: Close, gentle, wide - perfect for testing
        CreateRamp("EasyRamp", new Vector3(0, 0, 12), new Vector3(-12, 0, 0), new Vector3(10, 1f, 8));
        
        // Ramp 2: Medium distance, medium angle - for building confidence
        CreateRamp("MediumRamp", new Vector3(15, 0, 20), new Vector3(-18, 0, 0), new Vector3(12, 1.5f, 10));
        
        // Ramp 3: Far, steeper - for advanced jumping
        CreateRamp("BigRamp", new Vector3(-15, 0, 25), new Vector3(-22, 0, 0), new Vector3(15, 2.5f, 12));
        
        // Create some obstacles/walls
        CreateWall("Wall1", new Vector3(-30, 1, 0), new Vector3(2, 2, 10));
        CreateWall("Wall2", new Vector3(30, 1, 10), new Vector3(2, 2, 8));
    }
    
    private static void CreateRamp(string name, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ramp.name = name;
        ramp.transform.position = position;
        ramp.transform.rotation = Quaternion.Euler(rotation);
        ramp.transform.localScale = scale;
        
        // Ramp physics - use default material for now to avoid errors
        // The anti-faceplant system in the controller will handle ramp physics
        Debug.Log($"✅ Created {name} with default physics material");
        
        // Make ramps different colors for easy identification
        var renderer = ramp.GetComponent<Renderer>();
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        
        // Color code the ramps
        if (name.Contains("Easy"))
        {
            material.color = Color.green; // Green = easy
        }
        else if (name.Contains("Medium"))
        {
            material.color = Color.yellow; // Yellow = medium
        }
        else if (name.Contains("Big"))
        {
            material.color = Color.red; // Red = hard
        }
        else
        {
            material.color = new Color(0.6f, 0.4f, 0.2f); // Brown default
        }
        
        renderer.material = material;
        
        Debug.Log($"✅ Created {name}: Position={position}, Rotation={rotation}, Scale={scale}");
    }
    
    private static void CreateWall(string name, Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.position = position;
        wall.transform.localScale = scale;
        
        // Make it red
        var renderer = wall.GetComponent<Renderer>();
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = Color.red;
        renderer.material = material;
    }
    
    private static GameObject CreateATV()
    {
        // Create main ATV GameObject
        GameObject atv = new GameObject("ATV");
        atv.transform.position = new Vector3(0, 2f, 0);
        
        // Add Rigidbody - LIGHTWEIGHT FOR MOVEMENT
        Rigidbody rb = atv.AddComponent<Rigidbody>();
        rb.mass = 300f; // Much lighter for easier movement
        rb.linearDamping = 0.05f; // Very low damping
        rb.angularDamping = 0.3f; // Low angular damping
        
        // Allow forward/back tilting but prevent side rolling
        rb.constraints = RigidbodyConstraints.FreezeRotationZ; // Can tilt forward/back, can't roll sideways
        
        // Set center of mass low for stability - PREVENTS FACEPLANTING
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Even lower center of mass
        Debug.Log($"✅ Center of mass set to: {rb.centerOfMass}");
        
        // Increase gravity effect on this rigidbody
        rb.useGravity = true;
        
        Debug.Log("✅ LIGHTWEIGHT PHYSICS: Mass=300, Very low damping, should move easily!");
        Debug.Log("✅ ANTI-FACEPLANT: Center of mass lowered to -0.5f for better ramp stability!");
        
        // Add our scripts (using the simple arcade controller)
        atv.AddComponent<ATVInputHandler>();
        var controller = atv.AddComponent<SimpleArcadeATVController>();
        atv.AddComponent<ATVDiagnostics>(); // Auto-diagnostic system
        
        // Create ATV Body (visual AND physics)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.parent = atv.transform;
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(2f, 1f, 3f);
        
        // Use default physics material for now to avoid errors
        // The traction system in the controller will handle grip
        Debug.Log("✅ ATV body created with default physics material");
        
        // Make it blue
        var bodyRenderer = body.GetComponent<Renderer>();
        var bodyMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        bodyMaterial.color = Color.blue;
        bodyRenderer.material = bodyMaterial;
        
        // Create Simple Visual Wheels (no physics, just decoration)
        var wheelPositions = new Vector3[]
        {
            new Vector3(-0.9f, -0.4f, 1.2f),  // Front Left
            new Vector3(0.9f, -0.4f, 1.2f),   // Front Right
            new Vector3(-0.9f, -0.4f, -1.2f), // Rear Left
            new Vector3(0.9f, -0.4f, -1.2f)   // Rear Right
        };
        
        for (int i = 0; i < wheelPositions.Length; i++)
        {
            GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = "Wheel_" + i;
            wheel.transform.parent = atv.transform;
            wheel.transform.localPosition = wheelPositions[i];
            wheel.transform.localScale = new Vector3(0.6f, 0.2f, 0.6f);
            wheel.transform.rotation = Quaternion.Euler(0, 0, 90);
            
            // Remove collider - just visual
            DestroyImmediate(wheel.GetComponent<CapsuleCollider>());
            
            // Make it black
            var wheelRenderer = wheel.GetComponent<Renderer>();
            var wheelMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            wheelMaterial.color = Color.black;
            wheelRenderer.material = wheelMaterial;
        }
        
        return atv;
    }
    
    private static void SetupCamera(GameObject atv)
    {
        // Find main camera or create one
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }
        
        // GOD OF WAR / CRAZY TAXI STYLE - Third person action camera
        GameObject cameraRig = new GameObject("CameraRig");
        cameraRig.transform.parent = atv.transform;
        cameraRig.transform.localPosition = Vector3.zero; // Follow the ATV center
        
        // Position camera behind and above for classic third-person view
        mainCamera.transform.parent = cameraRig.transform;
        mainCamera.transform.localPosition = new Vector3(0, 4f, -6f); // Behind and above
        mainCamera.transform.localRotation = Quaternion.Euler(12f, 0, 0); // Slight downward angle
        
        // Perfect FOV for action and NPC interaction
        mainCamera.fieldOfView = 60f;
        
        // Add slight camera smoothing for that cinematic feel
        var smoothCam = cameraRig.AddComponent<ActionCameraSmooth>();
        
        Debug.Log("✅ GOD OF WAR CAMERA: Perfect for driving and talking to NPCs!");
    }
    
    private static void CreateEnvironment()
    {
        // Add just 3 trees - good balance for environment
        Vector3[] treePositions = {
            new Vector3(-40f, 1f, -30f),
            new Vector3(35f, 1f, 25f), 
            new Vector3(-25f, 1f, 40f)
        };
        
        for (int i = 0; i < treePositions.Length; i++)
        {
            GameObject tree = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tree.name = "Tree" + i;
            tree.transform.position = treePositions[i];
            tree.transform.localScale = new Vector3(0.5f, 2f, 0.5f);
            
            // Make it green
            var renderer = tree.GetComponent<Renderer>();
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.color = Color.green;
            renderer.material = material;
        }
        
        Debug.Log("✅ Environment: 3 trees placed away from ramps");
    }
}

// Simple camera follow script
public class SimpleATVCameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    
    private Vector3 velocity = Vector3.zero;
    private Vector3 offset;
    
    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
            Debug.Log("Camera follow started - target: " + target.name + ", offset: " + offset);
        }
        else
        {
            Debug.LogError("Camera follow script has no target!");
        }
    }
    
    void LateUpdate()
    {
        if (target == null) 
        {
            Debug.LogWarning("Camera target is null!");
            return;
        }
        
        // If target falls too far below ground, don't follow downward
        Vector3 targetPosition = target.position + offset;
        if (target.position.y < -10f)
        {
            targetPosition.y = 5f; // Keep camera above ground level
        }
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
} 