using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    void Start()
    {
 
    }

    public void PlaySound()
    {
        audioSource.PlayOneShot(clickSound);
    }
}
