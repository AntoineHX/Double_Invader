using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D),typeof(Rigidbody2D))]
public class Player : EntityBase
{
    [SerializeField]
    float shoot_cooldown=1.0f; //s

    //Recovery
    bool recovering = false; //TODO : link repair UI to set method
    [SerializeField]
    float recoveryTime = 2.0f; //Time for preparation of product
    float recoveryTimer= 0.0f;
    [SerializeField]
    UITimer recoveryUI = null; //Script of the UI display

    //User input
    [SerializeField]
    bool player1Controller=true;
    float move_input, shoot_input;

    Rigidbody2D rigidbody2d;
    Collider2D collider2d;

    // Start is called before the first frame update
    void Start()
    {
         if(recoveryUI is null)
            Debug.LogWarning(gameObject.name+" doesn't have a recoveryUI set");
        else
            recoveryUI.gameObject.SetActive(false);

        rigidbody2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Read inputs (See Edit/Project setting / Input Manager)
        move_input = Input.GetAxis(player1Controller ? "Horizontal1" : "Horizontal2");
        shoot_input = Input.GetAxis(player1Controller ? "Fire1" : "Fire2");

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
        if (shoot_input != 0 && shoot_cd<=0 && !recovering)
        {
            Shoot();
            shoot_cd = shoot_cooldown; //Reset cooldown
        }

        if(recovering && recoveryTimer<recoveryTime) //Update repair UI
        {
            recoveryTimer+=Time.deltaTime;
            if(recoveryUI != null)
                recoveryUI.SetValue(recoveryTimer/recoveryTime);
        }
        else //Finished recovery
        {
            recovering=false;
            recoveryUI.gameObject.SetActive(recovering);
        }
    }

    // Update used by the Physics engine
    void FixedUpdate()
    {
        //Movement of a physic object
        Vector2 position = rigidbody2d.position;
        if(!recovering) //Don't move if recovering
            position.x += move_input * mvt_speed * Time.deltaTime;

        // Clamp the position of the character so they do not go out of bounds
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        position.x = Mathf.Clamp(position.x, leftEdge.x, rightEdge.x);

        rigidbody2d.MovePosition(position); //Movement processed by the phyisc engine for Collision, etc.
    }
    // void OnTriggerEnter2D(Collider2D other) {
    //     Debug.Log(gameObject.name+" collide with "+other.name);
    // }

    [ContextMenu("PlayerHit")]
    public override bool Hit()
    {
        if(recovering) //Hit during repair => Disabled
        {
            Debug.Log(gameObject.name+": Disabled !");
            gameObject.SetActive(false); //Disabled
        }
        else //Hit => start recovery
        {
            Debug.Log(gameObject.name+": Recovering...");
            recovering=true;
            if(recoveryUI != null) //Display repair UI
            {
                recoveryTimer=0.0f;
                recoveryUI.SetValue(recoveryTimer/recoveryTime);
                recoveryUI.gameObject.SetActive(recovering);
            }
        }
        return true; //Consume/Destroy damaging object
    }
    
    // [ContextMenu("Shoot")]
    // protected override void Shoot()
    // {
    //     Debug.Log(gameObject.name+": Fire !");
    // }
}
