using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dron_Move : MonoBehaviour
{

    public float speed;
    public int HP;

    [SerializeField] [Range(0f, 3f)] float contactDistance = 1f;
    float maxFollowingDistance = 5f;

    public CircleCollider2D circleCollider2D;
    Transform target;
    Rigidbody2D rigid;
    Animator anim;

    bool isSearch = false;

    
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

        if (distance > contactDistance && distance < maxFollowingDistance)
        {
            isSearch = true;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            anim.SetBool("IsSearch", isSearch);
        }
        else
        {
            rigid.velocity = Vector2.zero;
            isSearch = false;
            anim.SetBool("IsSearch", isSearch);
        }
    }
}
