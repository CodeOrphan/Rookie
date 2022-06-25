using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimitPosition : MonoBehaviour
{
    public float MaxPositionX;
    public float MinPositionX;
    public float attenuation = 10f;
    public bool banSlider = false;

    public Slider Slider;
    private Rigidbody _rigidbody;

    private RectTransform _rect;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rect = Slider.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, MinPositionX, MaxPositionX),
            transform.position.y, transform.position.z);

        if (transform.position.x <= MinPositionX || transform.position.x >= MaxPositionX)
        {
            _rigidbody.velocity = Vector3.zero;
        }


        _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, Time.deltaTime * attenuation);

        if (banSlider)
        {
            return;
        }

        if (Slider == null)
        {
            return;
        }

        float progress = (transform.position.x + Mathf.Abs(MinPositionX)) /
                         (Mathf.Abs(MinPositionX) + Mathf.Abs(MaxPositionX));

        Slider.value = progress;
    }
}