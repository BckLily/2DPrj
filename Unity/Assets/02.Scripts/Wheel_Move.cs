using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_Move : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float HP;

    Vector2 dir;

    float contactDistance;
    float maxFollowingDistance;

    public CircleCollider2D circleCollider2D;
    Transform target;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;

    bool isShoot;

    void Start()
    {
        speed = 4f;

        HP = 5f;
        
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        sprite = GetComponent<SpriteRenderer>();

        contactDistance = 1.5f;

        maxFollowingDistance = 3f;

        dir = Vector2.right;
    }


    void Update()
    {
        SearchTarget();
    }

    //void SpriteChange()
    //{
    //    if(dir.x<=0)
    //    {
    //        sprite.flipX = true;
    //    }
    //    else
    //    {
    //        sprite.flipX = false;
    //    }
    //}

    void SearchTarget()
    {
        float distance = Vector2.Distance(transform.position, target.position);

        //이동 모션
        if (distance > contactDistance && distance < maxFollowingDistance)
        {
            Vector3 normalDir = dir.normalized;
            transform.Translate(new Vector2(normalDir.x, 0) * speed * Time.deltaTime);
            isShoot = false;
            anim.SetBool("Self-destruction", isShoot);
        }
        //자폭 모션
        else if (distance <= contactDistance)
        {
            dir = target.position - transform.position;
            Vector3 normalDir = dir.normalized;
            isShoot = true;
            //transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            transform.Translate(new Vector2(0, 0)/* * speed * Time.deltaTime*/);
            anim.SetBool("Self-destruction", isShoot);
            Destroy(this.gameObject, 2f);
        }
    }

    //void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.tag == "WALL")
    //    {
    //        dir.x *= -1;
    //        Debug.Log("벽");
    //    }
    //    else if (collision.tag == "FLOOR")
    //    {
    //        dir.x *= -1;
    //    }
    //}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "WALL")
        {
            dir.x *= -1;
            Debug.Log("벽");
        }
    }
}
