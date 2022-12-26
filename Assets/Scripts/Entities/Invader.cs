using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Invader : EntityBase
{
    [SerializeField]
    float min_shoot_cooldown=1.0f, max_shoot_cooldown=5.0f; //s
    //Navigation
    [SerializeField]
    bool random_waypoint_order=false;
    public Transform[] waypoints;
    int destPoint = 0;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //Prevent rotation of the ground at movement
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        //Prevent slowdown when approaching destination
        agent.autoBraking = false;

        //Assign Random priority to prevent two agent blocking each other
        agent.avoidancePriority=Random.Range(0, 99);

        //Randomize waypoints order
        if(random_waypoint_order){
            List<Transform> tf_list = new List<Transform>(waypoints);
            Utilities.Shuffle(tf_list);
            waypoints= tf_list.ToArray();
        }
        GotoNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        //Navigation
        // Debug.Log(gameObject.name + " navigation : "+ agent.isStopped + " " + agent.remainingDistance);
        Debug.DrawLine(gameObject.transform.position, agent.destination, Color.blue, 0.0f);
        if(!agent.pathPending && agent.remainingDistance<0.2f) //Close to target, go to next one
        {
            GotoNextPoint();
        }

        //Shoot
        if (shoot_cd > 0)
        {
            shoot_cd-=Time.deltaTime;
        }
        else
        {
            Shoot();
            shoot_cd = Random.Range(min_shoot_cooldown, max_shoot_cooldown); //Reset cooldown
        }
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     Debug.Log(gameObject.name+" collide with "+other.name);
    // }

    [ContextMenu("NextPoint")]
    void GotoNextPoint() 
    {
        // Returns if no points have been set up
        if (waypoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = waypoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % waypoints.Length;
    }

    [ContextMenu("Hit")]
    public override bool Hit()
    {
        bool dead = base.Hit();
        if(dead)
        {
            InvaderManager.Instance.invaderKilled(this);
        }
        return dead; //Consume/Destroy damaging object
    }
}
