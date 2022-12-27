using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shield : MonoBehaviour, IHitable
{
    [SerializeField]
    uint Charges = 3;
    uint currentCharge;

    [SerializeField]
    SpriteRenderer spriteRenderer;
    private void Start() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentCharge=Charges;
    }

    [ContextMenu("Hit")]
    public virtual bool Hit()
    {
        currentCharge-=1;
        Debug.Log(gameObject.name+" energy: "+currentCharge+"/"+Charges);
        if(currentCharge<1) //Destroy when out of charges
        {
            Debug.Log(gameObject.name+": Disabled !");
            gameObject.SetActive(false); //Disabled
        }
        //Change shield color
        Color new_color = spriteRenderer.color;
        switch (currentCharge)
        {
            case(2):
                new_color = Color.magenta;
                new_color.a = spriteRenderer.color.a;
                break;
            case(1):
                new_color = Color.red;
                new_color.a = spriteRenderer.color.a;
                break;
            default:
                break;
        }
        spriteRenderer.color = new_color;

        return true; //Consume/Destroy damaging object
    }
}
