using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit_Move : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float HP;
    Vector2 dir;

    float contactDistance;
    float maxFollowingDistance;

    float delayTime;
    float delay;

    public GameObject bullet;
    public CircleCollider2D circleCollider2D;
    Transform target;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;

    bool isSearch = false;


    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        sprite = GetComponent<SpriteRenderer>();

        speed = 5f;
        HP = 10f;

        contactDistance = 2.5f;
        maxFollowingDistance = 3f;

        delayTime = 2f;
        delay = 0f;

        dir = Vector2.left;

    }


    void Update()
    {
        SearchTarget();
        SpriteChange();
    }

    void SpriteChange()
    {
        if (dir.x <= 0)
        {
            sprite.flipX = false;
        }
        else
        {
            sprite.flipX = true;
        }
    }

    void SearchTarget()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        //Debug.Log("Distance: " + distance);
        dir = target.position - transform.position;

        // 추적 중일 때
        if (distance > contactDistance && distance < maxFollowingDistance)
        {
            Vector3 normalDir = dir.normalized;

            //transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            isSearch = true;
            anim.SetBool("IsShoot", isSearch);
        }
        //공격 범위에 있을때
        else if (distance <= contactDistance)
        {
            //Debug.Log("____asdf____");
            delay += Time.deltaTime;

            if (delayTime <= delay)
            {
                delay -= delayTime;
                GameObject Rb = Instantiate(bullet, new Vector3(transform.position.x + 0.4f, transform.position.y + 0.5f, 0), Quaternion.identity);
                if (sprite.flipX == true)
                {
                    Rb.GetComponent<Rabbit_Bullet>().dir = Vector2.right;
                }
                else
                {
                    Rb.GetComponent<Rabbit_Bullet>().dir = Vector2.left;
                }

            }

        }
        // 대기 상태일 때
        else
        {
            Vector3 normalDir = dir.normalized;

            //rigid.velocity = Vector2.zero;
            isSearch = false;
            anim.SetBool("IsShoot", isSearch);
            transform.Translate(new Vector2(normalDir.x, 3f).normalized * Time.deltaTime * speed);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "WALL")
        {
            dir.x *= -1;
            Debug.Log("토끼 벽");
        }
    }
}
