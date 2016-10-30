using UnityEngine;
using System.Collections;

public class PhysicsSolver : MonoBehaviour {

    public PhysicsIntegrators.Integrator defaultIntegrator
    {
        set
        {
            defaultIntegratorInt = (int)value;
        }
    }
    public int defaultIntegratorInt;

    public float gravity_constant;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        PhysicsBody[] list = FindObjectsOfType<PhysicsBody>();
        foreach (PhysicsBody b in list)
        {
            if (!b.isActiveAndEnabled) continue;

            // Turn forces into acceleration
            b.acceleration = ComputeGravityForce(list, b);
            // Integrate position and velocity
            PhysicsIntegrators.Integrate(defaultIntegrator, b);
        }
	}

    private Vector3 ComputeGravityForce(PhysicsBody[] list, PhysicsBody body)
    {
        // F_g = G * m_1 * m_2 / r^2
        Vector3 Fsum = Vector3.zero;
        foreach(PhysicsBody b in list)
        {
            if (b == body) continue;
            Vector3 direction = b.position - body.position;
            float sqdist = direction.sqrMagnitude;
            direction.Normalize();
            Fsum += (gravity_constant * b.mass * body.mass / sqdist) * direction;
        }
        return Fsum / body.mass;
    }

        
}

