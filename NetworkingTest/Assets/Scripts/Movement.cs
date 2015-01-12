using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    public float speed = 1f;
	
	// Update is called once per frame
	void Update ()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.Translate(x * Time.deltaTime * speed, y * Time.deltaTime * speed, 0f);
	}
}
