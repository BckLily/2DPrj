using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBreath : MonoBehaviour
{
    // breath 오브젝트 transform
    Transform breathTr;

    SpriteRenderer breahthSpriteRenderer;
    BoxCollider2D boxCollider2D;
    // ice Breath Sprite
    public Sprite[] iceSprites;

    public Vector2 moveVector;
    float moveSpeed = 5f;



    // Start is called before the first frame update
    void Start()
    {
        breathTr = GetComponent<Transform>();
        breahthSpriteRenderer = GetComponent<SpriteRenderer>();

        boxCollider2D = GetComponent<BoxCollider2D>();

        StartCoroutine(CoSpriteChagne());
        Destroy(this.gameObject, 0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        breathTr.Translate(new Vector3(moveVector.x, moveVector.y, 0f) * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PLAYER"))
        {
            //Debug.Log("HIT PLAYER: ICE BREATH");

            Vector2 cloestPoint = collision.ClosestPoint(gameObject.GetComponent<BoxCollider2D>().bounds.center);

            Vector2 hitNormal = new Vector2(cloestPoint.x - boxCollider2D.bounds.center.x, cloestPoint.y - boxCollider2D.bounds.center.y).normalized;

            megaman megaman= collision.GetComponent<megaman>();
            //megaman.Damaged(0, cloestPoint, hitNormal);
        }

    }

    IEnumerator CoSpriteChagne()
    {
        int spriteNumber = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.075f);
            spriteNumber++;
            spriteNumber %= 6;

            breahthSpriteRenderer.sprite = iceSprites[spriteNumber];

        }
    }
}
