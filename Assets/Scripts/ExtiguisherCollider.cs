using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtiguisherCollider : MonoBehaviour {
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 25)
        {
            other.gameObject.transform.parent.GetComponent<WasteBinController>().AreParticlesInitiated = false;

            gameObject.GetComponentInParent<FireExtinguisher>().StartCoroutine(gameObject.GetComponentInParent<FireExtinguisher>().DestroyFire(other.gameObject));
        }
    }
}
