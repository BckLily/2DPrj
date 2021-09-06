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

    // Start is called before the first frame update
    void Start()
    {
        boxCollider2D.isTrigger = true;

    }

    // Update is called once per frame
    void Update()
    {
        DoorOpen();
    }

    public void DoorOpen()
    {
        Vector3 doorPos = DoorSpriteObj.transform.position;
        DoorSpriteObj.transform.position = Vector3.Lerp(doorPos, new Vector3(transform.position.x, transform.position.y + 2.5f, 0f), 1.5f * Time.deltaTime);
    }

    public void DoorClose()
    {
        Vector3 doorPos = DoorSpriteObj.transform.position;
        DoorSpriteObj.transform.position = Vector3.Lerp(doorPos, transform.position, 1.5f * Time.deltaTime);
        boxCollider2D.isTrigger = false;
    }


}
