using System;
using Bullets;
using Events;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    private Animator animator;
    private SpriteRenderer sr;

    [SerializeField]
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

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    private float lastDashTime;
    private bool isDashing;
    private bool shot;

    private bool walk_grass;
    private bool walk_tile;


    // [Header("Audio Controller")]
    // [SerializeField] private AudioSource dash_audioSource;
    // [SerializeField] private AudioSource walk_audioSource;
    // [SerializeField] private AudioSource shoot1_audioSource; 
    //
    // [SerializeField] private AudioClip playerDashClip;
    // [SerializeField] private AudioClip playerShootClip1;
    //
    // [SerializeField] private AudioClip walk_grassa;
    // [SerializeField] private AudioClip walk_tilea;

    private bool isWalkingSoundPlaying;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletPooling = GetComponent<BulletPooling>();

        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        animator = GetComponent<Animator>(); 

        GameEventManager.Instance.inputEvents.MovePressed += UpdatePlayerMoveDirection;
        GameEventManager.Instance.inputEvents.AttackPressed += Attack;
        GameEventManager.Instance.levelEvents.LevelTimerFinished += DisableControlsOnLevelTimerEnd;
        GameEventManager.Instance.sceneEvents.SceneLoaded += EnableControlsOnSceneChanged;
        
        //invincibleTime = PlayerStatManager.Instance.invincibilityTimer;
        movementSpeed = PlayerStatManager.Instance.speed;
        attackRate = PlayerStatManager.Instance.fireRate;
        
        invincibleTimeBuffer = invincibleTime;
    }

    private void OnEnable()
    {
        // GameEventManager.Instance.inputEvents.MovePressed += UpdatePlayerMoveDirection;
        // GameEventManager.Instance.inputEvents.AttackPressed += Attack;
        
    }

    private void OnDisable()
    {
        GameEventManager.Instance.inputEvents.MovePressed -= UpdatePlayerMoveDirection;
        GameEventManager.Instance.inputEvents.AttackPressed -= Attack;
        GameEventManager.Instance.levelEvents.LevelTimerFinished -= DisableControlsOnLevelTimerEnd;
        GameEventManager.Instance.sceneEvents.SceneLoaded -= EnableControlsOnSceneChanged;
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
        if (isDashing) return;

        // walk_audioSource.PlayOneShot(walk_grassa);
        //if (walk_grass)
        //{
        //    PlaySfx(walk_grassa);
        //}
        //else PlaySfx(walk_tilea);

            rb.MovePosition(rb.position + moveDirection * (movementSpeed * Time.fixedDeltaTime));
    }

    private void HandleWalkSound()
    {
        bool isMoving = moveDirection.magnitude > 0.1f;

        if (isMoving && !isWalkingSoundPlaying)
        {
            // walk_audioSource.clip = walk_grassa;
            // walk_audioSource.loop = true;
            // walk_audioSource.Play();

            isWalkingSoundPlaying = true;
        }
        else if (!isMoving && isWalkingSoundPlaying)
        {
            // walk_audioSource.Stop();
            isWalkingSoundPlaying = false;
        }
    }

    private void Update()
    {
        UpdateAnimations();
        HandleWalkSound();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time >= lastDashTime + dashCooldown)
            {
                StartCoroutine(Dash());
            }
        }

        invincibleTimeBuffer -= Time.deltaTime;
        attackRateBuffer -= Time.deltaTime;

        if (isFiring)
        {
            // shoot1_audioSource.Play();
            Shoot();
        }
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
        shot = true;
        if (attackRateBuffer > 0) return;

        var bullet = bulletPooling.GetPooledObject();
        if (!bullet) return;

        bullet.Initialize(transform.position, shootDirection);
        GetComponent<ParticleThingo>()?.SpawnParticle();
        attackRateBuffer = attackRate;
    }

    private void DisableControlsOnLevelTimerEnd()
    {
        GameEventManager.Instance.inputEvents.MovePressed -= UpdatePlayerMoveDirection;
        GameEventManager.Instance.inputEvents.AttackPressed -= Attack;
        moveDirection = Vector2.zero;
        isFiring = false;
    }

    private void EnableControlsOnSceneChanged()
    {
        GameEventManager.Instance.inputEvents.MovePressed += UpdatePlayerMoveDirection;
        GameEventManager.Instance.inputEvents.AttackPressed += Attack;
        Debug.Log("skibidi"); 
        transform.position = new Vector3(0, 0, transform.position.z);
    }

    private void UpdateAnimations()
    {
        float speed = moveDirection.magnitude;

        animator.SetFloat("MoveX", moveDirection.x);
        animator.SetFloat("MoveY", moveDirection.y);
        animator.SetFloat("Speed", speed);

        if (speed < 0.1f)
            animator.SetFloat("Speed", 0);
    }

    IEnumerator Dash()
    {
        isDashing = true;
        // dash_audioSource.PlayOneShot(playerDashClip);
        lastDashTime = Time.time;

        Vector2 inputDir = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (inputDir == Vector2.zero)
            inputDir = moveDirection;

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.fixedDeltaTime;

            rb.MovePosition(rb.position + inputDir * (dashSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
    }
}