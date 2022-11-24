using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] float travelTime;

    [SerializeField] LayerMask _playerLayermask;

    Rigidbody _rb;
    Vector3 _currentPos;

    Rigidbody _playerRB;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        _currentPos = Vector3.Lerp(startPoint.position, endPoint.position, Mathf.Cos(Time.time / travelTime * Mathf.PI * 2) * -.5f + .5f);
        _rb.MovePosition(_currentPos);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if ((_playerLayermask.value & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
    //    {
    //        Debug.Log("enterrrrrrr");
    //        _playerRB = other.GetComponentInParent<Rigidbody>();
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    if ((_playerLayermask.value & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
    //    {
    //        //forcemode.acceleration
    //        _playerRB.AddForce(_rb.velocity * 100 * Time.deltaTime);
    //        Debug.Log("asdsadasdasda");
    //    }
    //}

}
