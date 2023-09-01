using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] BoxCollider2D CollLeg; // �ٸ��� ���� �ν��ϴ� �ݶ��̴�
    [SerializeField] private Rigidbody2D rigid;

    [SerializeField] private bool isGrouned; // ���� ���ִ��� 

    private float verticalVelocity = 0f;//�������� �ް��ִ� ��
    private float gravity = 9.81f;
    private float fallingLimit = -10.0f;

    void Start()
    {
        
    }

    
    void Update()
    {
        checkGround();
        checkGravity();
    }
    private void checkGround() //ĳ������ ����ġ���� Raycast�� �Ʒ��� ��Ƽ� �ٴ��� �Ǵ��ϴ� �Լ�
    {
        bool beforeGrouned = isGrouned;
        isGrouned = false;

        RaycastHit2D hit = Physics2D.BoxCast(CollLeg.bounds.center, CollLeg.bounds.size, 0f, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        if (hit)
        { 
            isGrouned = true;
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
            verticalVelocity = 0f;
        }

        rigid.velocity = new Vector2(rigid.velocity.x, verticalVelocity);
    }
}
