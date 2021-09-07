using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundChangeTrigger : MonoBehaviour
{
    public int triggerIdx;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PLAYER"))
        {
            GameEvents.currrent.BackgroundChange(triggerIdx);
        }
    }


}
