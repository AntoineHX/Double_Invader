using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represent main entities of the game
[RequireComponent(typeof(AudioSource))]
public abstract class EntityBase : MonoBehaviour, IHitable
{
    [SerializeField]
    protected float mvt_speed = 5.0f; //Movement speed
    [SerializeField]
    protected Projectile projectile;
    [SerializeField]
    protected float projectile_speed=5.0f;
    
    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip projectile_sound, hit_sound;
    protected float shoot_cd;

    // Start is called before the first frame update
    public virtual void Start()
    {
        shoot_cd=0.0f;

        audioSource = GetComponent<AudioSource>();

        //Check components
        if(projectile is null)
            Debug.LogWarning(gameObject.name+" doesn't have a projectile set");
        if(projectile_sound is null)
            Debug.LogWarning(gameObject.name+" doesn't have a projectile_sound set");
        if(hit_sound is null)
            Debug.LogWarning(gameObject.name+" doesn't have a hit_sound set");
    }

    [ContextMenu("Hit")]
    public virtual bool Hit()
    {
        if(hit_sound != null)
        {
            audioSource.PlayOneShot(hit_sound); //TODO: Play by higher instance, since the gameObjects are destroyed
        }
        Debug.Log(gameObject.name+": Destroyed !");
        Destroy(gameObject);
        return true; //Consume/Destroy damaging object
    }
    [ContextMenu("Shoot")]
    protected virtual void Shoot()
    {
        if(projectile is null)
        {
            Debug.LogWarning(gameObject.name+" cannot shoot as it doesn't have a projectile set");
            return;
        }
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
        if(new_projectile.speed<0) //Reoriente projectile if necessary
        {
            new_projectile.transform.rotation*=Quaternion.AngleAxis(180.0f,Vector3.forward);
        }
        new_projectile.tag = gameObject.tag; //Owner of the 
        
        //Play projectile sound
        if(projectile_sound != null)
        {
            audioSource.PlayOneShot(projectile_sound);
        }
    }
}