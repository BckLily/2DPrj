using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ostrich_Move : MonoBehaviour
{
    public int HP;
    Vector2 dir;

    public GameObject Bullet;

    [SerializeField] [Range(0f, 3f)] float contactDistance = 1f;
    float maxFollowingDistance = 5f;

    public CircleCollider2D circleCollider2D;
    Transform target;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;

    bool isShoot;
    bool IsSearch;

    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
    }

    void Update()
    {
        SearchTarget();
    }
    void SearchTarget()
    {
        float distance = Vector2.Distance(transform.position, target.position);

        //총알 발사 시
        if (distance > contactDistance && distance < maxFollowingDistance)
        {
            dir = target.position - transform.position;
            Vector3 normalDir = dir.normalized;

            isShoot = true;
            IsSearch = false;
            //transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            rigid.velocity = Vector2.zero;
            anim.SetBool("IsSearch", IsSearch);
            anim.SetBool("IsShoot", isShoot);
        }
        //대기 상태일때
        else
        {
            rigid.velocity = Vector2.zero;
            isShoot = true;
            IsSearch = false;
            anim.SetBool("IsSearch", IsSearch);
            anim.SetBool("IsShoot", isShoot);
        }
    }

}
