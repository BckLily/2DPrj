using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
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
            ChillPenguinAI ai = collision.GetComponent<ChillPenguinAI>();
            ai.Damaged(0, Vector3.zero, Vector3.zero);
        }


    }


}
