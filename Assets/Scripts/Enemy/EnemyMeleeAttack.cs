using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D rb;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 10f;
    public float chargeSpeed = 7f;

    [Header("Ranges")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    [Header("Attack Timing")]
    public float attackCooldown = 2f;

    public bool isAttacking;
    public bool jumpAttack;
    public bool slashAttack;
    private float lastAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float dist = Vector2.Distance(transform.position, player.position);
        //Debug.Log(dist);

        if (dist > detectionRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(DoAttackDecision());
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    IEnumerator DoAttackDecision()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > 7f)
        {
            if (Random.value > 0.5f)
                yield return StartCoroutine(JumpToPositionAOE());
            else
                yield return StartCoroutine(DashSlashThrough());
        }

        else if (dist > 4f)
        {
            if (Random.value > 0.5f)
                yield return StartCoroutine(ChargeDashHeavy());
            else
                yield return StartCoroutine(RunChargeHeavy());
        }

        else if (dist > 2f)
        {
            yield return StartCoroutine(ChargeDashHeavy());
        }

        else if (dist > 1f)
        {
            yield return StartCoroutine(SmallWalkAttack());
        }

        lastAttackTime = Time.time;
        isAttacking = false;
    }

    // 1. charge -> dash -> heavy attack
    IEnumerator ChargeDashHeavy()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        Vector2 dir = (player.position - transform.position).normalized;

        float dashDistance = 5f;
        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + dir * dashDistance;

        float dashTime = 0.2f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / dashTime;

            transform.position =
                Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        HeavyAttack();

        yield return new WaitForSeconds(1f);

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    // 2. dash through player
    IEnumerator DashSlashThrough()
    {
        isAttacking = true;
        slashAttack = true;

        Vector2 dir = (player.position - transform.position).normalized;

        float dashDistance = 3f;
        Vector2 startPos = transform.position;
        Vector2 targetPos = (Vector2)player.position + dir * dashDistance;

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.15f);

        float t = 0f;
        float duration = 0.25f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        SlashAttack();

        slashAttack = false;
        isAttacking = false;
    }

    // 3. walk to player -> small attack
    IEnumerator SmallWalkAttack()
    {
        isAttacking = true;

        while (Vector2.Distance(transform.position, player.position) > attackRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * walkSpeed;

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.1f);

        SmallAttack();

        isAttacking = false;
    }

    // 4. run -> charge -> heavy attack
    IEnumerator RunChargeHeavy()
    {
        isAttacking = true;
        slashAttack = true;

        Vector2 targetPos = player.position;

        float runTime = 0.4f;
        float t = 0f;

        Vector2 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / runTime;

            Vector2 nextPos =
                Vector2.Lerp(startPos, targetPos, t);

            rb.MovePosition(nextPos);

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.3f);

        Vector2 dir =
            (player.position - transform.position).normalized;

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.2f);

        rb.linearVelocity = dir * chargeSpeed;

        yield return new WaitForSeconds(0.25f);

        rb.linearVelocity = Vector2.zero;

        RunHeavyAttack();

        yield return new WaitForSeconds(0.6f);

        slashAttack = false;
        isAttacking = false;
    }

    // 5. jump to position aoe
    IEnumerator JumpToPositionAOE()
    {
        isAttacking = true;

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(2.5f);

        float duration = 0.6f;
        float t = 0f;

        Vector2 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            Vector2 targetPos = player.position;

            Vector2 flatPos = Vector2.Lerp(startPos, targetPos, t);

            float height = 2f;
            float arc = Mathf.Sin(t * Mathf.PI) * height;

            transform.position = new Vector2(flatPos.x, flatPos.y + arc);

            yield return null;
        }

        transform.position = player.position;
        jumpAttack = true;

        AOEAttack(player.position);

        yield return new WaitForSeconds(0.4f);

        jumpAttack = false;
        isAttacking = false;
    }


    void SmallAttack()
    {
        Debug.Log("Small Attack");
    }

    void HeavyAttack()
    {
        Debug.Log("Charge-Dash-Heavy Attack");
    }

    void RunHeavyAttack()
    {
        Debug.Log("Run-Heavy Attack");
    }

    void SlashAttack()
    {
        Debug.Log("Slash Through Attack");
    }

    void AOEAttack(Vector2 pos)
    {
        Debug.Log("AOE at " + pos);
    }
}
