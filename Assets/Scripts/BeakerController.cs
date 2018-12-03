using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeakerController : MonoBehaviour
{
	public LayerMask HeldLayerMask;

	public GameObject WaterSubstance;
	public GameObject RedSubstance;

	private bool _isObjectOverBeaker = false;
	
	void Update () {
		if (PointerController.IsHoldingItem)
		{
			GameObject heldObject = PointerController.CurrentlyHoldingObjectForBeakers;
			Color heldObjectColor;
				
			// If the beater is below the object being held...
			if (_isObjectOverBeaker)
			{
				Debug.Log("RISE");
			}
			if (_isObjectOverBeaker &&
			    // Making sure the player has the item facing down when pouring.
			    (heldObject.transform.eulerAngles.x > 240 && heldObject.transform.eulerAngles.x < 300 ||
			    heldObject.transform.eulerAngles.x > 80 && heldObject.transform.eulerAngles.x < 90))
			{
				// This helps signal the player that he can now spill the
				// substance he is holding into the empty beaker.
				heldObjectColor = heldObject.GetComponent<MeshRenderer>().material.color;
				heldObjectColor.g = 255;

				if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
				    Input.GetKeyDown(KeyCode.L))
				{
					// We instantiate the substance that the player is holding and we place it
					// in the empty beaker.
					if (gameObject.name == "Empty Beaker" && heldObject.name == "Water(Clone)")
					{
						Instantiate(WaterSubstance, transform.position, Quaternion.identity, transform);
						// We also have to rename the current game object from empty because
						// the beaker is no longer empty and instead has water.
						gameObject.name = "Water Beaker";
					}

					if (gameObject.name == "Water Beaker" && heldObject.name == "Red Substance(Clone)")
					{
						// We destroy the existing water and replace it with the
						// substance the player is holding (red).
						Destroy(gameObject.transform.GetChild(0).gameObject);
						Instantiate(RedSubstance, transform.position, Quaternion.identity, transform);
						gameObject.name = "Red Substance Beaker";
					}
				}
			}
			else
			{
				heldObjectColor = heldObject.GetComponent<MeshRenderer>().material.color;
				heldObjectColor.g = 1;
			}
			
			heldObject.GetComponent<MeshRenderer>().material.color = heldObjectColor;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == 22)
		{
			_isObjectOverBeaker = true;
			Debug.Log("ENTER");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 22)
		{
			_isObjectOverBeaker = false;
			Debug.Log("LEAVE");
		}
	}
}
