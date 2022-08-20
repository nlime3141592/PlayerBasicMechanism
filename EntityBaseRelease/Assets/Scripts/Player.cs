using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    #region Player Constants

    // All Available Player States
    private const int stIdleBasic           = 0; // 정지(일반)
    private const int stIdleLong            = 1; // 정지(장시간)
    private const int stIdleWall            = 2; // 벽 붙기
    private const int stAir                 = 3; // 체공
    private const int stMove                = 4; // 바닥에서의 움직임
    private const int stJump                = 5; // 점프(일반)
    private const int stJumpAir             = 6; // 점프(공중)
    private const int stJumpWall            = 7; // 점프(벽)
    private const int stJumpDown            = 8; // 점프(하향)
    private const int stWallSliding         = 9; // 벽 슬라이딩
    private const int stLedgeHold           = 10; // 난간 잡기
    private const int stLedgeClimb          = 11; // 난간 오르기
    private const int stSit                 = 12; // 앉기
    private const int stHeadUp              = 13; // 고개 들기
    private const int stRoll                = 14; // 구르기
    private const int stDash                = 15; // 대쉬
    private const int stTakeDown            = 16; // 내려 찍기

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
    public int continuousJumpCount = 1;
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
    private DiscreteLinearGraph airJumpGraph;

    // WallSliding related options
    public float maxWallSlidingSpeed = 5.0f;

    public int wallSlidingAccelFrame = 13;

    // Ledge related options

    // Sit & HeadUp related options
    public int sitCameraMoveFrame = 120;
    public int headUpCameraMoveFrame = 120;

    // Roll related options
    public float rollSpeed = 8.0f;

    public int rollDashFrame = 6;
    public int rollInvincibilityFrame = 18;
    public int rollWakeUpFrame = 6;
    public int rollCoolFrame = 180;

    private DiscreteLinearGraph rollGraph;

    // Dash related options
    public float dashSpeed = 8.0f;

    public int dashIdleFrame = 6;
    public int dashInvincibilityFrame = 9;

    private DiscreteLinearGraph dashGraph;

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
    public int leftContinuousJumpCount;
    public int leftJumpFrame;
    public int leftAirJumpIdleFrame;
    public int leftAirJumpFrame;

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
        m_machine.SetCallbacks(stJumpAir, Input_JumpAir, Logic_JumpAir, Enter_JumpAir, End_JumpAir);
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
        airJumpGraph = new DiscreteLinearGraph(airJumpFrame);
        rollGraph = new DiscreteLinearGraph(rollDashFrame);
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

    private void Logic_JumpAir()
    {
        if(leftAirJumpIdleFrame > 0)
        {
            if(rigid.gravityScale != 0.0f)
                rigid.gravityScale = 0.0f;

            SetVelocity(0.0f, 0.0f);
            leftAirJumpIdleFrame--;

            if(leftAirJumpIdleFrame == 0)
            {
                leftAirJumpFrame = airJumpFrame;
            }
        }
        else if(leftAirJumpFrame > 0)
        {
            if(rigid.gravityScale != 1.0f)
                rigid.gravityScale = 1.0f;

            SetVelocityX(xInput == 0 ? 0.0f : moveSpeed * lookingDirection);
            SetVelocityY(airJumpSpeed * airJumpGraph[leftAirJumpFrame-- - 1]);
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
        if(jumpDown && leftContinuousJumpCount > 0)
        {
            m_machine.ChangeState(stJumpAir);
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
        if(jumpDown && leftContinuousJumpCount > 0)
        {
            m_machine.ChangeState(stJumpAir);
            return;
        }
        if(jumpUp && !isReleasedJumpKey)
        {
            isReleasedJumpKey = true;
            leftJumpFrame /= 2;
            return;
        }
        if(leftJumpFrame == 0)
        {
            m_machine.ChangeState(stAir);
            return;
        }
    }

    private void Input_JumpAir()
    {
        if(jumpDown && leftContinuousJumpCount > 0)
        {
            m_machine.ChangeState(stJumpAir);
            return;
        }
        if(jumpUp && !isReleasedJumpKey)
        {
            if(leftAirJumpIdleFrame > 0)
            {
                leftAirJumpIdleFrame = 0;
                leftAirJumpFrame = airJumpFrame / 2;
            }
            else
            {
                leftAirJumpFrame /= 2;
            }

            isReleasedJumpKey = true;
            return;
        }
        if(leftAirJumpFrame == 0 && leftAirJumpIdleFrame == 0)
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
        leftContinuousJumpCount = continuousJumpCount;

        // 선입력 체크
    }

    private void Enter_IdleLong()
    {
        rigid.gravityScale = 0.0f;
    }

    private void Enter_Jump()
    {
        leftContinuousJumpCount--;
        leftJumpFrame = jumpFrame;
        isReleasedJumpKey = false;
    }

    private void Enter_JumpAir()
    {
        leftContinuousJumpCount--;
        leftAirJumpIdleFrame = airJumpIdleFrame;
        leftAirJumpFrame = airJumpFrame;
        isReleasedJumpKey = false;
        rigid.gravityScale = 1.0f;
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

    private void End_JumpAir()
    {
        leftAirJumpIdleFrame = 0;
        leftAirJumpFrame = 0;
        isReleasedJumpKey = false;
        rigid.gravityScale = 1.0f;
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