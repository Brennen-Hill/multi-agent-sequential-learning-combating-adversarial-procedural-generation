using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class attacker_script : MonoBehaviour
{
    private System.Random random = new System.Random();
    public GameObject Unborn_Spawn;
    public ArrayList spawns = new ArrayList();

    private int max_energy = 1000;
    private int energy;
    private const int energy_refill_rate = 10;
    private const int max_energy_rate = 1;
    // Start is called before the first frame update
    void Start()
    {
        //Reduces the framerate to make visualization easier
        Application.targetFrameRate = 4;

        energy = max_energy;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the next action
        int[] known_information = get_known_information();
        int[] action = get_action(known_information);
        int choice = action[0];

        increase_energy();
        
        switch(choice) {
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

    //To assist in energy management, sometimes does nothing
    void do_nothing() {
        print_action("do_nothing");
    }

    //Spawns a spawn that moves torward the other side of the board
    void spawn(int[] spawn_data) {
        //Extract data from spawn_data
        int x = spawn_data[0];
        int speed = spawn_data[1];
        int range = spawn_data[2];
        int damage = spawn_data[3];
        int life = spawn_data[4];
        
        //Calculate cost of the action
        //Ratios currently are set to 1.0 and have no effect, but can be modified to weight curtain rates as mattering
            //more than others
        double speed_ratio = 1.0; double range_ratio = 1.0; double damage_ratio = 1.0; double life_ratio = 1.0;
        int cost = (int) Math.Ceiling(
            Math.Pow(speed, speed_ratio) *
            Math.Pow(range, range_ratio) *
            Math.Pow(damage, damage_ratio) *
            Math.Pow(life, life_ratio)
        ) * 3;

        //If the cost of the action is greater than the currrent energy, do nothing
        if(cost > energy) {
            print_action("cannot_aford_action | Costs: " + cost + " of " + energy);
            return;
        }
        energy -= cost;

        GameObject spawn_instance = Instantiate(Unborn_Spawn);
        spawn_script spawn_code = spawn_instance.GetComponent<spawn_script>();
        spawn_code.initialize(x, speed, range, damage, life);
        spawns.Add(spawn_code);
    }

    void increase_energy() {
        energy = Math.Min(max_energy, energy + energy_refill_rate);
        max_energy += max_energy_rate;
    }

    //Prints out an action in a formatted manner
    void print_action(string action) {
        //print("Unit: Attacker || Action: " + action);  
    }

}
