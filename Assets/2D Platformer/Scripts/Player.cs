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
    [SerializeField] private float jumpForce = 5.0f;
    private bool _doJump = false;
    private bool doJump // 변수의 property 
    {
        get
        {
            return _doJump;
        }
        //get => _doJump; // 위 get 부분과 동일한 의미 
        set
        {
            anim.SetBool("Jump", value);
            _doJump = value;
        }
    }

    [Header("벽점프")]
    [SerializeField]private bool wallJump = false; // 내가 벽을 찰 수 있는지 확인하는 변수
    private bool doWallJump = false;
    private bool doWallJumpTimer = false;
    private float wallJumpTimer = 0.0f;
    private float wallJumpTime = 0.3f;

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

        checkDoJumpwallTimer();
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
                if (beforeGrouned == false && doJump == true)
                {
                    doJump = false;
                }

                isGrouned = true;
            }
        }        
    }
    private void checkGravity()
    {
        if (doWallJump == true)
        {
            doWallJump = false;

            Vector2 dir = rigid.velocity;
            dir.x *= -1;
            rigid.velocity = dir;

            verticalVelocity = jumpForce;

            doWallJumpTimer = true;
        }

        else if (isGrouned == false)//공중에 떠있는 상태
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
                doJump = true;
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
        if(doWallJumpTimer == true)
        {
            return;
        }

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
        if (isGrouned == false)// 공중에 떠있는 상태
        {
            //Mathf.Abs(moveDir.x) !=0.0f  // moveDir.x != 0 같은 표현
            if (Input.GetKeyDown(KeyCode.Space) && wallJump == true && moveDir.x != 0) // Abs는 절대값
            {
                doWallJump = true;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJump = true;
        }
    }

    private void checkDoJumpwallTimer()
    {
        if (doWallJumpTimer == true)
        {
            wallJumpTimer += Time.deltaTime;
            if (wallJumpTimer >= wallJumpTime)
            {
                wallJumpTimer = 0.0f;
                doWallJumpTimer = false;
            }
        }
    }

    public void TriggerEnter(eHitType _type, Collider2D _coll)
    {
        switch (_type) // if문 보다 빠르다 if문이 느리다는건 아님
        {
            case eHitType.WallCheck:
                wallJump = true;
                break;
            case eHitType.ItemCheck:
                break;
        }
    }
    public void TriggerExit(eHitType _type, Collider2D _coll)
    {
        switch (_type) // if문 보다 빠르다 if문이 느리다는건 아님
        {
            case eHitType.WallCheck:
                wallJump = false;
                break;
            case eHitType.ItemCheck:
                break;
        }
    }
}

