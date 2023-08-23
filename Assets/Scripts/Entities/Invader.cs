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
    public override void Start()
    {
        base.Start(); //Call EntityBase Start method
        shoot_cd=max_shoot_cooldown; //Override shoot cooldown to prevent invaders shooting at spawn time

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
         //TODO : Different movement strategies ? (Multi projectile, Single player focus, etc.)
        // Returns if no points have been set up
        if (waypoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = waypoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % waypoints.Length;
    }

    [ContextMenu("DestroyInvader")]
    protected override void DestroyEntity(bool delete=true)
    {
        if(delete)
            InvaderManager.Instance.invaderKilled(this); //Unregister invader
        base.DestroyEntity(delete); //Destroy/Disable invader
    }
    
    [ContextMenu("InvaderShoot")]
    protected override void Shoot()
    {
        //TODO : Only shoot in alive player direction
        //TODO : Different shoot strategies (Multi projectile, Single player focus, etc.)
        float tmp = shot_spd_multiplier;
        //Random projectile direction
        if(Random.value<0.5)
        {
            shot_spd_multiplier *= -1;
        }
        base.Shoot();
        shot_spd_multiplier = tmp; //Restore original parameter value
    }
}
