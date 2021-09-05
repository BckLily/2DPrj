using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public BoxCollider2D boxCollider2D;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.right * Time.deltaTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("BOSS"))
        {
            Vector2 cloestPoint = collision.ClosestPoint(gameObject.GetComponent<Collider2D>().bounds.center);

            Vector2 hitNormal = new Vector2(cloestPoint.x - boxCollider2D.bounds.center.x, cloestPoint.y - boxCollider2D.bounds.center.y).normalized;

            ChillPenguinAI penguin = collision.GetComponent<ChillPenguinAI>();
            penguin.Damaged(0, cloestPoint, hitNormal);
        }


    }


}
