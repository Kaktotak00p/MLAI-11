using UnityEngine;
using Unity.MLAgents.Sensors;

public class SoundSensor : SensorComponent
{
    public float hearingRange = 10f; 
    public LayerMask soundLayer;   

    private Vector3 ballDirection = Vector3.zero;


    public bool CanHearSound(Vector3 soundPosition)
    {
        float distance = Vector3.Distance(transform.position, soundPosition);
        return distance <= hearingRange;
    }

    // Do we need this??
    public bool CanHearSoundWithObstacleCheck(Vector3 soundPosition)
    {
        if (!CanHearSound(soundPosition)) return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, soundPosition - transform.position, out hit, hearingRange))
        {
            return hit.collider.gameObject.layer == soundLayer.value; 
        }
        return false;
    }

    // !!!
    public void ReceiveBallDirection(Vector3 direction)
    {
        ballDirection = direction;
        Debug.Log($"Direction to ball: {ballDirection}");
    }

  
    public override ISensor[] CreateSensors()
    {
        return new ISensor[] { new SoundObservationSensor("SoundSensor", this) };
    }

    
    public Vector3 GetBallDirection()
    {
        return ballDirection;
    }
}


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
        return ObservationSpec.Vector(3); 
    }

    public int Write(ObservationWriter writer)
    {
       
       Vector3 ballDirection;
if (soundSensor != null)
{
    ballDirection = soundSensor.GetBallDirection();
}
else
{
    ballDirection = Vector3.zero;
}

        //  add to observer
        writer.Add(ballDirection); 

        //Debug.Log($"Writing observation: {ballDirection}");

        return 1; 
    }

    public byte[] GetCompressedObservation()
    {
        return null; 
    }

    public SensorCompressionType GetCompressionType()
    {
        return SensorCompressionType.None;
    }

    public CompressionSpec GetCompressionSpec()
    {
        return CompressionSpec.Default(); 
    }

    public void Reset()
    {
        
    }

    public string GetName()
    {
        return sensorName;
    }

    public void Update() { }
}
