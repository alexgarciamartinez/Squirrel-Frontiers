using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class Rat : Enemy
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
        ChangeState(EnemyStates.Rat_Idle);
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
            case EnemyStates.Rat_Idle:
                rb.velocity = new Vector2(0, 0);

                if (_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Rat_Chase);
                }
                break;
            case EnemyStates.Rat_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                if(_dist > chaseDistance)
                {
                    ChangeState(EnemyStates.Rat_Idle);
                }
                FlipRat();
                break;
            case EnemyStates.Rat_Stunned:
                timer += Time.deltaTime;

                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.Rat_Idle);
                    timer = 0;
                }
                break;
            case EnemyStates.Rat_Death:
                Death(1);
                break;
                
        }
    }

    public override void EnemyHit(float _damageDone , Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone , _hitDirection, _hitForce);

        if (health > 0)
        {
            ChangeState(EnemyStates.Rat_Stunned);
        }
        else 
        {
            GameManager.Instance.SumarPuntos(10);
            ChangeState(EnemyStates.Rat_Death);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Rat_Idle);
        anim.SetBool("Run", GetCurrentEnemyState == EnemyStates.Rat_Chase);
        anim.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.Rat_Stunned);

        if (GetCurrentEnemyState == EnemyStates.Rat_Death)
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
        anim.SetTrigger("Slash");
        ChangeState(EnemyStates.Rat_Chase);
    }
}
