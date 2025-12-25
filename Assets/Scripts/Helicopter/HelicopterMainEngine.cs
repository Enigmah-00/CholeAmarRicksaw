using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HelicopterMainEngine : MonoBehaviour
{
    Rigidbody helicopterRigid;
    public BladeRotator MainBlade;
    public BladeRotator SubBlade;
    
    [Header("Audio")]
    [SerializeField] private AudioClip bladeRotorSound;
    private AudioSource audioSource;
    
    private float enginePower;
    public float EnginePower {
        get {
            return enginePower;
        }
        set {
            MainBlade.BladeSpeed = value * 250;
            SubBlade.BladeSpeed = value * 500;
            enginePower = value;
            
            // Update audio pitch based on engine power
            UpdateBladeAudio();
        }
    }
    
    public float effectiveHeight;
    public float EngineLift = 0.0075f;

    public float ForwardForce;
    public float BackwardForce;
    public float StrafeForce;
    public float RotationSpeed = 50f;
    public float TiltAmount = 15f;
    public float TiltSpeed = 2f;

    private Vector2 movement = Vector2.zero;
    private float rotation = 0f;
    private float currentTiltX = 0f;
    private float currentTiltZ = 0f;
    
    private bool isOnGround;
    private float distanceToGround;
    public LayerMask groundLayer;

    
    // Start is called before the first frame update
    void Start()
    {
        helicopterRigid = GetComponent<Rigidbody>();
        SetupBladeAudio();
    }
    
    void SetupBladeAudio()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure for looping blade sound
        audioSource.clip = bladeRotorSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.7f;
        audioSource.pitch = 0.5f; // Start with low pitch
        
        // Play if clip is assigned
        if (bladeRotorSound != null)
        {
            audioSource.Play();
        }
    }
    
    void UpdateBladeAudio()
    {
        if (audioSource != null && bladeRotorSound != null)
        {
            // Map engine power to pitch (0-12 -> 0.5-2.0 pitch)
            float targetPitch = Mathf.Lerp(0.5f, 2.0f, enginePower / 12f);
            audioSource.pitch = targetPitch;
            
            // Also adjust volume slightly based on power
            float targetVolume = Mathf.Lerp(0.3f, 0.9f, enginePower / 12f);
            audioSource.volume = targetVolume;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();
    }
    
    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    protected void FixedUpdate()
    {
        HelicopterHover();
        HelicopterMovements();
        HelicopterRotation();
        HelicopterTilt();
    }
    
    void HandleInputs()
    {
        if (!isOnGround)
        {
            // Get movement input
            movement.x = 0;
            movement.y = 0;
            rotation = 0;
            
            if (Keyboard.current != null)
            {
                // Arrow keys and WASD for movement
                if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
                    movement.y += 1f;
                if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
                    movement.y -= 1f;
                if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
                    movement.x += 1f; // Reversed: positive for left
                if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
                    movement.x -= 1f; // Reversed: negative for right
                
                // Q/E for rotation
                if (Keyboard.current.qKey.isPressed)
                    rotation = -1f;
                if (Keyboard.current.eKey.isPressed)
                    rotation = 1f;
            }
        }
        else
        {
            // Reset movement when on ground
            movement = Vector2.zero;
            rotation = 0f;
        }
        
        // Altitude controls work regardless of ground state
        if (Keyboard.current != null)
        {
            // Space for ascending
            if (Keyboard.current.spaceKey.isPressed)
            {
                EnginePower += EngineLift;
            }
            
            // Left Shift for descending
            if (Keyboard.current.leftShiftKey.isPressed)
            {
                EnginePower -= EngineLift;
                
                if (EnginePower < 0)
                {
                    EnginePower = 0;
                }
            }
        }
    }
    
    void HelicopterHover()
    {
        float upForce = 1 - Mathf.Clamp(helicopterRigid.transform.position.y / effectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0, EnginePower, upForce) * helicopterRigid.mass;
        
        // Use world space up force to ensure vertical lift
        helicopterRigid.AddForce(Vector3.up * upForce);
    }
    
    void HelicopterMovements()
    {
        // Only apply movement forces when not on ground
        if (!isOnGround)
        {
            // Forward/Backward movement (reverse the direction)
            if (movement.y != 0)
            {
                helicopterRigid.AddRelativeForce(Vector3.forward * (-movement.y * ForwardForce * helicopterRigid.mass));
            }
            
            // Left/Right strafe movement
            if (movement.x != 0)
            {
                helicopterRigid.AddRelativeForce(Vector3.right * (movement.x * StrafeForce * helicopterRigid.mass));
            }
        }
        else
        {
            // Stop horizontal movement when on ground
            Vector3 velocity = helicopterRigid.linearVelocity;
            velocity.x = 0;
            velocity.z = 0;
            helicopterRigid.linearVelocity = velocity;
        }
    }
    
    void HelicopterRotation()
    {
        if (!isOnGround && rotation != 0)
        {
            // Apply rotation around the Y axis
            transform.Rotate(Vector3.up, rotation * RotationSpeed * Time.fixedDeltaTime);
        }
    }
    
    void HelicopterTilt()
    {
        if (!isOnGround)
        {
            // Calculate target tilt based on movement
            float targetTiltX = -movement.y * TiltAmount; // Pitch (forward/backward)
            float targetTiltZ = -movement.x * TiltAmount; // Roll (left/right)
            
            // Smoothly interpolate to target tilt
            currentTiltX = Mathf.Lerp(currentTiltX, targetTiltX, TiltSpeed * Time.fixedDeltaTime);
            currentTiltZ = Mathf.Lerp(currentTiltZ, targetTiltZ, TiltSpeed * Time.fixedDeltaTime);
            
            // Apply tilt to the helicopter
            Quaternion targetRotation = Quaternion.Euler(currentTiltX, transform.eulerAngles.y, currentTiltZ);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TiltSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Reset tilt when on ground
            currentTiltX = 0f;
            currentTiltZ = 0f;
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TiltSpeed * Time.fixedDeltaTime);
        }
    }
    
    void OnCollisionStay(Collision collision)
    {
        // Check if the helicopter is colliding with the ground layer
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isOnGround = true;
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        // Check if the helicopter left the ground layer
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isOnGround = false;
        }
    }
}
