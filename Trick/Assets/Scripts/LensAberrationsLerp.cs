using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CinematicEffects;


public class LensAberrationsLerp : MonoBehaviour
{
    private LensAberrations _lensAberrations;

    public float Speed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        _lensAberrations = GetComponent<LensAberrations>();
        if (GameController.Instance.PassLevelCount <= 0)
        {
            _lensAberrations.enabled = false;
        }
        else
        {
            _lensAberrations.enabled = true;
        }
    }

    // Update is called once per frame

    private float timer;
    void Update()
    {
        if (GameController.Instance.PassLevelCount <= 0)
        {
            _lensAberrations.enabled = false;
        }
        else
        {
            _lensAberrations.enabled = true;
        }
        if (_lensAberrations && _lensAberrations.enabled)
        {
            timer += Time.deltaTime * Speed;
            _lensAberrations.chromaticAberration.tangential = Mathf.Sin(timer) * 2;
        }
    }
}
