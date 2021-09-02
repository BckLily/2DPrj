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
    bool isSpawn = true;
    bool isWall = false;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        anim.SetBool("IsSpawn", isSpawn);
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

        // 플레이어 벽점프

        WallJumping(movedir);



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

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGround = false;
    }


    IEnumerator CoDash()  // 무한 대쉬 방지 하기 위한 코루틴함수
    {
        while (true)
        {
            isDash = true;
            maxspeed *= 1.5f; // 대쉬 속도 = 일반 속도 2배

            yield return new WaitForSeconds(0.5f);
            isDash = false;
            maxspeed /= 1.5f; // 이동 속도 원상 복귀
            yield break;
        }
    }

    void WallJumping(float moveDir)
    {
        //Physics2D.OverlapCircle(Vector2., 0.1f, 8);
        //Debug.Log(coll2D.bounds.size);
        CapsuleCollider2D coll2D = GetComponent<CapsuleCollider2D>();
        RaycastHit2D hit = Physics2D.Raycast(coll2D.bounds.center, new Vector2(moveDir, 0), Mathf.Abs(moveDir * ((coll2D.bounds.size.x / 2) + 0.05f)), 1 << LayerMask.NameToLayer("WALL"));
        //
        if (hit && !isGround)
        {
            //Debug.Log("aosdfha;sdfkjk;zljcxv");
            isWall = !isWall;
            isJump = false;
            anim.SetBool("IsWall", isGround);
            isGround = !isGround;
            if (Input.GetButtonDown("Jump"))
            {
                isJump = !isJump;
                rigid.AddForce(Vector2.up * 4, ForceMode2D.Impulse);
                anim.SetBool("IsJump", isGround);
            }
        }

        //Debug.Log("size: " + coll2D.bounds.size);
        //Debug.Log("DIR: " + new Vector2(moveDir, 0));
        //Debug.Log("DIS: " + moveDir * ((coll2D.bounds.size.x / 2) + 0.05f));
        //Debug.Log("Layer: " + LayerMask.NameToLayer("WALL"));

        //if (hit.collider != null)
        //{
        //    Debug.Log(hit.collider.gameObject);
        //    Debug.Log("hit: " + hit.collider.name);
        //}
        //if (hit.collider == null)
        //{
        //    Debug.Log("12341234");
        //}

        //Debug.DrawRay(coll2D.bounds.center, new Vector2(moveDir * ((coll2D.bounds.size.x / 2) + 0.05f), 0), Color.red);
    }
}
