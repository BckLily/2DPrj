using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee_Boom : MonoBehaviour
{
    public int Damage;


    SpriteRenderer sprite;
    Animator anim;


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "FLOOR")
        {
            Destroy(this.gameObject, 3f);
        }
    }
}
