using UnityEngine;
using System.Collections;

public class Selection : MonoBehaviour {
	Unit _selectedUnit;

	// Use this for initialization
	void Start () {
	
	}

	Vector3 endPos;
	// Update is called once per frame
	void Update () {
		if (GUIUtility.hotControl == 0)
		{
			if (Input.GetMouseButtonDown(0)){ // if left button pressed...
				RaycastHit hit = new RaycastHit();
				Ray ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit) && (hit.transform.tag == "Team1" || hit.transform.tag == "Team2")){
					Debug.Log("Selected Unit");

					SelectUnit(hit.transform.gameObject);
				}
				else
				{
					//Debug.Log("Deselect Unit " + hit.transform.name);
					DeselectUnit();
				}
			}
			else if (_selectedUnit != null)
			{
				if (Input.GetMouseButtonDown(1)){ // if left button pressed...
					RaycastHit hit = new RaycastHit();
					Ray ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out hit))
					{
						if (_selectedUnit.Moving)
						{
							endPos = hit.point;
							_selectedUnit.MoveTo(endPos);
						}
						else if (_selectedUnit.Attacking && hit.transform.tag == "Unit")
						{
							Unit attackee = hit.transform.GetComponent<Unit>();
							if (attackee.Team != TurnManager.CurrentTeam)
							{
								_selectedUnit.Attack(attackee);
								Debug.Log("Attack");
							}
						}
					}
				}
			}
		}
	}

	float _menuHeight = 80;
	void OnGUI()
	{
		GUI.Box(new Rect(0, Screen.height - _menuHeight, 115, _menuHeight), "Unit data");
		GUI.Box(new Rect(115, Screen.height - _menuHeight, Screen.width - 215, _menuHeight), "Unit Action");
		GUI.Box(new Rect(Screen.width - 100, Screen.height - _menuHeight, 100, _menuHeight), "");

		if (_selectedUnit != null)
		{			
			GUI.Label(new Rect(5, Screen.height - 65, 100, 20), "Health: " + _selectedUnit.CurrentHealth);
			GUI.Label(new Rect(5, Screen.height - 50, 100, 20), "Moves left: " + _selectedUnit.LeftMoves);
			GUI.Label(new Rect(5, Screen.height - 35, 100, 20), "Health Packs: " + _selectedUnit.HealthPacks);

			GUI.enabled = !(TurnManager.UseAI && TurnManager.TeamUsingAI == TurnManager.CurrentTeam) && TurnManager.CurrentTeam == _selectedUnit.Team;
			if (GUI.Button(new Rect(120, Screen.height - 55, 100, 40), "Move"))
			{
				_selectedUnit.UseTurn(UnitActions.Move, null);
			}
			if (GUI.Button(new Rect(225, Screen.height - 55, 100, 40), "Attack"))
			{
				_selectedUnit.UseTurn(UnitActions.Attack, null);
			}
			if (GUI.Button(new Rect(330, Screen.height - 55, 100, 40), "Heal"))
			{
				_selectedUnit.UseTurn(UnitActions.Heal, null);
			}
			if (GUI.Button(new Rect(435, Screen.height - 55, 100, 40), "Guard"))
			{
				_selectedUnit.UseTurn(UnitActions.Guard, null);
			}
			GUI.enabled = true;
		}

		GUI.enabled = !(TurnManager.UseAI && TurnManager.TeamUsingAI == TurnManager.CurrentTeam);
		if (GUI.Button(new Rect(Screen.width - 95, Screen.height - 55, 90, 40), "End Turn"))
		{
			ChangeTurn();
		}
		GUI.enabled = true;
	}

	void ChangeTurn()
	{
		DeselectUnit();

		TurnManager.SwitchPlayer();
	}

	void SelectUnit(GameObject obj)
	{
		DeselectUnit();

		_selectedUnit = obj.GetComponent<Unit>();

		if (_selectedUnit.Alive)
		{
			if (TurnManager.UseAI && TurnManager.TeamUsingAI == TurnManager.CurrentTeam)
			{
			}
			else
			{
				_selectedUnit.Active = true;
			}
		}
		else
		{
			Debug.Log("Unit is dead");
			DeselectUnit();
		}
	}

	void DeselectUnit()
	{
		if (_selectedUnit != null)
			_selectedUnit.Active = false;

		_selectedUnit = null;
	}
}
