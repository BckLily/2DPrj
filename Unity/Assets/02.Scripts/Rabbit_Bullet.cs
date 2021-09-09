using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit_Bullet : MonoBehaviour
{
    public int Damage;

    SpriteRenderer sprite;
    Animator anim;
    Rigidbody2D rigid;

    public Vector2 dir;

    private float speed;

    void Start()
    {
        anim = GetComponent<Animator>();
        speed = 5f;

        Destroy(this.gameObject, 3.0f);
    }

    void Update()
    {
        transform.Translate(new Vector3(dir.x, 0, 0) * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PLAYER")
        {
            Destroy(this.gameObject);
            Debug.Log("뻥");
        }
        if (collision.tag == "WALL")
        {
            Destroy(this.gameObject);
        }
    }

}
