using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet_Tracker : MonoBehaviour
{

    [Header("Planet to Track")]
    public GameObject Planet_Obj = null;
    [SerializeField]
    private float mass_of_planet;
    [Header("Start Tracking")]
    public bool start;
    private float timer;
    [Header("Clock Timer (Sec)")]
    public float clock;

    [Header("Stegmetod")]
    [Header("False = Semi-Implicit Euler, True = RK4")]
    public bool calculator_method = false; //false = simple, true = RK4
    public float start_time;
    public float end_time;
    public float absorb_radius = 1f;
    public static Planet_Tracker instance;
    public static Planet_Tracker MyInstance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<Planet_Tracker>();
            }
            return instance;
        }
    }

    [Header("Data Interval")]
    // [Tooltip("Arbitary text message")]

    public List<Vector3> position;
    public List<Vector3> velocity;
    public List<Vector3> acceleration;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        clock = 0f;

        start_time = -1.0f;
        end_time = -1.0f;
    //start = false;
    Attractor planet = Planet_Obj.GetComponent<Attractor>();
        if (Planet_Obj != null)
        {
            mass_of_planet = planet.mass;
        }
    }

    // Update is called once per frame
    void Update()
    {
        clock += Time.deltaTime;
        if (clock >= 65.0f && clock <= 75.5f)
        {
            start = true;
            if (start_time == -1.0)
                start_time = clock;
        }
        else if (clock >= 75.5f)
        {
            start = false;
            if (end_time == -1.0)
                end_time = clock;
        }
        if ((start == true ) && Planet_Obj != null  )
        {
            timer += Time.deltaTime;
            if (timer >= 1f )
            {
                timer = 0f;
                Attractor planet = Planet_Obj.GetComponent<Attractor>();
                position.Add(Planet_Obj.transform.position);
                velocity.Add(planet.velocity);
                acceleration.Add(planet.acceleration);
                mass_of_planet = planet.mass;
            }

        }
 

    }
}
