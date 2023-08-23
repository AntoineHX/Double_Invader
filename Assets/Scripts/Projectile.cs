using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Projectile : MonoBehaviour
{
    public int power = 1; // Dammage / Penetration potential
    public float speed; //Movement 
    private HashSet<string> dammage_tags = new HashSet<string>(new [] {"Invader", "Player", "Player2"});

    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip projectile_sound;

    // Start is called before the first frame update
    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        
        if(projectile_sound is null)
            Debug.LogWarning(gameObject.name+" doesn't have a projectile_sound set");
        else //Play projectile sound
            audioSource.PlayOneShot(projectile_sound);
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
        // Debug.Log(gameObject.name+" collide with "+other.name);
        if (dammage_tags.Contains(other.gameObject.tag) && gameObject.tag != other.gameObject.tag) //other.gameObject.layer == LayerMask.NameToLayer("Projectiles")
        {
            IHitable entity = other.gameObject.GetComponent<IHitable>();
            power -= entity.Hit(); //Dammage entity & Consume power
            Debug.Log(other.name+": Hit by "+ gameObject.name+". Power remaining ["+power+"].");
            if(power<=0) //Consumed by hit
                Destroy(gameObject); 
        }
    }
}
