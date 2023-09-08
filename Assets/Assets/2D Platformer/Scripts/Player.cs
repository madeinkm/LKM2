using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] BoxCollider2D CollLeg; // �ٸ��� ���� �ν��ϴ� �ݶ��̴�
    [SerializeField] private Rigidbody2D rigid;

    [SerializeField] private bool isGrouned; // ���� ���ִ���
    [SerializeField] private float groundRatio = 0.1f;                                        

    private float verticalVelocity = 0f;//�������� �ް��ִ� ��
    private float gravity = 9.81f;
    private float fallingLimit = -10.0f;
    private Vector2 moveDir;
    private Animator anim;

    [SerializeField] private float moveSpeed = 1.0f;

    private bool isJump = false;
    [SerializeField] private float jumpForce = 5.0f;
    private bool _doJump = false;
    
    private bool doJump // ������ property 
    {
        get
        {
            return _doJump;
        }
        //get => _doJump; // �� get �κа� ������ �ǹ� 
        set
        {
            anim.SetBool("Jump", value);
            _doJump = value;
        }
    }

    [Header("������")]
    [SerializeField]private bool wallJump = false; // ���� ���� �� �� �ִ��� Ȯ���ϴ� ����
    private bool doWallJump = false;
    private bool doWallJumpTimer = false;
    private float wallJumpTimer = 0.0f;
    private float wallJumpTime = 0.3f;

    [Header("�뽬")]
    private bool dash = false;
    private float dashTimer = 0.0f;
    [SerializeField] private TrailRenderer dashEffect; // �ڿ� ����ó�� �׸��� ����
    [SerializeField] private float dashSpeed = 20.0f;
    [SerializeField] private float dashTime = 0.2f;

    [Header("������ô")]
    [SerializeField] private Transform trsHands;

    void Start()
    {
        anim = GetComponent<Animator>();
        if(dashEffect != null ) 
        {
        dashEffect.enabled = false;
        }
    }
        
    void Update()
    {
        checkGround();
        checkGravity();

        moving();
        jumping();
        doAnim();

        checkDoJumpwallTimer();
        checkDash();
        checkAim();
    }
    private void checkGround() //ĳ������ ����ġ���� Raycast�� �Ʒ��� ��Ƽ� �ٴ��� �Ǵ��ϴ� �Լ�
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
        if (dash == true)
        {
            return;
        }

        if (doWallJump == true)
        {
            doWallJump = false;

            Vector2 dir = rigid.velocity;
            dir.x *= -1;
            rigid.velocity = dir;

            verticalVelocity = jumpForce;

            doWallJumpTimer = true;
        }

        else if (isGrouned == false)//���߿� ���ִ� ����
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
        if(doWallJumpTimer == true || dash == true)
        {
            return;
        }

        moveDir.x = Input.GetAxisRaw("Horizontal");//��,�� ux������ ���ֻ���ϴ� �¿�Ű a,d , �¿� ����Ű��� �ϴ� �ڵ�
                                                   //if (Input.GetKey(KeyCode.A))�ε� ����
        rigid.velocity = new Vector2(moveDir.x * moveSpeed, rigid.velocity.y);
    }
    private void doAnim()
    {
        anim.SetInteger("VelocityX", (int)moveDir.x); // �¿� �ִϸ��̼� ����

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
        if (isGrouned == false)// ���߿� ���ִ� ����
        {
            //Mathf.Abs(moveDir.x) !=0.0f  // moveDir.x != 0 ���� ǥ��
            if (Input.GetKeyDown(KeyCode.Space) && wallJump == true && moveDir.x != 0) // Abs�� ���밪
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
    private void checkDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && dash == false)
        {
            dash = true;
            verticalVelocity = 0.0f; // ������ �ȶ�����

            bool isRight = transform.localScale.x == -1.0f;

            rigid.velocity = new Vector2(dashSpeed * transform.localScale.x * -1, 0.0f);
            if (dashEffect != null)
            {
                dashEffect.enabled = true;
            }

        }
        else if (dash == true)
        {
            dashTimer += Time.deltaTime;
            if(dashTimer >= dashTime) 
            {
                dashTimer = 0.0f;
                if (dashEffect != null)
                {
                    dashEffect.enabled = false;
                    dashEffect.Clear();
                }
                dash = false;
            }
        }
    }
    private void checkAim()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);//��ũ������ ���콺�� ��� ��ġ�� �ִ����� data

        Vector3 dir = mouseWorldPos - transform.position;

        float angle = Quaternion.FromToRotation(dir.x > 0 ? Vector3.right : Vector3.left, dir).eulerAngles.z;
                                                    //���� ��� �ϴ� �� ���ʹϾ��� ���Ϸ��� �Ἥ 4�������� -> 3�������� ����
        trsHands.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -angle);
    }

    public void TriggerEnter(eHitType _type, Collider2D _coll)
    {
        switch (_type) // if�� ���� ������ if���� �����ٴ°� �ƴ�
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
        switch (_type) // if�� ���� ������ if���� �����ٴ°� �ƴ�
        {
            case eHitType.WallCheck:
                wallJump = false;
                break;
            case eHitType.ItemCheck:
                break;
        }
    }
}

