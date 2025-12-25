using UnityEngine;
using UnityEngine.SceneManagement;

public class CarLevelHandler : MonoBehaviour
{
    [SerializeField] private float targetDistance = 1500f;
    [SerializeField] private string endSceneName = "End_Scene";
    
    private CarHandler playerCarHandler;
    private string previousSceneName;
    private bool levelCompleted = false;

    void Start()
    {
        playerCarHandler = GameObject.FindGameObjectWithTag("Player")?.GetComponent<CarHandler>();
        
        if (playerCarHandler == null)
        {
            Debug.LogError("CarLevelHandler: Player car not found!");
            enabled = false;
            return;
        }

        // Get the previous scene name from PlayerPrefs
        previousSceneName = PlayerPrefs.GetString("PreviousSceneName", "");
        
        // Store current scene as previous for next transition
        PlayerPrefs.SetString("PreviousSceneName", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        
        Debug.Log($"Current scene: {SceneManager.GetActiveScene().name}, Previous scene: {previousSceneName}");
    }

    void Update()
    {
        if (levelCompleted || playerCarHandler == null)
            return;

        // Check if player has reached the target distance
        if (playerCarHandler.DistannceTravelled >= targetDistance)
        {
            levelCompleted = true;
            
            Debug.Log($"Target distance reached! Previous scene: '{previousSceneName}'");
            
            // Load End_Scene only if previous scene was NOT "Main Manu" or "End_Manu"
            // Check various possible spellings
            bool isFromMainManu = previousSceneName == "Main Manu" || 
                                  previousSceneName == "Main_Manu" || 
                                  previousSceneName == "MainManu" ||
                                  previousSceneName == "Main Menu";
                                  
            bool isFromEndManu = previousSceneName == "End_Manu" || 
                                 previousSceneName == "EndManu" ||
                                 previousSceneName == "End Manu";
            
            if (!isFromMainManu && !isFromEndManu)
            {
                Debug.Log($"Loading {endSceneName}");
                SceneManager.LoadScene(endSceneName);
            }
            else
            {
                Debug.Log($"Not loading {endSceneName} because previous scene was {previousSceneName}");
            }
        }
    }
}
