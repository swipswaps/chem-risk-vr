using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteBinController : MonoBehaviour {
    public string BinType;
    public GameObject Leftovers;
    
    private void OnTriggerStay(Collider other)
    {
        if ((other.tag == "Beaker" || other.tag == "Dropper"))
        {
            // We check if the right beaker's liquid is being thrown away
            Destroy(other.transform.GetChild(0).gameObject);

            Instantiate(Leftovers,
                other.transform.position,
                Quaternion.identity,
                other.transform);

            if (other.GetComponent<BeakerController>().BeakerType !=
                BinType)
            {
                Debug.Log("Wrong type!");
            }
            else
            {
                Debug.Log("Correct type!");
            }
            //Debug.Log(other.GetComponent<BeakerController>().BeakerType + " | " + BinType);
        }
    }

}
