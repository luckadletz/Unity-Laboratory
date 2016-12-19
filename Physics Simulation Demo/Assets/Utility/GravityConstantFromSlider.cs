using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GravityConstantFromSlider : MonoBehaviour {

    public void Update()
    {
        FindObjectOfType<PhysicsSolver>().gravityConstant = GetComponent<Slider>().value;
        GetComponentInChildren<Text>().text = 
            "Gravity Constant: " + FindObjectOfType<PhysicsSolver>().gravityConstant.ToString("N2");

    }
}
