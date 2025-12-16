using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    CarHandler carHandler;

    void Awake()
    {
        if (carHandler == null)
        {
            // Try to find in parent first
            carHandler = GetComponentInParent<CarHandler>();
        }
        if (carHandler == null)
        {
            // Try in same GameObject
            carHandler = GetComponent<CarHandler>();
        }
        if (carHandler == null)
        {
            // Try in children
            carHandler = GetComponentInChildren<CarHandler>();
        }
        if (carHandler == null)
        {
            // Last resort: find anywhere in scene
            carHandler = FindAnyObjectByType<CarHandler>();
        }
        if (carHandler == null)
        {
            Debug.LogError("InputHandler: CarHandler not found! Make sure CarHandler script is attached to the player car.");
        }
        else
        {
            Debug.Log($"InputHandler: Successfully found CarHandler on {carHandler.gameObject.name}");
        }
    }

    public void OnMove(InputValue value)
    {
        if (carHandler == null) return;
        Vector2 input = value.Get<Vector2>();
        carHandler.SetInput(input);
    }

    public void OnRestart(InputValue value)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
