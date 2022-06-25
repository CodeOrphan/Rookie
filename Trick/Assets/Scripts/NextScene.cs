using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextScene : MonoBehaviour
{
    public int NextSceneId;
    public string StartPositionName;

    public BoxCollider BoxCollider;

    public void Start()
    {
        BoxCollider = GetComponent<BoxCollider>();
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.Instance.SetScene(this);
        }
    }
}
