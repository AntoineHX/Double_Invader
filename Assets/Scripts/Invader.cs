using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Invader : MonoBehaviour
{
    [SerializeField]
    private float shoot_cooldown=1.0f; //s
    private float shoot_cd;
    [SerializeField]
    private Projectile projectile;
    [SerializeField]
    private float projectile_speed=-5.0f;
    HashSet<string> danger_projectile = new HashSet<string>(new [] {"Player"}); //Projectile tags inflicting dammage
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

        //Shoot
        if (shoot_cd > 0)
        {
            shoot_cd-=Time.deltaTime;
        }
        else
        {
            Shoot();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(gameObject.name+" collide with "+other.name);
        //Handle dammage in projectile instead ?
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles") && danger_projectile.Contains(other.gameObject.tag))
        {
            Debug.Log(gameObject.name+": Hit by projectile !");
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

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

    [ContextMenu("Shoot")]
    private void Shoot()
    {
        Debug.Log(gameObject.name+": Fire !");
        //TODO : Cleaner instantiate position to prevent collision
        // Debug.Log((collider2d.bounds.size.y/2)+(projectile.GetComponent<Collider2D>().bounds.size.y/2));
        // float y_offset=collider2d.bounds.size.y/2+projectile.GetComponent<Collider2D>().bounds.size.y/2+0.1f;
        float y_offset = 0; //collider2d.bounds.size.y*Mathf.Sign(projectile_speed);
        //Spawn projectile at above current position
        Projectile new_projectile = Instantiate<Projectile>(projectile, 
            new Vector3(transform.position.x, transform.position.y+y_offset, transform.position.z), 
            transform.rotation
        ); 
        new_projectile.speed = projectile_speed; //Set projectile speed & direction
        new_projectile.tag = "Invader";

        shoot_cd = shoot_cooldown; //Reset cooldown
    }
}
