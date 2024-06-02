using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField] private float flipWaitTime;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;
    float timer;
    float stunDuration = 2;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Boss_Idle);
        rb.gravityScale = 12f;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        /*if (!isRecoiling)
        {
            transform.position = Vector2.MoveTowards
                (transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }*/
        UpdateEnemyStates();
    }

    protected override void UpdateEnemyStates()
    {
        /*if (health <= 0)
        {
            Death(0.05f);
        }*/
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Boss_Idle:
                rb.velocity = new Vector2(0, 0);

                if (_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Boss_Chase);
                }
                break;
            case EnemyStates.Boss_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                if(_dist > chaseDistance)
                {
                    ChangeState(EnemyStates.Boss_Idle);
                }
                FlipRat();
                break;
            case EnemyStates.Boss_Death:
                Death(1);
                break;
                
        }
    }

    public override void EnemyHit(float _damageDone , Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone , _hitDirection, _hitForce);

        if (health <= 0)
        {
            ChangeState(EnemyStates.Boss_Death);
            GameManager.Instance.WinGame();
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Boss_Idle);
        anim.SetBool("Run", GetCurrentEnemyState == EnemyStates.Boss_Chase);

        if (GetCurrentEnemyState == EnemyStates.Boss_Death)
        {
            anim.SetTrigger("Die");
        }
    }

    void FlipRat()
    {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }

    protected override void Attack()
    {
        Debug.Log("TE ATACO");
        base.Attack();
        anim.SetTrigger("Attack");
        ChangeState(EnemyStates.Boss_Attack);
        ChangeState(EnemyStates.Boss_Chase);
    }
}
