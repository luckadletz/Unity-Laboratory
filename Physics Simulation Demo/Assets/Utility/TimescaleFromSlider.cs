using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class TimescaleFromSlider : MonoBehaviour
{
    public void Update()
    {
        Time.timeScale = GetComponent<Slider>().value;
        GetComponentInChildren<Text>().text = "Timescale: " + Time.timeScale.ToString("N2");

    }
}
