using UnityEngine;
// this makes so sound is made when ball hits sth

public class BallCollisionSound : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}