using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteBinController : MonoBehaviour {
    public string BinType;
    public GameObject Leftovers;
    public GameObject FireParticles;
    public GameObject SmokeParticles;
    public GameObject ExplosionParticles;
    private bool _areParticlesInitiated = false;

    private GameObject _labEquipment;
    private GameObject _player;
    private GameObject _lobbyPoint;

    private GameObject _currentParticles;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _lobbyPoint = GameObject.FindGameObjectWithTag("Restart Position");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Beaker" || other.tag == "Dropper")
        {
            // **************************************
            // Warm colors trash bin effects
            if (_areParticlesInitiated == false && (BinType == "Warm" &&  other.gameObject.name == "Blue Substance Beaker(Clone)" || other.gameObject.name == "Purple Substance Beaker(Clone)"))
            {
                _currentParticles = Instantiate(FireParticles, gameObject.transform);
                _areParticlesInitiated = true;
            }
            // End-game statement
            if (_areParticlesInitiated == false && (BinType == "Warm" &&
                other.gameObject.name == "Green Substance Beaker(Clone)"))
            {
                _currentParticles = Instantiate(ExplosionParticles, gameObject.transform);
                Invoke("GameOver", 0.5f);
                _areParticlesInitiated = true;
            }

            // **************************************
            // Cold colors trash bin effects
            if (_areParticlesInitiated == false && (BinType == "Cold" &&  other.gameObject.name == "Red Substance Beaker(Clone)" || other.gameObject.name == "Orange Substance Beaker(Clone)" || other.gameObject.name == "Yellow Substance Beaker(Clone)"))
            {
                Invoke("MeltBin", 0.5f);
                //Instantiate(SmokeParticles, gameObject.transform);
                //_areParticlesInitiated = true;
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
