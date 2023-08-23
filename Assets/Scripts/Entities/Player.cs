using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D),typeof(Rigidbody2D),typeof(AudioSource))]
public class Player : ActiveEntity
{
    [SerializeField]
    float shoot_cooldown=1.0f; //
    [SerializeField]
    float charge_time=1.0f; //Time to charge the shoot
    float shoot_charge=0.0f; //Charge of the shoot
    [SerializeField]
    protected Projectile super_projectile; //Charged shot

    //Recovery
    bool recovering = false; //TODO : link repair UI to set method
    [SerializeField]
    float recoveryTime = 2.0f; //Time for preparation of product
    float recoveryTimer= 0.0f;
    [SerializeField]
    UITimer recoveryUI = null; //Script of the UI display

    [SerializeField]
    AudioClip recovery_sound;

    //User input
    [SerializeField]
    bool player1Controller=true;
    float move_input, shoot_input;

    Rigidbody2D rigidbody2d;
    Collider2D collider2d;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start(); //Call EntityBase Start method

        rigidbody2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();

        //Check components
        if(recoveryUI is null)
            Debug.LogWarning(gameObject.name+" doesn't have a recoveryUI set");
        else
            recoveryUI.gameObject.SetActive(false);
        if(recovery_sound is null)
            Debug.LogWarning(gameObject.name+" doesn't have a recovery_sound set");
        if(super_projectile is null)
            Debug.LogWarning(gameObject.name+" doesn't have a super_projectile set");
    }

    // Update is called once per frame
    void Update()
    {
        //Read inputs (See Edit/Project setting / Input Manager)
        move_input = Input.GetAxis(player1Controller ? "Horizontal1" : "Horizontal2");
        shoot_input = Input.GetAxis(player1Controller ? "Fire1" : "Fire2");

        //Shoot
        if (shoot_cd > 0)
        {
            shoot_cd-=Time.deltaTime;
        }
        if (shoot_cd<=0 && !recovering)
        {
            if(shoot_input == 0 && shoot_charge>0.0f) //Release shot
            {
                Shoot();
                shoot_cd = shoot_cooldown; //Reset 
                shoot_charge = 0.0f;
            }
            else if(shoot_input!=0) //Charge shoot
            {
                shoot_charge+=Time.deltaTime;
            }
        }

        if(recovering)
            if(recoveryTimer<recoveryTime) //Update repair UI
            {
                recoveryTimer+=Time.deltaTime;
                recoveryUI?.SetValue(recoveryTimer/recoveryTime);
            }
            else //Finished recovery
            {
                hp= (int)max_hp; //Reset health

                recovering=false;
                recoveryUI.gameObject.SetActive(recovering);
                //Play recovery sound
                if(recovery_sound != null)
                {
                    audioSource.PlayOneShot(recovery_sound);
                }
            }
    }

    // Update used by the Physics engine
    void FixedUpdate()
    {
        //Movement of a physic object
        Vector2 position = rigidbody2d.position;
        if(!recovering && shoot_charge==0.0f) //Don't move if recovering or charging shot
            position.x += move_input * mvt_speed * Time.deltaTime;
        // if(!recovering)
        // {
        //     Debug.Log("Move :" + move_input * mvt_speed);
        //     // rigidbody2d.velocity = Vector2.left * move_input * mvt_speed*Time.deltaTime; //rigidbody velocity isn't moving object
        //     rigidbody2d.AddForce(Vector2.left * move_input * mvt_speed - rigidbody2d.velocity, ForceMode2D.Force);
        // }

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
    public override int Hit(int dmg=1)
    {
        int projectile_dmg = hp;
        
        if(recovering) //Hit during repair
        {
           DestroyEntity();//Disable
        }
        else //Hit while active
        {
            //Play hit sound
            if(hit_sound != null)
            {
                audioSource.PlayOneShot(hit_sound);
            }

            hp-=dmg; //take dammage
            Debug.Log(gameObject.name+": Take "+dmg+" dmg! "+hp+"/"+max_hp+" HP left");
            
            if(hp<=-max_hp) //Excessive dammage => One-shot
            {
                DestroyEntity();//Disabled
            }
            else if(hp<=0) //Normal dammage => Recovery
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
        }
        return projectile_dmg; //Consume damaging object
    }
    
    [ContextMenu("PlayerShoot")]
    protected override void Shoot()
    {
        if(shoot_charge<charge_time) //Base shot
            base.Shoot();
        else //Charged shot
        {
            if(super_projectile is null)
                Debug.LogWarning(gameObject.name+" cannot shoot as it doesn't have a super projectile set");
            else
            {
                //Spawn projectile at current position
                Projectile new_projectile = Instantiate<Projectile>(super_projectile, transform.position, transform.rotation);
                new_projectile.speed *= shot_spd_multiplier; //Set projectile speed & direction
                if(new_projectile.speed<0) //Reoriente projectile if necessary
                {
                    new_projectile.transform.rotation*=Quaternion.AngleAxis(180.0f,Vector3.forward);
                }
                new_projectile.tag = gameObject.tag; //Owner of the projectile
            }
        }
    }
}
