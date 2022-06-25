using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Rendering;

public enum XPlayerState
{
    Idle,
    WalkTop,
    WalkBottom,
    Walk,
    
}

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private BoxCollider _boxCollider;
    
    public Vector3 Movement;

    public GameObject SpriteModel;
    
    public const string AxisHorizontal = "Horizontal";
    public const string AxisVertical = "Vertical";

    public const float InputThreshold = 0.1f;

    public const float SmallValue = 0.0001f;

    public bool SmoothMovement = true;

    public float MovementSpeed = 5f;

    public float MovementFactor = 15;

    public GameObject StartLevel;
    public GameObject Level1;
    public GameObject Level2;
    public GameObject Level3;
    public GameObject Root;
    public string CurrentSceneName;


    public GameObject StartUi;
    public GameObject Level1Ui;
    public GameObject Level2Ui;
    public GameObject Level3Ui;
    

    public HashSet<int> _animatorParameters { get; set; }
    public MMStateMachine<XPlayerState> MoveState;

    private int _idleParameter;
    private int _walkTopParameter;
    private int _walkBottomParameter;
    private int _walkParameter;

    public Vector3 Speed => _rigidbody.velocity;

    private Vector3 _lastPosition;
    private Vector3 _positionTolerance;
    private Transform _mainCamera;
    public Vector3 OnExitLevelPosition;

    private bool _initFinish;
    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider>();
        
        _animatorParameters = new HashSet<int>();
        MoveState = new MMStateMachine<XPlayerState>(gameObject, true);
        if (Camera.main is not null) _mainCamera = Camera.main.transform;

        if (SpriteModel != null)
        {
            _animator = SpriteModel.gameObject.GetComponent<Animator>();
        }

        Physics2D.autoSyncTransforms = true;
        RegisterAnimatorParameter("Idle", AnimatorControllerParameterType.Bool, out _idleParameter);
        RegisterAnimatorParameter("WalkTop", AnimatorControllerParameterType.Bool, out _walkTopParameter);
        RegisterAnimatorParameter("WalkBottom", AnimatorControllerParameterType.Bool, out _walkBottomParameter);
        RegisterAnimatorParameter("Walk", AnimatorControllerParameterType.Bool, out _walkParameter);


        Root = new GameObject("Scence3D");
        Root.transform.position = Vector3.zero;

        if (_currentSceneId == 0)
        {
            if (LoadSceneEx(1))
            {
                _initFinish = true;
            }
        }
    }
    private bool _stopInput = false;
    public GameObject LoadSceneEx(int id)
    {
        if (id == 1 && _currentSceneId == 0)
        {
            FadeInOut.FadeInOutInstance.Image.color = Color.black;
        }

        var t = Loadscene(id);
        if (t == null)
        {
            return null;
        }

        ActiveUi(id);
        FadeInOut.FadeInOutInstance.BackGroundControl(false);
        return t;
    }

    /// <summary>
    /// start = 1 level1 = 2....
    /// </summary>
    /// <param name="name"></param>
    public GameObject Loadscene(int id)
    {
        foreach (var s in _instScene)
        {
            if (s.Scene.activeSelf)
            {
                s.Scene.SetActive(false);
            }
        }
        
        var scene = _instScene.Find(x => x.Id == id);
        if (scene == null)
        {
            var s = Instantiate(GetPrefab(id), Root.transform, true);
            if (s == null)
            {
                return null;
            }

            scene = new XGameScene()
            {
                Id = id,
                Scene = s
            };
            
            _instScene.Add(scene);
        }
        scene.Scene.transform.position = Vector3.zero;

        if (_mainCamera != null)
        {
            Camera.main.orthographicSize = 20f;
        }

        var start =  scene.Scene.transform.Find("StartGame");
        transform.position = start?.position ?? Vector3.zero;
        
        _currentSceneId = id;
        return scene.Scene;
    }

    private GameObject GetPrefab(int id)
    {
        switch (id)
        {
            case 1: return StartLevel;
            case 2: return Level1;
            case 3: return Level2;
            case 4: return Level3;
        }

        return StartLevel;
    }
    
    private void ActiveUi(int id)
    {
        if (StartUi != null && StartUi.activeSelf)
        {
            StartUi.SetActive(false);
        }

        if (Level1 != null && Level1.activeSelf)
        {
            Level1Ui.SetActive(false);
        }
        if (Level2Ui != null&& Level2Ui.activeSelf)
        {
            Level2Ui.SetActive(false);
        }
        if (Level3Ui != null&& Level3Ui.activeSelf)
        {
            Level3Ui.SetActive(false);
        }

        var t = GetUI(id);
        if (t != null && !t.activeSelf)
        {
            t.SetActive(true);
        }
    }
    
    private GameObject GetUI(int id)
    {
        switch (id)
        {
            case 1: return StartUi;
            case 2: return Level1Ui;
            case 3: return Level2Ui;
            case 4: return Level3Ui;
        }

        return StartLevel;
    }

    private List<XGameScene> _instScene = new List<XGameScene>();
    private int _currentSceneId;

    private class XGameScene
    {
        public int Id;
        public GameObject Scene;
    }
    
    public virtual void RegisterAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType,
        out int parameter)
    {
        parameter = Animator.StringToHash(parameterName);

        if (_animator == null)
        {
            return;
        }

        if (_animator.MMHasParameterOfType(parameterName, parameterType))
        {
            _animatorParameters.Add(parameter);
        }
    }

    public void Update()
    {
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _idleParameter, MoveState.CurrentState == XPlayerState.Idle,
            _animatorParameters, true);
        
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _walkParameter, MoveState.CurrentState == XPlayerState.Walk,
            _animatorParameters, true);
        
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _walkTopParameter, MoveState.CurrentState == XPlayerState.WalkTop,
            _animatorParameters, true);
        
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _walkBottomParameter, MoveState.CurrentState == XPlayerState.WalkBottom,
            _animatorParameters, true);

        _positionTolerance = transform.position - _lastPosition;

        if (!_initFinish)
        {
            return;
        }

        UpdateLoadScene();
        
        if (!_stopInput)
        {
            UpdateMovement();
        }

        
        
        Vector3 dir = _mainCamera.position - transform.position;

        // SpriteModel.transform.LookAt(dir);

        if (Input.GetKey(KeyCode.Alpha1))
        {
            Camera.main.GetComponent<FadeInOut>().BackGroundControl(false);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            Camera.main.GetComponent<FadeInOut>().BackGroundControl(true);
        }
    }

    public void LateUpdate()
    {
        _lastPosition = transform.position;
    }

    private Vector3 _normalizedHorizontalSpeed;

    public void UpdateMovement()
    {
        if (string.IsNullOrEmpty(AxisHorizontal) || string.IsNullOrEmpty(AxisVertical))
        {
            return;
        }

        Movement.x = Input.GetAxis(AxisHorizontal);
        Movement.z = Input.GetAxis(AxisVertical);

        _normalizedHorizontalSpeed = Movement;
        if (Movement.x > InputThreshold)
        {
            if (SpriteModel != null)
            {
                if (SpriteModel.transform.localScale.x < 0)
                {
                    SpriteModel.transform.localScale =
                        Vector3.Scale(SpriteModel.transform.localScale, new Vector3(-1, 1, 1));
                }
            }
            else if (transform.localScale.x < 0)
            {
                
                transform.localScale =
                    Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            }
        }
        else if (Movement.x < -InputThreshold)
        {
            if (SpriteModel != null)
            {
                if (SpriteModel.transform.localScale.x > 0)
                {
                    SpriteModel.transform.localScale =
                        Vector3.Scale(SpriteModel.transform.localScale, new Vector3(-1, 1, 1));
                }
            }
            else if (transform.localScale.x > 0)
            {
                transform.localScale =
                    Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            }
        }

        Vector3 realHorizontalForce;
        if (SmoothMovement)
        {
            realHorizontalForce = Vector3.Lerp(_rigidbody.velocity, _normalizedHorizontalSpeed * MovementSpeed,
                Time.deltaTime * MovementFactor);
        }
        else
        {
            realHorizontalForce = _normalizedHorizontalSpeed * MovementSpeed;
        }

        if (Mathf.Abs(_normalizedHorizontalSpeed.x) > SmallValue)
        {
            MoveState.ChangeState(XPlayerState.Walk);
        }
        
        
        if (_normalizedHorizontalSpeed.z > SmallValue)
        {
            MoveState.ChangeState(XPlayerState.WalkTop);
        }
        
        if (_normalizedHorizontalSpeed.z < -SmallValue)
        {
            MoveState.ChangeState(XPlayerState.WalkBottom);
        }
        

        if (Mathf.Abs(_normalizedHorizontalSpeed.x) < SmallValue &&
            Mathf.Abs(_normalizedHorizontalSpeed.z) < SmallValue)
        {
            MoveState.ChangeState(XPlayerState.Idle);
        }
        
        _rigidbody.velocity = realHorizontalForce;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ExitLevel") && other.isTrigger)
        {
            NextScene nextScene = other.gameObject.GetComponent<NextScene>();
            if (nextScene == null)
            {
                return;
            }

            _nextScene = nextScene;
        }
    }

    private NextScene _nextScene;
    private bool _isFate;
    private void UpdateLoadScene()
    {
        if (_nextScene != null)
        {
            _stopInput = true;
            _rigidbody.velocity = Vector3.zero;
            MoveState.ChangeState(XPlayerState.Idle);
            
            if (!_isFate)
            {
                FadeInOut.FadeInOutInstance.BackGroundControl(true);
                _isFate = true;
            }
            
            if (FadeInOut.FadeInOutInstance.Image.color != Color.black)
            {
                return;
            }
            
            var t = LoadSceneEx(_nextScene.NextSceneId);
            if (t == null)
            {
                throw new SystemException("next is null");
            }
            var nextPos =  t.transform.Find(_nextScene.StartPositionName);
            if (nextPos != null)
            {
                transform.position = nextPos.position;
            }

            _nextScene = null;
            _isFate = false;
            _stopInput = false;
        }
    }
    

    private IEnumerator StartLoadScene(NextScene nextScene)
    {
        while (FadeInOut.FadeInOutInstance.Image.color != Color.black)
        {
            
        }
        
        var t = LoadSceneEx(nextScene.NextSceneId);
        if (t == null)
        {
            yield return null;;
        }
        var nextPos =  t.transform.Find(nextScene.StartPositionName);
        if (nextPos != null)
        {
            transform.position = nextPos.position;
        }
        
    }
    
    public void OnTriggerExit(Collider other)
    {
        
    }
}