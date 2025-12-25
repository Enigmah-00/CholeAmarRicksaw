using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Conversation : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip audioClip1;
    [SerializeField] private AudioClip audioClip2;
    [SerializeField] private AudioClip audioClip3;
    
    private AudioSource audioSource;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Start playing the audio clips in sequence
        StartCoroutine(PlayAudioSequence());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator PlayAudioSequence()
    {
        // Play first audio clip
        if (audioClip1 != null)
        {
            audioSource.clip = audioClip1;
            audioSource.Play();
            Debug.Log("Playing Audio 1");
            
            // Wait until 1 second before it finishes
            yield return new WaitForSeconds(audioClip1.length - 1f);
        }
        
        // Play second audio clip
        if (audioClip2 != null)
        {
            audioSource.clip = audioClip2;
            audioSource.Play();
            Debug.Log("Playing Audio 2");
            
            // Wait for it to finish
            yield return new WaitForSeconds(audioClip2.length);
        }
        
        // Play third audio clip
        if (audioClip3 != null)
        {
            audioSource.clip = audioClip3;
            audioSource.Play();
            Debug.Log("Playing Audio 3");
            
            // Wait for it to finish
            yield return new WaitForSeconds(audioClip3.length);
        }
        
        Debug.Log("All audio clips finished!");
        
        // Load the next scene
        // Note: Make sure "Character_Run_Scene" is added to Build Settings
        // Go to File > Build Profiles and add the scene
        SceneManager.LoadScene("Character_Run_Scene");
    }
}
