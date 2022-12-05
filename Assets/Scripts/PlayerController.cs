using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using Cinemachine;

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
    public CinemachineVirtualCamera follwingCamera;

    public static PlayerController instance;

    private bool onGround = true;
    private bool jumped = false;
    private bool canMoveInAir = true;

    void Start()
    {
        instance = this;
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
        if (canMoveInAir) {
            rigid.velocity = new Vector2(moveDir * speed, rigid.velocity.y);
            anim.SetBool("isMoving", moveDir != 0);

            if (moveDir != 0)
            {
                spriteRender.flipX = moveDir == -1;
            }
        }
        else if (rigid.velocity.y <= 0.1)
        {
            canMoveInAir = true;
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
            canMoveInAir = true;
        }
        else if (!grounded && onGround)
        {
            anim.SetTrigger(jumped ? "Jump" : "Falling");
            jumped = false;
        }
        onGround = grounded;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(checkPos.position, Vector3.back, radius);
    }
#endif
    public void KillPlayer()
    {
        follwingCamera.Follow = null;
        EndGameCanvasController.instance.EndGame();
    }

    public void PushFromExplosion(Vector2 pushAmount)
    {
        canMoveInAir = false;
        rigid.velocity = pushAmount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Damage Area")
        {
            KillPlayer();
        }
    }
}
