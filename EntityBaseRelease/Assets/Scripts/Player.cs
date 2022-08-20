using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    #region Player Constants

    // All Available Player States
    private const int stIdleBasic           = 0;
    private const int stIdleLong            = 1;
    private const int stIdleWall            = 2;
    private const int stAir                 = 3;
    private const int stMove                = 4;
    private const int stJump                = 5;
    private const int stJumpAir             = 6;
    private const int stJumpWall            = 7;
    private const int stJumpDown            = 8;
    private const int stWallSliding         = 9;
    private const int stLedgeHold           = 10;
    private const int stLedgeClimb          = 11;
    private const int stSit                 = 12;
    private const int stHeadUp              = 13;
    private const int stRoll                = 14;
    private const int stDash                = 15;
    private const int stTakeDown            = 16;

    [Header("Player Constants")]
    // Idle related options
    public int longIdleFrame = 900;

    // Air related options
    public float maxGlidingSpeed = 4.5f;

    public int freeFallAccelFrame = 39;
    public int glidingAccelFrameX = 26;
    public int glidingAccelFrameY = 39;

    // Move related options
    public float runningWeight = 1.2f;

    public int runningAccelFrame = 13;

    // Jump related options
    public float jumpSpeed = 7.0f;
    public float airJumpSpeed = 7.0f;
    public float downJumpPreSpeed = 1.5f;
    public float wallJumpSpeed = 7.0f;

    public int jumpFrame = 13;
    public int airJumpIdleFrame = 3;
    public int airJumpFrame = 13;
    public int downJumpPreFrame = 30;
    public int wallJumpFrame = 13;
    public int wallJumpForceFrame = 6;

    private DiscreteLinearGraph jumpGraph;

    // WallSliding related options
    public float maxWallSlidingSpeed = 5.0f;

    public int wallSlidingAccelFrame = 13;

    // Ledge related options

    // Sit & HeadUp related options
    public int sitCameraMoveFrame = 120;
    public int headUpCameraMoveFrame = 120;

    // Roll related options
    public float rollSpeed = 8.0f;

    public int rollPreDashFrame = 6;
    public int rollInvincibilityFrame = 18;
    public int rollWakeUpFrame = 6;
    public int rollCoolFrame = 180;

    // Dash related options
    public float dashSpeed = 8.0f;

    public int dashIdleFrame = 6;
    public int dashInvincibilityFrame = 9;

    // TakeDown related options
    public float takeDownSpeed = 12.0f;

    public int takeDownPreIdleFrame = 18;
    public int takeDownPostIdleFrame = 12;

    // State Machine
    private StateMachine m_machine;

    #endregion

    #region Player Variables

    [Header("Input Options")]
    // Input options
    private int xNegative;
    private int xPositive;
    private int xInput;

    private bool jumpDown;
    private bool jumpUp;
    private bool jumpPressing;

    [Header("Player Variables")]
    // Idle related options
    public int currentIdleBasicFrame = 0;

    // Air related options

    // Move related options

    // Jump related options
    public bool isReleasedJumpKey;
    public int leftJumpFrame;

    // WallSliding related options

    // Ledge related options

    // Sit & HeadUp related options

    // Roll related options

    // Dash related options

    // TakeDown related options

    #endregion
    
    #region Initializer

    protected override void Start()
    {
        base.Start();

        m_SetStateMachine();
        m_SetGraphs();
    }

    private void m_SetStateMachine()
    {
        m_machine = new StateMachine(stIdleBasic);

        m_machine.SetCallbacks(stIdleBasic, Input_IdleBasic, Logic_IdleBasic, Enter_IdleBasic, End_IdleBasic);
        m_machine.SetCallbacks(stIdleLong, Input_IdleLong, Logic_IdleLong, Enter_IdleLong, End_IdleLong);
        m_machine.SetCallbacks(stIdleWall, null, null, null, null);
        m_machine.SetCallbacks(stAir, Input_Air, Logic_Air, null, null);
        m_machine.SetCallbacks(stMove, Input_Move, Logic_Move, null, null);
        m_machine.SetCallbacks(stJump, Input_Jump, Logic_Jump, Enter_Jump, End_Jump);
        m_machine.SetCallbacks(stJumpAir, null, null, null, null);
        m_machine.SetCallbacks(stJumpWall, null, null, null, null);
        m_machine.SetCallbacks(stWallSliding, null, null, null, null);
        m_machine.SetCallbacks(stLedgeHold, null, null, null, null);
        m_machine.SetCallbacks(stLedgeClimb, null, null, null, null);
        m_machine.SetCallbacks(stSit, null, null, null, null);
        m_machine.SetCallbacks(stHeadUp, null, null, null, null);
        m_machine.SetCallbacks(stRoll, null, null, null, null);
        m_machine.SetCallbacks(stDash, null, null, null, null);
        m_machine.SetCallbacks(stTakeDown, null, null, null, null);
    }

    private void m_SetGraphs()
    {
        jumpGraph = new DiscreteLinearGraph(jumpFrame);
    }

    #endregion

    #region Physics Utilities

    #endregion

    #region Execute Object Actions

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        m_machine.UpdateLogic();
    }

    private void Logic_IdleBasic()
    {
        currentIdleBasicFrame++;
        SetVelocity(0.0f, 0.0f);
    }

    private void Logic_IdleLong()
    {
        SetVelocity(0.0f, 0.0f);
    }

    private void Logic_Air()
    {
        SetVelocityX(xInput == 0 ? 0.0f : moveSpeed * lookingDirection);

        if(rigid.velocity.y < -maxFreeFallSpeed)
        {
            SetVelocityY(-maxFreeFallSpeed);
        }
    }

    protected override void Logic_Move()
    {
        if(xInput != 0)
        {
            base.Logic_Move();
        }
    }

    private void Logic_Jump()
    {
        SetVelocityX(xInput == 0 ? 0.0f : moveSpeed * lookingDirection);

        if(leftJumpFrame > 0)
        {
            SetVelocityY(jumpSpeed * jumpGraph[leftJumpFrame-- - 1]);
        }
    }

    #endregion

    #region State Transition Input Checker

    private void CheckInput()
    {
        xNegative = InputHandler.data.xNegative;
        xPositive = InputHandler.data.xPositive;
        xInput = xNegative + xPositive;

        jumpDown = InputHandler.data.jumpDown;
        jumpUp = InputHandler.data.jumpUp;
        jumpPressing = InputHandler.data.jumpPressing;
    }
    
    #endregion

    #region State Transition Logics

    protected override void Update()
    {
        base.Update();

        CheckInput();
        CheckLookingDirection(xInput);

        m_machine.UpdateInput();
    }

    private void Input_IdleBasic()
    {
        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(jumpDown)
        {
            m_machine.ChangeState(stJump);
            return;
        }
        if(xInput != 0)
        {
            m_machine.ChangeState(stMove);
            return;
        }
        if(currentIdleBasicFrame >= longIdleFrame)
        {
            m_machine.ChangeState(stIdleLong);
            return;
        }
    }

    private void Input_IdleLong()
    {
        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(jumpDown)
        {
            m_machine.ChangeState(stJump);
            return;
        }
        if(xInput != 0)
        {
            m_machine.ChangeState(stMove);
            return;
        }
    }

    private void Input_Air()
    {
        if(isOnGround)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
    }

    private void Input_Move()
    {
        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(jumpDown)
        {
            m_machine.ChangeState(stJump);
            return;
        }
        if(xInput == 0)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
    }

    private void Input_Jump()
    {
        if(jumpDown /*&& leftContinuousJumpCount > 0*/)
        {
            
        }
        if(jumpUp && !isReleasedJumpKey)
        {
            isReleasedJumpKey = true;
            leftJumpFrame /= 2;
        }
        if(leftJumpFrame == 0)
        {
            m_machine.ChangeState(stAir);
            return;
        }
    }

    #endregion

    #region On State Enter

    private void Enter_IdleBasic()
    {
        rigid.gravityScale = 0.0f;
        currentIdleBasicFrame = 0;
    }

    private void Enter_IdleLong()
    {
        rigid.gravityScale = 0.0f;
    }

    private void Enter_Jump()
    {
        leftJumpFrame = jumpFrame;
        isReleasedJumpKey = false;
    }

    #endregion

    #region On State End

    private void End_IdleBasic()
    {
        rigid.gravityScale = 1.0f;
        currentIdleBasicFrame = 0;
    }

    private void End_IdleLong()
    {
        rigid.gravityScale = 1.0f;
    }

    private void End_Jump()
    {
        leftJumpFrame = 0;
        isReleasedJumpKey = false;
    }

    #endregion

    #region Debug Logics
    #if UNITY_EDITOR

    private void LateUpdate()
    {
        Debug.Log(string.Format("current state: {0}", m_machine.state));
    }

    #endif
    #endregion
}