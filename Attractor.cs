using System.Collections;
using System.Collections.Generic;
using UnityEngine;
struct Derivative
{
    public Vector3 dpos;//delta_position;
    public Vector3 dv;//delta_velocity;
}
public class Attractor : MonoBehaviour
{
    const float G = 0.6674f; //Gravitational constant 6.674×10^(−11)
    public static List<Attractor> Attractors;
    public float mass;
    public Vector3 velocity;
    public Vector3 acceleration;

    void OnEnable()
    {
        if (Attractors==null)
            Attractors = new List<Attractor>();
        Attractors.Add(this);
    }
    void OnDisable()
    {
        Attractors.Remove(this);
    }
    void FixedUpdate()
    {
        //Attractor[] attractors = FindObjectsOfType<Attractor>();
        foreach (Attractor attractor in Attractors)
        {
            if (attractor != this)
                Attract(attractor);
        }
    }

    void Attract (Attractor objToAttract)
    {
        if (objToAttract != null)
        {
            bool calc_m = Planet_Tracker.MyInstance.calculator_method;
            if(calc_m == false)
                formula( objToAttract);
            else if(calc_m == true)
                integrate(Time.deltaTime, objToAttract); //double as fast?? check this out
        }
    }
    void absorb(Attractor objToAttract)
    {
        objToAttract.mass = objToAttract.mass + this.mass;
        objToAttract.transform.localScale = objToAttract.transform.localScale + this.transform.localScale;

        Destroy(this.gameObject);
        print("Planet Absorbed");
    }
    void checkMasses(Attractor objToAttract)
    {
        if (this.mass < objToAttract.mass)
            absorb(objToAttract);
        else if (this.name.GetHashCode() < objToAttract.name.GetHashCode())
            absorb(objToAttract);
    }
    //float cmp = (this.transform.localScale.magnitude + objToAttract.transform.localScale.magnitude)/8;
    void formula(Attractor objToAttract)
    {
        Vector3 dir = this.transform.position - objToAttract.transform.position;
        float dist = dir.magnitude;
        float absorb = Planet_Tracker.MyInstance.absorb_radius;
        if (dist >= absorb)
        {
            //f=G*(m1*m2)/(r^2)
            float forceMagnitude = G * (this.mass * objToAttract.mass) / (dist * dist);
            Vector3 force = dir.normalized * forceMagnitude;

            //f=ma  ---> a=f/m
            acceleration = force / mass;

            //a= v1-v0/t  --->  a*t= v1-v0  --->  v1=v0 + at ----> v += at
            this.velocity += acceleration * Time.deltaTime;
            this.transform.position -= velocity * Time.deltaTime;
        }
        else
            checkMasses(objToAttract);
    }
    Vector3 accelerationRK(Vector3 dir, float dist, Attractor objToAttract) // gravity here 
    {
        //f=G*(m1*m2)/(r^2)
        float forceMagnitude = G * (this.mass * objToAttract.mass) / (dist * dist);
        Vector3 force = dir.normalized * forceMagnitude;

        //f=ma  ---> a=f/m
        acceleration = force / mass;
        return acceleration;
    }
    Derivative evaluate(Vector3 dir, float dist, float dt, Derivative d, Attractor objToAttract)
    {
        //a= v1-v0/t  --->  a*t= v1-v0  --->  v1=v0 + at ----> v += at
        Vector3 tempVelocity = this.velocity + d.dv * dt;
        Vector3 tempPosition = this.transform.position - d.dpos * dt;

        Derivative output;
        output.dpos = tempVelocity;
        output.dv = accelerationRK(dir, dist, objToAttract);

        return output;
    }
    //float cmp = (this.transform.localScale.magnitude + objToAttract.transform.localScale.magnitude) / 8;
    void integrate(float dt, Attractor objToAttract)
    {
        Derivative a, b, c, d;
        Vector3 dir = this.transform.position - objToAttract.transform.position;
        float dist = dir.magnitude;
        float absorb = Planet_Tracker.MyInstance.absorb_radius;
        if (dist >= absorb)
        {
            a = evaluate(dir, dist, 0.0f, new Derivative(), objToAttract);
            b = evaluate(dir, dist, dt * 0.5f, a, objToAttract);
            c = evaluate(dir, dist, dt * 0.5f, b, objToAttract);
            d = evaluate(dir, dist, dt, c, objToAttract);

            Vector3 dxdt = 1.0f / 6.0f * (a.dpos + 2.0f * (b.dpos + c.dpos) + d.dpos);
            Vector3 dvdt = 1.0f / 6.0f * (a.dv + 2.0f * (b.dv + c.dv) + d.dv);

            //a= v1-v0/t  --->  a*t= v1-v0  --->  v1=v0 + at ----> v += at
            this.velocity += dvdt * dt;
            this.transform.position -= dxdt * dt; //minus here
        }
        else
            checkMasses(objToAttract);
    }
}
