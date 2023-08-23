using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represent active entities (shooting/moving) of the game
public abstract class ActiveEntity : EntityBase
{
    [SerializeField]
    protected float mvt_speed = 5.0f; //Movement speed
    [SerializeField]
    protected Projectile projectile;
    [SerializeField]
    protected float shot_spd_multiplier=1.0f;
    
    protected float shoot_cd;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start(); //Call EntityBase Start method
        shoot_cd=0.0f;

        //Check components
        if(projectile is null)
            Debug.LogWarning(gameObject.name+" doesn't have a projectile set");
    }

    [ContextMenu("Shoot")]
    protected virtual void Shoot()
    {
        if(projectile is null)
            Debug.LogWarning(gameObject.name+" cannot shoot as it doesn't have a projectile set");
        else
        {
            // Debug.Log(gameObject.name+": Fire !");
            //TODO : Cleaner instantiate position to prevent collision
            // Debug.Log((collider2d.bounds.size.y/2)+(projectile.GetComponent<Collider2D>().bounds.size.y/2));
            // float y_offset=collider2d.bounds.size.y/2+projectile.GetComponent<Collider2D>().bounds.size.y/2+0.1f;
            // float y_offset = 0; //collider2d.bounds.size.y*Mathf.Sign(shot_spd_multiplier);
            // Projectile new_projectile = Instantiate<Projectile>(projectile, 
            //     new Vector3(transform.position.x, transform.position.y+y_offset, transform.position.z), 
            //     transform.rotation
            // ); 
            //Spawn projectile at current position
            Projectile new_projectile = Instantiate<Projectile>(projectile, transform.position, transform.rotation);
            new_projectile.speed *= shot_spd_multiplier; //Set projectile speed & direction
            if(new_projectile.speed<0) //Reoriente projectile if necessary
            {
                new_projectile.transform.rotation*=Quaternion.AngleAxis(180.0f,Vector3.forward);
            }
            new_projectile.tag = gameObject.tag; //Owner of the projectile
        }
    }
}