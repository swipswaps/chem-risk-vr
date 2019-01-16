using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WriteTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log(Application.persistentDataPath);

        var sw = new StreamWriter(Application.persistentDataPath + "/test.txt");
        sw.WriteLine("Test");
        sw.Close();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
