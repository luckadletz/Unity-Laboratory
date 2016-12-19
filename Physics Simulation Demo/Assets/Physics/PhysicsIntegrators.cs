using UnityEngine;
using System.Collections;

public static class PhysicsIntegrators
{
    public enum Integrator
    {
        TaylorSeries,
        EulerExplicit,
        EulerCromer,
        EulerCromerMidpoint,
        VelocityVerlet,
        RK2,
        RK4
    }

    public delegate void IntegratorDelegate 
        (PhysicsBody body);

    public static void Integrate(Integrator i, PhysicsBody body)
    {
        GetDelegate(i)(body);
    }

    static IntegratorDelegate GetDelegate(Integrator integrator)
    {
        switch (integrator)
        {
            default:
            case Integrator.EulerExplicit:
                return EulerExplicitIntegrator;
            case Integrator.TaylorSeries:
                return TaylorSeriesIntegrator;
            case Integrator.EulerCromer:
                return EulerCromerIntegrator;
            case Integrator.EulerCromerMidpoint:
                return EulerCromerMidpointIntegrator;
            case Integrator.VelocityVerlet:
                return VelocityVerletIntegrator;
            case Integrator.RK2:
                return RK2Integrator;
            case Integrator.RK4:
                return RK4Integrator;
        }
    }

    public static void EulerExplicitIntegrator
        (PhysicsBody body)
    {
        body.position += body.velocity * Time.deltaTime;
        body.velocity += body.acceleration * Time.deltaTime;
    }


    public static void TaylorSeriesIntegrator
        (PhysicsBody body)
    {
        body.position += Time.deltaTime * body.velocity
            + 0.5f * Time.deltaTime * Time.deltaTime * body.acceleration;
        body.velocity += Time.deltaTime * body.acceleration;
    }

    public static void EulerCromerIntegrator
        (PhysicsBody body)
    {
        body.velocity += body.acceleration * Time.deltaTime;
        body.position += body.velocity * Time.deltaTime;
    }

    public static void EulerCromerMidpointIntegrator
        (PhysicsBody body)
    {
        Vector3 oldVelocity = body.velocity;
        body.velocity += Time.deltaTime * body.acceleration;
        body.position += Time.deltaTime * (oldVelocity + body.velocity) * 0.5f;
    }

    public static void VelocityVerletIntegrator
        (PhysicsBody body)
    {
        Debug.LogError("Integrator not yet implemented!");

    }

    public static void RK2Integrator
        (PhysicsBody body)
    {
        Debug.LogError("Integrator not yet implemented!");

    }

    public static void RK4Integrator
        (PhysicsBody body)
    {
        Debug.LogError("Integrator not yet implemented!");

    }
}