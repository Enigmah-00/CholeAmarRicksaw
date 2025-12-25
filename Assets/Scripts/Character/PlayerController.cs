using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    [Header("References")]
    private UnityEngine.CharacterController controller;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;
    private AudioSource audioSource;
    private AudioSource jumpAudioSource;

    [Header("Audio")]
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private float footstepInterval = 0.5f;
    private float footstepTimer = 0f;
    [SerializeField] private AudioClip jumpSound;


    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float turningSpeed = 5f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpHeight = 2f;
    private float verticalVelocity;
    [Header("Input")]
    private float moveInput;
    private float turnInput;

    private void inputManagement()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            // Vertical input (Up/Down arrows or W/S)
            moveInput = 0f;
            if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed)
                moveInput = 1f;
            else if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed)
                moveInput = -1f;
            
            // Horizontal input (Left/Right arrows or A/D)
            turnInput = 0f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
                turnInput = 1f;
            else if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
                turnInput = -1f;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<UnityEngine.CharacterController>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
        
        // Create a second AudioSource for jump sounds
        jumpAudioSource = gameObject.AddComponent<AudioSource>();
        jumpAudioSource.playOnAwake = false;
        jumpAudioSource.spatialBlend = 1f; // 3D sound
        
        // Debug checks
        if (animator == null)
        {
            Debug.LogError("No Animator component found on " + gameObject.name);
        }
        else
        {
            Debug.Log("Animator found!");
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogError("No Animator Controller assigned to Animator!");
            }
            else
            {
                Debug.Log("Animator Controller assigned: " + animator.runtimeAnimatorController.name);
            }
            
            if (animator.avatar == null)
            {
                Debug.LogWarning("No Avatar assigned to Animator! Animations may not work.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        inputManagement();
        Movement();
    }
    private void Movement()
    {
        GroundMovement();
        Turn();
    }
    private void Turn(){
        if(Mathf.Abs(turnInput) > 0 || Mathf.Abs(moveInput) > 0){
            Vector3 currentLookDirection = controller.velocity.normalized;
            currentLookDirection.y = 0;

            currentLookDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(currentLookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,Time.deltaTime*turningSpeed);
        }

    }

    private float VerticalForceCalculation(){
        if(controller.isGrounded){
            verticalVelocity = -1f;
            Keyboard keyboard = Keyboard.current;
            if(keyboard != null && keyboard.spaceKey.wasPressedThisFrame){
                verticalVelocity = Mathf.Sqrt(2*gravity*jumpHeight);
                
                // Trigger jump animation
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
                
                // Play jump sound
                if (jumpSound != null && jumpAudioSource != null)
                {
                    jumpAudioSource.PlayOneShot(jumpSound);
                }
            }
        }
        else{
            verticalVelocity -= gravity*Time.deltaTime;
        }
        return verticalVelocity;
    }
    private void GroundMovement()
    {
        // Calculate gravity first
        VerticalForceCalculation();
        
        Vector3 move = new Vector3(turnInput,0,moveInput);
        move = cameraTransform.transform.TransformDirection(move);
        
        move *= walkSpeed;
        move.y = verticalVelocity;
        controller.Move(move*Time.deltaTime);
        
        // Update animator parameters for running animation
        if (animator != null)
        {
            // Calculate movement speed (0 = idle, 1 = walking/running)
            float speed = new Vector3(turnInput, 0, moveInput).magnitude;
            
            // Disable running animation while jumping (not grounded)
            if (!controller.isGrounded)
            {
                speed = 0f;
            }
            
            animator.SetFloat("Speed", speed);
            animator.SetBool("IsGrounded", controller.isGrounded);
            
            // Play footstep sound when moving and grounded
            if (speed > 0.1f && controller.isGrounded)
            {
                PlayFootstepSound();
            }
            else
            {
                footstepTimer = 0f; // Reset timer when not moving
            }
        }
    }
    
    private void PlayFootstepSound()
    {
        if (footstepSound == null || audioSource == null) return;
        
        footstepTimer += Time.deltaTime;
        
        if (footstepTimer >= footstepInterval)
        {
            audioSource.PlayOneShot(footstepSound);
            footstepTimer = 0f;
        }
    }
}
