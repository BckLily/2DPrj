using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat_Move : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float HP;

    Vector2 dir;

    float contactDistance = 1f;
    float maxFollowingDistance = 10f;

    public CircleCollider2D circleCollider2D;
    Transform target;
    Rigidbody2D rigid;
    Animator anim;

    bool isFollow = false;

    void Start()
    {
        contactDistance = 1f;
        maxFollowingDistance = 10f;

        speed = 4f;
        HP = 3f;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
    }

    void Update()
    {
        followTarget();
    }

    void followTarget()
    {
        float distance = Vector2.Distance(transform.position, target.position);

        dir = target.position - transform.position;

        if (target != null)
        {
            isFollow = false;
            anim.SetBool("IsFollow", isFollow);
        }
        if (distance > contactDistance && distance < maxFollowingDistance)
        {
            isFollow = true;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            anim.SetBool("IsFollow", isFollow);
        }
        else if (distance <= contactDistance)
        {
            Vector3 normalDir = dir.normalized;
            isFollow = true;
            transform.position += Vector3.up * Time.deltaTime * 4;
            anim.SetBool("IsFollow", isFollow);
        }

    }


}
