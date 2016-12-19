using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ElectroConstantFromSlider : MonoBehaviour {

    public void Update()
    {
        FindObjectOfType<PhysicsSolver>().electrostaticConstant = GetComponent<Slider>().value;
        GetComponentInChildren<Text>().text = 
            "Electrostatic Constant: " + FindObjectOfType<PhysicsSolver>().electrostaticConstant.ToString("N2");

    }
}
