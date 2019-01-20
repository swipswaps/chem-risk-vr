using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour {
    public LayerMask leverLayer;
    public GameObject LabCoat;
    public GameObject LaundryChute;

    public GameObject Pointer;
    public bool IsLeverDown = false;
    [Range(0, 30)]
    public int RotationMargin;
    private GameObject _lever;
    public static bool IsLabcoatDropped = false;
    public bool CanPushLever = true;

    private void Start()
    {
        _lever = gameObject.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        Ray pointRay = new Ray(Pointer.transform.position, Pointer.transform.forward);
        RaycastHit pointHit;
        Debug.DrawRay(Pointer.transform.position, Pointer.transform.forward);
        if (Physics.Raycast(pointRay, out pointHit, 100, leverLayer))
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                    Input.GetKeyDown(KeyCode.K) && CanPushLever)
            {
                // Depending on the direction of the last time the lever
                // was pulled, we will pull it to the opposite direction
                // the next time.
                StartCoroutine(PushLever(IsLeverDown ? -RotationMargin : RotationMargin));

                if (IsLabcoatDropped == false)
                {
                    GameObject.Find("Lab Coat").GetComponent<Rigidbody>().isKinematic = false;
                } else
                {
                    GameObject.Find("Lab Coat").GetComponent<Rigidbody>().isKinematic = true;
                }

                CanPushLever = false;
            }
        }
    }

    public IEnumerator PushLever(int rotation)
    {
        if (IsLeverDown)
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

        IsLeverDown = !IsLeverDown;
    }
}
