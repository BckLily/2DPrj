using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDieTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PLAYER"))
        {
            megaman _megaman = collision.GetComponent<megaman>();
            _megaman.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
        }
    }


}
