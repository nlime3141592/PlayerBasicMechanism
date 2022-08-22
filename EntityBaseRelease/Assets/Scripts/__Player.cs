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
    public int maxJumpCount = 1;
    public float jumpBasicSpeed = 7.0f;
    public int jumpBasicDeaccelFrame = 13;
    private DiscreteGraph jumpBasicDeaccelGraph;

    // JumpAir options
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
    public int leftJumpCount;
    public int leftJumpBasicDeaccelFrame;

    // JumpAir options
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

        m_machine.SetCallbacks(stIdleBasic, Input_IdleBasic, Logic_IdleBasic, Enter_IdleBasic, End_IdleBasic);
        m_machine.SetCallbacks(stIdleLong, Input_IdleLong, Logic_IdleLong, Enter_IdleLong, End_IdleLong);
        m_machine.SetCallbacks(stIdleWall, null, null, null, null);
        m_machine.SetCallbacks(stAir, Input_Air, Logic_Air, Enter_Air, End_Air);
        m_machine.SetCallbacks(stGliding, null, null, null, null);
        m_machine.SetCallbacks(stMoveWalk, Input_MoveWalk, Logic_MoveWalk, Enter_MoveWalk, null);
        m_machine.SetCallbacks(stMoveRun, Input_MoveRun, Logic_MoveRun, Enter_MoveRun, End_MoveRun);
        m_machine.SetCallbacks(stJumpBasic, null, null, null, null);
        m_machine.SetCallbacks(stJumpAir, null, null, null, null);
        m_machine.SetCallbacks(stJumpWall, null, null, null, null);
        m_machine.SetCallbacks(stJumpDown, null, null, null, null);
        m_machine.SetCallbacks(stWallSliding, null, null, null, null);
        m_machine.SetCallbacks(stLedgeClimb, null, null, null, null);
        m_machine.SetCallbacks(stSit, Input_Sit, Logic_Sit, Enter_Sit, End_Sit);
        m_machine.SetCallbacks(stHeadUp, Input_HeadUp, Logic_HeadUp, Enter_HeadUp, End_HeadUp);
        m_machine.SetCallbacks(stRoll, null, null, null, null);
        m_machine.SetCallbacks(stDash, null, null, null, null);
        m_machine.SetCallbacks(stTakeDown, null, null, null, null);
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

    #region State Changer

    private bool CanChangeToIdleLong()
    {
        return proceedLongIdleTransitionFrame >= longIdleTransitionFrame;
    }

    private bool CanChangeToIdleWall()
    {
        bool isCanChange = true;

        isCanChange &= (!isDetectedGround);
        isCanChange &= (inputData.xInput != 0);
        isCanChange &= (inputData.xInput == lookingDirection);
        isCanChange &= (isOnWallFeet == lookingDirection);
        isCanChange &= (isOnWallCeil == lookingDirection);
        isCanChange &= (inputData.yNegative == 0);

        return isCanChange;
    }

    // NOTE:
    // CanChangeToAir()를 구현하지 않은 이유
    // 다양한 상태에서 다양한 조건에 의해 stAir로 상태전이 할 수 있어서 Air는 Input_XXX 함수 내부에서 자체적으로 구현함.

    private bool CanChangeToGliding()
    {
        return inputData.yPositive != 0;
    }

    private bool CanChangeToMoveWalk()
    {
        bool isCanChange = true;

        isCanChange &= (!isRunState);
        isCanChange &= (inputData.xInput != 0);
        
        return isCanChange;
    }

    private bool CanChangeToMoveRun()
    {
        bool isCanChange = true;

        isCanChange &= (isRunState);
        isCanChange &= (inputData.xInput != 0);
        
        return isCanChange;
    }

    private bool CanChangeToJumpBasic()
    {
        bool isCanChange = true;

        isCanChange &= (inputData.jumpDown);
        isCanChange &= (isOnGround);
        isCanChange &= (leftJumpCount > 0);

        return isCanChange;
    }

    private bool CanChangeToJumpAir()
    {
        bool isCanChange = true;

        isCanChange &= (inputData.jumpDown);
        isCanChange &= (leftJumpCount  > 0);

        return isCanChange;
    }

    private bool CanChangeToJumpWall()
    {
        return inputData.jumpDown;
    }

    private bool CanChangeToWallSliding()
    {
        bool isCanChange = true;

        isCanChange &= (inputData.xInput == 0);
        isCanChange &= (isOnWallFeet == lookingDirection);
        isCanChange &= (isOnWallCeil == lookingDirection);

        return isCanChange;
    }

    private bool CanChangeToLedgeClimb()
    {
        return isOnLedge;
    }

    private bool CanChangeToSit()
    {
        return inputData.yNegative != 0;
    }

    private bool CanChangeToHeadUp()
    {
        return inputData.yPositive != 0;
    }

    private bool CanChangeToRoll()
    {
        return inputData.dashDown;
    }

    private bool CanChangeToDash()
    {
        return inputData.dashDown;
    }

    private bool CanChangeToTakeDown()
    {
        bool isCanChange = true;

        isCanChange &= (inputData.yNegative != 0);
        isCanChange &= (inputData.jumpPressing);

        return isCanChange;
    }

    #endregion

    #region Implement State; stIdleBasic

    private void Enter_IdleBasic()
    {
        rigid.gravityScale = 0.0f;
        leftJumpCount = maxJumpCount;
        leftDashCount = maxDashCount;
    }

    private void Input_IdleBasic()
    {
        // 체공, 장시간대기, 앉기, 고개들기, 점프, 구르기, 달리기, 걷기

        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(CanChangeToIdleLong())
        {
            m_machine.ChangeState(stIdleLong);
            return;
        }
        if(CanChangeToSit())
        {
            m_machine.ChangeState(stSit);
            return;
        }
        if(CanChangeToHeadUp())
        {
            m_machine.ChangeState(stHeadUp);
            return;
        }
        if(CanChangeToJumpBasic())
        {
            m_machine.ChangeState(stJumpBasic);
            return;
        }
        if(CanChangeToRoll())
        {
            m_machine.ChangeState(stRoll);
            return;
        }
        if(CanChangeToMoveRun())
        {
            m_machine.ChangeState(stMoveRun);
            return;
        }
        if(CanChangeToMoveWalk())
        {
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
        // 체공, 앉기, 고개들기, 점프, 구르기, 달리기, 걷기
        if(!isDetectedGround)
        {
            m_machine.ChangeState(stAir);
            return;
        }
        if(CanChangeToSit())
        {
            m_machine.ChangeState(stSit);
            return;
        }
        if(CanChangeToHeadUp())
        {
            m_machine.ChangeState(stHeadUp);
            return;
        }
        if(CanChangeToRoll())
        {
            m_machine.ChangeState(stRoll);
            return;
        }
        if(CanChangeToMoveRun())
        {
            m_machine.ChangeState(stMoveRun);
            return;
        }
        if(CanChangeToMoveWalk())
        {
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

    #endregion

    #region Implement State; stAir

    private void Enter_Air()
    {
        int i;

        rigid.gravityScale = 1.0f;

        if(leftJumpCount == maxJumpCount)
            leftJumpCount--;

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
    }

    private void Input_Air()
    {
        if(isOnGround)
        {
            m_machine.ChangeState(stIdleBasic);
            return;
        }
        if(CanChangeToJumpAir())
        {
            m_machine.ChangeState(stJumpAir);
            return;
        }
        if(CanChangeToDash())
        {
            m_machine.ChangeState(stDash);
            return;
        }
        if(CanChangeToGliding())
        {
            m_machine.ChangeState(stGliding);
            return;
        }
        if(CanChangeToTakeDown())
        {
            m_machine.ChangeState(stTakeDown);
            return;
        }
        if(CanChangeToIdleWall())
        {
            m_machine.ChangeState(stIdleWall);
            return;
        }
        if(CanChangeToLedgeClimb())
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

    #endregion

    #region Implement State; stMoveWalk

    private void Enter_MoveWalk()
    {
        rigid.gravityScale = 1.0f;
    }

    private void Input_MoveWalk()
    {
        // 체공, 대기, 앉기, 고개들기, 점프, 구르기, 달리기
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
        if(CanChangeToSit())
        {
            m_machine.ChangeState(stSit);
            return;
        }
        if(CanChangeToHeadUp())
        {
            m_machine.ChangeState(stHeadUp);
            return;
        }
        if(CanChangeToJumpBasic())
        {
            m_machine.ChangeState(stJumpBasic);
            return;
        }
        if(CanChangeToRoll())
        {
            m_machine.ChangeState(stRoll);
            return;
        }
        if(CanChangeToMoveRun())
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
        // 체공, 대기, 앉기, 고개들기, 점프, 구르기, 달리기
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
        if(CanChangeToSit())
        {
            m_machine.ChangeState(stSit);
            return;
        }
        if(CanChangeToHeadUp())
        {
            m_machine.ChangeState(stHeadUp);
            return;
        }
        if(CanChangeToJumpBasic())
        {
            m_machine.ChangeState(stJumpBasic);
            return;
        }
        if(CanChangeToRoll())
        {
            m_machine.ChangeState(stRoll);
            return;
        }
        if(CanChangeToMoveWalk())
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

    #endregion

    #region Implement State; stJumpAir

    #endregion

    #region Implement State; stJumpWall

    #endregion

    #region Implement State; stJumpDown

    #endregion

    #region Implement State; stWallSliding

    #endregion

    #region Implement State; stLedgeClimb

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
        // TODO: 아래점프, 위점프 구현
        if(CanChangeToRoll())
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
        // TODO: 위점프 구현
        if(CanChangeToRoll())
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

    #endregion

    #region Implement State; stDash

    #endregion

    #region Implement State; stTakeDown

    #endregion
}
