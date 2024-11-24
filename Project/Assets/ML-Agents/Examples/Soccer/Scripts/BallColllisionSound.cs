using System.Media;
using UnityEngine;
using UnityEngine.Audio;

public class BallCollisionSound : MonoBehaviour
{
    private AudioSource audioSource;
    public bool audioTrain;
     public AudioClip  TrainMusic; //It's only used when audioTrain is True
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioTrain == true){
            if (TrainMusic != null){
                audioSource.clip = TrainMusic;
                audioSource.Play();
            }
            else {
                Debug.LogWarning("BackgroundMusic is empty");
            }
        }        
    }

    void OnCollisionEnter(Collision collision)
    {
        // Play the collision sound
        if (audioSource != null)
        {
            audioSource.Play();
            Debug.Log("Sound");
        }

        // Notify all agents about the sound
        NotifyAgentsAboutSound();
    }

    private void NotifyAgentsAboutSound()
    {
        // Find all agents with the BlueAgent tag
        GameObject[] blueAgents = GameObject.FindGameObjectsWithTag("blueAgent");
        // Find all agents with the PurpleAgent tag
        GameObject[] purpleAgents = GameObject.FindGameObjectsWithTag("purpleAgent");

        // Combine the arrays into one
        GameObject[] allAgents = new GameObject[blueAgents.Length + purpleAgents.Length];
        blueAgents.CopyTo(allAgents, 0);
        purpleAgents.CopyTo(allAgents, blueAgents.Length);

        // Notify each agent about the sound
        foreach (GameObject agent in allAgents)
        {
            SoundSensor sensor = agent.GetComponentInChildren<SoundSensor>();
            if (sensor != null && sensor.CanHearSound(transform.position))
            {
                // Pass the direction of the ball relative to the agent
                Vector3 directionToBall = (transform.position - agent.transform.position).normalized;
                sensor.ReceiveBallDirection(directionToBall);
            }
        }
    }
}
