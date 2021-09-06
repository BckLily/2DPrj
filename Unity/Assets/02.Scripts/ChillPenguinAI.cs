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
    // Sprite의 반전에 사용되는 flipX
    bool flipX = false;
    [SerializeField]
    RectTransform rectTr; // Boos의 Rect Transform
    [SerializeField]
    private Transform playerTr; // Player Transform


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
    bool isAttack = false;
    public Sprite[] fireSprites;
    // 원거리 공격 중인지
    bool isFire = false;


    // 특정 횟수마다 Component Reset 실행
    short settingCount = 0;

    // Sprite Renderer의 Draw Mode
    SpriteDrawMode drawMode;
    // Sprite Renderer의 Tile Mode
    SpriteTileMode tileMode;
    // Sprite Renderer의 Order in Layout 값
    int sortLayer;

    // Animation Coroutines
    IEnumerator IBossPattern;
    IEnumerator IDashAnim;
    IEnumerator IJumpAnim;
    IEnumerator IDamageAnim;
    IEnumerator IIceBreathAnim;
    IEnumerator IIceBallAnim;
    IEnumerator IStartAnim;
    IEnumerator IFallAnim;
    IEnumerator IReadyAnim;
    IEnumerator IIdleAnim;
    IEnumerator IBlankAnim;
    IEnumerator ILookAnim;
    IEnumerator IFillHpGauge;



    [Header("Dash")]
    [SerializeField]
    private Vector2 attackVector;
    [SerializeField]
    private float dashSpeed = 22.5f;
    [SerializeField]
    private float currSpeed;
    [SerializeField]
    private float dashTime;
    [SerializeField]
    private float jumpVerticalSpeed;
    [SerializeField]
    private float jumpHorizontalSpeed;
    [SerializeField]
    private float jumpTime;
    //private float gravity = -0.085f;
    //[SerializeField]
    //private int gravityCount = 0;
    //[Header("Ground")]
    //[SerializeField]
    // Boss가 땅에 붙어있을 경우
    // 피해를 받았을 때 뒤로 살짝떠서 밀려났다가
    // 바닥에 닿으면 다음 동작을 수행하거나
    // 점프했다가 바닥에 내려오면 다음 동작을 하는 등...
    // 여러가지 경우에 사용된다.
    bool isGround;
    bool isStageStart = false;

    [Header("Mask")]
    // Mask 게임 오브젝트
    public GameObject maskObj;
    RectTransform maskRectTransform;
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
    int gaugeDiff = 20;

    [Header("Range Attack Object")]
    public GameObject iceBreathObj;
    public GameObject iceBallObj;
    public GameObject snowManObj;

    [Header("Attack Parameter")]
    [SerializeField]
    float currDamage;
    float dashDamage = 50f;
    float contactDamage = 10f;

    [Header("Die Effect")]
    public GameObject dieEffect;


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        polyCollider2D = GetComponent<PolygonCollider2D>();

        maskRenderer = maskObj.GetComponent<SpriteRenderer>();
        maskSprite = maskObj.GetComponent<SpriteMask>();

        rectTr = GetComponent<RectTransform>();

        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();

        maskRectTransform = maskObj.GetComponent<RectTransform>();

        attackVector = new Vector3();

        contactDamage = 10f;
        dashDamage = 50f;

        currDamage = contactDamage;

        //IsGround();
        //sprite = spriteRenderer.sprite;
        SpriteChange(spriteRenderer.sprite);
        IBossPattern = CoBossPattern();
        StartCoroutine(IBossPattern);
        //StartCoroutine(CoComponentReset());

        Hp = maxHp;
        dashSpeed = 22.5f;

        jumpHorizontalSpeed = 3f;
        jumpVerticalSpeed = 12f;

    }

    private void FixedUpdate()
    {

    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log(polyCollider2D.bounds.size.y);
        //Debug.DrawRay(polyCollider2D.bounds.center, -transform.up, Color.red, (polyCollider2D.bounds.size.y / 2) + 0.01f);
        // 바닥에 붙어있는지 확인한다.
        IsGround();
        // Trigger 였을 때 중력 만들 때 사용한 것.
        //GetGravity();

        // 체력바 상태 값 변경
        if (isStageStart)
        {
            HealthBarChange();
        }

        //if(gameObject.compon)
        if (settingCount >= 10)
        {
            // 일정 주기마다 Sprite Renderer Reset
            ComponentReset();
        }

        // 대쉬 중일 경우 실행
        if (isDash)
        {
            // attackVector(방향) * 현재 속도 * 델타 타임 만큼의 속도로 대쉬
            rectTr.Translate(new Vector3(attackVector.x * (currSpeed * Time.deltaTime), 0, 0));
            // 현재 대쉬 시간을 증가
            dashTime += Time.deltaTime * 23f;
            // Cos에 사용될 값이라 절대값을 사용해서 양수 값으로 만든다.
            // 시간상 0~90 Rad의 값이 사용될 것이다.
            dashTime = Mathf.Abs(dashTime);
            // Cos을 사용해서 천천히 움직이는 속도가 줄어들게 하였다.
            currSpeed *= Mathf.Cos(dashTime * Mathf.Deg2Rad);
            //if (currDashSpeed <= 0) { currDashSpeed = 0; }
        }

        // 점프 중일 경우 실행
        if (isJump)
        {
            //Debug.Log("Is Jump: " + isJump);

            // 공격 방향 * 고정 횡 이동 속도 * 델타 타임 / 현재속도(Vertical) * 델타 타임으로 이동.
            rectTr.Translate(new Vector3(attackVector.x * (jumpHorizontalSpeed * Time.deltaTime), currSpeed * Time.deltaTime, 0f));

            // 점프 속도는 점점 느려졌다가 빨라져야 한다. 위로 갔다가 아래로 내려와야 한다.
            jumpTime += Time.deltaTime * 120f;
            currSpeed = jumpVerticalSpeed * Mathf.Cos(jumpTime * Mathf.Deg2Rad);

        }

    }


    private void LateUpdate()
    {
        // Sprite Renderer를 제거한 다음에 바로 다시 만들어야 한다.
        if (settingCount++ >= 10)
        {
            ComponentReset();
            settingCount = 0;
        }
        //prePos = rectTr.position;

        // 새로운 스프라이트를 적용해주는 코드.
        if (TryGetComponent<SpriteRenderer>(out spriteRenderer))
        {
            //Debug.Log("Sprite Change");
            spriteRenderer.flipX = flipX;
            spriteRenderer.sprite = sprite;
            //maskRenderer.sprite = sprite;
            //maskRenderer.flipX = flipX;
            maskSprite.sprite = sprite;

            // Sprite가 반전되어있을 경우
            if (flipX == true)
            {
                // 마스크의 로컬 스케일을 저장
                Vector3 maskScale = maskRectTransform.localScale;
                // 스케일의 x방향을 -1로 설정(뒤집어 준다)
                maskScale.x = -1f;
                maskRectTransform.localScale = maskScale;
            }
            else
            {
                Vector3 maskScale = maskRectTransform.localScale;
                // 스케일의 x방향을 1로 설정(원복 해준다.)
                maskScale.x = 1f;
                maskRectTransform.localScale = maskScale;
            }
        }
    }


    // 체력 바를 채우는 함수
    void HealthBarChange()
    {
        // 체력 바 이미지는 20개의 게이지로 되어있으므로
        // 체력이 어느정도 깎이면 게이지가 한 칸씩 줄어들도록 하였다.
        int gaugeValue = Mathf.CeilToInt(_hp / (maxHp / gaugeDiff));
        HPGauge.fillAmount = (float)gaugeValue / gaugeDiff;
    }


    // 보스 패턴을 설정하는 코루틴
    #region Boss Pattern Coroutine
    IEnumerator CoBossPattern()
    {
        //Debug.Log("Pattern Start");
        // 보스가 처음 등장할 때 동작하는 애니메이션 코루틴
        IStartAnim = CoStartAnim();
        yield return StartCoroutine(IStartAnim);

        while (true)
        {
            // 플레이어의 위치를 쳐다보는 코루틴
            ILookAnim = CoLookPlayer();
            yield return StartCoroutine(ILookAnim);
            // 혹시 다른 애니메이션 코루틴이 동작하고 있을 수 있으므로 애니메이션과 관련된 코루틴을 모두 정지시킨다.
            yield return StartCoroutine(StopAnimationCoroutine());

            // 여러 개의 패턴이 있으므로 각 패턴에 맞게 동작하기 위한 랜덤 값.
            // 특정 값의 확률을 높이고 싶으면 여러 개의 case를 break 없이 연결할 수 있다.
            int num = Random.Range(0, 4);
            // 공격 중임을 표시하기 위해서 isAttack을 true로 표시한다.
            isAttack = true;
            switch (num)
            {
                // 대쉬 패턴
                case 0:
                    //Debug.Log("_____DASH_____");
                    // Dash의 경우 공격력이 높으므로 현재의 공격력을 대쉬 공격력으로 바꿔준다.
                    currDamage = dashDamage;
                    // 혹시 다른 애니메이션 코루틴이 동작하고 있을 수 있으므로 애니메이션과 관련된 코루틴을 모두 정지시킨다.
                    //yield return StartCoroutine(StopAnimationCoroutine());
                    // 대쉬 애니메이션 코루틴을 실행한다.
                    IDashAnim = CoDashAnim();
                    yield return StartCoroutine(IDashAnim);
                    // 대쉬가 끝나면 현재 공격력을 단순 접촉 시 공격력으로 변경한다.
                    currDamage = contactDamage;
                    break;
                // 점프 패턴
                case 1:
                    //Debug.Log("____JUMP____");

                    //yield return StartCoroutine(StopAnimationCoroutine());
                    // 점프 애니메이션 코루틴 실행
                    IJumpAnim = CoJumpAnim();
                    yield return StartCoroutine(IJumpAnim);

                    break;
                // 브래스 패턴
                case 2:
                    //Debug.Log("____BREATH____");

                    //yield return StartCoroutine(StopAnimationCoroutine());
                    // 얼음 브레스 애니메이션 코루틴 실행
                    IIceBreathAnim = CoIceBreathAnim();
                    yield return StartCoroutine(IIceBreathAnim);

                    break;
                // 얼음 구체 패턴
                case 3:
                    //Debug.Log("____ICEBALL____");

                    //yield return StartCoroutine(StopAnimationCoroutine());
                    // 얼음 발사하는 애니메이션 코루틴 실행
                    IIceBallAnim = CoIceBallAnim();
                    yield return StartCoroutine(IIceBallAnim);

                    break;
                default:


                    break;
            }
            // 공격 상태가 종료되었으므로 isAttack을 false로 한다.
            isAttack = false;

            // 혹시 애니메이션이 동작하고 있을 수 있으므로 종료시켜준다.
            yield return StartCoroutine(StopAnimationCoroutine());
            // 공격이 끝난 후 플레이어의 방향을 보도록 한다.
            ILookAnim = CoLookPlayer();
            yield return StartCoroutine(ILookAnim);
            // 기본 대기 상태 애니메이션 코루틴을 실행한다.
            IIdleAnim = CoIdleAnim();
            yield return StartCoroutine(IIdleAnim);

            //Debug.Log("Pattern Loop");
        }

        //Debug.Log("Pattern End");
        yield break;
    }
    #endregion



    // 발 아래 쪽으로 Raycast를 하여 Floor와 접촉한 상태인지 확인하는 함수.
    void IsGround()
    {
        //Physics2D.Raycast(transform.)
        // Raycast(시작 지점, 방향, 최대 거리, 레이어)
        // collider.bounds.center >> 현재 object가 가지고 있는 collider의 중앙(world좌표)
        // collider.bounds.size >> collider의 Vector3 기준으로의 크기
        if (Physics2D.Raycast(polyCollider2D.bounds.center, -transform.up, (polyCollider2D.bounds.size.y / 2) + 0.05f, 1 << LayerMask.NameToLayer("FLOOR")))
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


    #region Trash GetGravity use velocity
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
    #endregion


    // sprite 변경하는 함수
    private void SpriteChange(Sprite nextSprite)
    {
        sprite = nextSprite;
        maskSprite.sprite = nextSprite;
    }

    // 통상 상태일 때 충돌 판정
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ////Debug.Log(rb2d.velocity);
        //if (collision.gameObject.CompareTag("WALL"))
        //{
        //    currDashSpeed *= -1f;
        //    flipX = !flipX;
        //    Debug.Log("____Reflect____");
        //    //Debug.Log(rb2d.velocity);
        //    //Vector2 vector = new Vector2(rb2d.velocity.x * -1, rb2d.velocity.y);
        //    //Vector2 vector = Vector2.Reflect(rb2d.velocity, -collision.contacts[0].normal);
        //    //rb2d.velocity = vector;
        //    //Debug.Log(rb2d.velocity);
        //}

        // 플레이어와 부딪히면
        if (collision.gameObject.CompareTag("PLAYER"))
        {
            megaman _megaman = collision.gameObject.GetComponent<megaman>();
            // 플레이어에게 데미지를 입힌다.
            _megaman.Damaged(currDamage, collision.contacts[0].point, collision.contacts[0].normal);

            //Debug.Log("COLL PLAYER");
        }


        //Collider2D coll2d = collision.gameObject.GetComponent<Collider2D>();
        //Vector2 pos = coll2d.ClosestPoint(polyCollider2D.bounds.center);
        //Debug.Log(collision.collider.name + ": " + pos);
    }

    // 대쉬 및 점프 중일 때 피격 판정
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.CompareTag("BULLET"))
        //{
        //    float v = collision.bounds.center.x - polyCollider2D.bounds.center.x;
        //    Debug.Log("V: " + v);
        //}

        // 벽에 부딪칠 경우
        if (collision.gameObject.CompareTag("WALL"))
        {
            // 대쉬 중이면
            if (isDash)
            {
                // 대쉬 방향을 반대로 해준다.
                currSpeed *= -1f;
            }
            // 점프 중이면
            else if (isJump)
            {
                // 점프 방향을 반대로 해준다.
                jumpHorizontalSpeed *= -1f;
            }

            // 스프라이트의 방향을 뒤집어 준다.
            flipX = !flipX;
            //Debug.Log("____Reflect____");
            //Debug.Log(rb2d.velocity);
            //Vector2 vector = new Vector2(rb2d.velocity.x * -1, rb2d.velocity.y);
            //Vector2 vector = Vector2.Reflect(rb2d.velocity, -collision.contacts[0].normal);
            //rb2d.velocity = vector;
            //Debug.Log(rb2d.velocity);

        }
        // 플레이어와 부딪치면
        else if (collision.CompareTag("PLAYER"))
        {
            //Debug.Log("TRIGGER PLAYER");
            
            // 가장 가까운 포인트
            Vector2 cloestPoint = collision.ClosestPoint(polyCollider2D.bounds.center);
            // 가까운 포인트의 노멀 벡터
            Vector2 hitNormal = new Vector2(cloestPoint.x - polyCollider2D.bounds.center.x, cloestPoint.y - polyCollider2D.bounds.center.y).normalized;

            // 메가맨의 스크립트
            megaman _megaman = collision.GetComponent<megaman>();
            // 현재 공격력, 인접한 포인트, 벡터 방향.
            _megaman.Damaged(currDamage, cloestPoint, hitNormal);
        }
        // 눈사람과 부딪치면
        else if (collision.CompareTag("SNOWMAN"))
        {
            SnowMan snowMan = collision.GetComponent<SnowMan>();
            // 맞은 포인트와 벡터 펭귄에게 맞으면 바로 파괴되므로 필요 없다.
            snowMan.Damaged(currDamage, Vector3.zero, Vector3.zero);
        }
    }



    #region Component Reset Function
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
            flipX = renderer.flipX;
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
            spriteRenderer.flipX = flipX;

            //spriteRenderer.sprite = sprite;
        }
    }
    #endregion


    #region Start Boss Animation Coroutine
    // 처음 보스전을 시작할 때 Animation
    IEnumerator CoStartAnim()
    {
        //Debug.Log("Start Start");

        //while (true)
        //{
        // 위에서 떨어지는 것부터 시작하니 떨어지는 애니메이션 코루틴
        IFallAnim = CoFallAnim();
        yield return StartCoroutine(IFallAnim);
        // 다른 코루틴이 동작하고 있을 수 있으므로 새로운 애니메이션을 시작하기 전에 다른 애니메이션 코루틴을 멈춰 준다.
        yield return StartCoroutine(StopAnimationCoroutine());
        // 떨어지고나서 체력 게이지가 찰때까지의 동작
        IReadyAnim = CoReadyAnim();
        yield return StartCoroutine(IReadyAnim);

        yield return StartCoroutine(StopAnimationCoroutine());
        // 체력 게이지를 체우는 코루틴 실행
        IFillHpGauge = CoFillHPGauge();
        yield return StartCoroutine(IFillHpGauge);
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(StopAnimationCoroutine());
        IIdleAnim = CoIdleAnim();
        yield return StartCoroutine(IIdleAnim);

        //Debug.Log("Start End");
        yield break;
        //}
    }
    #endregion


    #region Fill HP Gauge Coroutine
    // 체력 게이지 0~100%로 채우는 코루틴
    IEnumerator CoFillHPGauge()
    {
        //Debug.Log("FILL Gauge Start");

        isFillGauge = true;
        while (true)
        {
            // 20칸으로 나뉘어져 있어 0.05씩 증가시켰다.
            HPGauge.fillAmount += 0.05f;
            yield return new WaitForFixedUpdate();

            if (HPGauge.fillAmount >= 1)
            {
                yield return new WaitForSeconds(0.3f);
                isFillGauge = false;

                //Debug.Log("Fill Gauge End");
                isStageStart = true;
                yield break;
            }
        }

    }
    #endregion


    #region Fall Animation Coroutine
    // 떨어지는 애니메이션 코루틴
    IEnumerator CoFallAnim()
    {
        //Debug.Log("Fall Start");

        isFall = true;
        //while (true)
        //{
        // 땅에 떨어질 때까지
        while (isGround == false)
        {
            // fallSprite의 0번
            SpriteChange(fallSprites[0]);
            yield return new WaitForSeconds(0.1f);
        }
        SpriteChange(fallSprites[1]);
        //sprite = fallSprites[1];
        yield return new WaitForSeconds(0.25f);
        SpriteChange(fallSprites[2]);
        //sprite = fallSprites[2];
        yield return new WaitForSeconds(0.25f);
        SpriteChange(fallSprites[3]);
        //sprite = fallSprites[3];
        yield return new WaitForSeconds(0.25f);
        SpriteChange(fallSprites[1]);
        //sprite = fallSprites[1];
        yield return new WaitForSeconds(0.25f);
        SpriteChange(fallSprites[2]);
        //sprite = fallSprites[2];
        yield return new WaitForSeconds(0.25f);
        SpriteChange(fallSprites[3]);
        //sprite = fallSprites[3];
        yield return new WaitForSeconds(0.25f);
        isFall = false;

        //Debug.Log("Fall End");
        yield break;
        //}
    }
    #endregion


    #region Idle Animation Coroutine
    // 기본 동작 애니메이션
    IEnumerator CoIdleAnim()
    {
        //Debug.Log("Idle Start");

        isIdle = true;
        float random = Random.Range(1.5f, 3.5f);
        float count = 0f;
        while (true)
        {
            count += 1f;
            SpriteChange(idleSprites[0]);
            //sprite = idleSprites[0];
            yield return new WaitForSeconds(0.4f);
            SpriteChange(idleSprites[1]);
            //sprite = idleSprites[1];
            yield return new WaitForSeconds(0.4f);
            ILookAnim = CoLookPlayer();
            StartCoroutine(ILookAnim);
            if (count >= random)
            {
                //Debug.Log("Idle End");

                yield break;
            }
        }
    }
    #endregion


    #region Damaged Animation Coroutine
    // 공격 받았을 때의 애니메이션
    IEnumerator CoDamagedAnim()
    {
        //Debug.Log("Damage Anim");

        //while (isDamaged)
        //{
        SpriteChange(damagedSprites[0]);
        //sprite = damagedSprites[0];
        yield return new WaitForSeconds(0.3f);

        if (isGround)
        {
            isDamaged = false;

            //Debug.Log("Damage End");

            yield break;
        }
        //}
    }
    #endregion


    #region Jump Animation Coroutine
    // 점프 애니메이션
    IEnumerator CoJumpAnim()
    {
        //Debug.Log("Jump Start");

        isJump = true;
        SetAttackState(isJump);

        attackVector.x = playerTr.position.x > polyCollider2D.bounds.center.x ? 1 : -1;
        jumpHorizontalSpeed = Mathf.Abs(jumpHorizontalSpeed);
        currSpeed = jumpVerticalSpeed;
        jumpTime = 0f;

        //while (true)
        //{
        SpriteChange(jumpSprites[0]);
        yield return new WaitForSeconds(0.05f);
        SpriteChange(jumpSprites[1]);
        yield return new WaitForSeconds(0.75f);

        while (isGround == false)
        {
            SpriteChange(fallSprites[0]);
            yield return new WaitForSeconds(0.1f);
        }
        isJump = false;
        SetAttackState(isJump);


        SpriteChange(fallSprites[1]);
        yield return new WaitForSeconds(0.1f);
        SpriteChange(fallSprites[2]);
        yield return new WaitForSeconds(0.2f);
        SpriteChange(fallSprites[3]);
        yield return new WaitForSeconds(0.1f);


        //Debug.Log("Jump End");

        yield break;
        //}
    }
    #endregion


    #region Ready Animation Coroutine
    // 준비 동작 애니메이션 (보스 등장시 체력 차는 중)
    IEnumerator CoReadyAnim()
    {
        isReady = true;
        //Debug.Log("Ready Start");

        //while (true)
        //{
        SpriteChange(readySprites[0]);
        //sprite = readySprites[0];
        yield return new WaitForSeconds(0.1f);
        SpriteChange(readySprites[1]);
        //sprite = readySprites[1];
        yield return new WaitForSeconds(0.1f);
        SpriteChange(readySprites[2]);
        //sprite = readySprites[2];
        yield return new WaitForSeconds(1.1f);
        isReady = false;

        //Debug.Log("Ready End");

        yield break;
        //}
    }
    #endregion


    #region Slide Dash Animation Coroutine
    // 돌진 애니메이션
    IEnumerator CoDashAnim()
    {
        SpriteChange(readySprites[0]);
        yield return new WaitForSeconds(0.25f);

        SpriteChange(dashSprites[0]);
        yield return new WaitForSeconds(0.11f);

        currSpeed = dashSpeed;
        dashTime = 0f;

        // 공격 방향 설정.
        // 플레이어가 왼쪽이면 -1, 플레이어가 오른쪽이면 1
        attackVector.x = playerTr.position.x > polyCollider2D.bounds.center.x ? 1 : -1;

        //Debug.Log("Player" + playerTr.position);
        //Debug.Log("Center: " + polyCollider2D.bounds.center);
        //Debug.Log("Attac Vector X: " + attackVector.x);

        isDash = true;

        SetAttackState(isDash);

        //Debug.Log("Dash Start");
        //while (true)
        //{

        SpriteChange(dashSprites[1]);
        //rb2d.AddForce(-transform.right * 60f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.3f);

        //Debug.Log("Dash End");

        isDash = false;

        SetAttackState(isDash);

        yield break;
        //}


    }

    #endregion


    #region Ice Breath Animation Coroutine
    IEnumerator CoIceBreathAnim()
    {
        isFire = true;

        SetAttackState(isFire);

        SpriteChange(readySprites[0]);
        yield return new WaitForSeconds(0.15f);
        SpriteChange(readySprites[1]);
        yield return new WaitForSeconds(0.15f);
        SpriteChange(fireSprites[0]);
        yield return new WaitForSeconds(0.25f);
        SpriteChange(fireSprites[1]);
        float fireCount = 0f;
        attackVector.x = playerTr.position.x > polyCollider2D.bounds.center.x ? 1 : -1;
        Instantiate(snowManObj, new Vector3(polyCollider2D.bounds.center.x + attackVector.x * 2f, polyCollider2D.bounds.center.y + 1, 0f), Quaternion.Euler(attackVector));
        Instantiate(snowManObj, new Vector3(polyCollider2D.bounds.center.x + attackVector.x * 3.5f, polyCollider2D.bounds.center.y + 1, 0f), Quaternion.Euler(attackVector));

        while (fireCount <= 4f)
        {
            fireCount += 0.15f;
            GameObject _iceBreathObj = Instantiate(iceBreathObj, new Vector3(polyCollider2D.bounds.center.x + attackVector.x, polyCollider2D.bounds.center.y, 0f), Quaternion.Euler(attackVector));
            _iceBreathObj.GetComponent<IceBreath>().moveVector = attackVector;
            yield return new WaitForSeconds(0.15f);

        }

        isFire = false;

        SetAttackState(isFire);

        yield break;
    }

    #endregion


    #region Ice Ball Animation Coroutine
    IEnumerator CoIceBallAnim()
    {
        isFire = true;

        SetAttackState(isFire);

        SpriteChange(readySprites[0]);
        yield return new WaitForSeconds(0.15f);
        SpriteChange(readySprites[1]);
        yield return new WaitForSeconds(0.15f);
        SpriteChange(fireSprites[0]);
        yield return new WaitForSeconds(0.25f);
        SpriteChange(fireSprites[1]);
        attackVector.x = playerTr.position.x > polyCollider2D.bounds.center.x ? 1 : -1;
        GameObject _iceBallObj = Instantiate(iceBallObj, new Vector3(polyCollider2D.bounds.center.x + attackVector.x, polyCollider2D.bounds.center.y, 0f), Quaternion.Euler(attackVector));
        _iceBallObj.GetComponent<IceBall>().moveVector = attackVector;
        yield return new WaitForSeconds(0.5f);

        isFire = false;

        SetAttackState(isFire);

        yield break;
    }
    #endregion

    #region Chill Penguin Die Animation Coroutine
    IEnumerator CoDieAnim()
    {
        StopCoroutine(IBossPattern);
        yield return StartCoroutine(StopAnimationCoroutine());
        SpriteChange(damagedSprites[0]);
        GameObject effect = Instantiate(dieEffect, rectTr.position, Quaternion.identity);
        Destroy(effect, 3f);
        yield return new WaitForSeconds(3f);

        Destroy(this.gameObject);

        yield break;
    }

    #endregion

    // 공격 중일 때의 상태로 변경하는 함수.
    void SetAttackState(bool _isState)
    {
        if (_isState)
        {
            // velocity 값을 0으로 설정해서 현재 받고 있는 속도 값을 0으로 만듬
            rb2d.velocity = Vector2.zero;
            // 중력 값을 0으로 변경
            rb2d.gravityScale = 0f;

            // Kinematic 상태로 bodyType 변경
            rb2d.bodyType = RigidbodyType2D.Kinematic;


            // Trigger 상태로 변경
            polyCollider2D.isTrigger = true;
        }
        else
        {
            // Trigger 상태를 끔
            polyCollider2D.isTrigger = false;

            // Dynamic 상태로 bodyType 변경
            rb2d.bodyType = RigidbodyType2D.Dynamic;

            // 중력 값을 1.5로 변경
            rb2d.gravityScale = 1.5f;

        }
    }


    #region All Stop Animation Coroutine
    // 모든 애니메이션 코루틴을 멈추는 함수
    IEnumerator StopAnimationCoroutine()
    {
        //Debug.Log("Stop Start");

        if (isFall)
        {
            isFall = false;
            StopCoroutine(IFallAnim);
        }
        if (isDamaged)
        {
            isDamaged = false;
            StopCoroutine(IDamageAnim);
        }
        if (isIdle)
        {
            isIdle = false;
            StopCoroutine(IIdleAnim);
        }
        if (isJump)
        {
            isJump = false;
            StopCoroutine(IJumpAnim);
        }
        if (isReady)
        {
            isReady = false;
            StopCoroutine(IReadyAnim);
        }
        if (isDash)
        {
            isDash = false;
            StopCoroutine(IDashAnim);
        }
        if (isFillGauge)
        {
            isFillGauge = false;
            StopCoroutine(IFillHpGauge);
        }
        if (isTurn)
        {
            isTurn = false;
            StopCoroutine(ILookAnim);
        }
        if (isFire)
        {
            isFire = false;
            StopCoroutine(IIceBallAnim);
            StopCoroutine(IIceBreathAnim);
        }

        //Debug.Log("Stop End");
        yield break;
    }
    #endregion


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
        if (!isDamaged && isAttack == false)
        {
            // 힘을 받는 방향으로 밀친다.
            // 공격 중이거나 피해를 받아서 이미 깜빡이고 있는 경우 이 동작을 다시 수행하지 않는다.
            rb2d.AddForce(new Vector2(hitNormal.x, Mathf.Abs(hitNormal.x * Mathf.Tan(60 * Mathf.Deg2Rad))).normalized * 11f, ForceMode2D.Impulse);

            //Debug.Log("Vector: " + new Vector2(hitNormal.x, hitNormal.x * Mathf.Tan(45 * Mathf.Deg2Rad)));

            StartCoroutine(StopAnimationCoroutine());
            isDamaged = true;
            IDamageAnim = CoDamagedAnim();
            StartCoroutine(IDamageAnim);
            Blank();
        }

        if (Hp <= 0f)
        {
            Debug.Log("Penguin Die");
            StartCoroutine(CoDieAnim());
        }

    }


    // 깜빡이는 코루틴을 실행할지 말지 판단하는 함수
    public void Blank()
    {
        if (!isBlank)
        {
            isBlank = true;
            IBlankAnim = CoBlank();
            StartCoroutine(IBlankAnim);
        }
    }


    #region Sprite Blank Coroutine
    // 피격시 깜빡이는 코루틴
    IEnumerator CoBlank()
    {
        //Debug.Log("Start Blink");

        int count = 0;
        while (true)
        {
            // 흰색을 하고 있는 마스크 Sprite를 켰다 껐다를 반복한다.
            maskRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
            // 켜져있는 시간을 조금 길게 함으로 보스에대해 인지를 하기 쉽게 해준다.
            maskRenderer.enabled = false;
            yield return new WaitForSeconds(0.2f);

            if (++count >= 8) { isBlank = false; yield break; }
        }
    }
    #endregion


    #region Looking Player Coroutine
    // 플레이어를 쳐다보게 만드는 코루틴
    IEnumerator CoLookPlayer()
    {
        //Debug.Log("Look Player");
        //while (true)
        //{
        //yield return new WaitForSeconds(0.1f);
        isTurn = true;
        SpriteChange(turnSprites[0]);

        yield return new WaitForSeconds(0.15f);

        Vector2 dir = playerTr.position - polyCollider2D.bounds.center;
        // 플레이어가 왼쪽에 있는 경우
        if (dir.x < 0) { flipX = false; }
        // 플레이어가 오른쪽에 있는 경우
        else { flipX = true; }

        isTurn = false;
        yield break;
    }
    //}
    #endregion

}
