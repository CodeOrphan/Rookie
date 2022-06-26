using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTimer : MonoBehaviour
{
    private Text txtTimer;
    public float second = 6000;
 
    private void Start()
    {
        txtTimer = this.GetComponent<Text>();
    }
 
    private void Update()
    {
 
        if (second > 0)
        {
            second = second - Time.deltaTime;
            if (second / 60 < 1)
            {
                if (second < 4)
                {
                    txtTimer.color = Color.red;
                }
                txtTimer.text = "Load Time:" + string.Format("{0}", (int)second) + "s";
            }
            else
            {
                txtTimer.text = "Load Time:" + string.Format("{0}", (int)second) + "s";
            }
        }
        else
        {
            txtTimer.text = "0s";
            txtTimer.color = Color.red;
        }
        
    }
}
