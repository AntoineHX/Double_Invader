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
    protected bool delete_on_destroy = true; //Delete on death
    [SerializeField]
    protected GameObject destroy_prefab; //Prefab to instantiate on destruction

    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip hit_sound;

    // Start is called before the first frame update
    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        //Check components (Note: Avoid 'is null' with Unity objects as it's not reliable)
        if(hit_sound == null)
            Debug.LogWarning(gameObject.name+" doesn't have a hit_sound set");
        if(destroy_prefab == null)
            Debug.LogWarning(gameObject.name+" doesn't have a destroy_prefab set");
    }

    [ContextMenu("Hit")]
    public virtual int Hit(int dmg=1)
    {
        int projectile_dmg = hp;
        hp-=dmg; //take damage
        if(hp<=0) //Destroy when out of hp
            DestroyEntity();

        //Play hit sound
        if(hit_sound != null)
        {
            audioSource.PlayOneShot(hit_sound);
        }

        Debug.Log(gameObject.name+": Take "+dmg+" dmg! "+hp+"/"+max_hp+" HP left");
        return projectile_dmg; //Consume damaging object
    }

    [ContextMenu("DestroyEntity")]
    protected virtual void DestroyEntity()
    {
        Debug.Log(gameObject.name+": "+(delete_on_destroy?"Destroyed":"Disabled")+" !");
        if(destroy_prefab)
            Instantiate(destroy_prefab, transform.position, transform.rotation, transform.parent); //Instantiate destruction effect
        
        if(delete_on_destroy)
            Destroy(gameObject);
        else
            gameObject.SetActive(false); //Disabled
    }
}