using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represent main entities of the game
public abstract class EntityBase : MonoBehaviour, IHitable
{
    [SerializeField]
    protected uint max_hp = 1; //Max health points
    [SerializeField]
    protected int hp = 1; //Current health points
    [SerializeField]
    protected float mvt_speed = 5.0f; //Movement speed
    [SerializeField]
    protected Projectile projectile;
    [SerializeField]
    protected float shot_spd_multiplier=1.0f;
    
    protected float shoot_cd;

    [SerializeField]
    protected GameObject destroy_prefab; //Prefab to instantiate on destruction

    // Start is called before the first frame update
    public virtual void Start()
    {
        shoot_cd=0.0f;

        //Check components
        if(projectile is null)
            Debug.LogWarning(gameObject.name+" doesn't have a projectile set");
    }

    [ContextMenu("Hit")]
    public virtual int Hit(int dmg=1)
    {
        int projectile_dmg = hp;
        hp-=dmg; //take dammage
        if(hp<=0) //Destroy when out of hp
            DestroyEntity();

        Debug.Log(gameObject.name+": Take "+dmg+" dmg! "+hp+"/"+max_hp+" HP left");
        return projectile_dmg; //Consume damaging object
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

    [ContextMenu("DestroyEntity")]
    protected virtual void DestroyEntity(bool delete=true)
    {
        Debug.Log(gameObject.name+": "+(delete?"Destroyed":"Disabled")+" !");
        if(destroy_prefab)
            Instantiate(destroy_prefab, transform.position, transform.rotation, transform.parent); //Instantiate destruction effect
        
        if(delete)
            Destroy(gameObject);
        else
            gameObject.SetActive(false); //Disabled
    }
}