using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeakerController : MonoBehaviour
{
	public LayerMask HeldLayerMask;

	public GameObject WaterSubstance;
	
	// Primary Colors
	public GameObject BlueSubstance;
	public GameObject RedSubstance;
	public GameObject YellowSubstance;
	
	// Secondary Colors
	public GameObject OrangeSubstance;
	public GameObject PurpleSubstance;
	public GameObject GreenSubstance;

	private bool _isObjectOverBeaker = false;
	
	void Update () {
		if (PointerController.IsHoldingItem)
		{
			GameObject heldObject = PointerController.CurrentlyHoldingObjectForBeakers;
			Color heldObjectColor;
				
			if (_isObjectOverBeaker //&&
			    // Making sure the player has the item facing down when pouring.
			    //(heldObject.transform.eulerAngles.x > 240 && heldObject.transform.eulerAngles.x < 300 ||
			     //heldObject.transform.eulerAngles.x > 80 && heldObject.transform.eulerAngles.x < 90)
			    )
			{
				// This helps signal the player that he can now spill the
				// substance he is holding into the empty beaker.
				heldObjectColor = heldObject.GetComponent<MeshRenderer>().material.color;
				heldObjectColor.g = 255;

				if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
				    Input.GetKeyDown(KeyCode.L))
				{
					// If we are holding a dropper above a used beaker and the dropper is empty, then
					// we can transfer liquid from the beaker to the dropper being held.
					if ((gameObject.name == "Yellow Substance Beaker" ||
					    gameObject.name == "Red Substance Beaker" ||
					    gameObject.name == "Blue Substance Beaker" ||
					    gameObject.name == "Water Beaker" ||
					    gameObject.name == "Orange Substance Beaker" ||
					    gameObject.name == "Purple Substance Beaker" ||
					    gameObject.name == "Green Substance Beaker") &&
					    (heldObject.name == "Dropper(Clone)" &&
					    heldObject.transform.childCount <= 0))
					{
						// We instantiate the new liquid in the dropper instead of the beaker this time
						// since we are *extracting*, not pouring liquids.
						Instantiate(gameObject.transform.GetChild(0).gameObject,
							heldObject.transform.position, Quaternion.identity, heldObject.transform);
					}
					
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
						CreateSubstanceInBeaker(RedSubstance, "Red");
						ObjectivesSelector.PourRedIntoTube = true;
					} else if (gameObject.name == "Water Beaker" && heldObject.name == "Yellow Substance(Clone)")
					{
						CreateSubstanceInBeaker(YellowSubstance, "Yellow");
						ObjectivesSelector.PourYellowIntoTube = true;
					} else if (gameObject.name == "Water Beaker" && heldObject.name == "Blue Substance(Clone)")
					{
						CreateSubstanceInBeaker(BlueSubstance, "Blue");
						ObjectivesSelector.PourBlueIntoTube = true;
					} else if ((gameObject.name == "Yellow Substance Beaker" &&
					            heldObject.name == "Red Substance(Clone)") ||
					           (gameObject.name == "Red Substance Beaker" &&
					            heldObject.name == "Yellow Substance(Clone)"))
					{
						CreateSubstanceInBeaker(OrangeSubstance, "Orange");
						ObjectivesSelector.MixRedAndYellow = true;
					} else if ((gameObject.name == "Red Substance Beaker" &&
					            heldObject.name == "Blue Substance(Clone)") ||
					           (gameObject.name == "Blue Substance Beaker" &&
					            heldObject.name == "Red Substance(Clone)"))
					{
						CreateSubstanceInBeaker(PurpleSubstance, "Purple");
						ObjectivesSelector.MixRedAndBlue = true;
					} else if ((gameObject.name == "Blue Substance Beaker" &&
					            heldObject.name == "Yellow Substance(Clone)") ||
					           (gameObject.name == "Yellow Substance Beaker" &&
					            heldObject.name == "Blue Substance(Clone)"))
					{
						CreateSubstanceInBeaker(GreenSubstance, "Green");
						ObjectivesSelector.MixBlueAndYellow = true;
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
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 22)
		{
			_isObjectOverBeaker = false;
		}
	}

	private void CreateSubstanceInBeaker(GameObject substance, string nameWithCapital)
	{
		Destroy(gameObject.transform.GetChild(0).gameObject);
		Instantiate(substance, transform.position, Quaternion.identity, transform);
		gameObject.name = nameWithCapital + " Substance Beaker";
	}
}
