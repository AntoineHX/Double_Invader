using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shield : MonoBehaviour, IHitable
{
    [SerializeField]
    uint max_hp = 3; //Max health points
    [SerializeField]
    int hp = 3; //Current health points

    [SerializeField]
    SpriteRenderer spriteRenderer;
    private void Start() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [ContextMenu("Hit")]
    public virtual int Hit(int dmg=1)
    {
        int projectile_dmg = hp;
        hp-=dmg;
        Debug.Log(gameObject.name+" Take "+dmg+" dmg! "+hp+"/"+max_hp+" HP left");
        if(hp<=0) //Destroy when out of hp
        {
            //TODO: Destroy animation
            Debug.Log(gameObject.name+": Disabled !");
            gameObject.SetActive(false); //Disabled
        }
        else
        {
            //Change shield color
            Color new_color = spriteRenderer.color;
            if(hp>2*max_hp/3)
            {
                new_color = Color.cyan;
                new_color.a = spriteRenderer.color.a;
            }
            else if(hp>max_hp/3)
            {
                new_color = Color.magenta;
                new_color.a = spriteRenderer.color.a;
            }
            else if(hp>0)
            {
                new_color = Color.red;
                new_color.a = spriteRenderer.color.a;
            }
            spriteRenderer.color = new_color;
        }
        return projectile_dmg; //Consume damaging object
    }
}
