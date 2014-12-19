//Sauce = https://www.youtube.com/watch?v=2wgeDQlwnQ0
using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour 
{
	//Selection both types
	public bool m_Selected = false;
	private bool m_SelectedOnClick = false;
	//Selection Polish
	public GameObject SelectionGlow = null;
	private GameObject Glow = null;
	//Movement
	private Vector3 MoveToDest = Vector3.zero;
	public float FloorOffSet = 1.0f;
	public float MoveSpeed = 1.0f;
	public float StopDistanceOffset = 0.5f;
	//Gradual Rotation
	public float RotationSpeed = 2;
	private float CurrentRotationSpeed = 2;
	private bool IsLockedAngle = false;
	private bool IsChoosenDirection = false;

	private void Update () 
	{
		if(renderer.isVisible && Input.GetMouseButton(0))//Performance no need to check if off screen
		{
			if(!m_SelectedOnClick)
			{
				Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position); //UnProject
				camPos.y = CameraManager.InvertMouseY(camPos.y);
				m_Selected = CameraManager.m_Selection.Contains(camPos);
			}
			if(m_Selected && Glow == null)
			{
				renderer.material.color = Color.red;
				Glow = (GameObject)GameObject.Instantiate(SelectionGlow);
				Glow.transform.parent = transform;
				Glow.transform.localPosition = new Vector3(0, (-GetComponent<MeshFilter>().mesh.bounds.extents.y)+0.25f, 0);
			}
			else if (!m_Selected && Glow !=null)
			{
				GameObject.Destroy(Glow);
				Glow = null;
				renderer.material.color = Color.white;//Eventually change to player default
			}
		}
		if(m_Selected && Input.GetMouseButtonUp(1))//Selected & Right Click
		{
			Vector3 _Destination = CameraManager.GetDestination();
			if(_Destination != Vector3.zero)
			{
				//GameObject.GetComponent<NavMeshAgent>().SetDestination(_Destination);
				MoveToDest = _Destination;
				MoveToDest.y += FloorOffSet;
			}
			IsLockedAngle = false;
			IsChoosenDirection = false;
		}
		UpdateMove();
	}
	private void OnMouseDown()
	{
		m_SelectedOnClick = true;
		m_Selected = true;
	}
	private void OnMouseUp()
	{
		//Reset once you lift up
		if(m_SelectedOnClick)
		{
			m_Selected = true;
		}
		m_SelectedOnClick = false;
	}
	private void UpdateMove()
	{
		if(MoveToDest !=Vector3.zero && transform.position !=MoveToDest)
		{
			Vector3 direction = (MoveToDest - transform.position).normalized;
			direction.y = 0;
			if(!IsLockedAngle)
			{
				float _angle = Vector3.Angle(direction, transform.forward);//Angle from current to where you need to face
				if(!IsChoosenDirection)
				{
					//CrossProduct
					Vector3 Cross = Vector3.Cross(transform.forward, direction);
					//DotProduct
					float Dot = Vector3.Dot(Cross,transform.up);

					if(Dot < 0)
					{
						CurrentRotationSpeed = -RotationSpeed;
					}
					else
					{
						CurrentRotationSpeed = RotationSpeed;
					}
					IsChoosenDirection = true;
				}
				if(_angle > RotationSpeed)
				{
					transform.Rotate(new Vector3(0, CurrentRotationSpeed, 0));
				}
				else
				{
					transform.LookAt(new Vector3(MoveToDest.x, transform.position.y, MoveToDest.z));
					IsLockedAngle = true;
				}
			}
			transform.rigidbody.velocity = direction * MoveSpeed;
			if(Vector3.Distance(transform.position, MoveToDest) < StopDistanceOffset)
			{
				MoveToDest = Vector3.zero;
			}
		}
		else
		{
			transform.rigidbody.velocity = Vector3.zero;
			IsLockedAngle = false;
			IsChoosenDirection = false;
		}
	}
}
