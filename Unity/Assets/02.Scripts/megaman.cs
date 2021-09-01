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
        //�÷��̾� ����
        if (Input.GetButtonDown("Jump") && isGround)
        {
            isJump = !isJump;
            isGround = !isGround;
            rigid.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
            anim.SetBool("IsJump", !isGround);
        }
        //�÷��̾� �̵� ����
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //�÷��̾� �̵�
        float movedir = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(movedir) >= 0.01f)  // ���밪���� movedir ���� �޴´�
        {
            isRun = true;
            anim.SetBool("IsRun", isRun);
        }
        else
        {
            isRun = false;
            anim.SetBool("IsRun", isRun);
        }
        rigid.velocity = new Vector2(maxspeed * movedir, rigid.velocity.y);  // movedir ���� ���� �÷��̾� �̵��ӵ�

        // �÷��̾� �뽬
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDash && isGround) // LeftShift ������ isDash�� true �̰� ���� ��� ������ 
        {                                                               // isGround�� ���� ���� = �����߿� �뽬�� ���� �ʵ��� �ϱ� ����
            anim.SetTrigger("IsDash");
            StartCoroutine(CoDash());  // ���� �뽬 ���� �ڷ�ƾ ����
            rigid.velocity = new Vector2(maxspeed * movedir, rigid.velocity.y);
        }

        if (movedir == 1) // ������ MaxSpeed
        {
            //���� ������ ���� ��ȯ
            spriteRenderer.flipX = false;
        }
        else if (movedir == -1) // ���� MaxSpeed
        {
            //���� ������ ���� ��ȯ
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
    private void OnCollisionEnter2D(Collision2D collision)  // ���� ������ �����ϱ����� �ݶ��̴� �浹 �Լ�
    {                                                       // ���� ���� ���� ������ ����
        if (!isGround)
        {
            isGround = !isGround;
            isJump = !isJump;
            anim.SetBool("IsJump", false);
        }
        
    }
    IEnumerator CoDash()  // ���� �뽬 ���� �ϱ� ���� �ڷ�ƾ�Լ�
    {
        while (true)
        {
            isDash = true;
            maxspeed *= 2f; // �뽬 �ӵ� = �Ϲ� �ӵ� 2��

            yield return new WaitForSeconds(0.5f);
            isDash = false;
            maxspeed /= 2f; // �̵� �ӵ� ���� ����
            yield break;
        }
    }
}
