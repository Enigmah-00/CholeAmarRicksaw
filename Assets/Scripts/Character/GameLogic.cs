using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameLogic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject finishObject;
    [SerializeField] private TextMeshProUGUI interactionText;
    
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 3f;
    
    private bool isNearFinish = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
        
        // Find finish object if not assigned
        if (finishObject == null)
        {
            finishObject = GameObject.FindGameObjectWithTag("Finish");
            if (finishObject == null)
            {
                Debug.LogWarning("No object with 'Finish' tag found!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckProximityToFinish();
        CheckInteractionInput();
    }
    
    void CheckProximityToFinish()
    {
        if (finishObject == null) return;
        
        float distance = Vector3.Distance(transform.position, finishObject.transform.position);
        
        if (distance <= interactionDistance)
        {
            if (!isNearFinish)
            {
                isNearFinish = true;
                ShowInteractionUI();
            }
        }
        else
        {
            if (isNearFinish)
            {
                isNearFinish = false;
                HideInteractionUI();
            }
        }
    }
    
    void CheckInteractionInput()
    {
        if (!isNearFinish) return;
        
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.fKey.wasPressedThisFrame)
        {
            Debug.Log("Loading Helicopter_Scene...");
            SceneManager.LoadScene("Helicopter_Scene");
        }
    }
    
    void ShowInteractionUI()
    {
        if (interactionText != null)
        {
            interactionText.text = "Press F to ride";
            interactionText.gameObject.SetActive(true);
        }
    }
    
    void HideInteractionUI()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }
}
