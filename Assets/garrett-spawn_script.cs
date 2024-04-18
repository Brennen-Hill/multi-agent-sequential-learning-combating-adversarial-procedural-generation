using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


public class spawn_script : MonoBehaviour
{
    public int x;
    private const int y = 0;
    public int z = 29;
    public int speed;
    public int range;
    public int damage;
    public int life;
    public int delayTime = 50;
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
        update_graphic();
    }

    //Update's the spawn's visual representation
    void update_graphic() {
        transform.position = new Vector3(x, y, z);
    }

    //Initialize the spawn with procedurally generated values
    public void initialize(int x, int speed, int range, int damage, int life) {
        this.x = x;
        this.speed = speed;
        this.range = range;
        this.damage = damage;
        this.life = life;
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
            var task1 = SwitchToDieAsync(delayTime);
        }
    }

    //Called to delay the scene from switching when attacker crosses defenders
    private static async Task SwitchToDieAsync(int milliseconds)
    {
        await Task.Delay(milliseconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        print("Switched Scenes");
    }

    //Attack the nearest defender in the same lane
    void attack(defender_script defender) {
        defender.take_damage(damage);
    }

    //take damage, as dealt by a defender and die if all life is lost
    public void take_damage(int damage_dealt, ArrayList spawns) {
        life -= damage_dealt;
        if(life <= 0) {
            spawns.Remove(this);
            Destroy(gameObject);
        }
    }
}
