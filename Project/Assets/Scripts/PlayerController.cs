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

public enum XPlayerPhysicsState
{
    ColliderTop,
    ColliderLeft,
    ColliderRight,
    ColliderBottom,
}

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private BoxCollider2D _boxCollider2D;
    public XPawnState PawnState;


    public Vector2 Movement;


    public const string AxisHorizontal = "Horizontal";
    public const string AxisVertical = "Vertical";

    public const float InputThreshold = 0.1f;

    public const float SmallValue = 0.0001f;

    [XRename("是否开启移动速度衰减")] public bool SmoothMovement = true;

    [XRename("移动速度")] public float MovementSpeed = 5f;

    [XRename("加速度")] public float MovementFactor = 15;

    [XRename("不可穿透图层")] public LayerMask PlatformMask;

    public MMStateMachine<XPlayerState> MoveState;


    public HashSet<int> _animatorParameters { get; set; }


    private int _idleParameter;
    private int _walkParameter;

    public Vector2 Speed => _rigidbody2D.velocity;

    private Vector3 _lastPosition;
    private Vector3 _positionTolerance;

    public void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _boxCollider2D = GetComponent<BoxCollider2D>();

        MoveState = new MMStateMachine<XPlayerState>(gameObject, true);
        _animatorParameters = new HashSet<int>();
        PawnState = new XPawnState(x => (int) x);

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

    public void Update()
    {
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _idleParameter, MoveState.CurrentState == XPlayerState.Idle,
            _animatorParameters, true);
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _walkParameter, MoveState.CurrentState == XPlayerState.Walk,
            _animatorParameters, true);

        _positionTolerance = transform.position - _lastPosition;

        UpdateRayTopBottom(1);
        UpdateRayTopBottom(-1);
        UpdateRaycastSide(1);
        UpdateRaycastSide(-1);
        UpdateMovement();
        UpdateBounds();
    }

    public void LateUpdate()
    {
        _lastPosition = transform.position;
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
        if (Movement.x > InputThreshold)
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

        if (Mathf.Abs(_normalizedHorizontalSpeed.x) > SmallValue ||
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

    //横轴偏移
    public float RayOffsetHorizontal = 0.01f;

    //纵坐标偏移
    public float RayOffsetVertical = 0.05f;

    public void UpdateRayTopBottom(float dirction)
    {
        float rayLength = _boundsHeight / 2 + Mathf.Abs(Speed.y * Time.deltaTime) + 0.02f;

        Vector2 verticalRayCastFromLeft = (_boundsBottomLeftCorner + _boundsTopLeftCorner) / 2;
        verticalRayCastFromLeft += (Vector2) transform.right * _positionTolerance.x;

        Vector2 verticalRayCastToRight = (_boundsBottomRightCorner + _boundsTopRightCorner) / 2;
        verticalRayCastFromLeft += (Vector2) transform.right * _positionTolerance.x;

        int verticalRays = 4;
        RaycastHit2D[] hitsStorage = new RaycastHit2D[verticalRays];
        for (int i = 0; i < hitsStorage.Length; i++)
        {
            Vector2 rayOriginPoint = Vector2.Lerp(verticalRayCastFromLeft, verticalRayCastToRight,
                (float) i / (float) (verticalRays - 1));

            hitsStorage[i] = RayCast(rayOriginPoint, dirction * (transform.up), rayLength,
                PlatformMask, Color.blue,
                true);


            if (hitsStorage[i].distance > 0)
            {
                if (dirction == 1)
                {
                    PawnState.SetTrue(XPawnStateDefine.IsCollidingAbove);
                }
                else
                {
                    PawnState.SetTrue(XPawnStateDefine.IsCollidingBelow);
                }

                return;
            }
        }

        if (dirction == 1)
        {
            PawnState.SetFalse(XPawnStateDefine.IsCollidingAbove);
        }
        else
        {
            PawnState.SetFalse(XPawnStateDefine.IsCollidingBelow);
        }
    }

    public void UpdateRaycastSide(float dirction)
    {
        float horizontalRayLength =
            Mathf.Abs(Speed.x * Time.deltaTime) + _boundsWidth / 2 + RayOffsetHorizontal * 2;

        //最下面的射线位置
        Vector2 horizontalRayCastFromBottom = (_boundsBottomLeftCorner + _boundsBottomRightCorner) / 2;
        horizontalRayCastFromBottom += (Vector2) transform.up * 0.05f;

        //最上面的射线位置
        Vector2 horizontalRayCastToTop = (_boundsTopLeftCorner + _boundsTopRightCorner) / 2;
        horizontalRayCastToTop -= (Vector2) transform.up * 0.05f;

        int raysCount = 4;
        RaycastHit2D[] hitsStorage = new RaycastHit2D[raysCount];
        for (int i = 0; i < hitsStorage.Length; i++)
        {
            Vector2 rayOriginPoint = Vector2.Lerp(horizontalRayCastFromBottom, horizontalRayCastToTop,
                (float) i / (float) (raysCount - 1));

            hitsStorage[i] = RayCast(rayOriginPoint, dirction * (Vector2.right),
                horizontalRayLength, PlatformMask, Color.red, true);

            if (hitsStorage[i].distance > 0)
            {
                if (dirction == 1)
                {
                    PawnState.SetTrue(XPawnStateDefine.IsCollidingRight);
                }
                else
                {
                    PawnState.SetTrue(XPawnStateDefine.IsCollidingLeft);
                }

                return;
            }
        }

        if (dirction == 1)
        {
            PawnState.SetFalse(XPawnStateDefine.IsCollidingRight);
        }
        else
        {
            PawnState.SetFalse(XPawnStateDefine.IsCollidingLeft);
        }
    }

    public static RaycastHit2D RayCast(Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance, LayerMask mask,
        Color color, bool drawGizmo = false)
    {
        if (drawGizmo)
        {
            Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        }

        return Physics2D.Raycast(rayOriginPoint, rayDirection, rayDistance, mask);
    }
}