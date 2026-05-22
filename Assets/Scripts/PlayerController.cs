using Bullets;
using Events;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BulletPooling bulletPooling;
    public float movementSpeed;
    private Vector2 moveDirection;
    [SerializeField]
    private float invincibleTime;
    [HideInInspector]
    public float invincibleTimeBuffer;

    [Header("Attacking")] 
    [SerializeField] 
    private float attackRate;
    private float attackRateBuffer;
    private bool isFiring;
    private Vector2 shootDirection;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletPooling = GetComponent<BulletPooling>();
    }

    private void Start()
    {
        invincibleTimeBuffer = invincibleTime;
    }

    private void OnEnable()
    {
        GameEventManager.Instance.inputEvents.MovePressed += UpdatePlayerMoveDirection;
        GameEventManager.Instance.inputEvents.AttackPressed += Attack;
        GameEventManager.Instance.levelEvents.LevelTimerFinished += DisableControlsOnLevelTimerEnd;
        GameEventManager.Instance.playerStatEvents.SpeedChange += SpeedChange;
        GameEventManager.Instance.playerStatEvents.FireRateChange += FireRateChange;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.inputEvents.MovePressed -= UpdatePlayerMoveDirection;
        GameEventManager.Instance.inputEvents.AttackPressed -= Attack;
        GameEventManager.Instance.levelEvents.LevelTimerFinished -= DisableControlsOnLevelTimerEnd;
        GameEventManager.Instance.playerStatEvents.SpeedChange -= SpeedChange;
        GameEventManager.Instance.playerStatEvents.FireRateChange -= FireRateChange;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void UpdatePlayerMoveDirection(InputAction.CallbackContext direction)
    {
        moveDirection = direction.ReadValue<Vector2>();
    }

    private void Move()
    {
        // Move
        rb.MovePosition(rb.position + moveDirection * (movementSpeed * Time.fixedDeltaTime));
    }

    private void Update()
    {
        invincibleTimeBuffer -= Time.deltaTime;
        attackRateBuffer -= Time.deltaTime;

        if (isFiring) Shoot();
    }

    public void MakeInvincible()
    {
        invincibleTimeBuffer = invincibleTime;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        shootDirection = context.ReadValue<Vector2>();
        isFiring = shootDirection.magnitude > .1f;
    }

    public void Shoot()
    {
        // Can't shoot yet
        if (attackRateBuffer > 0) return;
        var bullet = bulletPooling.GetPooledObject();
        // Failsafe
        if (!bullet) return;
        bullet.Initialize(transform.position, shootDirection);
        GetComponent<ParticleThingo>().SpawnParticle();
        attackRateBuffer = attackRate;
    }

    private void DisableControlsOnLevelTimerEnd()
    {
        enabled = false;
    }

    private void SpeedChange(int amount)
    {
        movementSpeed += amount;
    }

    /// <summary>
    /// Fire rate needs to DECREASE to get better!!! Unlike literally everything else
    /// </summary>
    /// <param name="amount"></param>
    private void FireRateChange(float amount)
    {
        attackRate -= amount;
    }
}
