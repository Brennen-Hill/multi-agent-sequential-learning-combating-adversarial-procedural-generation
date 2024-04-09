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
    public attacker_script attacker;
    System.Random random = new System.Random();
    // Start is called before the first frame update
    void Start()
    {
        x = random.Next(10);
        life = max_life;
        update_graphic();
    }

    // Update is called once per frame
    void Update()
    {
        take_action();
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
        }
    }

    //Gather the information known to the defender and format it for processing
    int[] get_known_information() {
        return null;
    }

    //Decides on an action based on the passed information
    int get_action(int[] known_information) {
        return random.Next(4);
    }

    //Move to the square to the left if it is available
    void move_left() {
        print_action("move_left");
        x -= 1;
        x = Math.Max(0, x);
    }

    //Move to the square to the right if it is available
    void move_right() {
        print_action("move_right");
        x += 1;
        x = Math.Min(x, 9);
    }

    //Shoot the closest spawn in the same lane if one is there
    void shoot() {
        print_action("shoot");
        ArrayList spawns = attacker.spawns;
        spawn_script closest_spawn = null;
        foreach(spawn_script spawn in spawns) {
            if(spawn.x == x && spawn.z >= z && (closest_spawn == null || spawn.z < closest_spawn.z)) {
                closest_spawn = spawn;
            }
        }
        if(closest_spawn != null)
            closest_spawn.take_damage(damage, spawns);
    }

    //Recover health
    void heal() {
        print_action("heal");
        life += heal_ammount;
        life = Math.Min(life, max_life);
    }

    //Update's the defender's visual representation
    void update_graphic() {
        transform.position = new Vector3(x, y, z);
    }

    //Prints out an action in a formatted manner
    void print_action(string action) {
        //print("Unit: Defender " + z + " || Action: " + action);  
    }

    //Take damage dealt by a spawn, and accept defeat if life is reduced to 0
    public void take_damage(int damage_dealt) {
        int old_life = life;
        life -= damage_dealt;
        if(life <= 0 && old_life > 0) {
            print("GAME OVER, DEFEATED DEFENDER");
        }
    }
}
