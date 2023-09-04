using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] BoxCollider2D CollLeg; // 다리로 땅을 인식하는 콜라이더
    [SerializeField] private Rigidbody2D rigid;

    [SerializeField] private bool isGrouned; // 땅에 서있는지
    [SerializeField] private float groundRatio = 0.1f;                                        

    private float verticalVelocity = 0f;//수직으로 받고있는 힘
    private float gravity = 9.81f;
    private float fallingLimit = -10.0f;
    private Vector2 moveDir;
    private Animator anim;

    [SerializeField] private float moveSpeed = 1.0f;

    private bool isJump = false;
    [SerializeField] private float jumpForce = 5f;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
        
    void Update()
    {
        checkGround();
        checkGravity();

        moving();
        jumping();
        doAnim();
    }
    private void checkGround() //캐릭터의 발위치에서 Raycast를 아래로 쏘아서 바닥을 판단하는 함수
    {
        bool beforeGrouned = isGrouned;
        isGrouned = false;

        if(verticalVelocity <= 0f)
        {
            RaycastHit2D hit = Physics2D.BoxCast(CollLeg.bounds.center, CollLeg.bounds.size, 0f, Vector2.down, groundRatio, LayerMask.GetMask("Ground"));
            
            if (hit)
            {
                isGrouned = true;
            }
        }        
    }
    private void checkGravity()
    {
        if (isGrouned == false)//공중에 떠있는 상태
        {
            verticalVelocity -= gravity * Time.deltaTime;
            if (verticalVelocity < fallingLimit)
            {
                verticalVelocity = fallingLimit;
            }
        }
        else
        {
            if (isJump == true)
            {
                isJump = false;
                verticalVelocity = jumpForce;
            }
            else
            {
                verticalVelocity = 0f;
            }            
        }

        rigid.velocity = new Vector2(rigid.velocity.x, verticalVelocity);
    }
    private void moving()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");//좌,우 ux적으로 자주사용하는 좌우키 a,d , 좌우 방향키사용 하는 코드
                                                   //if (Input.GetKey(KeyCode.A))로도 가능
       rigid.velocity = new Vector2(moveDir.x * moveSpeed, rigid.velocity.y);
    }
    private void doAnim()
    {
        anim.SetInteger("VelocityX", (int)moveDir.x); // 좌우 애니매이션 설정

        if(moveDir.x > 0f && transform.localScale.x != -1.0f)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else if (moveDir.x < 0f && transform.localScale.x != 1.0f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        if (isJump == true)
        {
            anim.SetTrigger("Jump");
        }
    }
    private void jumping()
    {
        if (isGrouned == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJump = true;
        }
    }
}

