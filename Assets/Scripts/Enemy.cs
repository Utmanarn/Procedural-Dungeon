using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform _target;
    private HealthSystem _targetHealthSystem;
    private Rigidbody2D _rb;
    private HealthSystem _healthSystem;

    private bool _targetInMoveRange;
    private bool _targetInAttackRange;
    private bool _canAttackTarget;

    private float _attackCooldown = 2f;
    private float _attackCooldownTimer = 0f;
    
    [SerializeField] private float targetingDistance = 2f;
    [SerializeField] private float attackDistance = 1.05f;
    [SerializeField] private float movementSpeed = 0.2f;
    [SerializeField] private int damage = 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _healthSystem = GetComponent<HealthSystem>();
    }
    

    private void Start()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
        if (objects.Length <= 0)
        {
            Debug.LogError("Failed to find object with the 'player' tag.");
            return;
        }
        if (objects.Length > 1) Debug.LogWarning("More than 1 object has been tagged as player, are you sure this is the intended behaviour?");
        _target = objects[0].transform;
        UpdateTargetHealthSystem(_target);
    }

    private void Update()
    {
        _attackCooldownTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        CheckForTargets();
        if (_targetInMoveRange) WalkTowardsTarget();
        if (_targetInAttackRange) AttackTarget();
    }

    private void CheckForTargets()
    {
        float distance = Vector2.Distance(transform.position, _target.position);

        if (distance < attackDistance)
        {
            _targetInMoveRange = false;
            _targetInAttackRange = true;
        }
        else if (distance < targetingDistance)
        {
            _targetInMoveRange = true;
            _targetInAttackRange = false;
        }
        else
        {
            _targetInMoveRange = false;
            _targetInAttackRange = false;
        }
    }

    private void WalkTowardsTarget()
    {
        Vector2 targetDifferenceNorm = _target.position - transform.position;
        _rb.MovePosition(_rb.position + targetDifferenceNorm * (movementSpeed * Time.fixedDeltaTime * 0.4f));
    }

    private void AttackTarget()
    {
        if (!_canAttackTarget)
        {
            Debug.Log("Can not attack this target type as it is missing a health system.");
            return;
        }

        if (_attackCooldownTimer > 0f) return;
        Debug.Log("Target attacked.");
        _targetHealthSystem.DamageUnit(damage);

        _attackCooldownTimer = _attackCooldown;
    }

    public void TakeDamage(int damage)
    {
        _healthSystem.DamageUnit(damage);
    }

    // This is not the best way to go about this in a larger project as it is very easy to miss updating this and thus causing unpredictable behaviour.
    private void UpdateTargetHealthSystem(Transform target)
    {
        if (target.TryGetComponent(out HealthSystem health))
        {
            _targetHealthSystem = health;
            _canAttackTarget = true;
        }
        else
        {
            _canAttackTarget = false;
        }
    }
}
