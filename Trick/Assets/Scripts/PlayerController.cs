using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
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
    public Rigidbody Rigibody;
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


    

    public HashSet<int> _animatorParameters { get; set; }
    public MMStateMachine<XPlayerState> MoveState;

    private int _idleParameter;
    private int _walkTopParameter;
    private int _walkBottomParameter;
    private int _walkParameter;

    public Vector3 Speed => Rigibody.velocity;

    private Vector3 _lastPosition;
    private Vector3 _positionTolerance;
    private Transform _mainCamera;
    public Vector3 OnExitLevelPosition;

    public Vector3 NewPosition;
    private bool _initFinish;
    public void Start()
    {
        Rigibody = GetComponent<Rigidbody>();
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
        RegisterAnimatorParameter("Walk", AnimatorControllerParameterType.Bool, out _walkParameter);

        FadeInOut.FadeInOutInstance.BackGroundControl(false);
    }
    
    public bool StopInput = false;

    
    
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

        _positionTolerance = transform.position - _lastPosition;
        
        if (!StopInput)
        {
            UpdateMovement();
        }
        

        // SpriteModel.transform.LookAt(dir);

        if (Input.GetKey(KeyCode.Alpha1))
        {
            Camera.main.GetComponent<FadeInOut>().BackGroundControl(false);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            Camera.main.GetComponent<FadeInOut>().BackGroundControl(true);
        }
        
        if (NewPosition != Vector3.zero)
        {
            timer += Time.deltaTime * 0.5f;
            transform.position = Vector3.Lerp(transform.position, NewPosition, Time.deltaTime);
            Debug.Log(timer);
            MoveState.ChangeState(XPlayerState.Walk);
        }

        if (timer >= 1)
        {
            NewPosition = Vector3.zero;
            MoveState.ChangeState(XPlayerState.Idle);
        }
    }

    private float timer;
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
                if (SpriteModel.transform.localScale.x > 0)
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
                if (SpriteModel.transform.localScale.x < 0)
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
            realHorizontalForce = Vector3.Lerp(Rigibody.velocity, _normalizedHorizontalSpeed * MovementSpeed,
                Time.deltaTime * MovementFactor);
        }
        else
        {
            realHorizontalForce = _normalizedHorizontalSpeed * MovementSpeed;
        }

        if (Mathf.Abs(_normalizedHorizontalSpeed.x) > SmallValue || Mathf.Abs(_normalizedHorizontalSpeed.z) > 0)
        {
            MoveState.ChangeState(XPlayerState.Walk);
        }
        

        if (Mathf.Abs(_normalizedHorizontalSpeed.x) < SmallValue &&
            Mathf.Abs(_normalizedHorizontalSpeed.z) < SmallValue)
        {
            MoveState.ChangeState(XPlayerState.Idle);
        }
        
        Rigibody.velocity = realHorizontalForce;
    }

    public void OnTriggerEnter(Collider other)
    {

    }
    
    public void OnTriggerExit(Collider other)
    {
        
    }
}