using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] BoxCollider2D CollLeg; // 다리로 땅을 인식하는 콜라이더
    [SerializeField] private Rigidbody2D rigid;

    [SerializeField] private bool isGrouned; // 땅에 서있는지 

    private float verticalVelocity = 0f;//수직으로 받고있는 힘
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
    private void checkGround() //캐릭터의 발위치에서 Raycast를 아래로 쏘아서 바닥을 판단하는 함수
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
            verticalVelocity = 0f;
        }

        rigid.velocity = new Vector2(rigid.velocity.x, verticalVelocity);
    }
}
