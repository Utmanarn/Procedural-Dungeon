using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0.7f;
    [SerializeField] private int damage = 1;
    [SerializeField] private Transform attackPoint;

    private Collider2D[] _detectedColliders = new Collider2D[] {};

    private float _attackCooldownTimer = 0f;
    private bool _attackOnCooldown = false;

    public void Attack()
    {
        if (_attackOnCooldown) return;
        Debug.Log("Attack performed.");
        float angle = transform.rotation.eulerAngles.z;
        Debug.Log("Angle: " + angle);
        _detectedColliders = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(2f, 1f), angle);
        DebugDrawBox(attackPoint.position, new Vector2(2f, 1f), angle, Color.blue, 0.7f); // DEBUGGING
        _attackOnCooldown = true;
        _attackCooldownTimer = attackCooldown;

        foreach (var detectedCollider in _detectedColliders)
        {
            if (detectedCollider.TryGetComponent(out Enemy enemy))
            {
                Debug.Log("Enemy hit.");
                // enemy.TakeDamage(damage);
            }
        }
    }

    private void Update()
    {
        AttackCooldownCheck();
    }

    private void AttackCooldownCheck()
    {
        _attackCooldownTimer -= Time.deltaTime;
        if (_attackCooldownTimer > 0) return;
        _attackOnCooldown = false;
    }
    
    void DebugDrawBox(Vector2 point, Vector2 size, float angle, Color color, float duration)
    {

        var orientation = Quaternion.Euler(0, 0, angle);

        // Basis vectors, half the size in each direction from the center.
        Vector2 right = orientation * Vector2.right * size.x / 2f;
        Vector2 up = orientation * Vector2.up * size.y / 2f;

        // Four box corners.
        var topLeft = point + up - right;
        var topRight = point + up + right;
        var bottomRight = point - up + right;
        var bottomLeft = point - up - right;

        // Now we've reduced the problem to drawing lines.
        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }
}
