using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteBinController : MonoBehaviour {
    public string BinType;
    public GameObject Leftovers;
    public GameObject FireParticles;
    public GameObject SmokeParticles;
    public GameObject ExplosionParticles;
    public bool AreParticlesInitiated = false;

    private GameObject _labEquipment;
    private GameObject _player;
    private GameObject _lobbyPoint;

    private GameObject _currentParticles;
    private bool _isBeakerDirty = false;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _lobbyPoint = GameObject.FindGameObjectWithTag("Restart Position");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Beaker" || other.tag == "Dropper")
        {
            // **************************************
            // Warm colors trash bin effects
            if (AreParticlesInitiated == false && (BinType == "Warm" &&  other.gameObject.name == "Blue Substance Beaker(Clone)" || other.gameObject.name == "Purple Substance Beaker(Clone)"))
            {
                _currentParticles = Instantiate(FireParticles, gameObject.transform);

                // We check if the right beaker's liquid is being thrown away
                Destroy(other.transform.GetChild(0).gameObject);

                GameObject leftOvers = Instantiate(Leftovers,
                    other.transform.position,
                    Quaternion.identity,
                    other.transform);
                Leftovers.tag = "Smelly Waste";

                _isBeakerDirty = true;

                AreParticlesInitiated = true;
            } else if (other.gameObject.name == "Orange Substance Beaker(Clone)" ||
                other.gameObject.name == "Yellow Substance Beaker(Clone)" ||
                other.gameObject.name == "Red Substance Beaker(Clone)" ||
                other.gameObject.name == "Green Substance Beaker(Clone)" ||
                other.gameObject.name == "Water Beaker(Clone)")
            {
                Destroy(other.transform.GetChild(0).gameObject);
            }
            // End-game statement
            if (AreParticlesInitiated == false && (BinType == "Warm" &&
                other.gameObject.name == "Green Substance Beaker(Clone)"))
            {
                _currentParticles = Instantiate(ExplosionParticles, gameObject.transform);

                GameOver();
            }
            else if (other.gameObject.name == "Blue Substance Beaker(Clone)" ||
              other.gameObject.name == "Purple Substance Beaker(Clone)" ||
              other.gameObject.name == "Orange Substance Beaker(Clone)" ||
              other.gameObject.name == "Yellow Substance Beaker(Clone)" ||
              other.gameObject.name == "Red Substance Beaker(Clone)" ||
              other.gameObject.name == "Water Beaker(Clone)")
            {
                Destroy(other.transform.GetChild(0).gameObject);
            }

            // **************************************
            // Cold colors trash bin effects
            if (AreParticlesInitiated == false && (BinType == "Cold" &&  other.gameObject.name == "Red Substance Beaker(Clone)" || other.gameObject.name == "Orange Substance Beaker(Clone)" || other.gameObject.name == "Yellow Substance Beaker(Clone)"))
            {
                Destroy(other.transform.GetChild(0).gameObject);

                GameObject leftOvers = Instantiate(Leftovers,
                    other.transform.position,
                    Quaternion.identity,
                    other.transform);
                Leftovers.tag = "Smelly Waste";

                _isBeakerDirty = true;

                MeltBin();
                //Instantiate(SmokeParticles, gameObject.transform);
                //_areParticlesInitiated = true;
            } else if (
                other.gameObject.name == "Blue Substance Beaker(Clone)" &&
                other.gameObject.name == "Purple Substance Beaker(Clone)" &&
                other.gameObject.name == "Green Substance Beaker(Clone)" &&
                other.gameObject.name == "Water Beaker(Clone)")
            {
                Destroy(other.transform.GetChild(0).gameObject);
            }

            if (other.CompareTag("Beaker"))
            {
                if (other.transform.gameObject.transform.GetChild(0).name != "Smelly Waste(Clone)")
                {
                    ProfileSystemController.TimesAnIncidentWasCaused++;
                    ProfileSystemController.UpdateProfileData();
                }

                if (other.GetComponent<MeshFilter>().mesh.name == "mod_laboratorium_flask_T2 Instance")
                {
                    other.name = "Round Empty Beaker";
                }
                else if (other.GetComponent<MeshFilter>().mesh.name == "mod_laboratorium_flask Instance")
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
            }
            if (other.CompareTag("Dropper"))
            {
                Destroy(other.transform.GetChild(0).gameObject);
                other.name = "Empty Dropper";
            }

            if (_isBeakerDirty)
            {
                other.gameObject.tag = "Dirty Beaker";
            }
        }
    }

    private void GameOver()
    {
        Vector3 newPlayerPos = _player.transform.position;
        newPlayerPos = _lobbyPoint.transform.position;
        _player.transform.position = newPlayerPos;

        HandinController.IsObjectiveHandedIn = true;
        ProfileSystemController.TimesAGuidelineIsMissed++;
        ProfileSystemController.PlayingALevel = false;
        ProfileSystemController.UpdateProfileData();

        ObjectivesSelector.CanWearEquipment = false;
        PointerController.IsWearingCoat = false;
        PointerController.IsWearingGlasses = false;
        PointerController.IsWearingGloves = false;

        PointerController.IsHoldingItem = false;
        PointerController.CurrentlyHoldingObjectForBeakers = null;
        _player.GetComponentInChildren<PointerController>().ReturnEquipment();

        _labEquipment = GameObject.FindGameObjectWithTag("Lab Equipment");
        Destroy(_currentParticles);
        Destroy(_labEquipment);
    }

    private void MeltBin()
    {
        // Disable the waste bin and melt it. For now it will be
        // destroyed because I lack the melted bin model
        Destroy(gameObject);
    }
}
