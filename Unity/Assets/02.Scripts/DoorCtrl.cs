using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCtrl : MonoBehaviour
{
    public GameObject DoorSpriteObj;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider2D;

    // 이벤트가 발생하면 문이 열린다.
    // 이벤트가 발생하면 문이 닫힌다.

    public int doorId;

    IEnumerator IDoorOpen;
    IEnumerator IDoorClose;


    // Start is called before the first frame update
    void Start()
    {
        GameEvents.currrent.onDoorTriggerEnter += OnDoorOpen;
        GameEvents.currrent.onDoorTriggerExit += OnDoorClose;

        //boxCollider2D.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        //DoorOpen();
    }

    private void OnDestroy()
    {
        GameEvents.currrent.onDoorTriggerEnter -= OnDoorOpen;
        GameEvents.currrent.onDoorTriggerExit -= OnDoorClose;
    }

    public void OnDoorOpen(int id)
    {
        if (id == this.doorId)
        {
            if (IDoorClose != null) { StopCoroutine(IDoorClose); IDoorClose = null; }

            if (IDoorOpen == null)
            {
                IDoorOpen = CoDoorOpen();
                StartCoroutine(IDoorOpen);
            }
        }
    }

    IEnumerator CoDoorOpen()
    {
        float loopTime = 0f;

        while (loopTime <= 8f)
        {

            Vector3 doorPos = DoorSpriteObj.transform.position;

            if (doorId % 2 == 0)
            {
                DoorSpriteObj.transform.position = Vector3.Lerp(doorPos, new Vector3(transform.position.x, transform.position.y + 2.5f, 0f), 1.5f * Time.deltaTime);
            }
            else
            {
                DoorSpriteObj.transform.position = Vector3.Lerp(doorPos, new Vector3(transform.position.x, transform.position.y - 2.5f, 0f), 1.5f * Time.deltaTime);
            }

            loopTime += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        boxCollider2D.isTrigger = true;

        IDoorOpen = null;
        yield break;

    }

    public void OnDoorClose(int id)
    {
        if (id == this.doorId)
        {
            if (IDoorOpen != null) { StopCoroutine(IDoorOpen); IDoorOpen = null; }

            if (IDoorClose == null)
            {
                IDoorClose = CoDoorClose();
                StartCoroutine(IDoorClose);
            }
        }
    }


    IEnumerator CoDoorClose()
    {
        float loopTime = 0f;
        boxCollider2D.isTrigger = false;
        while (loopTime <= 8f)
        {
            Vector3 doorPos = DoorSpriteObj.transform.position;
            DoorSpriteObj.transform.position = Vector3.Lerp(doorPos, transform.position, 1.5f * Time.deltaTime);

            yield return new WaitForSeconds(0.01f);
        }

        IDoorClose = null;
        yield break;
    }


}
