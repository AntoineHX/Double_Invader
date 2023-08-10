using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//Define the system managing the global audio. (Singleton)
[RequireComponent(typeof(AudioSource))]
public sealed class AudioManager : MonoBehaviour
{
    public static string Manager_path="/GameSystems/AudioManager";
    //Singleton
    private static AudioManager _instance=null;
    public static AudioManager Instance { get 
        { 
        if(_instance is null) //Force Awakening if needed
            GameObject.Find(Manager_path).GetComponent<AudioManager>().Awake();
        return _instance; 
        } 
    }

    [HideInInspector]
    public bool ready = false; //Wether the AudioManager is initialized

    AudioSource audioSource;

    public void playSound(AudioClip sound)
    {
        if(sound != null)
            audioSource.PlayOneShot(sound);
    }

    //Awake is called when the script instance is being loaded.
    void Awake()
    {
        //Singleton
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        if(!ready)
        {
            audioSource = GetComponent<AudioSource>();

            ready = true;
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDisable()
    {
    }

    void OnDestroy()
    {
    }
}
