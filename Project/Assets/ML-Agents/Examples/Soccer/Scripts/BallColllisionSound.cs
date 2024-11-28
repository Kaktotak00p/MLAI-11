using UnityEngine;
using UnityEngine.Audio;

public class BallCollisionSound : MonoBehaviour
{
    private AudioSource audioSource; // Reference to the AudioSource component
    public bool audioTrain;          // Whether to play training audio
    public AudioClip TrainMusic;     // Music clip to play during training

    void Start()
    {
        // Try to get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on this GameObject.");
            return;
        }

        // Play training music if enabled
        if (audioTrain)
        {
            if (TrainMusic != null)
            {
                audioSource.clip = TrainMusic;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("TrainMusic is null. No audio will play.");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Play the collision sound
        if (audioSource != null)
        {
            audioSource.Play();
            Debug.Log("Collision sound played.");
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
            if (sensor == null)
            {
                Debug.LogWarning($"No SoundSensor component found on agent: {agent.name}");
                continue;
            }

            // Check if the agent can hear the sound
            if (sensor.CanHearSound(transform.position))
            {
                // Calculate and pass the direction of the ball relative to the agent
                Vector3 directionToBall = (transform.position - agent.transform.position).normalized;
                Debug.Log($"Notifying agent {agent.name} with direction: {directionToBall}");
                sensor.ReceiveBallDirection(directionToBall);
            }
            else
            {
                Debug.Log($"Agent {agent.name} cannot hear the sound.");
            }
        }
    }
}
