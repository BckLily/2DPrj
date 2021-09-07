using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public int triggerId;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PLAYER"))
        {
            int idx = triggerId * 2;
            GameEvents.currrent.DoorTriggerEnter(idx);
            GameEvents.currrent.DoorTriggerEnter(idx + 1);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PLAYER"))
        {
            int idx = triggerId * 2;
            GameEvents.currrent.DoorTriggerExit(idx);
            GameEvents.currrent.DoorTriggerExit(idx + 1);
        }
    }



}
