using UnityEngine;
using UnityEngine.UI;//注意添加RawImage命名空间

public class FadeInOut : MonoBehaviour
{
    [HideInInspector]
    public bool isBlack = false;//不透明状态
    
    public float fadeSpeed = 0.5f;//透明度变化速率
    public Image Image;
    public RectTransform rectTransform;

    void Start()
    {
        //rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);//使背景满屏
        Image.color = Color.clear;
    }

    void Update()
    {
        if (isBlack == false)
        {
            Image.color = Color.Lerp(Image.color, Color.clear, Time.deltaTime * fadeSpeed * 0.5f);//渐亮
        }
        else if (isBlack)
        {
            Image.color = Color.Lerp(Image.color, Color.black, Time.deltaTime * fadeSpeed);//渐暗
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
    }
}