using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D),typeof(Rigidbody2D))]
public class Player : EntityBase
{
    [SerializeField]
    float shoot_cooldown=1.0f; //s
    //Last user input
    float move_input, shoot_input;

    Rigidbody2D rigidbody2d;
    Collider2D collider2d;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Read inputs (See Edit/Project setting / Input Manager)
        move_input = Input.GetAxis("Horizontal");
        shoot_input = Input.GetAxis("Fire1");

/*        if(move_input != 0)//Move
        {
            Debug.Log("Move :" + move_input * mvt_speed);
            rigidbody2d.velocity = Vector2.left * move_input * mvt_speed;
        }*/

        //Shoot
        if (shoot_cd > 0)
        {
            shoot_cd-=Time.deltaTime;
        }
        if (shoot_input != 0 && shoot_cd<=0)
        {
            Shoot();
            shoot_cd = shoot_cooldown; //Reset cooldown
        }
    }

    // Update used by the Physics engine
    void FixedUpdate()
    {
        //Movement of a physic object
        Vector2 position = rigidbody2d.position;
        position.x += move_input * mvt_speed * Time.deltaTime;

        // Clamp the position of the character so they do not go out of bounds
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        position.x = Mathf.Clamp(position.x, leftEdge.x, rightEdge.x);

        rigidbody2d.MovePosition(position); //Movement processed by the phyisc engine for Collision, etc.
    }
    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log(gameObject.name+" collide with "+other.name);
    }

    // public override bool Hit()
    // {
    //     //TODO : Reparation/Destruction state
    //     return true; //Consume/Destroy damaging object
    // }
    
    // [ContextMenu("Shoot")]
    // protected override void Shoot()
    // {
    //     Debug.Log(gameObject.name+": Fire !");
    //     //TODO : Cleaner instantiate position to prevent collision
    //     // Debug.Log((collider2d.bounds.size.y/2)+(projectile.GetComponent<Collider2D>().bounds.size.y/2));
    //     // float y_offset=collider2d.bounds.size.y/2+projectile.GetComponent<Collider2D>().bounds.size.y/2+0.1f;
    //     float y_offset = 0; //collider2d.bounds.size.y*Mathf.Sign(projectile_speed);
    //     //Spawn projectile at above current position
    //     Projectile new_projectile = Instantiate<Projectile>(projectile, 
    //         new Vector3(transform.position.x, transform.position.y+y_offset, transform.position.z), 
    //         transform.rotation
    //     ); 
    //     new_projectile.speed = projectile_speed; //Set projectile speed & direction
    //     new_projectile.tag = gameObject.tag; //Owner of the projectile

    //     shoot_cd = shoot_cooldown; //Reset cooldown
    // }
}
