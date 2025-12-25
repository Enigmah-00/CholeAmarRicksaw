using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip backgroundMusic;
    private AudioSource audioSource;
    
    void Start()
    {
        SetupAudio();
    }
    
    void SetupAudio()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure for looping
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = 0.5f;
        
        // Play if not already playing
        if (backgroundMusic != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    
    // Function to load Start_Conversation scene
    public void LoadStartConversation()
    {
        Debug.Log("Loading Start_Conversation scene...");
        SceneManager.LoadScene("Start_Convo_Scene");
    }
    
    // Function to load Car_Scene
    public void LoadCarScene()
    {
        Debug.Log("Loading Car_Scene...");
        SceneManager.LoadScene("Car_Scene");
    }
    
    // Function to load EndlessCar scene
    public void LoadEndlessCarScene()
    {
        Debug.Log("Loading EndlessCar scene...");
        SceneManager.LoadScene("EndlessCar");
    }
    
    // Function to quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
