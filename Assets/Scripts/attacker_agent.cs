using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class attacker_agent : MonoBehaviour
{
    private System.Random random = new System.Random();
    public GameObject Unborn_Spawn;
    public ArrayList spawns = new ArrayList();
    // Start is called before the first frame update
    void Start()
    {
        //Reduces the framerate to make visualization easier
        Application.targetFrameRate = 2;
    }

    // Update is called once per frame
    void Update()
    {
        int[] known_information = get_known_information();
        int[] action = get_action(known_information);
        switch(action[0]) {
            case 0:
                do_nothing();
                break;
            case 1:
                int[] spawn_data = action.Skip(1).ToArray();
                spawn(spawn_data);
                break;
        }
    }

    //Gather the information known to the attacker and format it for processing
    int[] get_known_information() {
        return null;
    }

    //Decides on an action based on the passed information
    int[] get_action(int[] known_information) {
        int num_actions = 2;
        int choice = random.Next(num_actions);
            //OVERRIDING choice: decrease number of times action 0 is taken in example
            choice = Math.Max(random.Next(num_actions + 3) - 3, 0);
        int min_x = 0;
        int max_x = 10;
        int min_speed = 1;
        int max_speed = 5;
        int min_range = 1;
        int max_range = 5;
        int min_damage = 1;
        int max_damage = 5;
        int min_life = 1;
        int max_life = 5;        
        return new int[] {choice, random.Next(min_x, max_x), random.Next(min_speed, max_speed),
                random.Next(min_range, max_range), random.Next(min_damage, max_damage), random.Next(min_life, max_life)};
    }

    //To assist in simulating energy management during the random actions, sometimes does nothing
    void do_nothing() {
        print_action("do_nothing");
    }

    //Spawns a spawn that moves torward the other side of the board
    void spawn(int[] spawn_data) {
        print_action("spawn");
        GameObject spawn_instance = Instantiate(Unborn_Spawn);
        spawn_script spawn_code = spawn_instance.GetComponent<spawn_script>();
        spawn_code.initialize(spawn_data[0], spawn_data[1], spawn_data[2], spawn_data[3], spawn_data[4]);
        spawns.Add(spawn_code);
    }

    //Prints out an action in a formatted manner
    void print_action(string action) {
        //print("Unit: Attacker || Action: " + action);  
    }

    public bool check_game_over(){
        foreach(spawn_script spawn in spawns) {
            if(spawn.z == 0) {
                return true;
            }
        }
        return false;
    }

    public void restart(){
        foreach(spawn_script spawn in spawns) {
            Destroy(spawn.gameObject);
        }
        spawns.Clear();
        
    }
}
