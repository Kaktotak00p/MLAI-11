using UnityEngine;

public class BallCollisionSound : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
                Debug.Log($"{agent.name} heard the sound!");
                // Optionally trigger agent behavior here
            }
        }
    }

}
