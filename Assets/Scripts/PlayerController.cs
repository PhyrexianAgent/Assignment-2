using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Move Values")]
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpHeight = 5;

    [Header("Ground Check Values")]
    [SerializeField] private float radius;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask platformMask;
    [SerializeField] private Transform checkPos;

    [Header("Component References")]
    public Rigidbody2D rigid;
    public Animator anim;
    public SpriteRenderer spriteRender;

    private bool onGround = true;
    private bool jumped = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TestForKeys();
        TestForGrounded();
    }

    private void FixedUpdate()
    {
        int moveDir = Convert.ToInt32(Input.GetKey(KeyCode.D)) - Convert.ToInt32(Input.GetKey(KeyCode.A));
        rigid.velocity = new Vector2(moveDir * speed, rigid.velocity.y);
        anim.SetBool("isMoving", moveDir != 0);
        
        if (moveDir != 0)
        {
            spriteRender.flipX = moveDir == -1;
        }
    }

    private void TestForKeys()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && onGround)
        {
            rigid.velocity += new Vector2(0, jumpHeight);
            jumped = true;
            //anim.SetTrigger("Jump");
        }
    }

    private void TestForGrounded()
    {
        bool groundedGround = Physics2D.OverlapCircle(checkPos.position, radius, groundMask);
        bool groundedPlayform = Physics2D.OverlapCircle(checkPos.position, radius, platformMask);
        bool grounded = groundedGround || groundedPlayform;
        if (grounded && !onGround)
        {
            anim.SetTrigger("Landed");
        }
        else if (!grounded && onGround)
        {
            anim.SetTrigger(jumped ? "Jump" : "Falling");
            jumped = false;
        }
        onGround = grounded;
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        //Handles.draw
        /*Gizmos.color = Color.green;
        Vector2 boxPos = (Vector2)transform.position + rectPosition;
        //Gizmos.DrawCube(transform.position + (Vector3)rectPosition, rectVals);
        Gizmos.DrawLine(boxPos + rectVals, boxPos + new Vector2(0, rectVals.y));
        Gizmos.DrawLine(boxPos, boxPos + new Vector2(0, rectVals.y));
        Gizmos.DrawLine(boxPos, boxPos + new Vector2(rectVals.x, 0));
        Gizmos.DrawLine(boxPos + rectVals, boxPos + new Vector2(rectVals.x, 0));*/
        Handles.DrawWireDisc(checkPos.position, Vector3.back, radius);
    }
}
