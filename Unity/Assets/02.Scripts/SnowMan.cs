using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InterfaceSet;


public class SnowMan : MonoBehaviour, IDamaged
{
    public Sprite[] snowManSprites;
    public float _hp { get; private set; }
    float maxHp = 50f;

    BoxCollider2D boxCollider2D;
    SpriteRenderer spriteRenderer;

    public GameObject breakEffect;

    bool makeComplete = false;
    bool isGround = false;
    float gravitySpeed = 0.98f * 5f;

    float damage = 10f;

    public void Damaged(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        _hp -= damage;

        if (_hp <= 0f)
        {
            // 파편 튀는 이펙트
            GameObject effect = Instantiate(breakEffect, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f);

            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer.sprite = snowManSprites[0];

        _hp = maxHp;

        StartCoroutine(CoMakeSnowMan());
    }

    // Update is called once per frame
    void Update()
    {
        // 땅에 붙지 않은 경우
        if (!isGround)
        {
            // 땅인지 확인
            IsGround();
            // 생성이 완성된 경우
            if (makeComplete)
            {

                transform.Translate(0f, -transform.up.y * gravitySpeed * Time.deltaTime, 0f);
            }
        }

    }

    IEnumerator CoMakeSnowMan()
    {
        yield return new WaitForSeconds(0.75f);
        spriteRenderer.sprite = snowManSprites[1];
        yield return new WaitForSeconds(0.35f);
        spriteRenderer.sprite = snowManSprites[2];

        makeComplete = true;

        yield break;
    }


    // 발 아래 쪽으로 Raycast를 하여 Floor와 접촉한 상태인지 확인하는 함수.
    void IsGround()
    {
        //Physics2D.Raycast(transform.)
        // Raycast(시작 지점, 방향, 최대 거리, 레이어)
        // collider.bounds.center >> 현재 object가 가지고 있는 collider의 중앙(world좌표)
        // collider.bounds.size >> collider의 Vector3 기준으로의 크기
        if (Physics2D.Raycast(boxCollider2D.bounds.center, -transform.up, (boxCollider2D.bounds.size.y / 2) + 0.05f, 1 << LayerMask.NameToLayer("FLOOR")))
        {
            //Debug.Log("Is Ground");
            // 땅에 붙어있음.
            isGround = true;
        }
        else
        {
            // 떨어져있음.
            isGround = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PLAYER"))
        {
            Debug.Log("SNOWMAN HIT PLAYER");
            //Debug.Log("HIT PLAYER ICE BALL");
            // 충돌한 물체와 현재 물체의 Collider에서 가장 가까운 지점 확인
            Vector2 cloestPoint = collision.ClosestPoint(gameObject.GetComponent<Collider2D>().bounds.center);
            // 충돌한 지점의 벡터 설정.
            // 가까운 지점에서 현 위치를 빼면 방향이 설정된다.
            Vector2 hitNormal = new Vector2(cloestPoint.x - boxCollider2D.bounds.center.x, cloestPoint.y - boxCollider2D.bounds.center.y).normalized;

            megaman megaman = collision.GetComponent<megaman>();
            megaman.Damaged(damage, cloestPoint, hitNormal);

        }
    }



}
