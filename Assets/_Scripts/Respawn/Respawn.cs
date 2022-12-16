using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{

    [SerializeField] Transform _spawnPoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = _spawnPoint.position;
        }
    }
}
