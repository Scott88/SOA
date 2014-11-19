using UnityEngine;
using System.Collections;

public class AudioPlaypos : MonoBehaviour {

	public float pos;

	void Start () 
	{
		audio.time = pos;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
