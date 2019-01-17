using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetWaterController : MonoBehaviour {
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Beaker"))
		{
			Destroy(other.transform.GetChild(0).gameObject);

            if (other.transform.gameObject.transform.GetChild(0).name != "Smelly Waste(Clone)")
            {
                ProfileSystemController.TimesAnIncidentWasCaused++;
                ProfileSystemController.UpdateProfileData();
            }

			if (other.GetComponent<MeshFilter>().mesh.name == "mod_laboratorium_flask_T2 Instance")
            {
                other.name = "Round Empty Beaker";
            } else if (other.GetComponent<MeshFilter>().mesh.name == "mod_laboratorium_flask Instance")
            {
                other.name = "Empty Beaker";
            }
            else if (other.GetComponent<MeshFilter>().mesh.name == "mod_laboratory_beaker Instance")
            {
                other.name = "Big Empty Beaker";
            }
            else if (other.GetComponent<MeshFilter>().mesh.name == "laboratory_testTube_tube Instance")
            {
                other.name = "Small Empty Beaker";
            }
            gameObject.tag = "Untagged";
		}
		if (other.CompareTag("Dropper"))
		{
			Destroy(other.transform.GetChild(0).gameObject);
			other.name = "Empty Dropper";
		}
	}
}
