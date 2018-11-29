using UnityEngine;
using UnityEngine.UI;

public class PointerController : MonoBehaviour
{
    //public GameObject Timer;
    private GameObject _lookedAtObject = null;
    private GameObject _currentlyHoldingObject = null;
    private Transform _parentOfTemporaryObject = null;
    public Material DefaultReturnItemMaterial;
    public Material DefaultItemMaterial;
    public Material HoverableObjectMaterial;
    private MeshRenderer _meshRenderer;
    private Image _image;
    public LayerMask CatchItemLayer;
    public LayerMask CatchButtonLayer;
    public LayerMask ReturnObjectLayer;
    public GameObject Point;

    private Transform _tempPosition = null;
    private static bool _canReturnObject = false;

    private static bool _isHoldingItem = false;

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
        
        // If the player happens to hover over UI elements such as
        // buttons, then they will be updated here on different events.
        if (Physics.Raycast(ray, out hit, 100, CatchButtonLayer))
        {
            var lookedAtButton = hit.collider.gameObject;
            _image = lookedAtButton.GetComponent<Image>();
            _image.color = Color.red;

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                _image.color = Color.green;
            }
            // If the player is holding an item and needs to return it, he must
            // be hovering over the original indicator for that item and press the
            // controller button to return the item and remove it from his hand.
        }
        else
        {
            if (_image != null)
            {
                _image.color = Color.white;
            }
        }

        if (_isHoldingItem)
        {
            // If the player is holding an item and needs to return it, he must
            // be hovering over the original indicator for that item and press the
            // controller button to return the item and remove it from his hand.
            if (Physics.Raycast(ray, out hit, 100, ReturnObjectLayer))
            {
                Debug.Log("mhm");
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                    Input.GetKeyDown(KeyCode.K))
                {
                    _lookedAtObject.tag = "Untagged";
                    _lookedAtObject.layer = 18;
                    _meshRenderer.material = DefaultItemMaterial;

                    if (_lookedAtObject.name == "Water") { ObjectivesController.PlacedBackWaterBottle = true; }
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
                    if (_lookedAtObject.name == "Water") { ObjectivesController.PickedUpWaterBottle = true; }
                    
                    _currentlyHoldingObject = Instantiate(_lookedAtObject, _parentOfTemporaryObject.transform);
                    _currentlyHoldingObject.GetComponent<MeshRenderer>().material = DefaultItemMaterial;
                    _currentlyHoldingObject.GetComponent<CapsuleCollider>().enabled = false;
                
                    _meshRenderer.material = DefaultReturnItemMaterial;
                    _lookedAtObject.layer = 21;
                    _lookedAtObject.tag = "Return Item";

                    Invoke("EnableReturningObject", 0.1f);
                }
            }
            else
            {
                if (_meshRenderer != null && _isHoldingItem == false)
                {
                    _meshRenderer.material = DefaultItemMaterial;
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
        if (_isHoldingItem && _currentlyHoldingObject != null)
        {
            var newItemPosition = _currentlyHoldingObject.transform.position;
            newItemPosition = transform.position;
            _currentlyHoldingObject.transform.position = newItemPosition;
				
            var newItemRotation = _currentlyHoldingObject.transform.rotation;
            newItemRotation = transform.rotation;
            _currentlyHoldingObject.transform.rotation = newItemRotation;
        }
    }

    private void EnableReturningObject()
    {
        _isHoldingItem = !_isHoldingItem;
    }
}