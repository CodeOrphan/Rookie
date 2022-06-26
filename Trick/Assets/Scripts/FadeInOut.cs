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
    private static Canvas _baseCanvas;
    private static Transform Root;

    public static FadeInOut FadeInOutInstance
    {
        get
        {
            if (_fadeInOut == null)
            {
                _fadeInOut = FindObjectOfType<FadeInOut>();
            }

            if (_fadeInOut == null)
            {
                Root = new GameObject("FadeCanvas").transform;
                var canvas = Root.gameObject.AddComponent<Canvas>();
                canvas.worldCamera = Camera.main;

                var image = new GameObject("FadeImage");
                image.AddComponent<Image>();
                _fadeInOut = image.AddComponent<FadeInOut>();
                image.transform.SetParent(Root);
            }

            return _fadeInOut;
        }
    }
    
    void Start()
    {
        //rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);//使背景满屏
        _fadeInOut = GetComponent<FadeInOut>();
    }

    private void Init()
    {
        if (Image == null)
        {
            Image = GetComponent<Image>();
        }
    }
    

    private const float _smallValue = 0.1f;
    private bool _open = false;
    void Update()
    {
        UpdateSceneFade();
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
    
    bool _isFadeIn = false;
    bool _isFadeout = false;
    bool _inFinish = false;
    private bool _isOpen = false;
    private GameObject _thisScene;
    private GameObject _nextScene;
    private PlayerController _thisScenePlayer;
    private PlayerController _nextScenePlayer;
    public void ReSpawnSceneFade(GameObject fadeIn, GameObject fadeOut)
    {
        _isFadeIn = false;
        _isFadeout = false;
        _inFinish = false;
        _isOpen = true;
        _thisScene = fadeIn;
        _nextScene = fadeOut;
        _thisScenePlayer = _thisScene.transform.Find("Player").GetComponent<PlayerController>();
    }
    
    private void UpdateSceneFade()
    {
        if (!_isOpen)
        {
            return;
        }
        
        if (!_isFadeIn)
        {
            FadeInOut.FadeInOutInstance.BackGroundControl(true);
            _isFadeIn = true;
        }

        _thisScenePlayer.StopInput = true;
        _thisScenePlayer.MoveState.ChangeState(XPlayerState.Idle);
        _thisScenePlayer.Rigibody.velocity = Vector3.zero;
        
        if (FadeInOut.FadeInOutInstance.Image.color == Color.black)
        {
            if (_thisScene)
                _thisScene.SetActive(false);
            _inFinish = true;
        }

        if (_inFinish)
        {
            if (!_isFadeout)
            {
                FadeInOut.FadeInOutInstance.BackGroundControl(false);
                _isFadeout = true;
            }

            if (_nextScene)
                _nextScene.SetActive(true);

            if (FadeInOut.FadeInOutInstance.Image.color == Color.clear)
            {
                var nextPlayer = _nextScene.transform.Find("Player").GetComponent<PlayerController>();
                if (nextPlayer)
                {
                    nextPlayer.StopInput = false;
                }
                _isOpen = false;
            }
        }
    }

    //切换状态
    public void BackGroundControl(bool b)
    {
        if (Image == null)
        {
            return;
        }
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