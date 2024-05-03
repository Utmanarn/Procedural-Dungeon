using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform _target;
    private Rigidbody2D _rb;

    private bool _targetInRange;
    
    [SerializeField] private float targetingDistance = 2f;
    [SerializeField] private float movementSpeed = 1f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
    }

    private void FixedUpdate()
    {
        CheckForTargets();
        if (!_targetInRange) return;
        WalkTowardsTarget();
        AttackTarget();
    }

    private void CheckForTargets()
    {
        if (Vector2.Distance(transform.position, _target.position) < targetingDistance)
        {
            _targetInRange = true;
        }
        else
        {
            _targetInRange = false;
        }
    }

    private void WalkTowardsTarget()
    {
        // Use the rigidbory MovePosition to move the enemy towards the player.
        Vector2 targetDifferenceNorm = _target.position - transform.position;
        _rb.MovePosition(_rb.position + targetDifferenceNorm * (movementSpeed * Time.fixedDeltaTime * 5f));
    }

    private void AttackTarget()
    {
        // Check if they are within attack range and then perform an attack if the statement returns true.
    }
}
