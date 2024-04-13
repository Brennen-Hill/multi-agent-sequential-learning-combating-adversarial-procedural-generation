using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public defender_agent[] defenders;
    public attacker_agent attacker;
    // Start is called before the first frame update
    void Start()
    {
        defenders = FindObjectsOfType<defender_agent>();
        attacker = FindObjectOfType<attacker_agent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(attacker.check_game_over()) {
            Debug.Log("Game Over: Attackers Win");
            foreach(defender_agent defender in defenders) {
                defender.game_over();
            }
            attacker.restart();
        }
    }
}
