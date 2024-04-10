using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class defender_script : MonoBehaviour
{
    //Store location in x,y,z
    public int x;
    private const int y = 0;
    public int z;
    private const int max_life = 100;
    public int life;
    private const int damage = 1;
    private const int heal_ammount = 1;
    private const int max_energy = 10000;
    private const int start_energy = 1000;

    private const int energy_refill_rate = 10;
    private const int physical_defense = 0;
    private const int magic_defense = 0;
    private const int physical_penetration = 0;
    private const int magic_penetration = 0;
    private const string damage_type = "physical";

    private int energy;

    public attacker_script attacker;
    System.Random random = new System.Random();
    // Start is called before the first frame update
    void Start()
    {
        //Initialize variables
        x = random.Next(10);
        life = max_life;
        energy = start_energy;

        update_graphic();
    }

    // Update is called once per frame
    void Update()
    {
        take_action();
        increase_energy();
        update_graphic();
    }
    
    //Calculate an action based on the defender's known information and take that action
    void take_action() {
        int[] known_information = get_known_information();
        int action = get_action(known_information);
        switch(action) {
            case 0:
                move_left();
                break;
            case 1:
                move_right();
                break;
            case 2:
                shoot();
                break;
            case 3:
                heal();
                break;
            case 4:
                do_nothing();
                break;
        }
    }

    //Gather the information known to the defender and format it for processing
    int[] get_known_information() {
        return null;
    }

    //Decides on an action based on the passed information
    int get_action(int[] known_information) {
        return random.Next(5);
    }

    //Move to the square to the left if it is available
    void move_left() {
        //Checks that the action is affordable; otherwise does nothing this tick
        if(!check_energy(10)) return;
        print_action("move_left");
        x -= 1;
        x = Math.Max(0, x);
    }

    //Move to the square to the right if it is available
    void move_right() {
        //Checks that the action is affordable; otherwise does nothing this tick
        if(!check_energy(10)) return;

        print_action("move_right");
        x += 1;
        x = Math.Min(x, 9);
    }

    //Shoot the closest spawn in the same lane if one is there
    void shoot() {
        //Checks that the action is affordable; otherwise does nothing this tick
        if(!check_energy(50)) return;

        print_action("shoot");
        ArrayList spawns = attacker.spawns;
        spawn_script closest_spawn = null;
        foreach(spawn_script spawn in spawns) {
            if(spawn.x == x && spawn.z >= z && (closest_spawn == null || spawn.z < closest_spawn.z)) {
                closest_spawn = spawn;
            }
        }
        if(closest_spawn != null)
            closest_spawn.take_damage(damage, spawns, physical_penetration, magic_penetration, damage_type);
    }

    //Recover health
    void heal() {
        //Checks that the action is affordable; otherwise does nothing this tick
        if(!check_energy(100)) return;

        print_action("heal");
        life += heal_ammount;
        life = Math.Min(life, max_life);
    }

    //To assist in energy management, sometimes does nothing
    void do_nothing() {
        print_action("do_nothing");
    }

    //Determines if energy is high enough for the intended action
    //Decrements energy if possible
    bool check_energy(int cost) {
        print("A: " + z + " | E: " + energy);
        if(energy < cost) {
            print_action("cannot_aford_action | Costs: " + cost + " of " + energy);
            return false;
        } else {
            energy -= cost;
            return true;
        }
    }

    //Update's the defender's visual representation
    void update_graphic() {
        transform.position = new Vector3(x, y, z);
    }

    //Increases the defender's action each tick
    void increase_energy() {
        energy = Math.Min(max_energy, energy + energy_refill_rate);
    }

    //Prints out an action in a formatted manner
    void print_action(string action) {
        //print("Unit: Defender " + z + " || Action: " + action);  
    }

    /* Take damage dealt by a spawn, and accept defeat if life is reduced to 0
    ** damage_dealt: the original damage dealt by a defender
    ** spawns: the list of spawns, which this spawn may be removing itself from
    ** physical_penetration: the ammount of physical_defense that is ignored by the attack
    ** magic_penetration: the ammount of magic_defense that is ignored by the attack
    ** damage_type: the type of damage, either physical or magic, which corresponds to defense and penetration values
    */
    public void take_damage(int damage_dealt, int physical_penetration, int magic_penetration, string damage_type) {
        int old_life = life;
        int total_damage = damage_dealt;

        //decrease damage by any defense of the damage type, after reducing defense by penetration
        int total_physical_penetration = Math.Max(0, physical_penetration - physical_defense);
        int total_magic_penetration = Math.Max(0, magic_penetration - magic_defense);
        if(damage_type == "physical") {
            total_damage = Math.Max(0, total_damage - total_physical_penetration);
        } else if(damage_type == "magical") {
            total_damage = Math.Max(0, total_damage - total_magic_penetration);
        }

        //Reduce life by the calculated damage
        life -= total_damage;
        if(life <= 0 && old_life > 0) {
            print("GAME OVER, DEFEATED DEFENDER");
        }
    }
}
