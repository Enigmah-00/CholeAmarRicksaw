using UnityEngine;

public class AudioHandlerCharacterRun : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip startAudioClip;
    
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
        
        // Play the audio clip once at start
        if (startAudioClip != null)
        {
            audioSource.PlayOneShot(startAudioClip);
            Debug.Log("Playing start audio clip");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
