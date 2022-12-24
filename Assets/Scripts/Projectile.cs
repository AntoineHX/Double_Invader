using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed; //Movement 
    private HashSet<string> dammage_tags = new HashSet<string>(new [] {"Invader", "Player"});

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        //Move
        Vector2 position = gameObject.transform.position;
        transform.position = position + Vector2.up * speed * Time.deltaTime;

        //Destroy if out of bounds TODO : Cleaner
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        Vector3 downEdge = Camera.main.ViewportToWorldPoint(Vector3.down);
        Vector3 upEdge = Camera.main.ViewportToWorldPoint(Vector3.up);
        if (transform.position.y > upEdge.y || transform.position.y < downEdge.y || transform.position.x < leftEdge.x || transform.position.x > rightEdge.x)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        Debug.Log(gameObject.name+" collide with "+other.name);
        if (dammage_tags.Contains(other.gameObject.tag) && gameObject.tag != other.gameObject.tag) //other.gameObject.layer == LayerMask.NameToLayer("Projectiles")
        {
            Debug.Log(other.gameObject.name+": Hit by projectile !");
            IHitable entity = other.gameObject.GetComponent<IHitable>();
            if(entity.Hit()) //Dammage entity
            {
                Destroy(gameObject); //Consumed by hit
            }
        }
    }
}
