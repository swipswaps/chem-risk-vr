using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeController : MonoBehaviour {
    public void GazeEnter()
    {
        gameObject.SetActive(false);
    }

    public void GazeExit()
    {
    }
}
