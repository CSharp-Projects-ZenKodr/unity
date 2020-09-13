﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
	private Player playerInst;
	private EnemyController enemyControllerInst;
	
	public enum Phase {player, enemy};
	[HideInInspector] public Phase currentPhase { get; private set;	}
	
	// dont' use Awake here, to avoid bootstrapping issues
    void Start() {
		// first phase goes to the player
		currentPhase = Phase.player;
		playerInst = GameManager.inst.player;
		enemyControllerInst = GameManager.inst.enemyController;
    }

    void Update() {
        // wait for phase objects to signal, and change phase for them
		if (currentPhase == Phase.player) {
			//
			// code spins here until player takes its phaseAction
			//
			
			if (playerInst.phaseActionTaken) {
				currentPhase = Phase.enemy;
				enemyControllerInst.TriggerPhase();
			}
		}
		else if (currentPhase == Phase.enemy) {
			//
			// code spins here until all enemies takes their phaseAction
			//
			
			if (enemyControllerInst.phaseActionTaken) {
				currentPhase = Phase.player;
				playerInst.TriggerPhase();
			}
		}
    }
}
