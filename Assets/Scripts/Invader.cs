using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(gameObject.name+" collide with "+other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            Destroy(gameObject);
        }
    }
}
