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
                txtTimer.text = "There is still room for cats to rule the world!!!:" + string.Format("{0:d2}", (int)second % 60);
            }
            else
            {
                txtTimer.text = "There is still room for cats to rule the world!!!:" + string.Format("{0:d2}:{1:d2}", (int)second / 60, (int)second % 60);
            }
        }
        else
        {
            txtTimer.text = "00:00";
            txtTimer.color = Color.red;
        }
        
    }
}
