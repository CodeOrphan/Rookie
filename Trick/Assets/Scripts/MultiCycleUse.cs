using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCycleUse : MonoBehaviour
{
    public int MultiCycleUseNum;

    public GameObject StartButton;

    public GameObject EndButton;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnEnable()
    {
        if (MultiCycleUseNum == GameController.Instance.PassLevelCount)
        {
            if (!StartButton.activeSelf)
            {
                StartButton.SetActive(true);
            }
            if (EndButton.activeSelf)
            {
                EndButton.SetActive(false);
            }
        }
        else
        {
            if (!EndButton.activeSelf)
            {
                EndButton.SetActive(true);
            }
            if (StartButton.activeSelf)
            {
                StartButton.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
