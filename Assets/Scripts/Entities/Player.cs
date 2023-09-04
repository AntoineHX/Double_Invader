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
    float _shoot_charge=0.0f; //Charge of the shoot (Backing field)
    float shoot_charge { get=>_shoot_charge; set => charge_shot(value); } //Charge of the shoot
    [SerializeField]
    protected Projectile super_projectile; //Charged shot

    //Recovery
    bool recovering = false; //TODO : link repair UI to set method
    [SerializeField]
    float recoveryTime = 2.0f; //Time for preparation of product
    float recoveryTimer= 0.0f;
    [SerializeField]
    UITimer recoveryUI; //Script of the UI display

    [SerializeField]
    AudioClip recovery_sound, charge_sound;

    //User input
    [SerializeField]
    bool player1Controller=true;
    float move_input, shoot_input;

    Rigidbody2D rigidbody2d;
    Collider2D collider2d;

    [SerializeField]
    ParticleSystem charge_effect;
    float charge_effect_emission_max;
    float charge_effect_velocity_max;

    [SerializeField]
    ParticleSystem charged_effect;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start(); //Call EntityBase Start method

        rigidbody2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();

        //Check components (Note: Avoid 'is null' with Unity objects as it's not reliable)
        if(recoveryUI == null)
            Debug.LogWarning(gameObject.name+" doesn't have a recoveryUI set");
        else
            recoveryUI.gameObject.SetActive(false);
        if(recovery_sound == null)
            Debug.LogWarning(gameObject.name+" doesn't have a recovery_sound set");
        if(charge_sound == null)
            Debug.LogWarning(gameObject.name+" doesn't have a charge_sound set");
        if(super_projectile == null)
            Debug.LogWarning(gameObject.name+" doesn't have a super_projectile set");
        if(charge_effect == null)
            Debug.LogWarning(gameObject.name+" doesn't have a charge_effect set");
        else
        {
            charge_effect_emission_max = charge_effect.emission.rateOverTime.constant;
            charge_effect_velocity_max = charge_effect.velocityOverLifetime.speedModifier.constant;
        }
        if(charged_effect == null)
            Debug.LogWarning(gameObject.name+" doesn't have a charged_effect set");
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
                shoot_charge = 0.0f; //Reset charge

                charged_effect?.Stop(); //Stop charged effect
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

        shoot_charge = 0.0f; //Reset charge
        
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
            if(super_projectile == null)
                Debug.LogWarning(gameObject.name+" cannot shoot as it doesn't have a super projectile set");
            else
            {
                //Spawn projectile at current position
                Projectile new_projectile = Instantiate<Projectile>(super_projectile, transform.position, transform.rotation);
                new_projectile.speed *= shot_spd_multiplier; //Set projectile speed & direction
                new_projectile.tag = gameObject.tag; //Owner of the projectile
            }
        }
    }

    void charge_shot(float charge)
    {
        _shoot_charge = Mathf.Clamp(charge, 0.0f,charge_time); //Set value

        if(shoot_charge==0.0f)
        {
            if(charge_effect!=null && charge_effect.isPlaying) //Stop charge effect
                charge_effect.Stop();
            if(charged_effect!=null && charged_effect.isPlaying) //Stop charged effect
                charged_effect.Stop();
            if(charge_sound != null) //Stop charge sound
                audioSource.Stop();
        }
        else if(shoot_charge>=charge_time) //Charged shot
        {
            if(charge_effect!=null && charge_effect.isPlaying) //Stop charge effect
                charge_effect.Stop();
            if(charged_effect!=null && charged_effect.isStopped) //Play charged effect
                charged_effect.Play();
        }
        else //Update charge effect
        {
            if(charge_effect!=null)
            {
                if(charge_effect.isStopped) //Start charge effect
                    charge_effect.Play();
                var em = charge_effect.emission;
                em.rateOverTime = charge_effect_emission_max*shoot_charge/charge_time;
                var vel = charge_effect.velocityOverLifetime;
                vel.speedModifier = charge_effect_velocity_max*shoot_charge/charge_time;
            }

            if(charge_sound != null && !audioSource.isPlaying && _shoot_charge>0.2) //Play charge sound after 0.2s of charge (prevent playing for simple shots)
                audioSource.PlayOneShot(charge_sound);
        }
    }
}
