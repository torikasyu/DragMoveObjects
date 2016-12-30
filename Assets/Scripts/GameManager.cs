using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public LayerMask panelLayer;

	private GameObject holdObj = null;	// Touched object
	private Vector3 beforePos = Vector3.zero;

	private GameObject[,] map = null;

	// Use this for initialization
	void Start () {
		map = new GameObject[5,5];

		for (int y = -2; y >= 2; y++)
		{
			for (int x = -2; x >= 2; x++) 
			{
				Collider2D col = Physics2D.OverlapPoint (new Vector2 (x, y), panelLayer);
				if (col) {
					map [x+2, y+2] = col.gameObject;
				} else {
					map [x+2, y+2] = null;
				}
			}
		}
			
	}
	
	// Update is called once per frame
	void Update () {

		// Hold and Move object
		if(Input.touchCount > 0)
		{
			Touch t = Input.GetTouch(0);

			switch(t.phase)
			{		
			case TouchPhase.Began:

				if (this.holdObj != null)
					return;

				Vector3 touchPoint_screen = new Vector3(t.position.x,t.position.y,1);
				Vector3 touchPoint_world = Camera.main.ScreenToWorldPoint(touchPoint_screen);

				Collider2D col =  Physics2D.OverlapPoint(touchPoint_world,panelLayer);	// Detect Touched Block
				if(col)
				{
					// Hold Touched Block										
					this.holdObj = col.gameObject;
					this.beforePos = col.transform.position;
					this.holdObj.layer = LayerMask.NameToLayer ("L_MovingPanel");
				}								
				break;

			case TouchPhase.Stationary :
				break;

			case TouchPhase.Moved :
				on_move(t);
				break;

			default:	// End or Cancel
				//print(t.phase);
				on_end ();				
				break;
			}
		}

	}

	private void on_move(Touch t)
	{
		if(this.holdObj == null)
		{
			return;
		}
		else
		{
			Vector3 curPos = holdObj.transform.position;
			Vector3 myDeltaPos = Camera.main.ScreenToWorldPoint(new Vector3(t.position.x,t.position.y,1)) - curPos;

			Vector3 newPos = Vector3.zero;

			// Move touched block
			newPos = Vector3.zero;
			newPos = curPos + myDeltaPos;

			if(newPos.x > 3 || newPos.x < -3 || newPos.y > 3 || newPos.y < -3)
			{
				print ("Out of Range!");
			}
			else
			{			
				this.holdObj.transform.position = newPos;
			}
		}
	}

	private void on_end()
	{		
		if(this.holdObj)
		{
			Vector3 fixPos;
			Vector3 curPos;

			curPos = this.holdObj.transform.position;
			fixPos = new Vector3(Mathf.RoundToInt(curPos.x),Mathf.RoundToInt(curPos.y),0);

			float x = fixPos.x;
			float y = fixPos.y;

			if (x > 2)
				x = 2;
			if (x < -2)
				x = -2;
			if (y > 2)
				y = 2;
			if (y < -2)
				y = -2;

			fixPos = new Vector3 (x, y, 0);

			Collider2D col = Physics2D.OverlapPoint (fixPos, panelLayer);
			if (col) {
				StartCoroutine ("SmoothMovement", beforePos);
			} else {
				StartCoroutine ("SmoothMovement", fixPos);

				print(beforePos.x + ":" + beforePos.y);

				map [(int)beforePos.x + 2, (int)beforePos.y + 2] = null;
				map [(int)fixPos.x + 2, (int)fixPos.y + 2] = this.holdObj;
				this.checkLine ();
			}

			this.holdObj.layer = LayerMask.NameToLayer ("L_Panel");
		}
	}

	protected IEnumerator SmoothMovement (Vector3 end) {

		float sqrRemainingDistance = (this.holdObj.transform.position - end).sqrMagnitude;
		//float inverseMoveTime = 1f / 0.2f;
		float speed = 100f;

		while (sqrRemainingDistance > float.Epsilon) {	
			Vector3 newPosition = Vector3.MoveTowards (this.holdObj.transform.position, end, speed * Time.deltaTime);
			this.holdObj.transform.position = newPosition;
			sqrRemainingDistance = (this.holdObj.transform.position - end).sqrMagnitude;
			yield return null;
		}
		//print ("coroutine finished");

		this.holdObj = null;
		this.beforePos = Vector3.zero;
	}

	private void checkLine()
	{
		for (int y = -2; y >= 2; y++)
		{
			for (int x = -2; x >= 2; x++) 
			{
				Debug.Log (map [x + 2, y + 2].gameObject.name);
			}
		}
	}

}
