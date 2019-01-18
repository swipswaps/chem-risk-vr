using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteBinController : MonoBehaviour {
    public string BinType;
    public GameObject Leftovers;
    public GameObject FireParticles;
    public GameObject SmokeParticles;
    private bool _areParticlesInitiated = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Beaker" || other.tag == "Dropper")
        {
            if (_areParticlesInitiated == false && (BinType == "Warm" && other.gameObject.name == "Green Substance Beaker(Clone)" || other.gameObject.name == "Blue Substance Beaker(Clone)" || other.gameObject.name == "Purple Substance Beaker(Clone)"))
            {
                Instantiate(FireParticles, gameObject.transform);
                _areParticlesInitiated = true;
            }

            if (_areParticlesInitiated == false && (BinType == "Warm" && other.gameObject.name == "Green Substance Beaker(Clone)" || other.gameObject.name == "Blue Substance Beaker(Clone)" || other.gameObject.name == "Purple Substance Beaker(Clone)"))
            {
                Instantiate(SmokeParticles, gameObject.transform);
                _areParticlesInitiated = true;
            }
            // We check if the right beaker's liquid is being thrown away
            Destroy(other.transform.GetChild(0).gameObject);

            Instantiate(Leftovers,
                other.transform.position,
                Quaternion.identity,
                other.transform);

            if (other.GetComponent<BeakerController>().BeakerType !=
                BinType)
            {
                //Debug.Log("Wrong type!");
            }
            else
            {
                //Debug.Log("Correct type!");
            }
            //Debug.Log(other.GetComponent<BeakerController>().BeakerType + " | " + BinType);
        }
    }

}
