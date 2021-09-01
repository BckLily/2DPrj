using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InterfaceSet;


//[RequireComponent(typeof(SpriteRenderer))]
public class ChillPenguinAI : MonoBehaviour, IAttack, IDamaged
{
    [Header("Component")]
    public Rigidbody2D rb2d;
    [SerializeField]
    private PolygonCollider2D polyCollider2D;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public Sprite sprite;

    [Header("Animation Clips")]
    bool isFall = true;
    public Sprite[] fallSprites;
    bool isIdle = false;
    public Sprite[] idleSprites;
    bool isDamaged = false;
    public Sprite[] damagedSprites;
    bool isJump = false;
    public Sprite[] jumpSprites;
    bool isReady = false;
    public Sprite[] readySprites;
    bool isTurn = false;
    public Sprite[] turnSprites;
    bool isDash = false;
    public Sprite[] dashSprites;

    short settingCount = 0;

    SpriteDrawMode drawMode;
    SpriteTileMode tileMode;
    int sortLayer;

    //private float gravity = -0.085f;
    //[SerializeField]
    //private int gravityCount = 0;
    bool isGround;



    // Start is called before the first frame update
    void Start()
    {
        //IsGround();
        sprite = spriteRenderer.sprite;
        StartCoroutine(CoStartAnim());
        //StartCoroutine(CoComponentReset());

        rb2d = GetComponent<Rigidbody2D>();
        polyCollider2D = GetComponent<PolygonCollider2D>();


    }

    // isTrigger일 때 사용했던 함수.
    // 발 아래 쪽으로 Raycast를 하여 Floor와 접촉한 상태인지 확인하는 함수.
    //void IsGround()
    //{
    //    //Physics2D.Raycast(transform.)
    //    if (Physics2D.Raycast(polyCollider2D.bounds.center, -transform.up, (polyCollider2D.bounds.size.y / 2) + 0.005f, 1 << LayerMask.NameToLayer("FLOOR")))
    //    {
    //        isGround = true;
    //    }
    //    else
    //    {
    //        isGround = false;
    //    }

    //}



    private void FixedUpdate()
    {

    }


    // Update is called once per frame
    void Update()
    {
        //IsGround();
        // Trigger 였을 때 중력 만들 때 사용한 것.
        //GetGravity();

        //if(gameObject.compon)
        if (settingCount >= 50)
        {
            ComponentReset();
        }

    }

    private void LateUpdate()
    {
        if (settingCount++ >= 50)
        {
            ComponentReset();
            settingCount = 0;
        }
        spriteRenderer.sprite = sprite;

    }

    //private void GetGravity()
    //{
    //    if (!isGround)
    //    {
    //        //gravityCount++;
    //        //rb2d.AddForce(new Vector2(0, gravity * Time.deltaTime));
    //        transform.Translate(0, ++gravityCount * gravity * Time.deltaTime, 0);
    //    }
    //    else
    //    {
    //        //rb2d.AddForce(new Vector2(0, gravity * Time.deltaTime * gravityCount));
    //        gravityCount = 0;
    //    }
    //}

    IEnumerator CoComponentReset()
    {
        while (true)
        {
            ComponentReset();
            yield return new WaitForSeconds(0f);
            ComponentReset();
            yield return new WaitForSeconds(1f);
        }
    }

    void ComponentReset()
    {
        //spriteRenderer.enabled = false;
        //spriteRenderer.enabled = true;

        //spriteRenderer.sprite = sprite;
        if (gameObject.TryGetComponent<SpriteRenderer>(out spriteRenderer))
        {
            //Debug.Log("DELETE");
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            Sprite sprite = renderer.sprite;
            drawMode = renderer.drawMode;
            tileMode = renderer.tileMode;
            sortLayer = renderer.sortingOrder;
            Destroy(renderer);
        }
        else
        {
            //Debug.Log("ADD");
            this.gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.drawMode = drawMode;
            spriteRenderer.tileMode = tileMode;
            spriteRenderer.sortingOrder = sortLayer;

            //spriteRenderer.sprite = sprite;
        }
    }


    IEnumerator CoStartAnim()
    {
        while (true)
        {
            yield return StartCoroutine(CoFallAnim());
            StopCoroutine();
            yield return StartCoroutine(CoReadyAnim());
            StopCoroutine();
            StartCoroutine(CoIdleAnim());
            yield break;
        }
    }

    IEnumerator CoFallAnim()
    {
        while (true)
        {
            while (!isGround)
            {
                sprite = fallSprites[0];
                yield return new WaitForSeconds(0.1f);
            }
            sprite = fallSprites[1];
            yield return new WaitForSeconds(0.1f);
            sprite = fallSprites[2];
            yield return new WaitForSeconds(0.1f);
            sprite = fallSprites[3];
            yield return new WaitForSeconds(0.1f);

            yield break;
        }
    }

    IEnumerator CoIdleAnim()
    {
        while (true)
        {
            sprite = idleSprites[0];
            yield return new WaitForSeconds(0.5f);
            sprite = idleSprites[1];
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CoDamagedAnim()
    {
        while (true)
        {
            sprite = damagedSprites[0];
            yield return new WaitForSeconds(0.3f);
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    // Player의 공격에 맞으면
    //    if (collision.CompareTag("BULLET"))
    //    {
    //        Damaged();
    //    }
    //}




    IEnumerator CoJumpAnim()
    {
        while (true)
        {

            yield break;
        }
    }

    IEnumerator CoReadyAnim()
    {
        sprite = readySprites[0];
        yield return new WaitForSeconds(0.2f);
        sprite = readySprites[1];
        yield return new WaitForSeconds(0.2f);
        sprite = readySprites[2];
        yield return new WaitForSeconds(1f);

        yield break;
    }

    IEnumerator CoDashAnim()
    {
        yield break;
    }

    void StopCoroutine()
    {
        if (isFall)
        {
            StopCoroutine(CoFallAnim());
        }
        if (isDamaged)
        {
            StopCoroutine(CoDamagedAnim());
        }
        if (isIdle)
        {
            StopCoroutine(CoIdleAnim());
        }
        if (isJump)
        {
            StopCoroutine(CoJumpAnim());
        }
        if (isReady)
        {
            StopCoroutine(CoReadyAnim());
        }
        if (isDash)
        {
            StopCoroutine(CoDashAnim());
        }
    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Damaged(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        throw new System.NotImplementedException();
    }
}
