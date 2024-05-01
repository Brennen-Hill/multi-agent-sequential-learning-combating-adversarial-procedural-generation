using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class TrainAttackManager : MonoBehaviour
{
    public defender_agent[] defenders;
    public attacker_agent attacker;
    public bool trainDefenders = true;
    // Start is called before the first frame update
    private ArrayList spawns = new ArrayList();
    void Start()
    {
        defenders = FindObjectsOfType<defender_agent>();
        attacker = FindObjectOfType<attacker_agent>();
    }

    // Update is called once per frame
    void Update()
    {
        bool defender_dead = false;
        ArrayList spawn_positions = get_spawn_positions(attacker.spawns);
        foreach(defender_agent defender in defenders) {
            defender.spawn_positions = spawn_positions;
            defender_dead = defender_dead || defender.is_dead();
        }
        if(defender_dead || attacker.check_game_over()) {
            Debug.Log("Game Over: Attackers Win");
            // foreach(defender_agent defender in defenders) {
            //     // defender.game_over();
            // }
            attacker.game_over();
        }
    }

    ArrayList get_spawn_positions(ArrayList spawns) {
        ArrayList positions = new ArrayList();
        foreach(spawn_script spawn in spawns) {
            positions.Add(spawn.transform.position);
        }
        return positions;
    }
}