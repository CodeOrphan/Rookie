using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

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
    
    public HashSet<int> _animatorParameters { get; set; }


    private int _idleParameter;
    private int _walkParameter;

    public Vector3 Speed => _rigidbody.velocity;

    private Vector3 _lastPosition;
    private Vector3 _positionTolerance;
    private Transform _mainCamera;

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider>();
        
        _animatorParameters = new HashSet<int>();
        if (Camera.main is not null) _mainCamera = Camera.main.transform;

        Physics2D.autoSyncTransforms = true;
        RegisterAnimatorParameter("Idle", AnimatorControllerParameterType.Bool, out _idleParameter);
        RegisterAnimatorParameter("Walk", AnimatorControllerParameterType.Bool, out _walkParameter);
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

    private bool IsIdle;
    private bool IsWalk;

    public void Update()
    {
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _idleParameter, IsIdle,
            _animatorParameters, true);
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _walkParameter, IsWalk,
            _animatorParameters, true);

        _positionTolerance = transform.position - _lastPosition;
        
        UpdateMovement();
        
        Vector3 dir = _mainCamera.position - transform.position;

        SpriteModel.transform.LookAt(dir);
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

        if (Mathf.Abs(_normalizedHorizontalSpeed.x) > SmallValue ||
            Mathf.Abs(_normalizedHorizontalSpeed.y) > SmallValue)
        {
            IsWalk = true;
            IsIdle = false;
        }

        if (Mathf.Abs(_normalizedHorizontalSpeed.x) < SmallValue &&
            Mathf.Abs(_normalizedHorizontalSpeed.y) < SmallValue)
        {
            IsWalk = false;
            IsIdle = true;
        }
        
        _rigidbody.velocity = realHorizontalForce;
    }

    public void OnTriggerEnter(Collider other)
    {
        
    }

    public void OnTriggerExit(Collider other)
    {
        
    }
}