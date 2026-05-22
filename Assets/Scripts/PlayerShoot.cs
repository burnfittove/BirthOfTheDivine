using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 12f;

    [Header("Cursor")]
    public Texture2D cursorTexture;

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
            Shoot();
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        Vector3 mouseWorld =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mouseWorld.z = 0f;

        Vector2 direction =
            (mouseWorld - firePoint.position).normalized;

        GameObject projectile =
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb =
            projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}