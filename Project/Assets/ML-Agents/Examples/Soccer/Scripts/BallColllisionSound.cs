using UnityEngine;
using UnityEngine.Audio;

public class BallCollisionSound : MonoBehaviour
{
    private AudioSource audioSource;
    public bool audioTrain;         
    public AudioClip TrainMusic;     

    void Start()
    {
       
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on this GameObject.");
            return;
        }

    
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

    void OnCollisionEnter(Collision collision)//built in unity function
    {

        if (audioSource != null)
        {
            audioSource.Play();
            Debug.Log("Collision sound played.");
        }

       
        NotifyAgentsAboutSound();
    }

    private void NotifyAgentsAboutSound()
    {
        GameObject[] blueAgents = GameObject.FindGameObjectsWithTag("blueAgent"); //check this
        GameObject[] purpleAgents = GameObject.FindGameObjectsWithTag("purpleAgent");

        GameObject[] allAgents = new GameObject[blueAgents.Length + purpleAgents.Length];
        blueAgents.CopyTo(allAgents, 0);
        purpleAgents.CopyTo(allAgents, blueAgents.Length);

        foreach (GameObject agent in allAgents)
        {
            SoundSensor sensor = agent.GetComponentInChildren<SoundSensor>();
            if (sensor == null)
            {
                Debug.LogWarning($"No SoundSensor component found on agent: {agent.name}");
                continue;
            }

            if (sensor.CanHearSound(transform.position))
            {
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
