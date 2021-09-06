using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public CircleCollider2D circleCollider2D;

    float damage = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.right * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BOSS"))
        {
            Vector2 cloestPoint = collision.ClosestPoint(gameObject.GetComponent<Collider2D>().bounds.center);

            Vector2 hitNormal = new Vector2(cloestPoint.x - circleCollider2D.bounds.center.x, cloestPoint.y - circleCollider2D.bounds.center.y).normalized;

            ChillPenguinAI penguin = collision.GetComponent<ChillPenguinAI>();
            penguin.Damaged(damage, cloestPoint, hitNormal);
        }


    }


}
