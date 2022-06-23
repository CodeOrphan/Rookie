using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;


public enum XPlayerState
{
    Idle,
    Walk,
    WalkRight,
    WalkRightTop,
    WalkRightBottom,
    WalkTop,
    WalkLeft,
    WalkLeftTop,
    WalkLeftBottom,
    WalkBottom,
}

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private BoxCollider2D _boxCollider2D;
    

    public Vector2 Movement;

    
    public const string AxisHorizontal = "Horizontal";
    public const string AxisVertical = "Vertical";

    public const float InputThreshold = 0.1f;

    public const float SmallValue = 0.0001f;
    
    [XRename("是否开启移动速度衰减")]
    public bool SmoothMovement = true;
    
    [XRename("移动速度")]
    public float MovementSpeed= 5f;
    
    [XRename("加速度")]
    public float MovementFactor = 15;
    
    public MMStateMachine<XPlayerState> MoveState;

    
    public HashSet<int> _animatorParameters { get; set; }


    private int _idleParameter;
    private int _walkParameter;
    public void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        
        MoveState = new MMStateMachine<XPlayerState>(gameObject, true);
        _animatorParameters = new HashSet<int>();

        RegisterAnimatorParameter("Idle", AnimatorControllerParameterType.Bool, out _idleParameter);
        RegisterAnimatorParameter("Walk", AnimatorControllerParameterType.Bool, out _walkParameter);

    }

    public virtual void RegisterAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType, out int parameter)
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
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _idleParameter, MoveState.CurrentState == XPlayerState.Idle,_animatorParameters, true);
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _walkParameter, MoveState.CurrentState == XPlayerState.Walk,_animatorParameters, true);
        UpdateMovement();
        UpdateBounds();
    }

    private Vector2 _normalizedHorizontalSpeed;
    
    public void UpdateMovement()
    {
        if (string.IsNullOrEmpty(AxisHorizontal) || string.IsNullOrEmpty(AxisVertical))
        {
            return;
        }

        if (SmoothMovement)
        {
            Movement.x = Input.GetAxis(AxisHorizontal);
            Movement.y = Input.GetAxis(AxisVertical);
        }
        else
        {
            Movement.x = Input.GetAxisRaw(AxisHorizontal);
            Movement.y = Input.GetAxisRaw(AxisVertical);
        }

        _normalizedHorizontalSpeed = Movement;
        if (Movement.x > InputThreshold )
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale =
                    Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            }
        }
        else if (Movement.x < -InputThreshold)
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale =
                    Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            }
        }
        
        Vector2 realHorizontalForce;
        if (SmoothMovement)
        {
            realHorizontalForce = Vector2.Lerp(_rigidbody2D.velocity, _normalizedHorizontalSpeed * MovementSpeed,
                Time.deltaTime * MovementFactor);
        }
        else
        {
            realHorizontalForce = _normalizedHorizontalSpeed * MovementSpeed;
        }

        if (MoveState.CurrentState == XPlayerState.Idle && Mathf.Abs(_normalizedHorizontalSpeed.x) > SmallValue ||
            Mathf.Abs(_normalizedHorizontalSpeed.y) > SmallValue)
        {
            MoveState.ChangeState(XPlayerState.Walk);
        }
        
        if (Mathf.Abs(_normalizedHorizontalSpeed.x) < SmallValue &&
            Mathf.Abs(_normalizedHorizontalSpeed.y) < SmallValue)
        {
            MoveState.ChangeState(XPlayerState.Idle);
        }
        
        _rigidbody2D.velocity = realHorizontalForce;
    }

    //左上角
    protected Vector2 _boundsTopLeftCorner;

    //左下角
    protected Vector2 _boundsBottomLeftCorner;

    //右上角
    protected Vector2 _boundsTopRightCorner;

    //右下角
    protected Vector2 _boundsBottomRightCorner;

    //宽度
    protected float _boundsWidth;

    //高度
    protected float _boundsHeight;

    //中心
    protected Vector2 _boundsCenter;
    public void UpdateBounds()
    {
        if (_boxCollider2D == null)
        {
            return;
        }
        var offset = _boxCollider2D.offset;
        var size = _boxCollider2D.size;

        float top = offset.y + (size.y / 2f);
        float bottom = offset.y - (size.y / 2f);
        float left = offset.x - (size.x / 2f);
        float right = offset.x + (size.x / 2f);

        _boundsTopLeftCorner.x = left;
        _boundsTopLeftCorner.y = top;

        _boundsTopRightCorner.x = right;
        _boundsTopRightCorner.y = top;

        _boundsBottomLeftCorner.x = left;
        _boundsBottomLeftCorner.y = bottom;

        _boundsBottomRightCorner.x = right;
        _boundsBottomRightCorner.y = bottom;

        //转成对应点
        _boundsTopLeftCorner = transform.TransformPoint(_boundsTopLeftCorner);
        _boundsTopRightCorner = transform.TransformPoint(_boundsTopRightCorner);
        _boundsBottomLeftCorner = transform.TransformPoint(_boundsBottomLeftCorner);
        _boundsBottomRightCorner = transform.TransformPoint(_boundsBottomRightCorner);
        _boundsCenter = _boxCollider2D.bounds.center;

        _boundsWidth = Vector2.Distance(_boundsBottomLeftCorner, _boundsBottomRightCorner);
        _boundsHeight = Vector2.Distance(_boundsBottomLeftCorner, _boundsTopLeftCorner);
    }
}