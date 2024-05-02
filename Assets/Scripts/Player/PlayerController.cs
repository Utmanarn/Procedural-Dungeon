using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private PlayerAttack _playerAttack;
    
    private float _movementSpeed = 1f;
    private Vector2 _playerMovementInputNorm;
    private bool _attackInput = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerAttack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        if (_attackInput)
        {
            PerformAttack();
            _attackInput = false;
        }
    }

    private void GetInput()
    {
        _playerMovementInputNorm = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (Input.GetKeyDown(KeyCode.F))
        {
            _attackInput = true;
        }
    }

    private void UpdateMovement()
    {
        if (_playerMovementInputNorm == Vector2.zero) return;
        _rb.MovePosition(_rb.position + _playerMovementInputNorm * (_movementSpeed * Time.fixedDeltaTime * 5f));
        _rb.transform.up = _playerMovementInputNorm;
    }

    private void PerformAttack()
    {
        _playerAttack.Attack();
    }
}
