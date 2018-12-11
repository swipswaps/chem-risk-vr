using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetWaterController : MonoBehaviour {
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Beaker"))
		{
			Destroy(other.transform.GetChild(0).gameObject);
			other.name = "Empty Beaker";
		}
		if (other.CompareTag("Dropper"))
		{
			Destroy(other.transform.GetChild(0).gameObject);
		}
	}
}
