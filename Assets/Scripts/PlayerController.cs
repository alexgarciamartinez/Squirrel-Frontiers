using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settigns")]
    [SerializeField] private float walkSpeed = 1;

    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 30;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0; 
    [SerializeField] private int maxAirJumps;
    private float gravity;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    private bool canDash = true, dashed;
    [Space(5)]

    private float CastDistance = 0.95f;
    private Vector2 boxSize = new Vector2(0.50f, 0.1f);
    private Vector3 boxOffset = new Vector3(0.06f, 0f);

    [HideInInspector] public PlayerStateList pState;
    private Rigidbody2D rb;
    private float xAxis, yAxis;
    Animator anim;

    [Header("Attack Settings:")]
    [SerializeField] private Transform SideAttackTransform; 
    [SerializeField] private Vector2 SideAttackArea; 
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float timeBetweenAttack; 
    [SerializeField] float damage;
    private float timeSinceAttack;
    private bool attack = false;
    [SerializeField] GameObject slashEffect;
    [Space(5)]

    [Header("Recoil Settings:")]
    [SerializeField] int recoilXSteps = 2;
    [SerializeField] int recoilYSteps = 2;
    [SerializeField] private float recoilXSpeed = 5;
    [SerializeField] private float recoilYSpeed = 5;
    private int stepsXRecoiled, stepsYRecoiled;
    [Space(5)]

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    [Space(5)]

    private int acornsCounter = 0;

    [SerializeField] private GameObject sceneTransition;
    private int callCount = 0;
    private System.DateTime lastCallTime = System.DateTime.Now;

    private AudioSource audioSource;

    bool hasLost = false;

    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else 
        {
            Instance = this;
        }

        health = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        pState = GetComponent<PlayerStateList>();

        gravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;
        
        GetInputs();
        UpdateJumpVariables();

        if (pState.dashing) return;
        Flip();
        Move();
        Jump();
        StartDash();
        Attack();
        Recoil();

        if (pState.supersquirrel)
        {
            LessAcorns();
        }
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetMouseButtonDown(0);
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false; 
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true; 
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Running", rb.velocity.x != 0 && Grounded());
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;

        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            AudioManager.Instance.SlashSFX();
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, Vector2.up, recoilXSpeed);
                //Instantiate(slashEffect, SideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                SlashEffectAngle(slashEffect, 90, SideAttackTransform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        List<Enemy> hitEnemies = new List<Enemy>();

        if(objectsToHit.Length > 0)
        {
            _recoilBool = true;
        } 

        for(int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy e = objectsToHit[i].GetComponent<Enemy>();
            if(e && !hitEnemies.Contains(e))
            {
                e.EnemyHit
                //(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
                (damage, _recoilDir, _recoilStrength);
                hitEnemies.Add(e);
                Instantiate(slashEffect, SideAttackTransform);
                Debug.Log("HITEADO");
            }
        }
    }

    void SlashEffectAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil()
    {
        if(pState.recoilingX)
        {
            if(pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if(pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {                
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else 
        {
            StopRecoilX();
        }

        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else 
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }

    }

    public void TakeDamage(float _damage)
    {
        health -= Mathf.RoundToInt(_damage);
        GameManager.Instance.PerderVida();
        StartCoroutine(StopTakingDamage());
    }

    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        anim.SetTrigger("TakeDamage");
        ClampHealth();
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    public bool Grounded()
    {
        /*if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) 
           || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
           || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else 
        {
            return false;
        }*/
        return Physics2D.BoxCast(transform.position + boxOffset, boxSize, 0, - transform.up, CastDistance, whatIsGround);
    }

    void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            pState.jumping = false;
            rb.velocity = new Vector2 (rb.velocity.x, 0);
        }

        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3 (rb.velocity.x, jumpForce);
                pState.jumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;
                airJumpCounter++;
                rb.velocity = new Vector3 (rb.velocity.x, jumpForce);

            }
        }

        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("NormalAcorn"))
        {
            Debug.Log("BELLOTA PILLADA");
            AudioManager.Instance.AcornSFX();
            acornsCounter++;
            GameManager.Instance.SumarPuntos(1);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("ChaosAcorn"))
        {
            Debug.Log("Bellota del caos pillada");
            AudioManager.Instance.AcornSFX();
            GameManager.Instance.SumarChaos(1);
            Destroy(other.gameObject);
            sceneTransition.SetActive(true);
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireCube(transform.position - transform.up * CastDistance + boxOffset, boxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
    }

    private void LessAcorns()
    {
        if (!hasLost && GameManager.Instance.PuntosTotales > 0)
        {
            if ((System.DateTime.Now - lastCallTime).TotalSeconds >= 1)
            {
                GameManager.Instance.RestarPuntos(1);
                callCount++;
                lastCallTime = System.DateTime.Now;
            }
        }
        else 
        {
            if (!hasLost) // Solo ejecuta el bloque si no se ha perdido previamente
            {
                GameManager.Instance.vidas = 1;
                GameManager.Instance.PerderVida();

                hasLost = true; // Marca que se ha perdido para evitar ejecuciones repetidas
            }
        }
    }
}
