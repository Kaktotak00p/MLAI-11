using UnityEngine;
using Unity.MLAgents.Sensors;

public class SoundSensor : SensorComponent
{
    public float hearingRange = 10f; // Maximum range the agent can "hear"
    public LayerMask soundLayer;    // Layer for sound-producing objects

    private Vector3 ballDirection = Vector3.zero; // Store the direction to the ball

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

    // Method to receive and store the ball's direction
    public void ReceiveBallDirection(Vector3 direction)
    {
        ballDirection = direction;
        Debug.Log($"Direction to ball: {ballDirection}");
    }

    // Override CreateSensors to integrate with ML-Agents
    public override ISensor[] CreateSensors()
    {
        return new ISensor[] { new SoundObservationSensor("SoundSensor", this) };
    }

    // Expose the ball direction for external access
    public Vector3 GetBallDirection()
    {
        return ballDirection;
    }
}

// Internal class to handle observations for SoundSensor
internal class SoundObservationSensor : ISensor
{
    private readonly string sensorName;
    private readonly SoundSensor soundSensor;

    public SoundObservationSensor(string name, SoundSensor sensor)
    {
        sensorName = name;
        soundSensor = sensor;
    }

    public ObservationSpec GetObservationSpec()
    {
        return ObservationSpec.Vector(3); // Observation is a 3D vector
    }

    public int Write(ObservationWriter writer)
    {
        // Retrieve the ball direction from the SoundSensor
        Vector3 ballDirection = soundSensor != null ? soundSensor.GetBallDirection() : Vector3.zero;

        // Add each component of the Vector3 to the observation writer
        writer.Add(ballDirection); // X component
        // Z component

        Debug.Log($"Writing observation: {ballDirection}");

        return 3; // Number of observations written
    }

    public byte[] GetCompressedObservation()
    {
        return null; // No compression
    }

    public SensorCompressionType GetCompressionType()
    {
        return SensorCompressionType.None;
    }

    public CompressionSpec GetCompressionSpec()
    {
        return CompressionSpec.Default(); // Default compression
    }

    public void Reset()
    {
        // No specific reset logic needed
    }

    public string GetName()
    {
        return sensorName;
    }

    public void Update() { }
}
