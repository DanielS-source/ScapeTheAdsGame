using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xController : MonoBehaviour
{
    void OnMouseDown()
    {
        CubeLevelController gameController = FindObjectOfType<CubeLevelController>();
        gameController.CubeRemoved();
        Destroy(transform.parent.gameObject);
    }
}
