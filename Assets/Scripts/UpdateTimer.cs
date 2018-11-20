using UnityEngine;
using UnityEngine.UI;

public class UpdateTimer : MonoBehaviour
{
	public GameObject TimerMenu;
	private bool _toggleTimerMenu;
	private float _startTime = 0;
	private float _elapsedTime = 0;
	
	void Start ()
	{
		TimerMenu.SetActive(_toggleTimerMenu);
		_startTime = Time.time;
	}
	
	void Update ()
	{
		if (_toggleTimerMenu)
		{
			_elapsedTime = Time.time - _startTime;
			gameObject.GetComponentInChildren<Text>().text = "Elapsed time: " + _elapsedTime;
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
			_toggleTimerMenu = !_toggleTimerMenu;
			TimerMenu.SetActive(_toggleTimerMenu);
		}
	}
}
