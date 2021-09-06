using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InterfaceSet;
public class megaman : MonoBehaviour, IDamaged
{
    // 이동 속도와 점프 강도 변수
    public float movespeed;
    public float jumpforce;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    // 상태 불 변수
    bool isRun = false;
    bool isJump = false;
    bool isGround = true;
    bool isDash = false;
    bool isSpawn = true;
    bool isWall = false;
    bool isAttack = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        anim.SetBool("IsSpawn", isSpawn);
    }
    private void Update()
    {
        //플레이어 이동
        Playermove();
        //플레이어 점프
        Playerjump();
        //플레이어 대쉬
        PlayerDash();
        //플레이어 공격
        PlayerIdleAttack();
        //플레이어 벽점프
        //Walljumping(1.5f);
    }

    void Playermove()
    {
        float movedir = Input.GetAxisRaw("Horizontal");

        // 절대값으로 movedir 값을 받아서 달리기 애미매이션 발동
        if (Mathf.Abs(movedir) >= 0.01f)
        {
            isRun = true;
            anim.SetBool("IsRun", isRun);
        }
        else
        {
            isRun = false;
            anim.SetBool("IsRun", isRun);
        }
        // movrdir 값에 따른 플레이어 이동속도
        rigid.velocity = new Vector2(movespeed * movedir, rigid.velocity.y);

        // 왼쪽 오른쪽 방향 전환
        if (movedir == 1)
            spriteRenderer.flipX = false;
        else if (movedir == -1)
            spriteRenderer.flipX = true;

        // 정지시 플레이어 이동 감속
        if (Input.GetButtonUp("Horizontal"))
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
    }
    void Playerjump()
    {
        if (Input.GetButtonDown("Jump") && isGround)
        {
            isJump = true;
            isGround = false;
            rigid.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
            anim.SetBool("IsJump", isJump);
        }
    }
    void PlayerDash()
    {
        float movedir = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDash && isGround)
        {
            anim.SetTrigger("IsDash");
            // 무한 대쉬 방지 코루틴 함수 실행
            StartCoroutine(CoDash());
            rigid.velocity = new Vector2(movespeed * movedir, rigid.velocity.y);
        }
    }
    void PlayerIdleAttack()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(CoAttack());
        }
    }
    void PlayerRunAttack()
    {
        if (isRun == true && isAttack == true)
        {
            StartCoroutine(CoAttack());
        }
    }
    void PlayerJumpAttack()
    {
        if (isJump == true && isAttack == true)
        {
            StartCoroutine(CoAttack());
        }
    }
    void playerDashAttack()
    {
        if(isDash == true && isAttack ==true)
        {
            StartCoroutine(CoAttack());
        }
    }
    void Walljumping(float moveDir)
    {
        CapsuleCollider2D coll2D = GetComponent<CapsuleCollider2D>();
        Debug.Log(coll2D.size);
        RaycastHit2D hit = Physics2D.Raycast(coll2D.bounds.center, new Vector2(moveDir, 0),
                                             Mathf.Abs(moveDir * ((coll2D.bounds.size.x / 2) + 0.05f)),
                                             1 << LayerMask.NameToLayer("WALL"));

        if (hit)
        {
            Debug.Log("WALL_HIT");
            isWall = true;
            isJump = true;
            isGround = false;
            anim.SetBool("IsWall", isWall);
            if (Input.GetButtonDown("Jump"))
            {
                isJump = true;
                isGround = false;
                rigid.AddForce(Vector2.up * jumpforce * 0.75f, ForceMode2D.Impulse);
                anim.SetBool("IsJump", isJump);
            }
            if (!isRun)
            {
                isWall = false;
                anim.SetBool("IsWall", isWall);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("FLOOR"))
        {
            isGround = true;
            isJump = false;
            isWall = false;
            anim.SetBool("IsJump", isJump);

        }
        if (collision.gameObject.tag.Equals("WALL"))
        {
            isGround = false;
            isJump = false;
            isWall = true;
            anim.SetBool("IsJump", isJump);
            anim.SetBool("IsWall", isWall);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("WALL"))
        {
            isWall = false;
        }
    }
    // 무한 대쉬 방지용 코루틴 함수
    IEnumerator CoDash()
    {
        while (true)
        {
            isDash = true;
            movespeed *= 1.75f;

            yield return new WaitForSeconds(0.5f);
            isDash = false;
            movespeed /= 1.75f;
            yield break;
        }
    }
    IEnumerator CoAttack()
    {
        isAttack = true;
        anim.SetBool("IsAttack", isAttack);

        yield return new WaitForSeconds(0.5f);
        isAttack = false;
        anim.SetBool("IsAttack", isAttack);
        yield break;
    }

    public void Damaged(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        Debug.Log("____PLAYER GET DAMAGED____");

    }


}
