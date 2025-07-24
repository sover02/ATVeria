using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private string npcName = "Buddy";
    
    [Header("Dialogue")]
    [SerializeField] private string greetingMessage = "Hey there, fellow ATV! How's the driving going?";
    [SerializeField] private string[] possibleResponses = {
        "Great! The ramps are awesome!",
        "Could be better... having some issues.",
        "Just exploring the world!",
        "Need some tips for jumping!"
    };
    
    private bool playerInRange = false;
    private bool dialogueActive = false;
    private int currentResponseIndex = 0;
    private Transform playerTransform;
    
    void Start()
    {
        Debug.Log("🎮 NPCInteraction script started!");
        
        // Find the player ATV
        GameObject playerATV = GameObject.Find("ATV");
        if (playerATV != null)
        {
            playerTransform = playerATV.transform;
            Debug.Log("✅ Found player ATV for NPC interaction");
        }
        else
        {
            Debug.LogError("❌ Could not find player ATV for NPC interaction!");
        }
    }
    
    void Update()
    {
        if (playerTransform == null) return;
        
        // Check if player is in range
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionDistance;
        
        // Debug proximity detection
        if (Time.frameCount % 60 == 0) // Log every 60 frames (about once per second)
        {
            Debug.Log($"🎮 NPC Proximity: Distance={distance:F1}, InRange={playerInRange}, DialogueActive={dialogueActive}");
        }
        
        // Show/hide interaction prompt
        if (playerInRange && !wasInRange)
        {
            Debug.Log($"🎯 {npcName} is nearby! Press SPACEBAR to talk!");
        }
        else if (!playerInRange && wasInRange)
        {
            Debug.Log($"👋 {npcName} waves goodbye!");
        }
        
        // Handle spacebar input for interaction
        if (playerInRange && !dialogueActive)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                StartDialogue();
            }
        }
        
        // Handle dialogue navigation
        if (dialogueActive)
        {
            HandleDialogueInput();
        }
    }
    
    void StartDialogue()
    {
        dialogueActive = true;
        currentResponseIndex = 0;
        Debug.Log($"💬 {npcName}: \"{greetingMessage}\"");
        Debug.Log("💬 Press SPACEBAR to respond, or ESC to exit");
        Debug.Log("🎮 GUI DIALOGUE ACTIVATED - Should see dialogue box on screen!");
        ShowResponseOptions();
    }
    
    void HandleDialogueInput()
    {
        if (Keyboard.current == null) return;
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Select current response
            string selectedResponse = possibleResponses[currentResponseIndex];
            Debug.Log($"💬 You: \"{selectedResponse}\"");
            
            // NPC response based on selection
            string npcResponse = GetNPCResponse(selectedResponse);
            Debug.Log($"💬 {npcName}: \"{npcResponse}\"");
            
            EndDialogue();
        }
        else if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log($"💬 {npcName}: \"See you around, friend!\"");
            EndDialogue();
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
        {
            // Navigate up in responses
            currentResponseIndex = (currentResponseIndex - 1 + possibleResponses.Length) % possibleResponses.Length;
            ShowResponseOptions();
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
        {
            // Navigate down in responses
            currentResponseIndex = (currentResponseIndex + 1) % possibleResponses.Length;
            ShowResponseOptions();
        }
    }
    
    void ShowResponseOptions()
    {
        Debug.Log("💬 Response options:");
        for (int i = 0; i < possibleResponses.Length; i++)
        {
            string indicator = (i == currentResponseIndex) ? " > " : "   ";
            Debug.Log($"💬 {indicator}{possibleResponses[i]}");
        }
    }
    
    string GetNPCResponse(string playerResponse)
    {
        if (playerResponse.Contains("awesome") || playerResponse.Contains("Great"))
        {
            return "That's fantastic! I love seeing other ATVs having fun with the ramps!";
        }
        else if (playerResponse.Contains("issues") || playerResponse.Contains("better"))
        {
            return "Don't worry, friend! It takes time to master the physics. Try the green ramp first!";
        }
        else if (playerResponse.Contains("exploring"))
        {
            return "The world is full of amazing places to explore! Have you tried all the ramps yet?";
        }
        else if (playerResponse.Contains("tips") || playerResponse.Contains("jumping"))
        {
            return "Pro tip: Build up speed before hitting ramps, and don't steer while airborne!";
        }
        else
        {
            return "That's interesting! I'm always here if you need a chat or some driving tips!";
        }
    }
    
    void EndDialogue()
    {
        dialogueActive = false;
        Debug.Log("💬 Press SPACEBAR near me to chat again!");
    }
    
    void OnGUI()
    {
        // Simple test GUI - always show something to test if GUI is working
        GUI.color = Color.red;
        GUI.Label(new Rect(10, 10, 200, 30), "GUI TEST - NPCInteraction OnGUI is working!");
        
        // Force GUI to always render when dialogue is active
        if (dialogueActive)
        {
            Debug.Log("🎮 RENDERING DIALOGUE GUI - This should appear on screen!");
            
            // Create a semi-transparent background
            GUI.color = new Color(0, 0, 0, 0.8f);
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            
            // Main dialogue box (top-left area, like the old debug GUI)
            GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            GUI.Box(new Rect(10, 10, 400, 200), "");
            
            // NPC name and message
            GUI.color = Color.cyan;
            GUI.Label(new Rect(20, 20, 380, 30), $"{npcName}:");
            GUI.color = Color.white;
            GUI.Label(new Rect(20, 50, 380, 40), greetingMessage);
            
            // Response options
            GUI.color = Color.yellow;
            GUI.Label(new Rect(20, 90, 380, 20), "Your response:");
            
            for (int i = 0; i < possibleResponses.Length; i++)
            {
                if (i == currentResponseIndex)
                {
                    GUI.color = Color.yellow;
                    GUI.Label(new Rect(30, 110 + i * 25, 360, 25), 
                        $"> {possibleResponses[i]}");
                }
                else
                {
                    GUI.color = Color.white;
                    GUI.Label(new Rect(30, 110 + i * 25, 360, 25), 
                        $"  {possibleResponses[i]}");
                }
            }
            
            // Controls help
            GUI.color = Color.gray;
            GUI.Label(new Rect(20, 170, 380, 20), 
                "SPACEBAR: Select | UP/DOWN: Navigate | ESC: Exit");
        }
        else if (playerInRange)
        {
            // Show interaction prompt when in range (top-left area)
            GUI.color = Color.yellow;
            GUI.Label(new Rect(10, 10, 300, 30), 
                $"Press SPACEBAR to talk to {npcName}");
        }
        
        // Debug: Always log GUI calls to see if it's being called
        if (dialogueActive || playerInRange)
        {
            Debug.Log($"🎮 GUI CALLED: dialogueActive={dialogueActive}, playerInRange={playerInRange}");
        }
    }
} 