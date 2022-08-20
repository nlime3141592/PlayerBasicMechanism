using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Entity : MonoBehaviour
{
    #region Entity Constants

    [Header("Entity Constants")]
    public float moveSpeed = 5.5f;
    public float maxFreeFallSpeed = 7.0f;

    #endregion

    #region Entity Variables

    protected Vector2 currentVelocity;
    protected Vector2 velocityWorkspace;

    protected float groundDetectingLength = 0.5f;
    protected float entityDetectingLength = 0.04f;
    protected RaycastHit2D detectedGround;
    protected bool isDetectedGround;
    protected bool isOnGround;
    protected Vector2 moveDirection;
    protected int lookingDirection;

    protected Rigidbody2D rigid { get; private set; }
    protected BoxCollider2D feetBox { get; private set; }
    protected Vector2 feetPosition;

    #endregion

    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        m_SetFeetBox();
    }

    #region Initializer

    private void m_SetFeetBox()
    {
        GameObject boxObj = GameObject.Find("FeetBox");

        if(boxObj != null && boxObj.transform.parent == transform)
            feetBox = boxObj.GetComponent<BoxCollider2D>();
    }

    #endregion

    #region Physics Utilities

    protected void SetVectorX(ref Vector2 vector, float x)
    {
        vector.Set(x, vector.y);
    }

    protected void SetVectorY(ref Vector2 vector, float y)
    {
        vector.Set(vector.x, y);
    }

    protected void SetVector(ref Vector2 vector, float x, float y)
    {
        vector.Set(x, y);
    }

    protected void SetVelocityX(float x)
    {
        velocityWorkspace.Set(x, currentVelocity.y);
        SetFinalVelocity();
    }

    protected void SetVelocityY(float y)
    {
        velocityWorkspace.Set(currentVelocity.x, y);
        SetFinalVelocity();
    }

    protected void SetVelocity(float x, float y)
    {
        velocityWorkspace.Set(x, y);
        SetFinalVelocity();
    }

    protected void SetFinalVelocity()
    {
        currentVelocity = velocityWorkspace;
        rigid.velocity = velocityWorkspace;
    }

    #endregion

    #region Physics Checker

    protected void CheckGround()
    {
        float posX = feetBox.bounds.min.x + feetBox.bounds.extents.x;
        float posY = feetBox.bounds.min.y;

        SetVector(ref feetPosition, posX, posY);

        // 1. Can player detect ground?
        detectedGround = Physics2D.Raycast(feetPosition, Vector2.down, groundDetectingLength, LayerInfo.groundMask);
        isDetectedGround = detectedGround;

        // 2. Does player hit ground?
        RaycastHit2D hitEntity;
        Vector2 dir = Vector2.one;

        if(isDetectedGround)
        {
            hitEntity = Physics2D.Raycast(detectedGround.point, Vector2.up, entityDetectingLength, LayerInfo.entityMask);
            isOnGround = hitEntity;

            dir = Vector2.Perpendicular(-detectedGround.normal).normalized;

            if(!isOnGround)
            {
                rigid.AddForce(Vector2.down * 80.0f, ForceMode2D.Force);
            }
        }
        else
        {
            isOnGround = false;
            dir = Vector2.right;
        }

        SetVector(ref moveDirection, dir.x, dir.y);
    }

    #endregion

    #region State Transition Input Checker

    protected void CheckLookingDirection(int xInput)
    {
        if(xInput != 0)
        {
            lookingDirection = xInput;
        }
    }

    #endregion

    #region Execute Object Actions

    protected virtual void FixedUpdate()
    {
        currentVelocity = rigid.velocity;

        CheckGround();
    }

    protected virtual void Logic_Move()
    {
        float angle = Vector2.Angle(Vector2.right, moveDirection) * Mathf.Deg2Rad;
        float tan = Mathf.Tan(angle);
        if(moveDirection.y < 0) tan = -tan;
        float x = moveSpeed * lookingDirection;
        float y = moveSpeed * lookingDirection * tan;

        SetVelocity(x, y);
    }

    #endregion

    #region State Transition Logics

    protected virtual void Update()
    {

    }

    #endregion
}