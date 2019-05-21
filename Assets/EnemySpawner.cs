using System;
using System.Collections;
using System.Collections.Generic;
using Content.Helpers.Combat;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour {
    public GameObject enemyPrefab;
    public int unitToAttackCount = 10;
    public int productionSpeed = 30;
    public GameObject target;
    private HPHandler.Faction ownFaction;
    private float progress = 0f;

    private void FixedUpdate() {
        progress += Time.deltaTime;
        if (progress > productionSpeed) {
            progress -= productionSpeed;
            spawnUnit();
        }
    }

    private void Start() {
        ownFaction = this.GetComponent<HPHandler>().faction;
    }

    private void updateTarget() {
        var potTargets = GameObject.FindGameObjectsWithTag("dropBase");
        var minDist = Single.MaxValue;
        foreach (var elem in potTargets) {
            var dist = Vector3.Distance(elem.transform.position, this.transform.position);
            if (dist < minDist) {
                minDist = dist;
                target = elem;
            }
        }
    }
    
    private void spawnUnit() {
        NavMeshHit hit;
        var suc = NavMesh.SamplePosition(this.transform.position, out hit, 20f, NavMesh.AllAreas);
        if (!suc) return;
        var x = GameObject.Instantiate(enemyPrefab, hit.position, Quaternion.identity);
        try {
            x.GetComponent<CombatUnit>().moveRand();
        }
        catch (NullReferenceException ex) {
        }

        //check if enough units are close
        var potTargets = HPHandler.factionMembers[ownFaction];
        var toCommand = new List<CombatUnit>();
        foreach (var unit in potTargets) {
            if (unit.GetComponent<CombatUnit>() == null) {
                continue;
            }

            var dist = Vector3.Distance(unit.transform.position, this.transform.position);
            if (dist < 30f) {
                toCommand.Add(unit.GetComponent<CombatUnit>());
            }
        }
            updateTarget();

        if (toCommand.Count >= unitToAttackCount) {
            foreach (var unit in toCommand) {
                //command to attack enemy base
                unit.moveTo(target.transform.position);
            }
        }
    }
}