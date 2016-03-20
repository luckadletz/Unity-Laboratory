using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class GUIOver3DPoint : MonoBehaviour
{

    public Transform target;

    [Range(1, 0)]
    public float Smoothing;

    public Vector3 Offset;

    private Camera MainCam;
    private RectTransform Rekt;

    // Use this for initialization
    void Start()
    {
        MainCam = Camera.main;
        Rekt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 nextPos = MainCam.WorldToScreenPoint(target.position + Offset);

        Rekt.anchoredPosition3D = Vector3.Lerp(Rekt.anchoredPosition3D, nextPos, Smoothing);
    }
}
