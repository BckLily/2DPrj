using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InterfaceSet;


//[RequireComponent(typeof(SpriteRenderer))]
public class ChillPenguinAI : MonoBehaviour, IAttack, IDamaged
{
    [Header("Component")]
    // Boss의 rb
    public Rigidbody2D rb2d;
    [SerializeField]
    // Boss의 Polygon Collider 2D
    // 자동으로 Collider의 형태를 바꾸기 위해서 사용
    private PolygonCollider2D polyCollider2D;
    [SerializeField]
    // Boss 의 Sprite Renderer
    // Sprite의 형태를 바꾸기 위해서 사용
    private SpriteRenderer spriteRenderer;
    // Sprite Renderer에 넣을 Sprite를 저장
    public Sprite sprite;

    [Header("Animation Clips")]
    bool isFall = true;
    // 떨어지는 동작에 사용되는 Sprite
    public Sprite[] fallSprites;
    bool isIdle = false;
    // 기본 동작에 사용되는 Sprite
    public Sprite[] idleSprites;
    bool isDamaged = false;
    // 공격 받았을 때 사용되는 Sprite
    public Sprite[] damagedSprites;
    bool isJump = false;
    // 점프했을 때 사용되는 Sprite
    public Sprite[] jumpSprites;
    bool isReady = false;
    // 대부분의 동작의 준비 동작에 사용되는 Sprite
    public Sprite[] readySprites;
    bool isTurn = false;
    // 돌아볼 때 사용되는 Sprite
    public Sprite[] turnSprites;
    bool isDash = false;
    // 슬라이딩 돌진할 때 사용되는 Sprite
    public Sprite[] dashSprites;

    short settingCount = 0;

    // Sprite Renderer의 Draw Mode
    SpriteDrawMode drawMode;
    // Sprite Renderer의 Tile Mode
    SpriteTileMode tileMode;
    // Sprite Renderer의 Order in Layout 값
    int sortLayer;

    //private float gravity = -0.085f;
    //[SerializeField]
    //private int gravityCount = 0;
    [Header("Ground")]
    [SerializeField]
    // Boss가 땅에 붙어있을 경우
    // 피해를 받았을 때 뒤로 살짝떠서 밀려났다가
    // 바닥에 닿으면 다음 동작을 수행하거나
    // 점프했다가 바닥에 내려오면 다음 동작을 하는 등...
    // 여러가지 경우에 사용된다.
    bool isGround;


    [Header("Mask")]
    // Mask 게임 오브젝트
    public GameObject maskObj;
    [SerializeField]
    // Mask Game Object의 Sprite Renderer
    private SpriteRenderer maskRenderer;
    [SerializeField]
    // Mask Game Object의 Sprite
    private SpriteMask maskSprite;
    // 깜빡거리고 있는지
    bool isBlank = false;


    [Header("Health")]
    private float maxHp = 100f;
    [SerializeField]
    // Boss의 HP
    private float _hp;
    // Boss의 HP 프로퍼티.
    // 이상한 사용방식
    public float Hp
    {
        get
        {
            return _hp;
        }
        private set
        {
            _hp = value;
        }
    }
    // Boss의 HP Gauge용 Image.
    // FillAmount 값을 변경하기 위해서 사용된다.
    public Image HPGauge;
    bool isFillGauge = false;



    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        polyCollider2D = GetComponent<PolygonCollider2D>();

        maskRenderer = maskObj.GetComponent<SpriteRenderer>();
        maskSprite = maskObj.GetComponent<SpriteMask>();

        //IsGround();
        //sprite = spriteRenderer.sprite;
        SpriteChange(spriteRenderer.sprite);
        StartCoroutine(CoStartAnim());
        //StartCoroutine(CoComponentReset());

