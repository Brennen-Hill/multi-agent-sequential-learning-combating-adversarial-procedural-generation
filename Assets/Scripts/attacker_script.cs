using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Unity.MLAgents;

public class attacker_script : Agent
{
    public System.Random random = new System.Random();
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
        //Application.targetFrameRate = 4;
        // register this object with the GameTicker event
        GameTicker.instance.BoardTick.AddListener(OnBoardTick);
        energy = max_energy;
    }

    // Will be called once every game tick/turn
     void OnBoardTick()
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
        spawns.Sort(new SpawnComparer());
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
        int min_regen = 0;
        int max_regen = 5;
        int min_leach = 0;
        int max_leach = 5;
        int min_physical_defense = 0;
        int max_physical_defense = 5;
        int min_magic_defense = 0;
        int max_magic_defense = 5;
        int min_physical_penetration = 0;
        int max_physical_penetration = 5;
        int min_magic_penetration = 0;
        int max_magic_penetration = 5;
        int min_damage_type = 0;
        int max_damage_type = 1;
        return new int[] {choice, random.Next(min_x, max_x), get_simulated_random(min_speed, max_speed),
            get_simulated_random(min_range, max_range), get_simulated_random(min_damage, max_damage), get_simulated_random(min_life, max_life),
            get_simulated_random(min_regen, max_regen), get_simulated_random(min_leach, max_leach),
            get_simulated_random(min_physical_defense, max_physical_defense), get_simulated_random(min_magic_defense, max_magic_defense),
            get_simulated_random(min_physical_penetration, max_physical_penetration), get_simulated_random(min_magic_penetration, max_magic_penetration),
            get_simulated_random(min_damage_type, max_damage_type)
        };
    }

    private int get_simulated_random(int min, int max) {
        if(random.Next(10) == 0) {
            return random.Next(min, max);
        } else {
            return min;
        }
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
        int regen = spawn_data[5];
        int leach = spawn_data[6];
        int physical_defense = spawn_data[7];
        int magic_defense = spawn_data[8];
        int physical_penetration = spawn_data[9];
        int magic_penetration = spawn_data[10];
        int damage_type = spawn_data[11];
        string damage_type_description = damage_type == 0 ? "physical" : "magic";

        //Used to increase cost of actions, rather than decreasing maximum energy,to ensure energy costs are integers
        int cost_multiplier = 2;
        
        //Calculate cost of the action
            //Ratios set to 1.0 have no effect
            //Ratios not set to 1 modify weight of certain rates to matter more than others
            //Some values are incremented by 1 because they may have 0 as a value
        double speed_ratio = 1.0; double range_ratio = 1.0; double damage_ratio = 1.0; double life_ratio = 1.0;
        double leach_ratio = 1.0; double physical_defense_ratio = 3.5; double magic_defense_ratio = 4.5;
        double regen_ratio = 2.0; double physical_penetration_ratio = 2.0; double magic_penetration_ratio = 2.5;
        double damage_type_ratio = 3.0;

        int cost = (int) Math.Ceiling(
            Math.Pow(speed, speed_ratio) *
            Math.Pow(range, range_ratio) *
            Math.Pow(damage, damage_ratio) *
            Math.Pow(life, life_ratio) * 
            Math.Pow(regen + 1, regen_ratio) * 
            Math.Pow(leach, leach_ratio + 1) * 
            Math.Pow(physical_defense + 1, physical_defense_ratio) * 
            Math.Pow(magic_defense + 1, magic_defense_ratio) *
            Math.Pow(physical_penetration + 1, physical_penetration_ratio) *
            Math.Pow(magic_penetration + 1, magic_penetration_ratio) *
            Math.Pow(damage_type + 1, damage_type_ratio) *
            cost_multiplier
        );

        //If the cost of the action is greater than the currrent energy, do nothing
        if(cost > energy) {
            print_action("cannot_aford_action | Costs: " + cost + " of " + energy);
            return;
        }
        energy -= cost;

        //Initialize the new spawn
        GameObject spawn_instance = Instantiate(Unborn_Spawn);
        spawn_script spawn_code = spawn_instance.GetComponent<spawn_script>();
        spawn_code.initialize(x, speed, range, damage, life, regen, leach, physical_defense, magic_defense,
            physical_penetration, magic_penetration, damage_type_description);
        spawns.Add(spawn_code);
    }


    //Increases the spawn's energy each tick
    void increase_energy() {
        energy = Math.Min(max_energy, energy + energy_refill_rate);
        max_energy += max_energy_rate;
    }

    //Prints out an action in a formatted manner
    void print_action(string action) {
        //print("Unit: Attacker || Action: " + action);  
    }


    public class SpawnComparer : IComparer
    {
        public int Compare(object a, object b)
        {
            spawn_script spawnA = (spawn_script)a;
            spawn_script spawnB = (spawn_script)b;
            if (spawnA == null || spawnB == null)
            {
                throw new ArgumentException("Objects are not of type Spawn");
            }
            return spawnA.z.CompareTo(spawnB.z);
        }
    }

    
    public bool check_game_over(){
        foreach(spawn_script spawn in spawns) {
            if(spawn.z <= 0) {
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
