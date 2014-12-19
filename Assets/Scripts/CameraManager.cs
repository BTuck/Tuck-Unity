//Sauce = https://www.youtube.com/watch?v=2wgeDQlwnQ0
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraManager : MonoBehaviour 
{
	//Selection
	public Texture2D SelectionHighlight = null;
	public static Rect m_Selection = new Rect(0,0,0,0);//Static so the calculation is only done once and not have to be repeated for each selectable unit(performance)
	private Vector3 StartClick = -Vector3.one;
	//Movement
	private static Vector3 MoveToDesitination = Vector3.zero;
	private static List<string> Passables = new List<string>() {"Floor"};
	//Camera Movement
	public float ZoomMaxY = 0;
	public float ZoomMinY = 0;
	public float ZoomSpeed = 0.5f;
	public float ZoomTime = 0.25f;
	public Vector3 ZoomDestination = Vector3.zero;
	void Update () 
	{
		CheckCamera();
		ZoomCamera();
		CleanUP();
	}
	private void ZoomCamera()
	{
		float MoveY = (Input.GetAxis("Mouse ScrollWheel"));
		if(MoveY !=0)
		{
			ZoomDestination = transform.position - new Vector3(0, MoveY * ZoomSpeed, 0);
		}
		if(ZoomDestination !=Vector3.zero)
		{
			transform.position = Vector3.Lerp(transform.position, ZoomDestination, ZoomTime);
			if(transform.position == ZoomDestination)//Reset
			{
				ZoomDestination = Vector3.zero;
			}
		}
		if(transform.position.y > ZoomMaxY)
		{
			transform.position = new Vector3(transform.position.x, ZoomMaxY, transform.position.z);
		}
		if(transform.position.y < ZoomMinY)
		{
			transform.position = new Vector3(transform.position.x, ZoomMinY, transform.position.z);
		}
	}
	private void CheckCamera()
	{
		if(Input.GetMouseButtonDown(0))
		{
			StartClick = Input.mousePosition;
		}
		else if(Input.GetMouseButtonUp(0))
		{
			StartClick = -Vector3.one;
		}
		if(Input.GetMouseButton(0))
		{
			m_Selection = new Rect(StartClick.x, InvertMouseY(StartClick.y), Input.mousePosition.x - StartClick.x, InvertMouseY( Input.mousePosition.y) - InvertMouseY(StartClick.y));
			if(m_Selection.width < 0)
			{
				m_Selection.x += m_Selection.width;
				//Needs to be positive
				m_Selection.width = -m_Selection.width;
			}
			if(m_Selection.height < 0)
			{
				m_Selection.y += m_Selection.height;
				//Needs to be positive
				m_Selection.height = -m_Selection.height;
			}
		}
	}
	private void OnGUI()
	{
		if(StartClick != -Vector3.one)
		{
			GUI.color = new Color(0,0,1,0.5f);
			GUI.DrawTexture(m_Selection,SelectionHighlight);
		}
	}
	public static float InvertMouseY(float y)
	{
		return Screen.height -y;
	}
	private void CleanUP()
	{
		if(!Input.GetMouseButtonUp(1))
		{
			MoveToDesitination = Vector3.zero;//Resets to "NULL"
		}
	}
	public static Vector3 GetDestination()
	{
		if (MoveToDesitination == Vector3.zero)
		{
			RaycastHit hit;
			Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(r, out hit))
			{
				while(!Passables.Contains(hit.transform.gameObject.name))//Switch to tags
				{
					if(!Physics.Raycast(hit.point, r.direction, out hit))
						break;
				}
			}
			if(hit.transform !=null)
			{
				MoveToDesitination = hit.point;
			}
		}
		return MoveToDesitination;//Should return 0 or a point
	}
}
