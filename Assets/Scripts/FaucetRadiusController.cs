using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetRadiusController : MonoBehaviour
{
	public GameObject Faucet;

	private void Start()
	{
		Faucet.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Beaker") || other.CompareTag("Dropper"))
		{
			Faucet.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Beaker") || other.CompareTag("Dropper"))
		{
			Faucet.SetActive(false);
		}
	}
}