        Hp = maxHp;
    }


    // 발 아래 쪽으로 Raycast를 하여 Floor와 접촉한 상태인지 확인하는 함수.
    void IsGround()
    {
        //Physics2D.Raycast(transform.)
        // Raycast(시작 지점, 방향, 최대 거리, 레이어)
        // collider.bounds.center >> 현재 object가 가지고 있는 collider의 중앙(world좌표)
        // collider.bounds.size >> collider의 Vector3 기준으로의 크기
        if (Physics2D.Raycast(polyCollider2D.bounds.center, -transform.up, (polyCollider2D.bounds.size.y / 2) + 0.01f, 1 << LayerMask.NameToLayer("FLOOR")))
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


    private void FixedUpdate()
    {

    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log(polyCollider2D.bounds.size.y);
        Debug.DrawRay(polyCollider2D.bounds.center, -transform.up, Color.red, (polyCollider2D.bounds.size.y / 2) + 0.01f);
        IsGround();
        // Trigger 였을 때 중력 만들 때 사용한 것.
        //GetGravity();

        //if(gameObject.compon)
        if (settingCount >= 10)
        {
            ComponentReset();
        }

    }


    private void LateUpdate()
    {
        if (settingCount++ >= 10)
        {
            ComponentReset();
            settingCount = 0;
        }
        // 새로운 스프라이트를 변경해주는 코드.
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


    // 정상 동작하지 않는 코루틴
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


    // sprite 변경하는 함수
    private void SpriteChange(Sprite nextSprite)
    {
        sprite = nextSprite;
        maskSprite.sprite = nextSprite;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Collider2D coll2d = collision.gameObject.GetComponent<Collider2D>();
        //Vector2 pos = coll2d.ClosestPoint(polyCollider2D.bounds.center);
        //Debug.Log(collision.collider.name + ": " + pos);
    }



    // Polygon Collider2D와 Sprite Renderer의 조합을 통해서 자동으로 Polygon이 바뀌게 만들려면
    // Sprite Renderer Component를 제거했다가 다시 생성하는 번거로움이 필요하다.
    void ComponentReset()
    {
        //spriteRenderer.enabled = false;
        //spriteRenderer.enabled = true;

        //spriteRenderer.sprite = sprite;
        // Sprite Renderer를 this.GameObject가 가지고 있는지 판단
        if (gameObject.TryGetComponent<SpriteRenderer>(out spriteRenderer))
        {
            // 가지고 있을 경우
            //Debug.Log("DELETE");
            // 현재 Sprite Renderer의 설정들을 저장한다.
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            Sprite sprite = renderer.sprite;
            drawMode = renderer.drawMode;
            tileMode = renderer.tileMode;
            sortLayer = renderer.sortingOrder;
            // Sprite Renderer를 제거한다.
            Destroy(renderer);
        }
        else
        {
            // 없을 경우
            //Debug.Log("ADD");
            // 새로운 Sprite Renderer를 추가해준다.
            this.gameObject.AddComponent<SpriteRenderer>();
            // spriteRenderer라는 변수를 사용해서 다른 곳에서 동작을 진행하고 있으므로
            // 새로 생성한 SpriteRenderer를 spriteRenderer에 대입해준다.
            // Component를 제거하기 전에 저장한 설정들을 다시 설정해준다.
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.drawMode = drawMode;
            spriteRenderer.tileMode = tileMode;
            spriteRenderer.sortingOrder = sortLayer;

            //spriteRenderer.sprite = sprite;
        }
    }


    // 처음 보스전을 시작할 때 Animation
    IEnumerator CoStartAnim()
    {
        while (true)
        {
            // 위에서 떨어지는 것부터 시작하니 떨어지는 애니메이션 코루틴
            yield return StartCoroutine(CoFallAnim());
            // 다른 코루틴이 동작하고 있을 수 있으므로 새로운 애니메이션을 시작하기 전에 다른 애니메이션 코루틴을 멈춰 준다.
            StopCoroutine();
            yield return StartCoroutine(CoReadyAnim());
            StopCoroutine();
            yield return StartCoroutine(CoFillHPGauge());
            StopCoroutine();
            StartCoroutine(CoIdleAnim());
            yield break;
        }
    }


    IEnumerator CoFillHPGauge()
    {
        Debug.Log("FILL Gauge Start");

        isFillGauge = true;
        while (true)
        {
            HPGauge.fillAmount += 0.05f;
            yield return new WaitForFixedUpdate();

            if(HPGauge.fillAmount >= 1)
            {
                isFillGauge = false;
                yield break;
            }
        }

    }


    // 떨어지는 애니메이션 코루틴
    IEnumerator CoFallAnim()
    {
        Debug.Log("Fall Start");
        while (true)
        {
            // 땅에 떨어질 때까지
            while (!isGround)
            {
                // fallSprite의 0번
                sprite = fallSprites[0];
                yield return new WaitForSeconds(0.1f);
            }
            SpriteChange(fallSprites[1]);
            //sprite = fallSprites[1];
            yield return new WaitForSeconds(0.3f);
            SpriteChange(fallSprites[2]);
            //sprite = fallSprites[2];
            yield return new WaitForSeconds(0.3f);
            SpriteChange(fallSprites[3]);
            //sprite = fallSprites[3];
            yield return new WaitForSeconds(0.3f);
            SpriteChange(fallSprites[1]);
            //sprite = fallSprites[1];
            yield return new WaitForSeconds(0.3f);
            SpriteChange(fallSprites[2]);
            //sprite = fallSprites[2];
            yield return new WaitForSeconds(0.3f);
            SpriteChange(fallSprites[3]);
            //sprite = fallSprites[3];
            yield return new WaitForSeconds(0.3f);

            yield break;
        }
    }


    // 기본 동작 애니메이션
    IEnumerator CoIdleAnim()
    {
        Debug.Log("Idle Start");

        while (true)
        {
            SpriteChange(idleSprites[0]);
            //sprite = idleSprites[0];
            yield return new WaitForSeconds(0.5f);
            SpriteChange(idleSprites[1]);
            //sprite = idleSprites[1];
            yield return new WaitForSeconds(0.5f);
        }
    }


    // 공격 받았을 때의 애니메이션
    IEnumerator CoDamagedAnim()
    {
        Debug.Log("Damage Anim");

        while (isDamaged)
        {
            SpriteChange(damagedSprites[0]);
            //sprite = damagedSprites[0];
            yield return new WaitForSeconds(0.3f);

            if (isGround)
            {
                isDamaged = false;
                yield break;
            }
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


    // 점프 애니메이션
    IEnumerator CoJumpAnim()
    {
        Debug.Log("Jump Start");

        while (true)
        {

            yield break;
        }
    }


    // 준비 동작 애니메이션
    IEnumerator CoReadyAnim()
    {
        SpriteChange(readySprites[0]);
        //sprite = readySprites[0];
        yield return new WaitForSeconds(0.1f);
        SpriteChange(readySprites[1]);
        //sprite = readySprites[1];
        yield return new WaitForSeconds(0.1f);
        SpriteChange(readySprites[2]);
        //sprite = readySprites[2];
        yield return new WaitForSeconds(1.1f);

        yield break;
    }


    // 돌진 애니메이션
    IEnumerator CoDashAnim()
    {
        Debug.Log("Dash Start");
        yield break;
    }


    // 모든 애니메이션 코루틴을 멈추는 함수
    void StopCoroutine()
    {
        Debug.Log("Stop Start");

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
        if (isFillGauge)
        {
            StopCoroutine(CoFillHPGauge());
        }
    }


    // 플레이어와 직접 충돌했을 때 피해를 주는?? 함수
    public void Attack()
    {
        throw new System.NotImplementedException();
    }


    // 총알을 맞았을 때 피해를 받는 함수
    public void Damaged(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // 총알을 맞으면 약간 뒤로 떠서 밀려나는 동작과
        // 5초정도 흰색으로 블링크 하는 동작이 있다.

        // 체력바 변경하는 동작
        Hp -= damage;
        HPGauge.fillAmount = Hp / maxHp;

        // 
        isDamaged = true;
        StartCoroutine(CoDamagedAnim());
        Blank();

    }


    // 깜빡이는 코루틴을 실행할지 말지 판단하는 함수
    public void Blank()
    {
        if (!isBlank)
        {
            isBlank = true;
            StartCoroutine(CoBlank());
        }
    }


    // 피격시 깜빡이는 코루틴
    IEnumerator CoBlank()
    {
        Debug.Log("Start Blink");

        int count = 0;
        while (true)
        {

            maskRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);

            maskRenderer.enabled = false;
            yield return new WaitForSeconds(0.2f);

            if (++count >= 8) { isBlank = false; yield break; }
        }
    }



}
