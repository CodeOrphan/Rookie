using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartView : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject StartBottom;
    public GameObject EndingBottom;
    void Start()
    {
        if (GameController.Instance.PassLevelCount <= 0)
        {
            StartBottom.SetActive(true);
            EndingBottom.SetActive(false);
        }
        else
        {
            StartBottom.SetActive(false);
            EndingBottom.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
