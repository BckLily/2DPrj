using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee_Move : MonoBehaviour
{
    public float speed;
    public int HP;
    Vector2 dir;

    public GameObject Bullet;

    [SerializeField] [Range(0f, 3f)] float contactDistance = 1f;
    float maxFollowingDistance = 4f;

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

        //폭탄 낙하시
        if (distance > contactDistance && distance < maxFollowingDistance)
        {
            dir = target.position - transform.position;
            Vector3 normalDir = dir.normalized;

            isShoot = true;
            //transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            anim.SetBool("IsShoot", isShoot);
        }
        //이동 시
        else
        {
            transform.Translate(new Vector2(-1, 0) * speed * Time.deltaTime);
            isShoot = false;
            anim.SetBool("IsShoot", isShoot);
        }
    }
    IEnumerator CoAttack()
    {
        yield return new WaitForSeconds(0.3f);
    }
}
