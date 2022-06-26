using UnityEngine;

public class SwitchBgmOnTouch : MonoBehaviour
{
    public GameObject bgm1;
    public GameObject bgm2;

    void OnTriggerEnter()
    {
        bgm1.SetActive(false);
        bgm2.SetActive(true);
        gameObject.SetActive(false);
    }
}
