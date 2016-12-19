using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsSolver : MonoBehaviour {

    public PhysicsIntegrators.Integrator defaultIntegrator;

    public float gravityConstant;
    public float electrostaticConstant;

    public bool doGravity;
    public bool doElectrostatic;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        PhysicsBody[] list = FindObjectsOfType<PhysicsBody>();
        foreach (PhysicsBody b in list)
        {
            // Skip inactive bodies
            if (!b.isActiveAndEnabled) continue;
            //  Accumulate all forces
            List<Vector3> forces = new List<Vector3>();
            if (doGravity)
                forces.Add(ComputeGravityForce(list, b));
            if(doElectrostatic)
                forces.Add(ComputeElectrostaticForce(list, b));
            // Turn forces into acceleration
            b.acceleration.Set(0, 0, 0);
            foreach(Vector3 f in forces)
                b.acceleration += f;

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
            Fsum += (gravityConstant * b.mass * body.mass / sqdist) * direction;
        }
        return Fsum / body.mass;
    }

    private Vector3 ComputeElectrostaticForce(PhysicsBody[] list, PhysicsBody body)
    {
        // F_g = G * m_1 * m_2 / r^2
        Vector3 Fsum = Vector3.zero;
        foreach (PhysicsBody b in list)
        {
            if (b == body) continue;
            Vector3 direction = b.position - body.position;
            float sqdist = direction.sqrMagnitude;
            direction.Normalize();
            Fsum += (electrostaticConstant * b.charge * body.charge / sqdist) * -direction;
        }
        return Fsum / body.mass;
    }

}

