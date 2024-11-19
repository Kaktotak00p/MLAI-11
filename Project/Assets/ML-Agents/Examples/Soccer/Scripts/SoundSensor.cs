using UnityEngine;

public class SoundSensor : MonoBehaviour
{
    public float hearingRange = 10f; // Maximum range the agent can "hear"
    public LayerMask soundLayer; // Layer for sound-producing objects

    // Method to check if the agent hears the sound
    public bool CanHearSound(Vector3 soundPosition)
    {
        float distance = Vector3.Distance(transform.position, soundPosition);
        return distance <= hearingRange;
    }

    // Optional: Use raycasting to check if the sound is blocked
    public bool CanHearSoundWithObstacleCheck(Vector3 soundPosition)
    {
        if (!CanHearSound(soundPosition)) return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, soundPosition - transform.position, out hit, hearingRange))
        {
            return hit.collider.gameObject.layer == soundLayer.value; // Check if the ray hits the sound source
        }
        return false;
    }
}
