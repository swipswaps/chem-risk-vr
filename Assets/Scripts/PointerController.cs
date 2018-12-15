using UnityEngine;
using UnityEngine.UI;

public class PointerController : MonoBehaviour
{
    //public GameObject Timer;
    private GameObject _lookedAtObject = null;
    private GameObject _currentlyHoldingObject = null;
    public static GameObject CurrentlyHoldingObjectForBeakers;
    private Transform _parentOfTemporaryObject = null;
    public Material DefaultReturnItemMaterial;
    public Material DefaultItemMaterial;
    public Material DefaultBeakerMaterial;
    public Material HoverableObjectMaterial;
    private MeshRenderer _meshRenderer;
    public LayerMask CatchItemLayer;
    public LayerMask ReturnObjectLayer;
    public GameObject Point;
    public GameObject PlayerBody;

    private Transform _tempPosition = null;
    private static bool _canReturnObject = false;
    
    public static bool IsWearingCoat = false;
    public static bool IsWearingGlasses = false;
    private GameObject _coat;
    private GameObject _glasses;

    public static bool IsHoldingItem = false;

    // Update is called once per frame
    void Update () {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        Ray pointRay = new Ray(transform.position, transform.forward);
        RaycastHit pointHit;

        if (Physics.Raycast(pointRay, out pointHit, 100))
        {
            Point.SetActive(true);
            // We want to visualize the point of impact where the
            // player is pointing it, so he knows where he is clicking.
            var pointPosition = Point.transform.position;
            pointPosition = pointHit.point;
            Point.transform.position = pointPosition;
        }
        else
        {
            // When nothing is being pointed at, disable the point of the pointer.
            Point.SetActive(false);
        }

        if (IsHoldingItem)
        {
            // If the player is holding an item and needs to return it, he must
            // be hovering over the original indicator for that item and press the
            // controller button to return the item and remove it from his hand.
            if (Physics.Raycast(ray, out hit, 100, ReturnObjectLayer))
            {
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                    Input.GetKeyDown(KeyCode.K))
                {
                    _lookedAtObject.tag = "Untagged";
                    _lookedAtObject.layer = 18;
                    if (_lookedAtObject.name != "Yellow Substance Beaker" &&
                        _lookedAtObject.name != "Red Substance Beaker" &&
                        _lookedAtObject.name != "Blue Substance Beaker" &&
                        _lookedAtObject.name != "Orange Substance Beaker" &&
                        _lookedAtObject.name != "Purple Substance Beaker" &&
                        _lookedAtObject.name != "Green Substance Beaker" &&
                        _lookedAtObject.name != "Water Beaker" &&
                        _lookedAtObject.name != "Empty Beaker" &&
                        _lookedAtObject.name != "Dropper")
                    {
                        _meshRenderer.material = DefaultItemMaterial;
                    }
                    else
                    {
                        if (_currentlyHoldingObject.name == "Empty Beaker")
                        {
                            Destroy(_lookedAtObject.transform.GetChild(0).gameObject);
                            _lookedAtObject.name = "Empty Beaker";
                        }

                        // If we are returning a dropper and it contains liquids inside it, then
                        // we return the dropper AND transfer the liquids to the original dropper.
                        if (_currentlyHoldingObject.name == "Dropper(Clone)" && _currentlyHoldingObject.transform.childCount > 0)
                        {
                            // Destroy the exusting liquid in the dropper because you dont want to
                            // keep placing liquid in the dropper, we only have one liquid substance in a dropper.
                            if (_lookedAtObject.transform.childCount > 0)
                            {
                                Destroy(_lookedAtObject.transform.GetChild(0).gameObject);
                            }
                            
                            Instantiate(_currentlyHoldingObject.transform.GetChild(0).gameObject,
                                _lookedAtObject.transform.position,
                                Quaternion.identity,
                                _lookedAtObject.transform);
                        } else if (_currentlyHoldingObject.name == "Empty Dropper")
                        {
                            // If we are returning an empty dropper, then we have to
                            // destroy the existing liquids in the original dropper.
                            Destroy(_lookedAtObject.transform.GetChild(0).gameObject);
                            _lookedAtObject.name = "Dropper";
                        }
                        
                        _meshRenderer.material = DefaultBeakerMaterial;
                        _currentlyHoldingObject.tag = "Untagged";
                    }

                    if (_lookedAtObject.name == "Water") { ObjectivesSelector.PlacedBackWaterBottle = true; }
                    Destroy(_currentlyHoldingObject);
                    
                    // This gives a small delay before the player can return
                    // the object back to its place, because for some reason oculus
                    // had a glitch and returned it immediately and it was very buggy.
                    Invoke("EnableReturningObject", 0.1f);
                }
            }
        }
        else
        {
            // We are looking with the controllerfor an object
            // that is of layer item
            if (Physics.Raycast(ray, out hit, 100, CatchItemLayer))
            {
                _lookedAtObject = hit.collider.gameObject;
                _parentOfTemporaryObject = _lookedAtObject.transform.parent;
            
                // Once it is found we change it to look as if it is hovered
                // so that it appears interact-able
                _meshRenderer = _lookedAtObject.GetComponent<MeshRenderer>();
                _meshRenderer.material = HoverableObjectMaterial;
                
                // If the player is not holding an interactable item and presses
                // the controller button, we append the item onto his hand and
                // make the one he clicked on become transparent, to indicate where
                // it must be returned after it is done being used.
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                    Input.GetKeyDown(KeyCode.K))
                {
                    if (_lookedAtObject.name == "Water") { ObjectivesSelector.PickedUpWaterBottle = true; }

                    if (_lookedAtObject.name == "Lab Coat")
                    {
                        _coat = _lookedAtObject;
                        _coat.GetComponent<MeshRenderer>().material = DefaultItemMaterial;
                        IsWearingCoat = true;
                    } else if (_lookedAtObject.name == "Lab Glasses")
                    {
                        _glasses = _lookedAtObject;
                        _glasses.GetComponent<MeshRenderer>().material = DefaultItemMaterial;
                        IsWearingGlasses = true;
                    }
                    else
                    {
                        _currentlyHoldingObject = Instantiate(_lookedAtObject, _parentOfTemporaryObject.transform);
                        if (_lookedAtObject.name != "Yellow Substance Beaker" &&
                            _lookedAtObject.name != "Red Substance Beaker" &&
                            _lookedAtObject.name != "Blue Substance Beaker" &&
                            _lookedAtObject.name != "Water Beaker" &&
                            _lookedAtObject.name != "Orange Substance Beaker" &&
                            _lookedAtObject.name != "Purple Substance Beaker" &&
                            _lookedAtObject.name != "Green Substance Beaker" &&
                            _lookedAtObject.name != "Empty Beaker" &&
                            _lookedAtObject.name != "Dropper")
                        {
                            _currentlyHoldingObject.GetComponent<MeshRenderer>().material = DefaultItemMaterial;
                            CurrentlyHoldingObjectForBeakers = _currentlyHoldingObject;
                        }
                        else
                        {
                            _currentlyHoldingObject.GetComponent<MeshRenderer>().material = DefaultBeakerMaterial;
                            if (_lookedAtObject.name == "Dropper")
                            {
                                _currentlyHoldingObject.tag = "Dropper";
                            }
                            else
                            {
                                _currentlyHoldingObject.tag = "Beaker";
                            }
                            CurrentlyHoldingObjectForBeakers = _currentlyHoldingObject;
                        }
                        _currentlyHoldingObject.layer = 22;
                
                        //_meshRenderer.material = DefaultReturnItemMaterial;
                        _lookedAtObject.layer = 21;
                        _lookedAtObject.tag = "Return Item";

                        Invoke("EnableReturningObject", 0.1f);   
                    }
                }
            }
            else
            {
                if (_meshRenderer != null && IsHoldingItem == false )
                {
                    if (_lookedAtObject.name != "Yellow Substance Beaker" &&
                        _lookedAtObject.name != "Red Substance Beaker" &&
                        _lookedAtObject.name != "Blue Substance Beaker" &&
                        _lookedAtObject.name != "Water Beaker" &&
                        _lookedAtObject.name != "Orange Substance Beaker" &&
                        _lookedAtObject.name != "Purple Substance Beaker" &&
                        _lookedAtObject.name != "Green Substance Beaker" &&
                        _lookedAtObject.name != "Empty Beaker" &&
                        _lookedAtObject.name != "Dropper")
                    {
                        _meshRenderer.material = DefaultItemMaterial;
                    }
                    else
                    {
                        
                        _meshRenderer.material = DefaultBeakerMaterial;
                    }
                }
            }
        }
        
        // If an item is to be returned, then it will be tracked and
        // highlighted in the world, because the rays were having trouble
        // updating its material.
        var itemToReturn = GameObject.FindGameObjectWithTag("Return Item");
        if (itemToReturn != null)
        {
            itemToReturn.GetComponent<MeshRenderer>().material = DefaultReturnItemMaterial;
        }
        
        // This makes sure the item the player is holding is being placed
        // at the spot where the controller hand is.
        if (IsHoldingItem && _currentlyHoldingObject != null)
        {
            var newItemPosition = _currentlyHoldingObject.transform.position;
            newItemPosition = transform.position;
            _currentlyHoldingObject.transform.position = newItemPosition;
				
            var newItemRotation = _currentlyHoldingObject.transform.rotation;
            newItemRotation = transform.rotation;
            _currentlyHoldingObject.transform.rotation = newItemRotation;
        }

        if (IsWearingCoat)
        {
            var newCoatPosition = _coat.transform.position;
            newCoatPosition = PlayerBody.transform.position;
            _coat.transform.position = newCoatPosition;
				
            var newCoatRotation = _coat.transform.rotation;
            newCoatRotation = PlayerBody.transform.rotation;
            _coat.transform.rotation = newCoatRotation;
        }

        if (IsWearingGlasses)
        {
            var newGlassesPosition = _glasses.transform.position;
            newGlassesPosition = PlayerBody.transform.position;
            newGlassesPosition.y += 1;
            _glasses.transform.position = newGlassesPosition;
				
            var newCoatRotation = _glasses.transform.rotation;
            newCoatRotation = PlayerBody.transform.rotation;
            _glasses.transform.rotation = newCoatRotation;
        }
    }

    private void EnableReturningObject()
    {
        IsHoldingItem = !IsHoldingItem;
    }
}