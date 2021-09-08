using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBall : MonoBehaviour
{
    Transform ballTr;
    BoxCollider2D boxCollider2D;

    public GameObject breakEffect;

    public Vector2 moveVector;

    float moveSpeed;
    float damage;

    // Start is called before the first frame update
    void Start()
    {
        ballTr = GetComponent<Transform>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        moveSpeed = 3f;
        damage = 25f;

        Destroy(this.gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        ballTr.Translate(new Vector3(moveVector.x, moveVector.y, 0f) * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WALL"))
        {
            Destroy(this.gameObject);
        }
        else if (collision.CompareTag("PLAYER"))
        {
            //Debug.Log("HIT PLAYER ICE BALL");
            // 충돌한 물체와 현재 물체의 Collider에서 가장 가까운 지점 확인
            Vector2 cloestPoint = collision.ClosestPoint(gameObject.GetComponent<Collider2D>().bounds.center);
            // 충돌한 지점의 벡터 설정.
            // 가까운 지점에서 현 위치를 빼면 방향이 설정된다.
            Vector2 hitNormal = new Vector2(cloestPoint.x - boxCollider2D.bounds.center.x, cloestPoint.y - boxCollider2D.bounds.center.y).normalized;

            megaman megaman = collision.GetComponent<megaman>();
            megaman.Damaged(damage, cloestPoint, hitNormal);

            Destroy(this.gameObject);
        }
        else if (collision.CompareTag("SNOWMAN"))
        {
            //Debug.Log("HIT SNOW MAN ICE BALL");
            SnowMan snowMan = collision.GetComponent<SnowMan>();
            snowMan.Damaged(damage, Vector3.zero, Vector3.zero);


            Vector2 cloestPoint = collision.ClosestPoint(gameObject.GetComponent<Collider2D>().bounds.center);
            Vector2 hitNormal = new Vector2(cloestPoint.x - boxCollider2D.bounds.center.x, cloestPoint.y - boxCollider2D.bounds.center.y).normalized;

            GameObject effect = Instantiate(breakEffect, transform.position, Quaternion.Euler(hitNormal));
            Destroy(effect, 0.5f);

            Destroy(this.gameObject);
        }
    }

}
