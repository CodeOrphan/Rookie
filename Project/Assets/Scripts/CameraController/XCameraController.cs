using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class XCameraController : MonoBehaviour
{
    public Transform FollowTarget;

    public Vector2 Speed
    {
#if USE_PAWNCORE
        get
        {
            if (Pawn2D == null)
            {
                return Vector2.zero;
            }

            return Pawn2D.Speed;
        }
    }

    public XPawn2D Pawn2D;

#else
        get
        {
            if (_rigidbody2D == null)
            {
                return Vector2.zero;
            }

            return _rigidbody2D.velocity;
        }
    }

    private Rigidbody2D _rigidbody2D;
#endif


    public BoxCollider2D ViewRangeCollider2D;

    protected Camera _camera;

    private Vector3 _lastTargetPosition;

    //摄像机上移下移深度
    public float ManualUpDownLookDistance = 3;
    public float ManualLeftRightLookDistance = 3;

    public float CameraSpeed = 0.5f;

    //重置摄像机的速度
    public float ResetSpeed = 0.5f;

    /// 最小移动触发距离
    [Tooltip("当左右移动发生的时候,摄像机同步到中心的最小宽度,当距离差大于这个值时后将会直接向外偏移")]
    public float LookAheadTrigger = 0.1f;

    //水平视距范围
    public float HorizontalLookDistance = 3;

    public Vector3 CameraOffset;

    private Vector3 _lookDirectionModifier;
    private Vector3 _currentVelocity;
    private Vector3 _lookAheadPos;

    //震动
    public float ShakeDuration;
    public float ShakeDecay;
    public float ShakeIntensity;
    public float _shakeTimer;

    //z轴偏移
    private float _offsetZ;

    //缩放
    private float _lookZoomModifier;
    private float _currentZoom;

    public static XCameraController CameraController;

    public static XCameraController CameraControllerInstance
    {
        get
        {
            if (Camera.main == null)
            {
                return null;
            }

            if (CameraController == null)
            {
                CameraController = Camera.main.GetComponent<XCameraController>();
            }

            return CameraController;
        }
    }

    public void Start()
    {
        var position = FollowTarget.position;
        _lastTargetPosition = position;
        _offsetZ = (transform.position - position).z;

        _camera = GetComponent<Camera>();
        if (_camera == null)
        {
            throw new SystemException("未设置摄像机");
        }

        if (Camera.main != null && !Camera.main.CompareTag(_camera.tag) && !_camera.CompareTag("MainCamera"))
        {
            throw new SystemException("这个组件必须设置在MainCamera上");
        }

        _currentZoom = _camera.orthographic ? MinimumZoom : MinimumAngleZoom;

        ResetSpeedTarget();
    }

    void ResetSpeedTarget()
    {
#if USE_PAWNCORE
        Pawn2D = FollowTarget.GetComponent<XPawn2D>();
#else
        _rigidbody2D = FollowTarget.GetComponent<Rigidbody2D>();
#endif
    }

    public void SetFollowTarget(Transform transform)
    {
        FollowTarget = transform;
        ResetSpeedTarget();
        UpdateLevelBounds();
    }

    public void SetViewCollider(BoxCollider2D collider2D)
    {
        ViewRangeCollider2D = collider2D;
        UpdateLevelBounds();
    }

    public void ResetColliderRange(BoxCollider2D collider2D)
    {
        if (collider2D == null)
        {
            return;
        }

        ViewRangeCollider2D = collider2D;
    }

    public void Update()
    {
        if (FollowTarget == null)
        {
            return;
        }

        ZoomInOut();
        Zoom();

        float xMoveDelta = (FollowTarget.position - _lastTargetPosition).x;

        bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > LookAheadTrigger;

        //左右移动
        if (updateLookAheadTarget)
        {
            _lookAheadPos = Vector3.right * (HorizontalLookDistance * Mathf.Sign(xMoveDelta));
        }
        else
        {
            _lookAheadPos = Vector3.MoveTowards(_lookAheadPos, Vector3.zero, Time.deltaTime * ResetSpeed);
        }

        Vector3 aheadTargetPos = FollowTarget.position + _lookAheadPos + Vector3.forward * _offsetZ +
                                 _lookDirectionModifier + CameraOffset;

        if (ViewRangeCollider2D != null)
        {
            aheadTargetPos.x = Mathf.Clamp(aheadTargetPos.x, _xMin, _xMax);
            aheadTargetPos.y = Mathf.Clamp(aheadTargetPos.y, _yMin, _yMax);
        }

        Vector3 newCameraPosition =
            Vector3.SmoothDamp(transform.position, aheadTargetPos, ref _currentVelocity, CameraSpeed);

        Vector3 shakeFactorPosition = Vector3.zero;

        if (_shakeTimer > 0)
        {
            shakeFactorPosition = Random.insideUnitSphere * (ShakeIntensity * ShakeDuration);
            _shakeTimer -= ShakeDecay * Time.deltaTime;
        }

        newCameraPosition = newCameraPosition + shakeFactorPosition;
        transform.position = newCameraPosition;


        _lastTargetPosition = FollowTarget.position;
    }

    private void LateUpdate()
    {
        UpdateLevelBounds();
    }

    #region ZoomSetting

    public bool OpenMotionZoom = true;

    //正交模式下最小大小
    [Range(0, 20)] public float MinimumZoom = 5f;

    //正交模式下最大大小
    [Range(0, 20)] public float MaximumZoom = 10f;

    //正透视模式下最小角度
    [Range(0, 179)] public float MinimumAngleZoom = 60f;

    //透视模式下最大角度
    [Range(0, 179)] public float MaximumAngleZoom = 70f;

    //运动模式下缩放速度
    public float MotionZoomSpeed = 0.4f;
    public float ZoomInOutSpeed = 0.1f;

    //深度偏移
    public float ManualZoomInOutLookDistance = 3;

    //角度便宜
    [Range(0, 179)] public float ManualZoomInOutLookAngle = 20f;

    private float _zoomInOutVelocity;
    private bool _displayMotionZoom;
    private float _originalZoomValue;
    private float _delayTime;

    private void Zoom()
    {
        if (!OpenMotionZoom)
            return;

        if (_delayTime > 0)
        {
            _delayTime -= Time.deltaTime;
        }

        //这里需要延迟开放运动缩放否则会被运动缩放快速修正
        else if (_delayTime <= 0 && !_displayMotionZoom)
        {
            float characterSpeed = Mathf.Abs(Speed.x);
            float currentVelocity = 0;

            if (_camera.orthographic)
            {
                _currentZoom = Mathf.SmoothDamp(_currentZoom,
                    (characterSpeed / 10) * (MaximumZoom - MinimumZoom) + MinimumZoom, ref currentVelocity,
                    MotionZoomSpeed);
                _camera.orthographicSize = _currentZoom;
            }
            else
            {
                _currentZoom = Mathf.SmoothDamp(_currentZoom,
                    (characterSpeed / 10) * (MaximumAngleZoom - MinimumAngleZoom) + MinimumAngleZoom,
                    ref currentVelocity, MotionZoomSpeed);
                _camera.fieldOfView = _currentZoom;
            }
        }
    }

    private void ZoomInOut()
    {
        if (_camera.orthographic)
        {
            var target = _currentZoom + _lookZoomModifier;
            var newZoom = Mathf.SmoothDamp(_camera.orthographicSize, target, ref _zoomInOutVelocity, ZoomInOutSpeed);
            _camera.orthographicSize = newZoom;
        }
        else
        {
            var target = _currentZoom + _lookZoomModifier;
            var newZoom = Mathf.SmoothDamp(_camera.fieldOfView, target, ref _zoomInOutVelocity, ZoomInOutSpeed);
            _camera.fieldOfView = newZoom;
        }
    }


    public void ZoomIn()
    {
        _displayMotionZoom = true;
        if (_camera.orthographic)
        {
            _lookZoomModifier = -ManualZoomInOutLookDistance;
        }
        else
        {
            _lookZoomModifier = -ManualZoomInOutLookAngle;
        }
    }

    public void ZoomOut()
    {
        _displayMotionZoom = true;
        if (_camera.orthographic)
        {
            _lookZoomModifier = ManualZoomInOutLookDistance;
        }
        else
        {
            _lookZoomModifier = ManualZoomInOutLookAngle;
        }
    }

    public void ResetZoomInOut()
    {
        _lookZoomModifier = 0;
        _delayTime = ZoomInOutSpeed * 2;
        _displayMotionZoom = false;
    }

    #endregion

    #region 场景可视范围设置 (未完成后续需要用多边形碰撞器做限制范围)

    protected float _xMin;
    protected float _xMax;
    protected float _yMin;
    protected float _yMax;

    protected virtual void UpdateLevelBounds()
    {
        if (ViewRangeCollider2D == null)
        {
            return;
        }


        var levelBounds = ViewRangeCollider2D.bounds;
        float cameraHeight = _camera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * _camera.aspect;

        _xMin = levelBounds.min.x + (cameraWidth / 2);
        _xMax = levelBounds.max.x - (cameraWidth / 2);
        _yMin = levelBounds.min.y + (cameraHeight / 2);
        _yMax = levelBounds.max.y - (cameraHeight / 2);

        if (levelBounds.max.x - levelBounds.min.x <= cameraWidth)
        {
            _xMin = levelBounds.center.x;
            _xMax = levelBounds.center.x;
        }

        if (levelBounds.max.y - levelBounds.min.y <= cameraHeight)
        {
            _yMin = levelBounds.center.y;
            _yMax = levelBounds.center.y;
        }
    }

    // public void InvalidatePathCache()
    // {
    //     m_pathCache = null;
    // }

    // private Vector3 ConfineScreenEdges(CinemachineVirtualCameraBase vcam, ref CameraState state)
    // {
    //     Quaternion rot = Quaternion.Inverse(state.CorrectedOrientation);
    //     float dy = state.Lens.OrthographicSize;
    //     float dx = dy * state.Lens.Aspect;
    //     Vector3 vx = (rot * Vector3.right) * dx;
    //     Vector3 vy = (rot * Vector3.up) * dy;
    //
    //     Vector3 displacement = Vector3.zero;
    //     Vector3 camPos = state.CorrectedPosition;
    //     const int kMaxIter = 12;
    //     for (int i = 0; i < kMaxIter; ++i)
    //     {
    //         Vector3 d = ConfinePoint((camPos - vy) - vx);
    //         if (d.AlmostZero())
    //             d = ConfinePoint((camPos - vy) + vx);
    //         if (d.AlmostZero())
    //             d = ConfinePoint((camPos + vy) - vx);
    //         if (d.AlmostZero())
    //             d = ConfinePoint((camPos + vy) + vx);
    //         if (d.AlmostZero())
    //             break;
    //         displacement += d;
    //         camPos += d;
    //     }
    //
    //     return displacement;
    // }
    //
    // bool ValidatePathCache()
    // {
    //     Type colliderType = m_BoundingShape2D == null ? null : m_BoundingShape2D.GetType();
    //     if (colliderType == typeof(PolygonCollider2D))
    //     {
    //         PolygonCollider2D poly = m_BoundingShape2D as PolygonCollider2D;
    //         if (m_pathCache == null || m_pathCache.Count != poly.pathCount ||
    //             m_pathTotalPointCount != poly.GetTotalPointCount())
    //         {
    //             m_pathCache = new List<List<Vector2>>();
    //             for (int i = 0; i < poly.pathCount; ++i)
    //             {
    //                 Vector2[] path = poly.GetPath(i);
    //                 List<Vector2> dst = new List<Vector2>();
    //                 for (int j = 0; j < path.Length; ++j)
    //                     dst.Add(path[j]);
    //                 m_pathCache.Add(dst);
    //             }
    //
    //             m_pathTotalPointCount = poly.GetTotalPointCount();
    //         }
    //
    //         return true;
    //     }
    //     else if (colliderType == typeof(CompositeCollider2D))
    //     {
    //         CompositeCollider2D poly = m_BoundingShape2D as CompositeCollider2D;
    //         if (m_pathCache == null || m_pathCache.Count != poly.pathCount || m_pathTotalPointCount != poly.pointCount)
    //         {
    //             m_pathCache = new List<List<Vector2>>();
    //             Vector2[] path = new Vector2[poly.pointCount];
    //             for (int i = 0; i < poly.pathCount; ++i)
    //             {
    //                 int numPoints = poly.GetPath(i, path);
    //                 List<Vector2> dst = new List<Vector2>();
    //                 for (int j = 0; j < numPoints; ++j)
    //                     dst.Add(path[j]);
    //                 m_pathCache.Add(dst);
    //             }
    //
    //             m_pathTotalPointCount = poly.pointCount;
    //         }
    //
    //         return true;
    //     }
    //
    //     InvalidatePathCache();
    //     return false;
    // }
    //
    // public Collider2D m_BoundingShape2D;
    // private List<List<Vector2>> m_pathCache;
    // private int m_pathTotalPointCount;
    //
    // private Vector3 ConfinePoint(Vector3 camPos)
    // {
    //     // 2D version
    //     Vector2 p = camPos;
    //     Vector2 closest = p;
    //     if (m_BoundingShape2D.OverlapPoint(camPos))
    //         return Vector3.zero;
    //     // Find the nearest point on the shape's boundary
    //     if (!ValidatePathCache())
    //         return Vector3.zero;
    //
    //     float bestDistance = float.MaxValue;
    //     for (int i = 0; i < m_pathCache.Count; ++i)
    //     {
    //         int numPoints = m_pathCache[i].Count;
    //         if (numPoints > 0)
    //         {
    //             Vector2 v0 = m_BoundingShape2D.transform.TransformPoint(m_pathCache[i][numPoints - 1]);
    //             for (int j = 0; j < numPoints; ++j)
    //             {
    //                 Vector2 v = m_BoundingShape2D.transform.TransformPoint(m_pathCache[i][j]);
    //                 Vector2 c = Vector2.Lerp(v0, v, p.ClosestPointOnSegment(v0, v));
    //                 float d = Vector2.SqrMagnitude(p - c);
    //                 if (d < bestDistance)
    //                 {
    //                     bestDistance = d;
    //                     closest = c;
    //                 }
    //
    //                 v0 = v;
    //             }
    //         }
    //     }
    //
    //     return closest - p;
    // }

    #endregion

    public virtual void Shake(float intensity, float duration, float decay)
    {
        ShakeIntensity = intensity;
        ShakeDuration = duration;
        ShakeDecay = decay;
        _shakeTimer = ShakeDuration;
    }

    public virtual void Shake()
    {
        _shakeTimer = ShakeDuration;
    }

    public virtual void LookUp()
    {
        _lookDirectionModifier = new Vector3(0, ManualUpDownLookDistance, 0);
    }

    public virtual void LookDown()
    {
        _lookDirectionModifier = new Vector3(0, -ManualUpDownLookDistance, 0);
    }

    public virtual void LookLeft()
    {
        _lookDirectionModifier = new Vector3(-ManualLeftRightLookDistance, 0, 0);
    }
    
    public virtual void LookRight()
    {
        _lookDirectionModifier = new Vector3(ManualLeftRightLookDistance, 0, 0);
    }
    
    protected virtual float GetTargetDirection()
    {
        if (FollowTarget.localScale.x > 0f)
        {
            return ManualLeftRightLookDistance;
        }
        else
        {
            return -ManualLeftRightLookDistance;
        }
    }

    public virtual void ResetLookUpDown()
    {
        _lookDirectionModifier = new Vector3(0, 0, 0);
    }
}