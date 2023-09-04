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
    private void checkGround() //ĳ������ ����ġ���� Raycast�� �Ʒ��� ��Ƽ� �ٴ��� �Ǵ��ϴ� �Լ�
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
        if (isGrouned == false)//���߿� ���ִ� ����
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

