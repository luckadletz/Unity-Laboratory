using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhysicsIntegratorFromDropdown : MonoBehaviour {

	// Use this for initialization
	public void OnNewSelection()
    {
        PhysicsSolver solver = FindObjectOfType<PhysicsSolver>();

        int selection = GetComponent<Dropdown>().value;
        PhysicsIntegrators.Integrator i;

        switch(selection)
        {
            default:
            case 0:
                i = PhysicsIntegrators.Integrator.TaylorSeries;
            break;
            case 1:
                i = PhysicsIntegrators.Integrator.EulerExplicit;
                break;
            case 2:
                i = PhysicsIntegrators.Integrator.EulerCromer;
                break;
            case 3:
                i = PhysicsIntegrators.Integrator.EulerCromerMidpoint;
                break;
        }

        solver.defaultIntegrator = i;

    }
}
