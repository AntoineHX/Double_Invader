using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Invader : MonoBehaviour
{
    //Navigation
    [SerializeField]
    Transform[] waypoints;
    private int destPoint = 0;
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
        // agent.avoidancePriority=Random.Range(0, 99);

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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(gameObject.name+" collide with "+other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            Destroy(gameObject);
        }
    }

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
}
