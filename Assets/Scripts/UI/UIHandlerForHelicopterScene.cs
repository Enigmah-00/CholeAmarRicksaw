using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class UIHandlerForHelicopterScene : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private GameObject popUpCanvas;
    [SerializeField] private GameObject foundHelicopterCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    
    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float countdownTime = 30f;
    private float currentTime;
    private bool countdownActive = true;
    
    [Header("Settings")]
    [SerializeField] private float popUpDuration = 3f;
    [SerializeField] private float interactionDistance = 3f;
    
    private GameObject finishObject;
    private GameObject player;
    private bool isNearFinish = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Show PopUp Canvas at the beginning
        if (popUpCanvas != null)
        {
            popUpCanvas.SetActive(true);
            StartCoroutine(HidePopUpAfterDelay());
        }
        
        // Hide Found Helicopter Canvas initially
        if (foundHelicopterCanvas != null)
        {
            foundHelicopterCanvas.SetActive(false);
        }
        
        // Hide Game Over Canvas initially
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        
        // Find player
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No object with 'Player' tag found!");
        }
        
        // Find finish object by tag
        finishObject = GameObject.FindGameObjectWithTag("Finish");
        if (finishObject == null)
        {
            Debug.LogWarning("No object with 'Finish' tag found!");
        }
        
        // Start countdown
        currentTime = countdownTime;
        StartCoroutine(CountdownCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        CheckProximityToFinish();
        UpdateCountdownDisplay();
    }
    
    IEnumerator CountdownCoroutine()
    {
        while (currentTime > 0 && countdownActive)
        {
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
        
        if (currentTime <= 0 && countdownActive)
        {
            TriggerGameOver();
        }
    }
    
    void UpdateCountdownDisplay()
    {
        if (countdownText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    void TriggerGameOver()
    {
        countdownActive = false;
        
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }
        
        // Disable player movement
        DisablePlayerMovement();
        
        Debug.Log("Game Over triggered!");
    }
    
    void DisablePlayerMovement()
    {
        if (player != null)
        {
            // Disable PlayerController if it exists
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
            
            // Disable CharacterController movement if it exists
            UnityEngine.CharacterController characterController = player.GetComponent<UnityEngine.CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
            }
            
            Debug.Log("Player movement disabled!");
        }
    }
    
    public void RestartGame()
    {
        Debug.Log("Restart button clicked!");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void StopCountdown()
    {
        countdownActive = false;
    }
    
    IEnumerator HidePopUpAfterDelay()
    {
        yield return new WaitForSeconds(popUpDuration);
        
        if (popUpCanvas != null)
        {
            popUpCanvas.SetActive(false);
        }
    }
    
    void CheckProximityToFinish()
    {
        if (finishObject == null || player == null) return;
        
        float distance = Vector3.Distance(player.transform.position, finishObject.transform.position);
        
        if (distance <= interactionDistance)
        {
            if (!isNearFinish)
            {
                isNearFinish = true;
                ShowFoundHelicopterCanvas();
            }
        }
        else
        {
            if (isNearFinish)
            {
                isNearFinish = false;
                HideFoundHelicopterCanvas();
            }
        }
    }
    
    void ShowFoundHelicopterCanvas()
    {
        if (foundHelicopterCanvas != null)
        {
            foundHelicopterCanvas.SetActive(true);
        }
    }
    
    void HideFoundHelicopterCanvas()
    {
        if (foundHelicopterCanvas != null)
        {
            foundHelicopterCanvas.SetActive(false);
        }
    }
    
    // Call this method to manually close the PopUp Canvas (e.g., from a button)
    public void ClosePopUpCanvas()
    {
        if (popUpCanvas != null)
        {
            popUpCanvas.SetActive(false);
            StopAllCoroutines(); // Stop the auto-hide timer if manually closed
        }
    }
}
