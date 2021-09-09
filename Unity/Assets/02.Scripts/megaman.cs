using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InterfaceSet;
public class megaman : MonoBehaviour, IDamaged
{
    // 이동 속도와 점프 강도 변수
    float movespeed = 5f;
    float jumpforce = 9.5f;
    float pressTime;
    float delay = 0f;
    float movedir;
    float damage = 10;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D coll2D;
    // 상태 불 변수
    bool isRun = false;
    bool isJump = false;
    bool isGround = true;
    bool isDash = false;
    bool isSpawn = true;
    bool isWall = false;
    bool isAttack = false;
    bool isDamaged = false;


    bool flipX = false;

    public GameObject Bulletobj;
    public GameObject BigBulletobj;
    GameObject Currbullet;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll2D = GetComponent<CapsuleCollider2D>();
    }
    private void Start()
    {
        anim.SetBool("IsSpawn", isSpawn);
        // 첫발은 입력값과 동시에 발사됨
        pressTime = delay;
        movedir = 0f;
    }
    public void Damaged(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {

        isDamaged = true;
        anim.SetBool("IsDamaged", isDamaged);
        new WaitForSeconds(1f);
        isDamaged = false;
        anim.SetBool("IsDamaged", isDamaged);
    }
    private void Update()
    {
        NowGround();
        //플레이어 이동
        Playermove();
        //플레이어 점프 & 벽점프
        PlayerJump();
        //플레이어 대쉬
        PlayerDash();
        //플레이어 공격
        PlayerAttack();
    }

    void Playermove()
    {
        movedir = Input.GetAxisRaw("Horizontal");

        // 절대값으로 movedir 값을 받아서 달리기 애미매이션 발동
        if (Mathf.Abs(movedir) >= 0.01f)
        {
            isRun = true;
            anim.SetBool("IsRun", isRun);
            anim.SetBool("IsGround", isGround);
        }
        else
        {
            isRun = false;
            anim.SetBool("IsRun", isRun);
            anim.SetBool("IsGround", isGround);
        }
        // movrdir 값에 따른 플레이어 이동속도
        rigid.velocity = new Vector2(movespeed * movedir, rigid.velocity.y);

        // 왼쪽 오른쪽 방향 전환
        if (movedir == 1)
        {
            flipX = false;
            spriteRenderer.flipX = flipX;
        }
        else if (movedir == -1)
        {
            flipX = true;
            spriteRenderer.flipX = flipX;
        }

        // 정지시 플레이어 이동 감속
        if (Input.GetButtonUp("Horizontal"))
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
    }
    void PlayerDash()
    {
        movedir = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDash && isGround)
        {
            anim.SetTrigger("IsDash");
            anim.SetBool("IsGround", isGround);
            // 무한 대쉬 방지 코루틴 함수 실행
            StartCoroutine(CoDash());
            rigid.velocity = new Vector2(movespeed * movedir, rigid.velocity.y);
        }
    }
    void PlayerAttack()
    {
        // 공격키 K
        if (Input.GetKey(KeyCode.K))
        {
            // 공격키를 누른 시간 체크
            pressTime += Time.deltaTime;
            // 누른시간이 3초 이상
            if (pressTime > 1.5f)
            {
                // 현재 생성되는 총알을 차징샷으로 변경
                Currbullet = BigBulletobj;
                this.transform.Find("ChargingAura").gameObject.SetActive(true);
            }
            // 누른시간이 3초 미만일시
            else
            {
                // 현재 생성되는 총알을 기본으로
                Currbullet = Bulletobj;
            }
        }
        // 공격키에 손을 떘을 때 총알이 발사
        if (Input.GetKeyUp(KeyCode.K))
        {
            this.transform.Find("ChargingAura").gameObject.SetActive(false);
            // 현재 총알의 상태에 따라 총알이 생성됨
            GameObject bullet = Instantiate(Currbullet, this.transform.position, Quaternion.identity);
            //플레이어 방향과 벽에 붙어있는지 여부에 따른 총알 발사 방향 조정
            if (flipX == true)
            {
                bullet.GetComponent<Bullet>().dir = Vector2.left;
                if (isWall == true)
                {
                    bullet.GetComponent<Bullet>().dir = Vector2.right;
                }
            }
            else
            {
                bullet.GetComponent<Bullet>().dir = Vector2.right;
                if (isWall == true)
                {
                    bullet.GetComponent<Bullet>().dir = Vector2.left;
                }
            }
            StartCoroutine(CoAttack());
            // 연속 공격이 가능하게끔 누른시간 초기화
            pressTime = delay;
        }
    }
    void PlayerJump()
    {
        // x 축으로 1.5f 만큼의 레이캐스트를 쏴서 벽을 인지한다.
        RaycastHit2D hit = Physics2D.Raycast(coll2D.bounds.center, new Vector2(1f, 0) * movedir,
                                             Mathf.Abs(1.5f * ((coll2D.bounds.size.x / 2) + 0.01f)),
                                             1 << LayerMask.NameToLayer("WALL"));
        //Debug.DrawRay(coll2D.bounds.center,new Vector2(1.5f, 0),Color.red, ((coll2D.bounds.size.y / 2) + 0.05f));
        // 땅에서 점프키를 누를 때
        if (Input.GetButtonDown("Jump"))
        {
            // 레이케스트로 벽인 인지 했을 때
            if (isWall == true && isGround == false)
            {
                Debug.Log("WALL HIT");
                if (isWall == true)
                {
                    rigid.AddForce(new Vector2(-movedir * 3f, transform.up.y) * jumpforce, ForceMode2D.Impulse);
                }
                isWall = false;
                isJump = true;
                // 벽에 붙은 상태로 점프를 했을 때
                // 벽에 붙어서 방향키로 이동 안 했을때 (방향키 손 땠을때)
                if (isRun == false && isGround == true)
                {
                    isWall = false;
                }
                anim.SetBool("IsJump", isJump);
                anim.SetBool("IsWall", isWall);
            }
            else if (isWall == false && isGround == true)
            {
                Debug.Log("WALL NOT HIT");
                isJump = true;
                rigid.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
            }
            anim.SetBool("IsWall", isWall);
            anim.SetBool("IsJump", isJump);
        }

    }

    void NowGround()
    {
        Debug.DrawRay(coll2D.bounds.center, Vector2.down, Color.red, ((coll2D.bounds.size.y / 2) + 0.075f));

        if (Physics2D.Raycast(coll2D.bounds.center, Vector2.down, ((coll2D.bounds.size.y / 2) + 0.075f), 1 << LayerMask.NameToLayer("FLOOR")))
        {
            isGround = true;
            anim.SetBool("IsGround", isGround);
            isWall = false;
            anim.SetBool("IsWall", isWall);
            //Debug.Log("Is Ground: " + isGround);
        }
        else
        {
            isGround = false;
            anim.SetBool("IsGround", isGround);
            //Debug.Log("Is Ground: " + isGround);
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
            anim.SetBool("IsGround", isGround);
            anim.SetBool("IsWall", isWall);
        }
        if (collision.gameObject.tag.Equals("WALL"))
        {
            isGround = false;
            isJump = false;
            isWall = true;
            anim.SetBool("IsJump", isJump);
            anim.SetBool("IsWall", isWall);
            anim.SetBool("IsGround", isGround);
        }
        if (collision.gameObject.CompareTag("BOSS"))
        {
            
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("WALL"))
        {
            if (Mathf.Abs(movedir) > 0)
            {
                isWall = true;
                anim.SetBool("IsWall", isWall);
            }
            else
            {
                isWall = false;
                anim.SetBool("IsWall", isWall);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("WALL"))
        {
            isWall = false;
            anim.SetBool("IsWall", isWall);
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

}
