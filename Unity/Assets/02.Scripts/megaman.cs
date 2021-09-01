using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class megaman : MonoBehaviour
{
    public float maxspeed;
    public float jumpforce;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    bool isRun = false;
    bool isJump = false;
    bool isGround = true;
    bool isDash = false;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        //플레이어 점프
        if (Input.GetButtonDown("Jump") && isGround)
        {
            isJump = !isJump;
            isGround = !isGround;
            rigid.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
            anim.SetBool("IsJump", !isGround);
        }
        //플레이어 이동 감속
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //플레이어 이동
        float movedir = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(movedir) >= 0.01f)  // 절대값으로 movedir 값을 받는다
        {
            isRun = true;
            anim.SetBool("IsRun", isRun);
        }
        else
        {
            isRun = false;
            anim.SetBool("IsRun", isRun);
        }
        rigid.velocity = new Vector2(maxspeed * movedir, rigid.velocity.y);  // movedir 값에 따른 플레이어 이동속도

        // 플레이어 대쉬
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDash && isGround) // LeftShift 누르고 isDash가 true 이고 땅에 닿아 있을때 
        {                                                               // isGround를 쓰는 이유 = 점프중에 대쉬가 되지 않도록 하기 위함
            anim.SetTrigger("IsDash");
            StartCoroutine(CoDash());  // 무한 대쉬 방지 코루틴 실행
            rigid.velocity = new Vector2(maxspeed * movedir, rigid.velocity.y);
        }

        if (movedir == 1) // 오른쪽 MaxSpeed
        {
            //왼쪽 오른쪽 방향 전환
            spriteRenderer.flipX = false;
        }
        else if (movedir == -1) // 왼쪽 MaxSpeed
        {
            //왼쪽 오른쪽 방향 전환
            spriteRenderer.flipX = true;
        }

            /*rigid.velocity = new Vector2(maxspeed * (-1), rigid.velocity.y);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetTrigger("IsDash");
                rigid.velocity = new Vector2(maxspeed * (-2), rigid.velocity.y);
            }
            else
                anim.SetTrigger("IsDash");*/


    }
    void FixedUpdate()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)  // 무한 점프를 방지하기위한 콜라이더 충돌 함수
    {                                                       // 현재 땅과 벽의 구분이 없음
        if (!isGround)
        {
            isGround = !isGround;
            isJump = !isJump;
            anim.SetBool("IsJump", false);
        }
        
    }
    IEnumerator CoDash()  // 무한 대쉬 방지 하기 위한 코루틴함수
    {
        while (true)
        {
            isDash = true;
            maxspeed *= 2f; // 대쉬 속도 = 일반 속도 2배

            yield return new WaitForSeconds(0.5f);
            isDash = false;
            maxspeed /= 2f; // 이동 속도 원상 복귀
            yield break;
        }
    }
}
