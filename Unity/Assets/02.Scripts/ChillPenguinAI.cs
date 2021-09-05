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

    // 특정 횟수마다 Component Reset 실행
    short settingCount = 0;

    // Sprite Renderer의 Draw Mode
    SpriteDrawMode drawMode;
    // Sprite Renderer의 Tile Mode
    SpriteTileMode tileMode;
    // Sprite Renderer의 Order in Layout 값
    int sortLayer;

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


        //IsGround();
        //sprite = spriteRenderer.sprite;
        SpriteChange(spriteRenderer.sprite);
        StartCoroutine(CoBossPattern());
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
        Debug.DrawRay(polyCollider2D.bounds.center, -transform.up, Color.red, (polyCollider2D.bounds.size.y / 2) + 0.01f);
        IsGround();
        // Trigger 였을 때 중력 만들 때 사용한 것.
        //GetGravity();

        //if(gameObject.compon)
        if (settingCount >= 10)
        {
            // 일정 주기마다 Sprite Renderer Reset
            ComponentReset();
        }

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

        if (settingCount++ >= 10)
        {
            ComponentReset();
            settingCount = 0;
        }
        //prePos = rectTr.position;

        // 새로운 스프라이트를 변경해주는 코드.
        if (TryGetComponent<SpriteRenderer>(out spriteRenderer))
        {
            //Debug.Log("Sprite Change");
            spriteRenderer.flipX = flipX;
            spriteRenderer.sprite = sprite;
            //maskRenderer.sprite = sprite;
            //maskRenderer.flipX = flipX;
            maskSprite.sprite = sprite;
            if (flipX == true)
            {
                Vector3 maskScale = maskRectTransform.localScale;
                maskScale.x = -1f;
                maskRectTransform.localScale = maskScale;
            }
            else
            {
                Vector3 maskScale = maskRectTransform.localScale;
                maskScale.x = 1f;
                maskRectTransform.localScale = maskScale;
            }

        }
    }


    #region Boss Pattern Coroutine
    // 보스 패턴을 설정하는 코루틴
    IEnumerator CoBossPattern()
    {
        Debug.Log("Pattern Start");

        yield return StartCoroutine(CoStartAnim());


        while (true)
        {
            yield return StartCoroutine(CoLookPlayer());

            int num = Random.Range(1, 2);
            isAttack = true;
            switch (num)
            {
                case 0:
                    Debug.Log("_____DASH_____");

                    yield return StartCoroutine(StopAnimationCoroutine());
                    yield return StartCoroutine(CoDashAnim());

                    break;
                case 1:
                    Debug.Log("____JUMP____");

                    yield return StartCoroutine(StopAnimationCoroutine());
                    yield return StartCoroutine(CoJumpAnim());


                    break;
                default:


                    break;
            }


            isAttack = false;

            yield return StartCoroutine(StopAnimationCoroutine());

            yield return StartCoroutine(CoLookPlayer());

            yield return StartCoroutine(CoIdleAnim());

            Debug.Log("Pattern Loop");
        }

        Debug.Log("Pattern End");
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



        if (collision.gameObject.CompareTag("PLAYER"))
        {

            Debug.Log("COLL PLAYER");
        }


        //Collider2D coll2d = collision.gameObject.GetComponent<Collider2D>();
        //Vector2 pos = coll2d.ClosestPoint(polyCollider2D.bounds.center);
        //Debug.Log(collision.collider.name + ": " + pos);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.CompareTag("BULLET"))
        //{
        //    float v = collision.bounds.center.x - polyCollider2D.bounds.center.x;
        //    Debug.Log("V: " + v);

        //}

        if (collision.gameObject.CompareTag("WALL"))
        {
            currSpeed *= -1f;
            flipX = !flipX;
            Debug.Log("____Reflect____");
            //Debug.Log(rb2d.velocity);
            //Vector2 vector = new Vector2(rb2d.velocity.x * -1, rb2d.velocity.y);
            //Vector2 vector = Vector2.Reflect(rb2d.velocity, -collision.contacts[0].normal);
            //rb2d.velocity = vector;
            //Debug.Log(rb2d.velocity);

        }

        if (collision.CompareTag("PLAYER"))
        {

            Debug.Log("TRIGGER PLAYER");
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
        Debug.Log("Start Start");

        //while (true)
        //{
        // 위에서 떨어지는 것부터 시작하니 떨어지는 애니메이션 코루틴
        yield return StartCoroutine(CoFallAnim());
        // 다른 코루틴이 동작하고 있을 수 있으므로 새로운 애니메이션을 시작하기 전에 다른 애니메이션 코루틴을 멈춰 준다.
        yield return StartCoroutine(StopAnimationCoroutine());
        yield return StartCoroutine(CoReadyAnim());

        yield return StartCoroutine(StopAnimationCoroutine());
        yield return StartCoroutine(CoFillHPGauge());
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(StopAnimationCoroutine());
        yield return StartCoroutine(CoIdleAnim());

        Debug.Log("Start End");
        yield break;
        //}
    }
    #endregion


    #region Fill HP Gauge Coroutine
    // 체력 게이지 0~100%로 채우는 코루틴
    IEnumerator CoFillHPGauge()
    {
        Debug.Log("FILL Gauge Start");

        isFillGauge = true;
        while (true)
        {
            HPGauge.fillAmount += 0.05f;
            yield return new WaitForFixedUpdate();

            if (HPGauge.fillAmount >= 1)
            {
                yield return new WaitForSeconds(0.3f);
                isFillGauge = false;

                Debug.Log("Fill Gauge End");
                yield break;
            }
        }

    }
    #endregion


    #region Fall Animation Coroutine
    // 떨어지는 애니메이션 코루틴
    IEnumerator CoFallAnim()
    {
        Debug.Log("Fall Start");

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

        Debug.Log("Fall End");
        yield break;
        //}
    }
    #endregion


    #region Idle Animation Coroutine
    // 기본 동작 애니메이션
    IEnumerator CoIdleAnim()
    {
        Debug.Log("Idle Start");

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
            StartCoroutine(CoLookPlayer());
            if (count >= random)
            {
                Debug.Log("Idle End");

                yield break;
            }
        }
    }
    #endregion


    #region Damaged Animation Coroutine
    // 공격 받았을 때의 애니메이션
    IEnumerator CoDamagedAnim()
    {
        Debug.Log("Damage Anim");

        //while (isDamaged)
        //{
        SpriteChange(damagedSprites[0]);
        //sprite = damagedSprites[0];
        yield return new WaitForSeconds(0.3f);

        if (isGround)
        {
            isDamaged = false;

            Debug.Log("Damage End");

            yield break;
        }
        //}
    }
    #endregion


    #region Jump Animation Coroutine
    // 점프 애니메이션
    IEnumerator CoJumpAnim()
    {
        Debug.Log("Jump Start");

        isJump = true;
        SetAttackState(isJump);

        attackVector.x = playerTr.position.x > polyCollider2D.bounds.center.x ? 1 : -1;

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


        Debug.Log("Jump End");

        yield break;
        //}
    }
    #endregion




    #region Ready Animation Coroutine
    // 준비 동작 애니메이션 (보스 등장시 체력 차는 중)
    IEnumerator CoReadyAnim()
    {
        isReady = true;
        Debug.Log("Ready Start");

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

        Debug.Log("Ready End");

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

        Debug.Log("Dash Start");
        //while (true)
        //{

        SpriteChange(dashSprites[1]);
        //rb2d.AddForce(-transform.right * 60f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.3f);

        Debug.Log("Dash End");


        isDash = false;

        SetAttackState(isDash);

        yield break;
        //}


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
        Debug.Log("Stop Start");

        if (isFall)
        {
            isFall = false;
            StopCoroutine(CoFallAnim());
        }
        if (isDamaged)
        {
            isDamaged = false;
            StopCoroutine(CoDamagedAnim());
        }
        if (isIdle)
        {
            isIdle = false;
            StopCoroutine(CoIdleAnim());
        }
        if (isJump)
        {
            isJump = false;
            StopCoroutine(CoJumpAnim());
        }
        if (isReady)
        {
            isReady = false;
            StopCoroutine(CoReadyAnim());
        }
        if (isDash)
        {
            isDash = false;
            StopCoroutine(CoDashAnim());
        }
        if (isFillGauge)
        {
            isFillGauge = false;
            StopCoroutine(CoFillHPGauge());
        }
        if (isTurn)
        {
            isTurn = false;
            StopCoroutine(CoLookPlayer());
        }

        Debug.Log("Stop End");
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
            StartCoroutine(CoDamagedAnim());
            Blank();
        }

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
