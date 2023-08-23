using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shield : EntityBase
{
    SpriteRenderer spriteRenderer;
    public override void Start() 
    {
        base.Start();//Call EntityBase Start method
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [ContextMenu("ShieldHit")]
    public override int Hit(int dmg=1)
    {
        int projectile_dmg = base.Hit(dmg); //Take damage

        if(hp>0)
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
