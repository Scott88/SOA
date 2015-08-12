using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Parallax : MonoBehaviour
{
	public Vector2 speed = new Vector2(10, 10);
	public Vector2 direction = new Vector2(1, 0);
	public bool isLooping = false;

	private List<Transform> backgroundPart;

	void Start()
	{

		if (isLooping)
		{
			backgroundPart = new List<Transform>();
			
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
			
				if (child.GetComponent<Renderer>() != null)
				{
					backgroundPart.Add(child);
				}
			}

			backgroundPart = backgroundPart.OrderBy(t => t.position.x).ToList();
		}
	}
	
	void FixedUpdate()
	{// paralax if the game is not won, not paused, if the player is moveing, or attached to a moving object.
		if (GameWin.Win != true && PauseButton.paused != true)
		{
			if (!GetComponent<Renderer>() || GetComponent<Renderer>().IsVisibleFrom(Camera.main))
			{
				if (Player.move == true || Player.isAttached == true)
				{ 
					Vector3 movement = new Vector3(
						speed.x * direction.x,
						speed.y * direction.y,
						0);

					movement *= Time.fixedDeltaTime;
					transform.Translate(movement);
				}
			}
			else
			{
				// for use on tiled background images, will paralax once past camera X
				if (isLooping)
				{
					Transform firstChild = backgroundPart.FirstOrDefault();

					if (firstChild != null && firstChild.position.x < Camera.main.transform.position.x)
					{
						Transform lastChild = backgroundPart.LastOrDefault();
						Vector3 lastPosition = lastChild.transform.position;
						Vector3 lastSize = (lastChild.GetComponent<Renderer>().bounds.max - lastChild.GetComponent<Renderer>().bounds.min);

						firstChild.position = new Vector3(lastPosition.x + lastSize.x, firstChild.position.y, firstChild.position.z);

						backgroundPart.Remove(firstChild);
						backgroundPart.Add(firstChild);
					}
				}
			}
		}
	}
}
