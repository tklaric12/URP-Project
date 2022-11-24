using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _orientation;
    [SerializeField] Transform _player;
    [SerializeField] Transform _playerObj;
    [SerializeField] Rigidbody rb;

    [SerializeField] GameObject _basicCamera;
    [SerializeField] GameObject _topdownCamera;

    public float rotationSpeed;

    CameraStyle _currentCameraStyle;

    private enum CameraStyle
    {
        Basic,
        Topdown
    }


    private void Start()
    {
        //despawnear el cursor de la pantalla + lockearlo en el lugar para q no se vaya de la ventana
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        TemporaryCameraStyleSwitching(); //ESTO ES PARA TESTEAR Y CAMBIAR LA CAMARA MAS RAPIDO. EL JUGADOR NO VA A DECIDIR QUE MODO PONERLE A LA CAMARA.

        //rotate orientation
        Vector3 viewDir = _player.position - new Vector3(transform.position.x, _player.position.y, transform.position.z);
        _orientation.forward = viewDir.normalized;

        //rotate player model
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = _orientation.forward * verticalInput + _orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
        {
            _playerObj.forward = Vector3.Slerp(_playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        _basicCamera.SetActive(false);
        _topdownCamera.SetActive(false);

        if (newStyle == CameraStyle.Basic)
        {
            _basicCamera.SetActive(true);
        }
        else if(newStyle == CameraStyle.Topdown)
        {
            _topdownCamera.SetActive(true);
        }

        _currentCameraStyle = newStyle;
    }

    private void TemporaryCameraStyleSwitching() //BORRAR FUNCION EVENTUALMENTE, ES SOLO PARA FACILITAR VISTAS DISTINTAS EN TESTEO.
    {
        if(Input.GetKeyUp(KeyCode.X))
        {
            if (_currentCameraStyle == CameraStyle.Basic)
            {
                SwitchCameraStyle(CameraStyle.Topdown);
            }
            else
            {
                SwitchCameraStyle(CameraStyle.Basic);
            }
        }

    }

}
