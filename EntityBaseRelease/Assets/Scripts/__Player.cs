using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class __Player : Entity
{
    public int CURRENT_STATE;
    #region Player State Constants

    // All Available Player States
    private const int stIdleBasic           = 0; // 바닥에서의 정지(일반)
    private const int stIdleLong            = 1; // 바닥에서의 정지(장시간)
    private const int stIdleWall            = 2; // 벽 붙기
    private const int stAir                 = 3; // 체공
    private const int stGliding             = 4; // 글라이딩
    private const int stMoveWalk            = 5; // 바닥에서의 움직임(걷기)
    private const int stMoveRun             = 6; // 바닥에서의 움직임(달리기)
    private const int stJumpBasic           = 7; // 점프(일반)
    private const int stJumpAir             = 8; // 점프(공중)
    private const int stJumpWall            = 9; // 점프(벽)
    private const int stJumpDown            = 10; // 점프(하향)
    private const int stWallSliding         = 11; // 벽 슬라이딩
    private const int stLedgeClimb          = 12; // 난간 오르기
    private const int stSit                 = 13; // 앉기
    private const int stHeadUp              = 14; // 고개 들기
    private const int stRoll                = 15; // 구르기
    private const int stDash                = 16; // 대쉬
    private const int stTakeDown            = 17; // 내려 찍기

    #endregion

    #region Player Constants
    [Header("Player Constants")]
    // IdleBasic options
    public int longIdleTransitionFrame = 900;

    // IdleLong options

    // IdleWall options

    // Air options
    // float base.maxFreeFallSpeedY;
    public int freeFallAccelFrame = 39;
    private DiscreteGraph freeFallAccelGraph;

    // Gliding options
    public float maxGlidingSpeedY = 2.5f;
    public int glidingAccelFrameX = 26;
    public int glidingDeaccelFrameX = 39;
    public int glidingAccelFrameY = 39;
    private DiscreteGraph glidingAccelGraphX;
    private DiscreteGraph glidingDeaccelGraphX;
    private DiscreteGraph glidingAccelGraphY;

    // MoveWalk options
    // float base.moveSpeed

    // MoveRun options
    public float runSpeed = 7.5f;
    public int runAccelFrame = 13;
    private DiscreteGraph runAccelGraph;

    // JumpBasic options
    public int maxJumpBasicCount = 1;
    public float jumpBasicSpeed = 7.0f;
    public int jumpBasicDeaccelFrame = 13;
    private DiscreteGraph jumpBasicDeaccelGraph;

    // JumpAir options
    public int maxJumpAirCount = 1;
    public float jumpAirSpeed = 8.0f;
    public int jumpAirIdleFrame = 3;
    public int jumpAirDeaccelFrame = 13;
    private DiscreteGraph jumpAirDeaccelGraph;
        
    // JumpWall options
    public float jumpWallSpeed = 7.0f;
    public int jumpWallDeaccelFrame = 13;
    public int jumpWallForceFrame = 6;
    private DiscreteGraph jumpWallDeaccelGraph;

    // JumpDown options
    public float jumpDownSpeed = 1.5f;
    public int jumpDownDeaccelFrame = 30;
    private DiscreteGraph jumpDownDeaccelGraph;

    // WallSliding options
    public float maxWallSlidingSpeedY = 2.0f;
    public int wallSlidingAccelFrame = 13;
    private DiscreteGraph wallSlidingAccelGraph;

    // LedgeClimb options
    public float ledgeCheckRangeY = 0.2f;
    public float ledgeDetectingLength = 0.04f;

    // Sit options
    public int sitCameraMoveFrame = 120;

    // HeadUp options
    public int headUpCameraMoveFrame = 120;

    // Roll options
    public float rollSpeed = 8.0f;
    public int rollPreparingFrame = 6;
    public int rollInvincibilityFrame = 18;
    public int rollWakeUpFrame = 6;
    public int rollCoolFrame = 180;
    private DiscreteGraph rollDeaccelGraph;

    // Dash options
    public float dashSpeed = 8.0f;
    public int maxDashCount = 1;
    public int dashIdleFrame = 6;
    public int dashInvincibilityFrame = 9;
    private DiscreteLinearGraph dashDeaccelGraph;

    // TakeDown options
    public float takeDownSpeed = 32.0f;
    public int takeDownStartingIdleFrame = 18;
    public int takeDownLandingIdleFrame = 12;

    // State Machine
    private StateMachine m_machine;

    #endregion

    #region Player Variables
    [Header("Player Variables")]
    // IdleBasic options
    public int proceedLongIdleTransitionFrame;

    // IdleLong options

    // IdleWall options

    // Air options
    // float base.maxFreeFallSpeedY;
    public int proceedFreeFallAccelFrame;

    // Gliding options
    public int proceedGlidingAccelFrameX;
    public int leftGlidingDeaccelFrameX;
    public int proceedGlidingAccelFrameY;

    // MoveWalk options
    public bool isRunState;

    // MoveRun options
    public int proceedRunAccelFrame;

    // JumpBasic options
    public int leftJumpBasicCount;
    public int leftJumpBasicDeaccelFrame;

    // JumpAir options
    public int leftJumpAirCount;
    public int leftJumpAirIdleFrame;
    public int leftJumpAirDeaccelFrame;
        
    // JumpWall options
    public int leftJumpWallDeaccelFrame;
    public int leftJumpWallForceFrame;

    // JumpDown options
    public int leftJumpDownDeaccelFrame;
    protected float throughableGroundThickness;
    protected Vector2 throughableGroundRaycasterPosition;
    protected RaycastHit2D detectedThroughableGround;

    // WallSliding options
    public int proceedWallSlidingAccelFrame;

    // LedgeClimb options
    public bool canDetectLedge;
    public bool isOnLedge;
    public bool isEndOfLedgeAnimation;
    protected RaycastHit2D detectedLedgeBottom;
    protected RaycastHit2D detectedLedgeTop;
    protected Vector2 ledgeCornerPosition;

    // Sit options
    public int proceedSitCameraMoveFrame;

    // HeadUp options
    public int proceedHeadUpCameraMoveFrame;

    // Roll options
    public int leftRollPreparingFrame;
    public int leftRollInvincibilityFrame;
    public int leftRollWakeUpFrame;
    public int leftRollCoolFrame;

    // Dash options
    public int leftDashCount;
    public int leftDashIdleFrame;
    public int leftDashInvincibilityFrame;

    // TakeDown options
    public int leftTakeDownStartingIdleFrame;
    public int leftTakeDownLandingIdleFrame;
    public bool isOnLandingAfterTakeDown;

    #endregion

    #region Input Options

    [Header("Input Options")]
    // Input options
    public InputData inputData;

    #endregion

    #region Internal Initializer (m_SetXXX)

    private void m_SetStateMachine()
    {
        m_machine = new StateMachine(stIdleBasic);

        m_machine.SetCallbacks(stIdleBasic, Input_IdleBasic, Logic_IdleBasic, Enter_IdleBasic, End_IdleBasic); // 완성
        m_machine.SetCallbacks(stIdleLong, Input_IdleLong, Logic_IdleLong, Enter_IdleLong, End_IdleLong); // 완성
        m_machine.SetCallbacks(stIdleWall, Input_IdleWall, null, Enter_IdleWall, null);
        m_machine.SetCallbacks(stAir, Input_Air, Logic_Air, Enter_Air, End_Air); // 완성
        m_machine.SetCallbacks(stGliding, Input_Gliding, null, null, null);
        m_machine.SetCallbacks(stMoveWalk, Input_MoveWalk, Logic_MoveWalk, Enter_MoveWalk, null); // 완성
        m_machine.SetCallbacks(stMoveRun, Input_MoveRun, Logic_MoveRun, Enter_MoveRun, End_MoveRun); // 완성
        m_machine.SetCallbacks(stJumpBasic, Input_JumpBasic, null, null, null);
        m_machine.SetCallbacks(stJumpAir, Input_JumpAir, null, null, null);
        m_machine.SetCallbacks(stJumpWall, Input_JumpWall, null, null, null);
        m_machine.SetCallbacks(stJumpDown, Input_JumpDown, null, null, null);
        m_machine.SetCallbacks(stWallSliding, Input_WallSliding, null, null, null);
        m_machine.SetCallbacks(stLedgeClimb, Input_LedgeClimb, null, null, null);
        m_machine.SetCallbacks(stSit, Input_Sit, Logic_Sit, Enter_Sit, End_Sit); // 완성
        m_machine.SetCallbacks(stHeadUp, Input_HeadUp, Logic_HeadUp, Enter_HeadUp, End_HeadUp); // 완성
        m_machine.SetCallbacks(stRoll, Input_Roll, null, null, null);
        m_machine.SetCallbacks(stDash, Input_Dash, null, null, null);
        m_machine.SetCallbacks(stTakeDown, Input_TakeDown, null, null, null);
    }

    private void m_SetGraphs()
    {
        freeFallAccelGraph                  = new DiscreteLinearGraph(freeFallAccelFrame);
        glidingAccelGraphX                  = new DiscreteLinearGraph(glidingAccelFrameX);
        glidingDeaccelGraphX                = new DiscreteLinearGraph(glidingDeaccelFrameX);
        glidingAccelGraphY                  = new DiscreteLinearGraph(glidingAccelFrameY);
        runAccelGraph                       = new DiscreteLinearGraph(runAccelFrame);
        jumpBasicDeaccelGraph               = new DiscreteLinearGraph(jumpBasicDeaccelFrame);
        jumpAirDeaccelGraph                 = new DiscreteLinearGraph(jumpAirDeaccelFrame);
        jumpWallDeaccelGraph                = new DiscreteLinearGraph(jumpWallDeaccelFrame);
        jumpDownDeaccelGraph                = new DiscreteLinearGraph(jumpDownDeaccelFrame);
        wallSlidingAccelGraph               = new DiscreteLinearGraph(wallSlidingAccelFrame);
        rollDeaccelGraph                    = new DiscreteLinearGraph(rollPreparingFrame + rollInvincibilityFrame + rollWakeUpFrame);
        dashDeaccelGraph                    = new DiscreteLinearGraph(dashInvincibilityFrame);
    }
    
    #endregion

    #region Unity Event Functions

    protected override void Start()
    {
        base.Start();

        m_SetStateMachine();
        m_SetGraphs();
    }

    protected override void Update()
    {
        inputData.Copy(InputHandler.data);
        CheckLookingDirection(inputData.xInput);

        base.Update();

        m_machine.UpdateInput();
        CURRENT_STATE = m_machine.state;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckLedge();

        m_machine.UpdateLogic();
    }

    #endregion

    #region Physics Checker

    protected void CheckLedge()
    {
        if(!canDetectLedge)
        {
            isOnLedge = false;
            detectedLedgeBottom = default(RaycastHit2D);
            detectedLedgeTop = default(RaycastHit2D);

            return;
        }

        float bPosX, bPosY;
        float tPosX, tPosY;

        bPosX = ceilBox.bounds.max.x * lookingDirection;
        bPosY = ceilBox.bounds.center.y;
        tPosX = bPosX;
        tPosY = bPosY + ledgeCheckRangeY;

        Vector2 ledgeCheckerPositionBottom = new Vector2(bPosX, bPosY);
        Vector2 ledgeCheckerPositionTop = new Vector2(tPosX, tPosY);

        detectedLedgeBottom = Physics2D.Raycast(ledgeCheckerPositionBottom, Vector2.right * lookingDirection, ledgeDetectingLength, LayerInfo.groundMask);
        detectedLedgeTop = default(RaycastHit2D);
        RaycastHit2D temp_detectedLedgeTop = Physics2D.Raycast(ledgeCheckerPositionTop, Vector2.right * lookingDirection, ledgeDetectingLength, LayerInfo.groundMask);

        // TODO:
        // 이 곳에 Ledge를 판정하는 Hinge를 찾는 구문을 추가, Hinge가 없는 Ledge라면 아래 코드를 실행하지 않는다.
        // 변수 detectedLedgeBottom을 이용한 구문을 작성한다.

        if(detectedLedgeBottom && !temp_detectedLedgeTop)
        {
            isOnLedge = true;

            float adder = 0.02f;
            float xDistance = detectedLedgeBottom.distance + adder;

            tPosX = bPosX + xDistance;
            tPosY = bPosY + ledgeCheckRangeY;
            ledgeCheckerPositionTop.Set(tPosX, tPosY);

            detectedLedgeTop = Physics2D.Raycast(ledgeCheckerPositionTop, Vector2.down, ledgeCheckRangeY + 0.02f, LayerInfo.groundMask);

            if(detectedLedgeTop)
            {
                SetVector(ref ledgeCornerPosition, detectedLedgeTop.point.x, detectedLedgeTop.point.y);
            }
            else
            {
                // TODO: 치명적인 오류 발생으로 프로그램 종료함. 따로 처리를 할 필요가 있음. 웬만해서는 일어나지 않을 듯?
            }
        }
        else
        {
            isOnLedge = false;
        }
    }

    #endregion

    #region Implement State; stIdleBasic

    private void Enter_IdleBasic()
    {
        rigid.gravityScale = 0.0f;
        leftJumpBasicCount = maxJumpBasicCount;
        leftDashCount = maxDashCount;

        // 선입력 프레임 수
        int bufferedFrame = 6;
        int i;
        InputData idat;

        for(i = 0; i < bufferedFrame; i++)
        {
            idat = InputBuffer.GetBufferedData(i);

            if(idat.jumpPressing && leftJumpBasicCount > 0)
            {
                m_machine.ChangeState(stJumpBasic);
                break;
            }
            if(idat.dashPressing)
            {
                m_machine.ChangeState(stRoll);
                break;
            }
            if(idat.yNegative != 0)
            {
                m_machine.ChangeState(stSit);
                break;
            }
        }
    }

    private void Input_IdleBasic()
    {
        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(proceedLongIdleTransitionFrame == longIdleTransitionFrame)
        {
            m_machine.ChangeState(stIdleLong);
            return;
        }
        if(inputData.yNegative != 0)
        {
            m_machine.ChangeState(stSit);
            return;
        }
        if(inputData.yPositive != 0)
        {
            m_machine.ChangeState(stHeadUp);
            return;
        }
        if(inputData.jumpDown && leftJumpBasicCount > 0)
        {
            m_machine.ChangeState(stJumpBasic);
            return;
        }
        if(inputData.dashDown && leftRollCoolFrame == 0)
        {
            m_machine.ChangeState(stRoll);
            return;
        }
        if(inputData.xInput != 0)
        {
            if(isRunState)
                m_machine.ChangeState(stMoveRun);
            else
                m_machine.ChangeState(stMoveWalk);

            return;
        }
    }

    private void Logic_IdleBasic()
    {
        SetVelocity(0.0f, 0.0f);

        if(proceedLongIdleTransitionFrame < longIdleTransitionFrame)
        {
            proceedLongIdleTransitionFrame++;
        }
    }

    private void End_IdleBasic()
    {
        proceedLongIdleTransitionFrame = 0;
    }

    #endregion

    #region Implement State; stIdleLong

    private void Enter_IdleLong()
    {
        rigid.gravityScale = 0.0f;
        proceedLongIdleTransitionFrame = 0;
    }

    private void Input_IdleLong()
    {
        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(inputData.yNegative != 0)
        {
            m_machine.ChangeState(stSit);
            return;
        }
        if(inputData.yPositive != 0)
        {
            m_machine.ChangeState(stHeadUp);
            return;
        }
        if(inputData.jumpDown && leftJumpBasicCount > 0)
        {
            m_machine.ChangeState(stJumpBasic);
            return;
        }
        if(inputData.dashDown && leftRollCoolFrame == 0)
        {
            m_machine.ChangeState(stRoll);
            return;
        }
        if(inputData.xInput != 0)
        {
            if(isRunState)
                m_machine.ChangeState(stMoveRun);
            else
                m_machine.ChangeState(stMoveWalk);

            return;
        }
    }

    private void Logic_IdleLong()
    {
        SetVelocity(0.0f, 0.0f);
    }

    private void End_IdleLong()
    {
        proceedLongIdleTransitionFrame = 0;
    }

    #endregion

    #region Implement State; stIdleWall

    private void Enter_IdleWall()
    {
        // 선입력 프레임 수
        int bufferedFrame = 6;
        int i;
        InputData idat;

        for(i = 0; i < bufferedFrame; i++)
        {
            idat = InputBuffer.GetBufferedData(i);

            if(idat.jumpPressing)
            {
                m_machine.ChangeState(stJumpWall);
                break;
            }
        }
    }

    private void Input_IdleWall()
    {
        if(isOnWallFeet == 0 || isOnWallCeil == 0 || inputData.yNegDown)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(inputData.xInput == 0)
        {
            m_machine.ChangeState(stWallSliding);
            return;
        }
        if(inputData.jumpDown)
        {
            m_machine.ChangeState(stJumpWall);
            return;
        }
    }

    #endregion

    #region Implement State; stAir

    private void Enter_Air()
    {
        int i;

        rigid.gravityScale = 1.0f;

        if(leftJumpBasicCount == maxJumpBasicCount)
            leftJumpBasicCount--;

        if(currentVelocity.y >= 0)
        {
            proceedFreeFallAccelFrame = 0;
        }
        else if(currentVelocity.y < -maxFreeFallSpeed * freeFallAccelGraph[freeFallAccelFrame - 1])
        {
            proceedFreeFallAccelFrame = freeFallAccelFrame - 1;
        }
        else
        {
            for(i = 0; i < freeFallAccelFrame; i++)
            {
                if(-maxFreeFallSpeed * freeFallAccelGraph[i] <= currentVelocity.y)
                {
                    proceedFreeFallAccelFrame = i;
                    break;
                }
            }
        }

        // 선입력 프레임 수
        int bufferedFrame = 6;
        InputData idat;

        for(i = 0; i < bufferedFrame; i++)
        {
            idat = InputBuffer.GetBufferedData(i);

            if(idat.jumpPressing && leftJumpAirCount > 0)
            {
                m_machine.ChangeState(stJumpAir);
                break;
            }
        }
    }

    private void Input_Air()
    {
        if(isOnGround)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
        if(inputData.jumpDown && leftJumpAirCount > 0)
        {
            m_machine.ChangeState(stJumpAir);
            return;
        }
        if(inputData.dashDown && leftDashCount > 0)
        {
            m_machine.ChangeState(stDash);
            return;
        }
        if(inputData.yPositive != 0)
        {
            m_machine.ChangeState(stGliding);
            return;
        }
        if(inputData.yNegative != 0 && inputData.jumpPressing)
        {
            m_machine.ChangeState(stTakeDown);
            return;
        }
        if(!isDetectedGround && inputData.xInput == lookingDirection && isOnWallFeet == lookingDirection && isOnWallCeil == lookingDirection && inputData.yNegative == 0)
        {
            m_machine.ChangeState(stIdleWall);
            return;
        }
        if(isOnLedge && inputData.xInput == lookingDirection)
        {
            m_machine.ChangeState(stLedgeClimb);
            return;
        }
    }

    private void Logic_Air()
    {
        if(inputData.xInput == 0 || isOnWallFeet == lookingDirection || isOnWallCeil == lookingDirection)
        {
            SetVelocityX(0.0f);
        }
        else
        {
            SetVelocityX(moveSpeed * lookingDirection);
        }

        if(currentVelocity.y < 0)
        {
            SetVelocityY(-maxFreeFallSpeed * freeFallAccelGraph[proceedFreeFallAccelFrame]);

            if(proceedFreeFallAccelFrame < freeFallAccelFrame - 1)
                proceedFreeFallAccelFrame++;
        }
    }

    private void End_Air()
    {
        proceedFreeFallAccelFrame = 0;
    }

    #endregion

    #region Implement State; stGliding

    private void Input_Gliding()
    {
        if(inputData.yPositive == 0)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(isOnGround)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
        if(inputData.jumpDown && leftJumpAirCount > 0)
        {
            m_machine.ChangeState(stJumpAir);
            return;
        }
        if(inputData.dashDown && leftDashCount > 0)
        {
            m_machine.ChangeState(stDash);
            return;
        }
        if(!isDetectedGround && inputData.xInput == lookingDirection && isOnWallFeet == lookingDirection && isOnWallCeil == lookingDirection && inputData.yNegative == 0)
        {
            m_machine.ChangeState(stIdleWall);
            return;
        }
        if(isOnLedge && inputData.xInput == lookingDirection)
        {
            m_machine.ChangeState(stLedgeClimb);
            return;
        }
    }

    #endregion

    #region Implement State; stMoveWalk

    private void Enter_MoveWalk()
    {
        rigid.gravityScale = 1.0f;
    }

    private void Input_MoveWalk()
    {
        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(inputData.xInput == 0)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
        if(inputData.yNegative != 0)
        {
            m_machine.ChangeState(stSit);
            return;
        }
        if(inputData.yPositive != 0)
        {
            m_machine.ChangeState(stHeadUp);
            return;
        }
        if(inputData.jumpDown && leftJumpBasicCount > 0)
        {
            m_machine.ChangeState(stJumpBasic);
            return;
        }
        if(inputData.dashDown && leftRollCoolFrame == 0)
        {
            m_machine.ChangeState(stRoll);
            return;
        }
        if(isRunState)
        {
            m_machine.ChangeState(stMoveRun);
            return;
        }
    }

    private void Logic_MoveWalk()
    {
        MoveOnGround(moveSpeed, lookingDirection);
    }

    #endregion

    #region Implement State; stMoveRun

    private void Enter_MoveRun()
    {
        rigid.gravityScale = 1.0f;
        proceedRunAccelFrame = 0;
    }

    private void Input_MoveRun()
    {
        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(inputData.xInput == 0)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
        if(inputData.yNegative != 0)
        {
            m_machine.ChangeState(stSit);
            return;
        }
        if(inputData.yPositive != 0)
        {
            m_machine.ChangeState(stHeadUp);
            return;
        }
        if(inputData.jumpDown && leftJumpBasicCount > 0)
        {
            m_machine.ChangeState(stJumpBasic);
            return;
        }
        if(inputData.dashDown && leftRollCoolFrame == 0)
        {
            m_machine.ChangeState(stRoll);
            return;
        }
        if(!isRunState)
        {
            m_machine.ChangeState(stMoveWalk);
            return;
        }
    }

    private void Logic_MoveRun()
    {
        MoveOnGround(runSpeed * runAccelGraph[proceedRunAccelFrame], lookingDirection);

        if(proceedRunAccelFrame < runAccelFrame - 1)
            proceedRunAccelFrame++;
    }

    private void End_MoveRun()
    {
        proceedRunAccelFrame = 0;
    }

    #endregion

    #region Implement State; stJumpBasic

    private void Input_JumpBasic()
    {
        if((currentVelocity.y <= 0.0f && leftJumpBasicDeaccelFrame == 0) || isOnCeil)
        {
            if(inputData.yPositive == 0)
                m_machine.ChangeState(stAir);
            else
                m_machine.ChangeState(stGliding);

            return;
        }
        if(inputData.jumpDown && leftJumpAirCount > 0)
        {
            m_machine.ChangeState(stJumpAir);
            return;
        }
        if(inputData.dashDown && leftDashCount > 0)
        {
            m_machine.ChangeState(stDash);
            return;
        }
        if(!isDetectedGround && inputData.xInput == lookingDirection && isOnWallFeet == lookingDirection && isOnWallCeil == lookingDirection && inputData.yNegative == 0)
        {
            m_machine.ChangeState(stIdleWall);
            return;
        }
        if(inputData.yNegative != 0 && inputData.jumpPressing)
        {
            m_machine.ChangeState(stTakeDown);
            return;
        }
        if(isOnLedge && inputData.xInput == lookingDirection)
        {
            m_machine.ChangeState(stLedgeClimb);
            return;
        }
    }

    #endregion

    #region Implement State; stJumpAir

    private void Input_JumpAir()
    {
        if((currentVelocity.y <= 0.0f && leftJumpAirDeaccelFrame == 0) || isOnCeil)
        {
            if(inputData.yPositive == 0)
                m_machine.ChangeState(stAir);
            else
                m_machine.ChangeState(stGliding);

            return;
        }
        if(inputData.jumpDown && leftJumpAirCount > 0)
        {
            m_machine.RestartState();
            return;
        }
        if(inputData.dashDown && leftDashCount > 0)
        {
            m_machine.ChangeState(stDash);
            return;
        }
        if(!isDetectedGround && inputData.xInput == lookingDirection && isOnWallFeet == lookingDirection && isOnWallCeil == lookingDirection && inputData.yNegative == 0)
        {
            m_machine.ChangeState(stIdleWall);
            return;
        }
        if(inputData.yNegative != 0 && inputData.jumpPressing)
        {
            m_machine.ChangeState(stTakeDown);
            return;
        }
        if(isOnLedge && inputData.xInput == lookingDirection)
        {
            m_machine.ChangeState(stLedgeClimb);
            return;
        }
    }

    #endregion

    #region Implement State; stJumpWall

    private void Input_JumpWall()
    {
        if(leftJumpWallForceFrame != 0)
            return;

        if((currentVelocity.y <= 0.0f && leftJumpWallDeaccelFrame == 0) || isOnCeil)
        {
            if(inputData.yPositive == 0)
                m_machine.ChangeState(stAir);
            else
                m_machine.ChangeState(stGliding);

            return;
        }
        if(inputData.jumpDown && leftJumpAirCount > 0)
        {
            m_machine.RestartState();
            return;
        }
        if(inputData.dashDown && leftDashCount > 0)
        {
            m_machine.ChangeState(stDash);
            return;
        }
        if(!isDetectedGround && inputData.xInput == lookingDirection && isOnWallFeet == lookingDirection && isOnWallCeil == lookingDirection && inputData.yNegative == 0)
        {
            m_machine.ChangeState(stIdleWall);
            return;
        }
        if(inputData.yNegative != 0 && inputData.jumpPressing)
        {
            m_machine.ChangeState(stTakeDown);
            return;
        }
        if(isOnLedge && inputData.xInput == lookingDirection)
        {
            m_machine.ChangeState(stLedgeClimb);
            return;
        }
    }

    #endregion

    #region Implement State; stJumpDown

    private void Input_JumpDown()
    {
        // TODO: 보류
    }

    #endregion

    #region Implement State; stWallSliding

    private void Input_WallSliding()
    {
        if(isOnWallFeet == 0 || isOnWallCeil == 0 || inputData.yNegDown)
        {
            m_machine.ChangeState(stAir);
        }
        if(isDetectedGround)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
        if(inputData.jumpDown)
        {
            m_machine.ChangeState(stJumpWall);
            return;
        }
        if(!isDetectedGround && inputData.xInput == lookingDirection && isOnWallFeet == lookingDirection && isOnWallCeil == lookingDirection && inputData.yNegative == 0)
        {
            m_machine.ChangeState(stIdleWall);
            return;
        }
    }

    #endregion

    #region Implement State; stLedgeClimb

    private void Input_LedgeClimb()
    {
        if(isEndOfLedgeAnimation)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
    }

    #endregion

    #region Implement State; stSit

    private void Enter_Sit()
    {
        rigid.gravityScale = 0.0f;
        proceedSitCameraMoveFrame = 0;
    }

    private void Input_Sit()
    {
        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(inputData.yNegative == 0)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
        if(inputData.jumpDown)
        {
            if(detectedThroughableGround)
            {
                m_machine.ChangeState(stJumpDown);
            }
            else
            {
                m_machine.ChangeState(stJumpBasic);
            }
        }
        if(inputData.dashDown)
        {
            m_machine.ChangeState(stRoll);
            return;
        }
    }

    private void Logic_Sit()
    {
        SetVelocity(0.0f, 0.0f);

        if(proceedSitCameraMoveFrame < sitCameraMoveFrame)
            proceedSitCameraMoveFrame++;
    }

    private void End_Sit()
    {
        proceedSitCameraMoveFrame = 0;
    }

    #endregion

    #region Implement State; stHeadUp

    private void Enter_HeadUp()
    {
        rigid.gravityScale = 0.0f;
        proceedHeadUpCameraMoveFrame = 0;
    }

    private void Input_HeadUp()
    {
        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(inputData.yNegative == 0)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
        if(inputData.jumpDown && leftJumpBasicCount > 0)
        {
            m_machine.ChangeState(stJumpBasic);
            return;
        }
        if(inputData.dashDown)
        {
            m_machine.ChangeState(stRoll);
            return;
        }
    }

    private void Logic_HeadUp()
    {
        SetVelocity(0.0f, 0.0f);

        if(proceedHeadUpCameraMoveFrame < headUpCameraMoveFrame)
            proceedHeadUpCameraMoveFrame++;
    }

    private void End_HeadUp()
    {
        proceedHeadUpCameraMoveFrame = 0;
    }

    #endregion

    #region Implement State; stRoll

    private void Input_Roll()
    {
        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(leftRollPreparingFrame == 0 && leftRollInvincibilityFrame == 0 && leftRollWakeUpFrame == 0)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
    }

    #endregion

    #region Implement State; stDash

    private void Input_Dash()
    {
        if(leftDashIdleFrame == 0 && leftDashInvincibilityFrame == 0)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(isOnLedge && inputData.xInput == lookingDirection)
        {
            m_machine.ChangeState(stLedgeClimb);
            return;
        }
        if(!isDetectedGround && inputData.xInput == lookingDirection && isOnWallFeet == lookingDirection && isOnWallCeil == lookingDirection && inputData.yNegative == 0)
        {
            m_machine.ChangeState(stIdleWall);
            return;
        }
    }

    #endregion

    #region Implement State; stTakeDown

    private void Input_TakeDown()
    {
        if(isOnLandingAfterTakeDown)
        {
            if(!isDetectedGround)
            {
                m_machine.ChangeState(stAir);
                return;
            }
            if(leftTakeDownLandingIdleFrame > 0)
            {
                m_machine.ChangeState(stIdleBasic);
                return;
            }
        }
    }

    #endregion
}
