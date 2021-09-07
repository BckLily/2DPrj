using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCtrl : MonoBehaviour
{
    // 플레이어의 위치
    public Transform playerTr;
    Vector3 prePlayerPos;
    // 배경화면의 Material
    Material mat;
    // 스프라이트 렌더러
    SpriteRenderer spriteRenderer;

    public Sprite[] bgSprites;

    // 배경화면의 Offset
    Vector2 offset;
    Vector2 moveDir;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mat = GetComponent<SpriteRenderer>().material;
        prePlayerPos = new Vector3();
        prePlayerPos = playerTr.position;

        GameEvents.currrent.onBackgroundChange += BgSpriteChange;

    }

    private void FixedUpdate()
    {
        prePlayerPos = playerTr.position;
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = playerTr.position - prePlayerPos;
        //Debug.Log(moveDir);

        offset.x += moveDir.x * 4.5f * Time.deltaTime;

        if(offset.x >= 1)
        {
            offset.x -= 1;
        }
        else if (offset.x <= -1)
        {
            offset.x += 1;
        }

        mat.mainTextureOffset = offset;

    }

    private void LateUpdate()
    {

    }

    public void BgSpriteChange(int idx)
    {
        Debug.Log("____BG Sprite Change____");
        spriteRenderer.sprite = bgSprites[idx];
    }

}
