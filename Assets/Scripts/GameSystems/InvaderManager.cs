using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//Define the system managing the invaders. (Singleton)
//TODO: Switch to a registering approach for invaders
public sealed class InvaderManager : MonoBehaviour
{
    public static string Manager_path="/GameSystems/InvaderManager";
    //Singleton
    private static InvaderManager _instance=null;
    public static InvaderManager Instance { get 
        { 
        if(_instance == null) //Force Awakening if needed
            GameObject.Find(Manager_path).GetComponent<InvaderManager>().Awake();
        return _instance; 
        } 
    }

    [HideInInspector]
    public bool ready = false; //Wether the InvaderManager is initialized

    [SerializeField]
    int nbMaxInvaders= 1; //Maximum active invaders
    public int MaxInvaders{ get{return nbMaxInvaders;} private set{nbMaxInvaders=value;}} //Accessor nbMaxInvaders
    [SerializeField]
    GameObject invaderWaypointsContainer; //Object containing invader waypoints
    [SerializeField]
    GameObject invaderSpawnsContainer; //Object containing invader spawn points
    [SerializeField]
    float invaderSpawnChance = 100.0f; //Chance of new invader every request
    [SerializeField]
    float invaderFrequency = 1.0f; //Time (s) between invaderRequest
    
    [SerializeField]
    string PrefabsRessourceFolder = "invaders"; //Ressource folder containing invaders prefabs
    private Invader[] invadersPrefabs;
    GameObject InvaderManagerContainer;

    private List<Invader> _invaderList = new List<Invader>();
    public List<Invader> invaderList
    {
        get{return _invaderList;}
        private set{_invaderList=value;}
    }

    List<Transform> invaderSpawns;
    List<Transform> invaderWaypoints;

    private List<IEnumerator> coroutines= new List<IEnumerator>(); //List of InvaderManager coroutines

    //Request new invader. Return wether a new invader was created
    public bool invaderRequest(float SpawnChance=100.0f)
    {
        Debug.Log("Invader count: "+invaderList.Count);
        if(Random.Range(0.0f, 99.9f)<SpawnChance && invaderList.Count<nbMaxInvaders)
        {
            
            int prefabChoice = Random.Range(0, invadersPrefabs.Length);
            int spawnChoice = Random.Range(0, invaderSpawns.Count);
            Debug.Log("Invader : "+prefabChoice+"/"+invadersPrefabs.Length+" at spawn : "+spawnChoice+"/"+invaderSpawns.Count);
            Invader newInvader = Instantiate(invadersPrefabs[prefabChoice], invaderSpawns[spawnChoice].position, invaderSpawns[spawnChoice].rotation, InvaderManagerContainer.transform); //Instantiate new invader inside InvaderManager
            // Debug.Log(newInvader.GetInstanceID());
            newInvader.name = newInvader.name.Split('(')[0]+newInvader.GetInstanceID(); //Rename new invader
            newInvader.waypoints = invaderWaypoints.ToArray();

            invaderList.Add(newInvader); //Save invader ref

            // invaderSpawnTimer=Random.Range(1.0f, maxTimenewInvaders); //Need more random ?
            // invaderSpawnReady=false;

            // Debug.Log("Spawning "+invaderPrefab.name+" at "+spawnPosition);
            return true; //New invader instantiated
        }
        return false; //No new invader
    }

    //Destroy an invader
    public void invaderKilled(Invader invader)
    {
        invaderList.Remove(invader);
    }

    //Coroutine to be started in a parallel process. It'll repeatidly request new invader.
    private IEnumerator requestCoroutine() 
    {
        while(InvaderManager.Instance!=null){
            
            InvaderManager.Instance.invaderRequest(invaderSpawnChance);
            yield return new WaitForSeconds(invaderFrequency);
        }
    }

    [ContextMenu("load Prefabs")]
    void loadPrefabs()
    {
        // Find all assets labelled with 'usable' :
        // string[] guids = AssetDatabase.FindAssets("", new string[] {"Assets/Prefabs/Characters/invaders"});

        // foreach (string guid in guids)
        // {
        //     Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
        //     Instantiate(guid, spawnPosition, Quaternion.identity);
        // }

        invadersPrefabs = Resources.LoadAll<Invader>(PrefabsRessourceFolder);
        foreach (var p in invadersPrefabs)
        {
            Debug.Log(gameObject.name+" : "+p.name + " loaded");
        }
    }

    [ContextMenu("load Reference points")]
    void loadRefPoints()
    {
        //TODO : Handle multiple spawn points
        // Load invader spawn points //
        if (invaderSpawnsContainer == null)
            throw new System.Exception("No invader spawns provided");

        invaderSpawns = new List<Transform>();
        Component[] spawns = invaderSpawnsContainer.GetComponentsInChildren<Transform>();
        if(spawns != null)
        {
            foreach(Transform s in spawns)
            {
                if(s.gameObject.name != invaderSpawnsContainer.name)
                {
                    invaderSpawns.Add(s);
                    // Debug.Log("invader spawn : "+ s.gameObject.name + s.position);
                }
            }
        }

        // Load invader targets //
        if (invaderWaypointsContainer == null)
            throw new System.Exception("No invader waypoints provided");

        invaderWaypoints = new List<Transform>();
        Component[] waypoints = invaderWaypointsContainer.GetComponentsInChildren<Transform>();
        if(waypoints != null)
        {
            foreach(Transform waypoint in waypoints)
            {
                if(waypoint.gameObject.name != invaderWaypointsContainer.name)
                {
                    invaderWaypoints.Add(waypoint);
                    // Debug.Log("invader waypoint : "+ waypoint.gameObject.name + waypoint.position);
                }
            }
        }
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
            InvaderManagerContainer = GameObject.Find(Manager_path);
            if (InvaderManagerContainer == null)
                throw new System.Exception("No InvaderManager found under GameSystems");

            // Load invaders prefabs //
            loadPrefabs();
            // Load spawn & target points //
            loadRefPoints();

            ready = true;
        }
    }

    void Start()
    {
        //Start coroutines in parallel
        coroutines.Add(requestCoroutine());
        foreach(IEnumerator c in coroutines)
            StartCoroutine(c);
    }

    // Update is called once per frame
    void Update()
    {
        // if(!invaderSpawnReady)
        // {
        //     invaderSpawnTimer-= Time.deltaTime;
        //     if(invaderSpawnTimer<=0)
        //         invaderSpawnReady=true;
        // }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
