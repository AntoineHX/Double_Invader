using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represent main entities of the game
public abstract class EntityBase : MonoBehaviour, IHitable
{
    [SerializeField]
    protected float mvt_speed = 5.0f; //Movement speed
    [SerializeField]
    protected Projectile projectile;
    [SerializeField]
    protected float projectile_speed=5.0f;
    protected float shoot_cd;

    protected EntityBase() {}
    [ContextMenu("Hit")]
    public virtual bool Hit()
    {
        Debug.Log(gameObject.name+": Destroyed !");
        Destroy(gameObject);
        return true; //Consume/Destroy damaging object
    }
    [ContextMenu("Shoot")]
    protected virtual void Shoot()
    {
        // Debug.Log(gameObject.name+": Fire !");
        //TODO : Cleaner instantiate position to prevent collision
        // Debug.Log((collider2d.bounds.size.y/2)+(projectile.GetComponent<Collider2D>().bounds.size.y/2));
        // float y_offset=collider2d.bounds.size.y/2+projectile.GetComponent<Collider2D>().bounds.size.y/2+0.1f;
        // float y_offset = 0; //collider2d.bounds.size.y*Mathf.Sign(projectile_speed);
        // Projectile new_projectile = Instantiate<Projectile>(projectile, 
        //     new Vector3(transform.position.x, transform.position.y+y_offset, transform.position.z), 
        //     transform.rotation
        // ); 
        //Spawn projectile at current position
        Projectile new_projectile = Instantiate<Projectile>(projectile, transform.position, transform.rotation); 
        new_projectile.speed = projectile_speed; //Set projectile speed & direction
        new_projectile.tag = gameObject.tag; //Owner of the projectile
    }
}