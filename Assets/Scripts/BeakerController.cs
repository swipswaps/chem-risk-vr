using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeakerController : MonoBehaviour
{
    //[System.NonSerialized]
    public string BeakerType;
    //[System.NonSerialized]
    public bool CleanedStatus;
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
    private GameObject _heldObject;
    private Color _heldObjectColor;
	
	void Update () {
		if (PointerController.IsHoldingItem)
		{
            _heldObject = PointerController.CurrentlyHoldingObjectForBeakers;
				
			if (_isObjectOverBeaker //&&
			    // Making sure the player has the item facing down when pouring.
			    //(heldObject.transform.eulerAngles.x > 240 && heldObject.transform.eulerAngles.x < 300 ||
			     //heldObject.transform.eulerAngles.x > 80 && heldObject.transform.eulerAngles.x < 90)
			    )
			{
                // This helps signal the player that he can now spill the
                // substance he is holding into the empty beaker.
                _heldObjectColor = _heldObject.GetComponent<MeshRenderer>().material.color;

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
					    (_heldObject.name == "Dropper(Clone)" ||
                        _heldObject.name == "Empty Dropper" &&
                        _heldObject.transform.childCount <= 0))
					{
						// We instantiate the new liquid in the dropper instead of the beaker this time
						// since we are *extracting*, not pouring liquids.
						Instantiate(gameObject.transform.GetChild(0).gameObject,
                            _heldObject.transform.position, new Quaternion(0, 0, 0, 0), _heldObject.transform);
					}
					
					// We instantiate the substance that the player is holding and we place it
					// in the empty beaker.
					if ((gameObject.name == "Empty Beaker" ||
                        gameObject.name == "Big Empty Beaker" ||
                        gameObject.name == "Round Empty Beaker" ||
                        gameObject.name == "Small Empty Beaker") && _heldObject.name == "Water(Clone)")
					{
						Instantiate(WaterSubstance, transform.position, new Quaternion(0, 0, 0, 0), transform);
                        BeakerType = "Water";
						// We also have to rename the current game object from empty because
						// the beaker is no longer empty and instead has water.
						gameObject.name = "Water Beaker";
						gameObject.tag = "Dirty Beaker";
					}

					if (gameObject.name == "Water Beaker" && _heldObject.name == "Red Substance(Clone)")
					{
						// We destroy the existing water and replace it with the
						// substance the player is holding (red).
						CreateSubstanceInBeaker(RedSubstance, "Red");
						ObjectivesSelector.PourRedIntoTube = true;
					} else if (gameObject.name == "Water Beaker" && _heldObject.name == "Yellow Substance(Clone)")
					{
						CreateSubstanceInBeaker(YellowSubstance, "Yellow");
						ObjectivesSelector.PourYellowIntoTube = true;
					} else if (gameObject.name == "Water Beaker" && _heldObject.name == "Blue Substance(Clone)")
					{
						CreateSubstanceInBeaker(BlueSubstance, "Blue");
						ObjectivesSelector.PourBlueIntoTube = true;
					} else if ((gameObject.name == "Yellow Substance Beaker" &&
                                _heldObject.name == "Red Substance(Clone)") ||
					           (gameObject.name == "Red Substance Beaker" &&
                                _heldObject.name == "Yellow Substance(Clone)"))
					{
						CreateSubstanceInBeaker(OrangeSubstance, "Orange");
						ObjectivesSelector.MixRedAndYellow = true;
					} else if ((gameObject.name == "Red Substance Beaker" &&
                                _heldObject.name == "Blue Substance(Clone)") ||
					           (gameObject.name == "Blue Substance Beaker" &&
                                _heldObject.name == "Red Substance(Clone)"))
					{
						CreateSubstanceInBeaker(PurpleSubstance, "Purple");
						ObjectivesSelector.MixRedAndBlue = true;
					} else if ((gameObject.name == "Blue Substance Beaker" &&
                                _heldObject.name == "Yellow Substance(Clone)") ||
					           (gameObject.name == "Yellow Substance Beaker" &&
                                _heldObject.name == "Blue Substance(Clone)"))
					{
						CreateSubstanceInBeaker(GreenSubstance, "Green");
						ObjectivesSelector.MixBlueAndYellow = true;
					}
				}
			}
			else
			{
                _heldObjectColor = _heldObject.GetComponent<MeshRenderer>().material.color;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == 22)
		{
			_isObjectOverBeaker = true;
            _heldObjectColor.g = 255;
            _heldObject.GetComponent<MeshRenderer>().material.color = _heldObjectColor;
        }
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 22)
		{
			_isObjectOverBeaker = false;
            _heldObjectColor.g = 1;
            _heldObject.GetComponent<MeshRenderer>().material.color = _heldObjectColor;
        }
    }

	private void CreateSubstanceInBeaker(GameObject substance, string nameWithCapital)
	{
		Destroy(gameObject.transform.GetChild(0).gameObject);
		Instantiate(substance, transform.position, new Quaternion(0, 0, 0, 0), transform);
		gameObject.name = nameWithCapital + " Substance Beaker";
	}
}
