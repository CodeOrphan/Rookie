using System;
using System.Runtime;
using Game.Player;
using UnityEngine;

public enum XPawnStateDefine
{
    // 墙面右碰撞 ?
    [ResetOnFrameBegin] IsCollidingRight = 0,

    // 墙面左碰撞 ?
    [ResetOnFrameBegin] IsCollidingLeft,

    // 墙面天花板碰撞 ?
    [ResetOnFrameBegin] IsCollidingAbove,

    // 墙面最下面碰撞 ?
    IsCollidingBelow,

    //最后一帧的时候是否在地面
    WasGroundedLastFrame,

    //最后一帧的时候是否触碰到了天花板
    WasTouchingTheCeilingLastFrame,

    //是否正在下落
    IsFalling,
    
    //是否在上升
    IsAscending,

    //是否在移动平台上移动
    OnAMovingPlatform,
    
    //是否在站台边缘
    OnPlatformEdge,

    //刚刚触碰到地面
    [ResetOnFrameBegin] JustGotGrounded,
    
    //是否在水中
    IsCollidingWater,
    
    WasCollidingWaterLastFrame,
    
    //是否浮出水面
    IsSurface,
}

[Serializable]
public class XPawnState : XStateBase<XPawnStateDefine>
{
    //触碰到斜面时 斜面的坡度
    [ReadOnly]
    [Tooltip("斜面的坡度")]
    public float LateralSlopeAngle;

    //与底面的斜度
    [ReadOnly]
    [Tooltip("与底面的斜度")]
    public float BelowSlopeAngle;
    
    public bool IsGrounded => GetState(XPawnStateDefine.IsCollidingBelow);
    public bool IsAbove => GetState(XPawnStateDefine.IsCollidingAbove);
    public bool WasGroundedLastFrame => GetState(XPawnStateDefine.WasGroundedLastFrame);

    public XPawnState(Func<XPawnStateDefine, int> covert) : base(covert)
    {
        CovertWeight = def => (int) def;
    }

    
    public override void SetState(XPawnStateDefine stateDefine, bool state)
    {
        bool needCall = GetState(stateDefine) == state;
        base.SetState(stateDefine, state);
        if (!needCall)
        {
        }
    }

    public  void SetTrue(XPawnStateDefine stateDefine)
    {
        SetState(stateDefine, true);
    }
    
    public  void SetFalse(XPawnStateDefine stateDefine)
    {
        SetState(stateDefine, false);
    }
}