using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    public Transform player;
    public Rigidbody2D playerRb;
    public EnemyMeleeAttack enemyMeleeAttack;

    [Header("Follow")]
    public float smoothTime = 0.2f;
    public Vector3 offset;

    [Header("Look Ahead")]
    public float lookAheadDistance = 2f;
    public float lookAheadSmooth = 5f;

    private Vector3 velocity;
    private Vector3 currentLookAhead;

    [Header("Shake")]
    public float jumpShakeIntensity = 0.5f;
    public float slashShakeIntensity = 0.1f;

    void LateUpdate()
    {
        if (player == null) return;

        Vector2 playerVelocity = playerRb.linearVelocity;

        Vector3 targetLookAhead =
            new Vector3(playerVelocity.x, playerVelocity.y, 0) * lookAheadDistance;

        currentLookAhead = Vector3.Lerp(
            currentLookAhead,
            targetLookAhead,
            lookAheadSmooth * Time.deltaTime
        );

        Vector3 targetPos =
            player.position + offset + currentLookAhead;

        targetPos.z = -10f;

        Vector3 basePos = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );

        Vector3 shake = Vector3.zero;

        if (enemyMeleeAttack != null && enemyMeleeAttack.jumpAttack)
        {
            shake = Random.insideUnitCircle * jumpShakeIntensity;
        }
        else if (enemyMeleeAttack != null && enemyMeleeAttack.slashAttack)
        {
            shake = Random.insideUnitCircle * slashShakeIntensity;
        }

        transform.position = basePos + shake;
    }
}