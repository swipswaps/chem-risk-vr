using UnityEngine;

public class PointerController : MonoBehaviour
{
	public GameObject Timer;
	private GameObject _lookedAtObject = null;
	
	// Update is called once per frame
	void Update () {
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		
		Debug.DrawRay(transform.position, transform.forward, Color.green);

		if (Physics.Raycast(ray, out hit, 100))
		{
			_lookedAtObject = hit.collider.gameObject;
			if (_lookedAtObject.transform.CompareTag("Crate"))
			{
				Timer.SetActive(true);
			}
		}
		else
		{
			Timer.SetActive(false);
		}
	}
}
