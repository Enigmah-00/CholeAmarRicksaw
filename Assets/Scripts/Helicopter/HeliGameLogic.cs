using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HeliGameLogic : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject gameOverCanvas;
    
    [Header("Settings")]
    [SerializeField] private float countdownTime = 40f;
    
    private float currentTime;
    private bool gameEnded = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = countdownTime;
        
        // Make sure time scale is normal at start
        Time.timeScale = 1f;
        
        // Hide game over canvas at start
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        
        UpdateCountdownDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameEnded) return;
        
        // Countdown timer
        currentTime -= Time.deltaTime;
        UpdateCountdownDisplay();
        
        // Check if time is up
        if (currentTime <= 0)
        {
            GameOver();
        }
    }
    
    void UpdateCountdownDisplay()
    {
        if (countdownText != null)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            countdownText.text = $"Time: {seconds}s";
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if the object has the "Finish" tag
        if (other.CompareTag("Finish"))
        {
            Debug.Log("Finish line reached!");
            LevelComplete();
        }
    }
    
    void LevelComplete()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        Debug.Log("Level Complete! Loading Car Scene...");
        SceneManager.LoadScene("Car_Scene");
    }
    
    void GameOver()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        Debug.Log("Time's up! Game Over");
        
        // Show game over canvas
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            Debug.Log("Game Over Canvas activated");
        }
        
        // Don't pause - let the button work
        // Time.timeScale = 0f; // Commented out to allow button clicks
    }
    
    // Public method to restart the current scene
    public void RestartLevel()
    {
        Debug.Log("Restart button clicked!");
        
        // Reset time scale BEFORE loading scene
        Time.timeScale = 1f;
        
        // Get current scene name
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Reloading scene: {currentSceneName}");
        
        // Reload current scene
        SceneManager.LoadScene(currentSceneName);
    }
    
    // Public method to quit game (optional)
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
