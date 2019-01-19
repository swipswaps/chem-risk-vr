using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetRadiusController : MonoBehaviour
{
    public GameObject[] FaucetWater = new GameObject[4];

	private void Start()
    {
        foreach (GameObject water in FaucetWater)
        {
            water.SetActive(false);
        }
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Beaker") || other.CompareTag("Dropper"))
		{
            foreach (GameObject water in FaucetWater)
            {
                water.SetActive(true);
            }
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Beaker") || other.CompareTag("Dropper"))
        {
            foreach (GameObject water in FaucetWater)
            {
                water.SetActive(false);
            }
		}
	}
}
