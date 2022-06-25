using UnityEngine;
using UnityEngine.UI;//注意添加RawImage命名空间

public class FadeInOut : MonoBehaviour
{
    [HideInInspector]
    public bool isBlack = false;//不透明状态
    
    public float fadeSpeed;//透明度变化速率
    public Image Image;
    public RectTransform rectTransform;

    private static FadeInOut _fadeInOut;

    public static FadeInOut FadeInOutInstance
    {
        get
        {
            if (_fadeInOut == null)
            {
                _fadeInOut = Camera.main.GetComponent<FadeInOut>();
            }

            return _fadeInOut;
        }
    }
    
    void Start()
    {
        //rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);//使背景满屏
    }

    private const float _smallValue = 0.01f;
    private bool _open = false;
    void Update()
    {
        if (!_open)
        {
            return;
        }
        if (isBlack == false)
        {
            Image.color = Color.Lerp(Image.color, Color.clear, Time.deltaTime * fadeSpeed * 0.5f);//渐亮
            if (Image.color.a < _smallValue)
            {
                Image.color = Color.clear;
                _open = false;
            }
        }
        else if (isBlack)
        {
            Image.color = Color.Lerp(Image.color, Color.black, Time.deltaTime * fadeSpeed);//渐暗
            if (Image.color.a > 1-_smallValue)
            {
                Image.color = Color.black;
                _open = false;
            }
        }
    }

    //切换状态
    public void BackGroundControl(bool b)
    {
        isBlack = b;

        if (!b)
        {
            Image.color = Color.black;
        }
        else
        {
            Image.color = Color.clear;
        }

        _open = true;
    }
}