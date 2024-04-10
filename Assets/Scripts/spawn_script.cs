using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class spawn_script : MonoBehaviour
{
    public int x;
    private const int y = 0;
    public int z = 29;
    private int speed;
    private int range;
    private int damage;
    private int max_life;
    private int life;
    private int regen;
    private int leach;
    private int physical_defense;
    private int magic_defense;
    private int physical_penetration;
    private int magic_penetration;
    private string damage_type;

    private defender_script[] defenders;


    // Start is called before the first frame update
    void Start()
    {
        collect_defenders();        
        update_graphic();
    }

    // Update is called once per frame
    void Update()
    {
        move_and_attack();
        regenerate();
        update_graphic();
    }

    //Update's the spawn's visual representation
    void update_graphic() {
        transform.position = new Vector3(x, y, z);
    }

    //Initialize the spawn with procedurally generated values
    public void initialize(int x, int speed, int range, int damage, int life, int regen, int leach, int physical_defense,
            int magic_defense, int physical_penetration, int magic_penetration, string damage_type) {
        this.x = x;
        this.speed = speed;
        this.range = range;
        this.damage = damage;
        this.max_life = life;
        this.life = life;
        this.regen = regen;
        this.leach = leach;
        this.physical_defense = physical_defense;
        this.magic_defense = magic_defense;
        this.physical_defense = physical_defense;
        this.magic_defense = magic_defense;
        this.damage_type = damage_type;
        this.physical_penetration = physical_penetration;
        this.magic_penetration = magic_penetration;
        this.damage_type = damage_type;
    }

    //Collect the defenders into a set of references for tracking
    void collect_defenders() {
        defenders = new defender_script[] {
            GameObject.Find("defender_1").GetComponent<defender_script>(),
            GameObject.Find("defender_2").GetComponent<defender_script>(),
            GameObject.Find("defender_3").GetComponent<defender_script>(),
            GameObject.Find("defender_4").GetComponent<defender_script>()
        };

    }

    //Move if possible and attack if possible
    void move_and_attack() {        
        defender_script closest_defender = null;
        foreach (defender_script defender in defenders) {
            if(defender.x == x && defender.z <= z && (closest_defender == null || defender.z > closest_defender.z)) {
                closest_defender = defender;
            }
        }

        int distance_to_move = 0;
        if(closest_defender == null) {
            distance_to_move = speed;
        } else {
            distance_to_move = Math.Min(z - closest_defender.z - range, speed);
        }
        if(distance_to_move > 0) move(distance_to_move);

        if(closest_defender != null && z - closest_defender.z <= range) {
            attack(closest_defender);
        }

    }

    //Move torwards the end of the board. Move no closer to a defender than needed to be in range to attack
    void move(int distance) {
        int old_z = z;
        z = Math.Max(z - distance, -1);
        if(z < 0 && old_z > 0) {
            print("GAME OVER, PASSED DEFENDERS");
        }
    }

    //Attack the nearest defender in the same lane
    void attack(defender_script defender) {
        defender.take_damage(damage, physical_penetration, magic_penetration, damage_type);
    }

    //Heal some life every tick if given regen on generation
    void regenerate() {
        life = Math.Min(max_life, life + regen);
    }

    /* take_damage: take damage, as dealt by a defender and die if all life is lost
    ** damage_dealt: the original damage dealt by a defender
    ** spawns: the list of spawns, which this spawn may be removing itself from
    ** physical_penetration: the ammount of physical_defense that is ignored by the attack
    ** magic_penetration: the ammount of magic_defense that is ignored by the attack
    ** damage_type: the type of damage, either physical or magic, which corresponds to defense and penetration values
    */
    public void take_damage(int damage_dealt, ArrayList spawns, int physical_penetration, int magic_penetration, string damage_type) {
        int total_damage = damage_dealt;

        //decrease damage by any defense of the damage type, after reducing defense by penetration
        int total_physical_defense = Math.Max(0, physical_defense - physical_penetration);
        int total_magic_defense = Math.Max(0, magic_defense - magic_penetration);
        if(damage_type == "physical") {
            total_damage -= total_physical_defense;
        } else if(damage_type == "magic") {
            total_damage -= total_magic_defense;
        }

        //Reduce life by the calculated damage
        total_damage = Math.Max(total_damage, 0);
        life -= damage_dealt;
        if(life <= 0) {
            spawns.Remove(this);
            Destroy(gameObject);
        }
    }
}
