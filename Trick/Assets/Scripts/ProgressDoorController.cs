using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressDoorController : MonoBehaviour
{
    public GameObject door;
    void Update()
    {
        if (door)
        {
            Slider slider = GetComponent<Slider>();
            if (slider.value >= slider.maxValue)
            {
                door.SetActive(true);
            }
        }
    }
}
