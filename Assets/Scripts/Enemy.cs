using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;

    [SerializeField] protected float damage;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;

    protected enum EnemyStates
    {
        //Rat
        Rat_Idle,
        Rat_Hitted,
        Rat_Flip,
        Rat_Chase,
        Rat_Stunned,
        Rat_Death,
        // Boss
        Boss_Idle,
        Boss_Chase,
        Boss_Attack,
        Boss_Death
    }

    protected EnemyStates currentEnemyState;

    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if (currentEnemyState != value)
            {
                currentEnemyState = value;
                ChangeCurrentAnimation();
            }
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        /*if (health <= 0)
        {
            Destroy(gameObject);
        }*/
        if (GameManager.Instance.gameIsPaused) return;

        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateEnemyStates();
        }
    }

    public virtual void EnemyHit(float _damageDone , Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if(!isRecoiling)
        {
            //rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            rb.velocity = -_hitForce * recoilFactor * _hitDirection;
            isRecoiling = true;
        }
    }

    protected void OnTriggerStay2D(Collider2D _other)
    {
        Debug.Log("Colisión realizada");
        if (_other.CompareTag("Player"))
        {
            Debug.Log("Ataque ENEMY");
            Attack();
        }
    }

    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }

    protected virtual void UpdateEnemyStates()
    {

    }

    protected virtual void ChangeCurrentAnimation()
    {

    }

    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }

    protected virtual void Attack()
    {
        if (!PlayerController.Instance.pState.invincible && !PlayerController.Instance.pState.supersquirrel)
        {
            PlayerController.Instance.TakeDamage(damage);
        }
    }
}
