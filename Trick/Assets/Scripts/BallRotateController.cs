using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BallRotateController : MonoBehaviour
{
    public float radius = 1;
    private float m_origionalX;

    void Start()
    {
        m_origionalX = transform.position.x;
    }

    void Update()
    {
        float x = transform.position.x;
        float distance = x - m_origionalX;
        float s = 2 * math.PI * radius;
        float angle = distance / s * 360;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);
    }
}