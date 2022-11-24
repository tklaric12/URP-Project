using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Resources utilizados: https://www.youtube.com/watch?v=f473C43s8nE + groundCheck de Brackeys xq el de este tipo no me parecia bueno + physics material al player xq se quedaba pegado en las paredes.
    //GUARDA CON EL PUTO PHYSICS MATERIAL PUESTO EN EL COLLIDER DEL PLAYER X SI NO SE OBTIENEN RESULTADOS DESEADOS SIGUIENDO OTROS TUTOS.
    [Header("Movement")]
    [SerializeField] float _walkingSpeed;
    [SerializeField] float _sprintSpeed;
    float _moveSpeed;

    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpCooldown;
    bool _isReadyToJump;
    [SerializeField] float _airMultiplier;

    
    float _groundDistance = 0.2f;
    [SerializeField] float _groundDrag;

    [Header("KEYBINDING MOMENTANEO VER COMENTARIO")] //se deja este keybind para probar movimiento. Se deberia tener una script aparte con los keybinds del jugador, cosa de cambiar solo esa script cuando haya opciones en el juego para hacerlo y que quien lo necesite recurra a esa script de keybinds, bien podria ser un singleton con read-onlys adentro, salvo modificables al tocar opciones claro.
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    [SerializeField] Transform _groundCheckTransform;
    [SerializeField] float _playerHeight; //para averiguar la scale creas un cubo que mide por default 1, escalas el cubo y lo q te quede de scale en Y es el valor.
    [SerializeField] LayerMask _whatIsGround; //Tener en cuenta que el drag se aplica unicamente cuando el player se encuentra sobre "ground". Si falta drag en otro lado, agregar otra layermask.
    bool _isGrounded;
    Rigidbody _movingObjectRBReference;

    [Header("Slope Handling")]
    [SerializeField] float _maxSlopeAngle; //si ya se pasa de cierto angulo, que se considere una pared
    private RaycastHit _slopeHit;
    private bool _isExitingSlope;

    [SerializeField] Transform _orientation;

    float _horizontalInput;
    float _verticalInput;

    Vector3 moveDirection;

    private Rigidbody rb;

    private MovementState _movementState;

    public enum MovementState
    {
        walking,
        sprinting,
        air
    }
    private void Awake()
    {
        _isReadyToJump = true;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        //ground check
        _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, _groundDistance, _whatIsGround, QueryTriggerInteraction.Ignore);

        MyInput();
        StateHandler();
        SpeedControl();
        

        if (_isGrounded)
        {
            Debug.Log("GROUNDED");
            rb.drag = _groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if (Input.GetKey(jumpKey) && _isReadyToJump && _isGrounded)
        {
            _isReadyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), _jumpCooldown);
        }
    }
    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

        if(IsOnSlope() && !_isExitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * _moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 160f, ForceMode.Force);
            }
        }
        else if(_isGrounded)
        {
            _movingObjectRBReference = GetRBOfObjectImStandingOn();
            if(_movingObjectRBReference != null)
            {
                rb.AddForce(_movingObjectRBReference.velocity * 10, ForceMode.Acceleration);
            }
            rb.AddForce(moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * _moveSpeed * _airMultiplier * 10f, ForceMode.Force);
        }

        rb.useGravity = !IsOnSlope();
    }
    private void StateHandler()
    {
        if(_isGrounded && Input.GetKey(sprintKey))
        {
            _movementState = MovementState.sprinting;
            _moveSpeed = _sprintSpeed;
        }
        else if(_isGrounded)
        {
            _movementState = MovementState.walking;
            _moveSpeed = _walkingSpeed;
        }
        else
        {
            rb.AddForce(Physics.gravity * rb.mass); //GRAVEDAD EXTRA XQ SINO FLOTAS
            _movementState = MovementState.air;
        }
    }
    private void SpeedControl()
    {
        if(IsOnSlope() && !_isExitingSlope)
        {
            if(rb.velocity.magnitude > _moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * _moveSpeed;
            }
        }
        else if(_movingObjectRBReference != null)
        {
            if (rb.velocity.magnitude > _moveSpeed + _movingObjectRBReference.velocity.magnitude)
            {
                rb.velocity = rb.velocity.normalized * (_moveSpeed + _movingObjectRBReference.velocity.magnitude);
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            //limits velocity if needed
            if (flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); //Esto no limita la velocidad en Y, por ende se puede caer muy rapido.
            }
        }

    }
    private void Jump()
    {
        _isExitingSlope = true;
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        _isReadyToJump = true;

        _isExitingSlope = false;
    }
    private Rigidbody GetRBOfObjectImStandingOn() 
    {
        Collider[] colliderArray = Physics.OverlapSphere(_groundCheckTransform.position, _groundDistance, _whatIsGround, QueryTriggerInteraction.Ignore);
        if(colliderArray.Length != 0)
        {
            if (colliderArray[0].GetComponent<Rigidbody>() != null) //check if on a moving object / if the object has an RB
            {
                return colliderArray[0].GetComponent<Rigidbody>();
            }
        }

        return null;
    }
    private bool IsOnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + 0.25f, _whatIsGround))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            if(angle < _maxSlopeAngle && angle != 0)
            {
                return true;
            }
            else
            {
                return false; //si el angulo excede el maxSlopeAngle, entonces estamos lo q hittie se considera pared
            }
        }
        return false; //raycast no hitteo nada, returnea false xq no esta en slope
    }
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, _slopeHit.normal).normalized;
    }

}
