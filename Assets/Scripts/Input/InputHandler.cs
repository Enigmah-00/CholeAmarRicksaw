using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    CarHandler carHandler;

    [SerializeField]
    [Range(0f, 0.5f)]
    float deadzone = 0.1f;

    [Header("Mobile Touch Controls")]
    [SerializeField]
    bool enableTouchControls = true;

    private Vector2 touchStartPos;
    private Vector2 currentTouchInput;
    private bool isTouching = false;

    void Awake()
    {
        if (carHandler == null)
        {
            carHandler = GetComponentInParent<CarHandler>();
        }
        if (carHandler == null)
        {
            carHandler = GetComponent<CarHandler>();
        }
        if (carHandler == null)
        {
            Debug.LogError("InputHandler: CarHandler reference not set and not found in parent or self.");
        }
    }

    void Update()
    {
        // Handle mobile touch input
        if (enableTouchControls && Application.isMobilePlatform)
        {
            HandleTouchInput();
        }

        // New Input System restart fallback (works even without a Restart action wired)
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            RestartScene();
        
        // Horn on H key press
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame && carHandler != null)
            carHandler.PlayHorn();

        // Touch-based horn (double tap anywhere)
        if (enableTouchControls && Input.touchCount == 2)
        {
            carHandler?.PlayHorn();
        }
    }

    void HandleTouchInput()
    {
        if (carHandler == null) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case UnityEngine.TouchPhase.Began:
                    touchStartPos = touch.position;
                    isTouching = true;
                    break;

                case UnityEngine.TouchPhase.Moved:
                case UnityEngine.TouchPhase.Stationary:
                    if (isTouching)
                    {
                        Vector2 delta = touch.position - touchStartPos;
                        
                        // Horizontal steering based on touch position relative to screen center
                        float screenCenterX = Screen.width * 0.5f;
                        float normalizedX = (touch.position.x - screenCenterX) / (Screen.width * 0.5f);
                        normalizedX = Mathf.Clamp(normalizedX, -1f, 1f);

                        // Vertical movement - always accelerate, swipe down to brake
                        float normalizedY = 1.0f; // Default: always accelerate
                        if (delta.y < -50f) // Swipe down to brake
                        {
                            normalizedY = -1.0f;
                        }

                        currentTouchInput = new Vector2(normalizedX, normalizedY);
                        
                        if (Mathf.Abs(currentTouchInput.x) < deadzone)
                            currentTouchInput.x = 0f;
                        
                        carHandler.SetInput(currentTouchInput);
                    }
                    break;

                case UnityEngine.TouchPhase.Ended:
                case UnityEngine.TouchPhase.Canceled:
                    isTouching = false;
                    // Return to center steering but keep accelerating
                    currentTouchInput = new Vector2(0f, 1f);
                    carHandler.SetInput(currentTouchInput);
                    break;
            }
        }
        else if (isTouching)
        {
            // No touches, reset
            isTouching = false;
            currentTouchInput = new Vector2(0f, 1f);
            carHandler.SetInput(currentTouchInput);
        }
    }

    public void OnMove(InputValue value)
    {
        if (carHandler == null)
            return;

        Vector2 input = value.Get<Vector2>();

        if (Mathf.Abs(input.x) < deadzone)
            input.x = 0f;
        if (Mathf.Abs(input.y) < deadzone)
            input.y = 0f;

        carHandler.SetInput(input);
    }

    public void OnRestart(InputValue value)
    {
        RestartScene();
    }

    void RestartScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
