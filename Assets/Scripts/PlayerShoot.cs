using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 12f;

    [Header("Cursor")]
    public Texture2D cursorTexture;

    [SerializeField] private AudioSource shoot1_audioSource;
    [SerializeField] private AudioClip playerShootClip1;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            shoot1_audioSource.Play();
            Shoot();
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 mouseWorld =
            Camera.main.ScreenToWorldPoint(mouseScreen);

        mouseWorld.z = 0f;

        // AIM FROM PLAYER CENTER
        Vector2 direction =
            (mouseWorld - transform.position).normalized;

        // SPAWN FROM WEAPON
        GameObject projectile =
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb =
            projectile.GetComponent<Rigidbody2D>();

        rb.linearVelocity = direction * projectileSpeed;
    }
}