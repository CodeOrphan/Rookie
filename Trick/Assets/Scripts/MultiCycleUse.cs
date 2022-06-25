using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCycleUse : MonoBehaviour
{
    public int MultiCycleUseNum;
    // Start is called before the first frame update
    void Start()
    {
        if (MultiCycleUseNum == GameController.Instance.PassLevelCount)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
        else
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
