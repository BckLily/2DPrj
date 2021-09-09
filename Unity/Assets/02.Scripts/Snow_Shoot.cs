using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snow_Shoot : MonoBehaviour
{
    public int HP;

    public GameObject Bullet;

    [SerializeField] [Range(0f, 3f)] float contactDistance = 1f;
    float maxFollowingDistance = 5f;

    public CircleCollider2D circleCollider2D;
    Transform target;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;

    bool isShoot;

    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        sprite = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        SearchTarget();
    }
    void SearchTarget()
    {
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > contactDistance && distance < maxFollowingDistance)
        {
            isShoot = true;
            //transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            rigid.velocity = Vector2.zero;
            anim.SetBool("IsShoot", isShoot);
        }
        else
        {
            rigid.velocity = Vector2.zero;
            isShoot = false;
            anim.SetBool("IsShoot", isShoot);
        }
    }
}
