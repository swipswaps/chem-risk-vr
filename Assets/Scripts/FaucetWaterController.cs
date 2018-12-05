using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetWaterController : MonoBehaviour {
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Beaker")
		{
			Destroy(other.transform.GetChild(0).gameObject);
			other.name = "Empty Beaker";
		}
	}
}
