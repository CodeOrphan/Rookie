using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimitPosition : MonoBehaviour
{
    private float MaxPositionX;
    private float MinPositionX;
    public float attenuation = 10f;

    public Scrollbar Scrollbar;
    private Rigidbody _rigidbody;

    private RectTransform _rect;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rect = Scrollbar.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Scrollbar == null)
        {
            return;
        }

        MinPositionX = _rect.position.x - _rect.rect.width / 2;
        MaxPositionX = _rect.position.x + _rect.rect.width / 2;

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, MinPositionX, MaxPositionX),
            transform.position.y, transform.position.z);

        if (transform.position.x <= MinPositionX || transform.position.x >= MaxPositionX)
        {
            _rigidbody.velocity = Vector3.zero;
        }


        _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, Time.deltaTime * attenuation);


        float progress = (transform.position.x + Mathf.Abs(MinPositionX)) /
                         (Mathf.Abs(MinPositionX) + Mathf.Abs(MaxPositionX));

        Scrollbar.size = progress;
    }
}