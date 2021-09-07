using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Bullet이 날아갈 방향
    public Vector2 dir = Vector2.right;

    float fireSpeed = 11.5f;

    public CircleCollider2D circleCollider2D;
    float damage = 5f;
    
    void Start()
    {
        Destroy(this.gameObject, 1f);
    }

    void Update()
    {
        transform.Translate(new Vector3(dir.x, dir.y, 0f) * fireSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BOSS"))
        {
            Vector2 cloestPoint = collision.ClosestPoint(gameObject.GetComponent<Collider2D>().bounds.center);

            Vector2 hitNormal = new Vector2(cloestPoint.x - circleCollider2D.bounds.center.x, cloestPoint.y - circleCollider2D.bounds.center.y).normalized;

            ChillPenguinAI penguin = collision.GetComponent<ChillPenguinAI>();
            penguin.Damaged(damage, cloestPoint, hitNormal);

            Destroy(this.gameObject);
        }
        if (collision.CompareTag("WALL") || collision.CompareTag("FLOOR"))
        {
            Destroy(this.gameObject);
        }


    }


}
