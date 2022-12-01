using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("references")]
    [SerializeField] Transform _orientation;
    [SerializeField] Transform _playerCamera; //guarda
    private Rigidbody _rb;
    private PlayerMovement _playerMovement;

    [Header("Dashing Stats")]
    [SerializeField] float _dashForce;
    [SerializeField] float _dashUpwardForce;
    [SerializeField] float _dashDuration;

    private Vector3 _delayedDashForceToApply;
    private Vector3 _dashDirection;

    [SerializeField] float _dashCooldown;
    private float _dashCooldownTimer;

    [Header("TEMPORARY KEYCODE HASTA Q ESTE EL SCRIPT DE INPUTS")]
    public KeyCode dashKey = KeyCode.E;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerMovement = GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        if(Input.GetKeyDown(dashKey))
        {
            Dash();
        }
        if (_dashCooldownTimer > 0)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }
    }
    private void Dash()
    {
        if(_dashCooldownTimer > 0)
        {
            return;
        }
        else
        {
            _dashCooldownTimer = _dashCooldown;

            _playerMovement.IsDashing = true;

            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);//Reset de velocidad en Y al dashear

            Vector3 direction = GetDirection();

            _delayedDashForceToApply = direction * _dashForce + _orientation.up * _dashUpwardForce;



            Invoke(nameof(DelayedDashForce), 0.025f);
            Invoke(nameof(ResetDash), _dashDuration);
        }
        
    }
    private void DelayedDashForce()
    {
        _rb.AddForce(_delayedDashForceToApply, ForceMode.Impulse);
    }
    private void ResetDash()
    {
        _playerMovement.IsDashing = false;
    }
    private Vector3 GetDirection() //no le paso nada a diferencia del tuto ya q quiero q tenga 8 direcciones para dashear.
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        _dashDirection = _orientation.forward * verticalInput + _orientation.right * horizontalInput;

        if(verticalInput == 0 && horizontalInput == 0)
        {
            _dashDirection = _orientation.forward;
        }
        return _dashDirection.normalized;
    }
}
