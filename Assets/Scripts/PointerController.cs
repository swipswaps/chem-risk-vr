using UnityEngine;
using UnityEngine.UI;

public class PointerController : MonoBehaviour
{
	//public GameObject Timer;
	private GameObject _lookedAtObject = null;
	public Material DefaultObjectMaterial;
	public Material HoverableObjectMaterial;
	private MeshRenderer _meshRenderer;
	private Image _image;
	public LayerMask CatchItemLayer;
	public LayerMask CatchButtonLayer;

	// Update is called once per frame
	void Update () {
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		
		// Checking if the player is hovering over an item with the controller
		if (Physics.Raycast(ray, out hit, 100, CatchItemLayer))
		{
			_lookedAtObject = hit.collider.gameObject;
			_meshRenderer = _lookedAtObject.GetComponent<MeshRenderer>();
			_meshRenderer.material = HoverableObjectMaterial;
		}
		// Checking if the player is hovering over a button with the controller
		else if (Physics.Raycast(ray, out hit, 100, CatchButtonLayer))
		{
			_lookedAtObject = hit.collider.gameObject;
			_image = _lookedAtObject.GetComponent<Image>();
			_image.color = Color.red;
			if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
			{
				_image.color = Color.green;
			}
		}
		else
		{
			if (_image != null)
			{
				_image.color = Color.white;
			}
			if (_meshRenderer != null)
			{
				_meshRenderer.material = DefaultObjectMaterial;
			}
		}
	}
}
