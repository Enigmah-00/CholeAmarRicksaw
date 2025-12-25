using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneHandler : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private string nextSceneName = "Main Manu";
    [SerializeField] private bool playOnStart = true;
    
    private AudioSource audioSource;

    void Start()
    {
        if (playOnStart)
        {
            PlayAndLoadOnEnd();
        }
    }

    public void PlayAndLoadOnEnd()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }

        if (audioSource.clip == null)
        {
            return;
        }

        audioSource.Play();
        StartCoroutine(WaitForAudioThenLoad());
    }

    private System.Collections.IEnumerator WaitForAudioThenLoad()
    {
        float duration = audioSource.clip.length;
        yield return new WaitForSecondsRealtime(duration);
        SceneManager.LoadScene(nextSceneName);
    }
}
