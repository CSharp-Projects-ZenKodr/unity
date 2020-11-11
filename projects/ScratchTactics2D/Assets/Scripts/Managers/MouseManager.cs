﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
	private Dictionary<Enum.GameState, OverlayTile> tileOptions;
	private GameGrid currentActiveGrid;
	
	private Vector3 prevMousePos;
	private float timeSinceLastMove = 0.0f;
	
	[HideInInspector] public Vector3Int prevMouseGridPos;
	[HideInInspector] public Vector3Int currentMouseGridPos;
	
	// assigned in inspector
	// currently NOT instantiated
	public MouseSelector mouseSelectorPrefab;
	[HideInInspector] public MouseSelector mouseSelector;
	
	// dont' use Awake here, to avoid bootstrapping issues
    void Start() {
		currentMouseGridPos = Vector3Int.zero;
		
		tileOptions = new Dictionary<Enum.GameState, OverlayTile>() {
			[Enum.GameState.overworld] = ScriptableObject.CreateInstance<SelectOverlayTile>() as SelectOverlayTile,
			[Enum.GameState.battle] = ScriptableObject.CreateInstance<SelectOverlayIsoTile>() as SelectOverlayIsoTile
		};
    }

    void Update() {
		prevMousePos = Input.mousePosition;
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		// in overworld mode:
		if (GameManager.inst.gameState == Enum.GameState.overworld) {
			currentActiveGrid = GameManager.inst.worldGrid;
			
		// in tactics mode:
		} else if (GameManager.inst.gameState == Enum.GameState.battle) {
			currentActiveGrid = GameManager.inst.tacticsManager.GetActiveGrid();
		}
		
		// store old position and get new position
		prevMouseGridPos = currentMouseGridPos;
		currentMouseGridPos = currentActiveGrid.Real2GridPos(mouseWorldPos);
		
		// reset idle timer
		if (HasMouseMovedGrid()) {
			timeSinceLastMove = 0.0f;
		} else {
			timeSinceLastMove += Time.deltaTime;
		}
		
		//
		// overlay selection tile
		//
		// remove previous highlighting
		// convert back to real after messing w/ active grid
		//RedrawSelectTile(currentActiveGrid.Grid2RealPos(currentMouseGridPos));
		OverlaySelectTile();
		
		// debug
		if (Input.GetMouseButtonDown(0)) {
			Debug.Log("worldPos: " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
			Debug.Log("currentMouseGridPos: " + currentMouseGridPos);
			Debug.Log("currentActiveGrid: " + currentActiveGrid);
		}
    }
	
	public void RedrawSelectTile(Vector3 pos) {
		if (HasMouseMovedGrid()) {
			mouseSelector.gameObject.SetActive(true);
			mouseSelector.MoveTo(pos);
		}
	}
	
	public void OverlaySelectTile() {
		if (HasMouseMovedGrid()) {		
			currentActiveGrid.ResetSelectionAt(prevMouseGridPos);
			if (currentActiveGrid.IsInBounds(currentMouseGridPos)) {
				currentActiveGrid.SelectAt(currentMouseGridPos, tileOptions[GameManager.inst.gameState]);
			}
		} else {
			if (timeSinceLastMove >= 3.0f) {
				currentActiveGrid.ResetSelectionAt(prevMouseGridPos, fadeRate: 0.005f);
			}
		}
	}
	
	public bool HasMouseMovedGrid() {
		return prevMouseGridPos != currentMouseGridPos;
	}
}