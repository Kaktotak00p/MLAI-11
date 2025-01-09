using System.Collections.Generic;
using System.Data;
using Unity.MLAgents;
using UnityEngine;

public class SoccerBallController : MonoBehaviour
{
    public GameObject area;
    [HideInInspector]
    public SoccerEnvController envController;
    public string purpleGoalTag; //will be used to check if collided with purple goal
    public string blueGoalTag; //will be used to check if collided with blue goal
    public enum RewardRule{GoalTouch,BallTouch}//Will choose the reward rule to be applied
    public RewardRule rewardRule;
    public bool randomizeBall; //if true, ball will be shoot at random direction at start of the game 

    public int kickforce = 10; //force to apply when ball is kicked
    void Start()
    {
        envController = area.GetComponent<SoccerEnvController>();
        if (randomizeBall)
        {
            RandomizeBall();
        }
    }
    void RandomizeBall()
    {
        // Generate a random direction for the ball on the x and z axes
        float x = Random.Range(-1.0f, 1.0f);
        float z = Random.Range(-1.0f, 1.0f);
        
        // Create a direction vector and normalize it to ensure consistent force
        Vector3 dir = new Vector3(x, 0, z).normalized;
        
        // Apply force to the ball's Rigidbody to shoot it in the random direction
        GetComponent<Rigidbody>().AddForce(dir * kickforce, ForceMode.VelocityChange);
    }
    void OnCollisionEnter(Collision col)
    {
        switch (rewardRule)
        {
            case RewardRule.GoalTouch:
                if (col.gameObject.CompareTag(purpleGoalTag))
                {
                    Debug.Log("Purple goal touched!");
                    envController.GoalTouched(Team.Blue);
                }
                if (col.gameObject.CompareTag(blueGoalTag))
                {
                    Debug.Log("Blue goal touched!");
                    envController.GoalTouched(Team.Purple);
                }
                break;

            //case for adding reward upon touching ball removed

            
            // case RewardRule.BallTouch:
            //     var agent = col.gameObject.GetComponent<AgentSoccer>();
            //     if (agent != null)
            //     {
            //         Debug.Log($"{agent.name} touched the ball!");
            //         envController.BallTouchedAgent(agent); 
            //     }
            //     break;
            }
            
        }

            // {
            //     if (col.gameObject.CompareTag("ball")) //ball touched
            //     {
            //         envController.BallTouched();
            //     }
            // }
}