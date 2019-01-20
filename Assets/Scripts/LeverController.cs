using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour {
    public LayerMask leverLayer;
    public GameObject LabCoat;
    public GameObject LaundryChute;

    public GameObject Player;
    private bool _isLeverDown = false;
    [Range(0, 30)]
    public int RotationMargin;
    private GameObject _lever;
    public static bool IsLabcoatDropped = false;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        _lever = gameObject.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        Ray pointRay = new Ray(Player.transform.position, Player.transform.forward);
        RaycastHit pointHit;
        Debug.DrawRay(Player.transform.position, Player.transform.forward);
        if (Physics.Raycast(pointRay, out pointHit, 100, leverLayer))
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                    Input.GetKeyDown(KeyCode.K))
            {
                // Depending on the direction of the last time the lever
                // was pulled, we will pull it to the opposite direction
                // the next time.
                StartCoroutine(PushLever(_isLeverDown ? -RotationMargin : RotationMargin));
                _isLeverDown = !_isLeverDown;

                if (IsLabcoatDropped == false)
                {
                    GameObject.Find("Lab Coat").GetComponent<Rigidbody>().useGravity = true;
                    GameObject.Find("Lab Coat").GetComponent<BoxCollider>().isTrigger = false;
                } else
                {
                    GameObject.Find("Lab Coat").GetComponent<Rigidbody>().useGravity = false;
                    GameObject.Find("Lab Coat").GetComponent<BoxCollider>().isTrigger = true;
                }
            }
        }
    }

    private IEnumerator PushLever(int rotation)
    {
        if (_isLeverDown)
        {
            for (int i = 0; i > rotation; i--)
            {
                _lever.transform.Rotate(new Vector3(i, 0, 0));
                yield return new WaitForSeconds(0.05f);
            }
        } else
        {
            for (int i = 0; i < rotation; i++)
            {
                _lever.transform.Rotate(new Vector3(i, 0, 0));
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
