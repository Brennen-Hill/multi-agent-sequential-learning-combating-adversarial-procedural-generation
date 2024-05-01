using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class spawn_script : MonoBehaviour
{
    public int x;
    private const int y = 0;
    public int z = 29;
    public int speed;
    public int range;
    public int damage;
    public int max_life;
    public int life;
    public int regen;
    public int leach;
    public int physical_defense;
    public int magic_defense;
    public int physical_penetration;
    public int magic_penetration;
    public string damage_type;
    public TextMesh header;

    // set this to the bullet prefab asset
    // (will be instantiated every time a bullet is shot)
    [SerializeField]
    private GameObject bulletPrefab;

    private Transform bulletParent;

    public void removeDefences()
    {
        this.physical_defense = 0;
        this.magic_defense = 0;
    }

    private defender_script[] defenders;


    // Start is called before the first frame update
    void Start()
    {
        collect_defenders();        
        update_graphic();

        // register this object with the GameTicker event
        GameTicker.instance.BoardTick.AddListener(OnBoardTick);

        // set the bulletParent
        bulletParent = GameObject.FindGameObjectWithTag("BulletParent").transform;
        if (bulletParent == null) {
            Debug.Log("couldn't find bulletParent (tag an object with \"BulletParent\" tag)");
            bulletParent = transform;
        }
    }

    // Will be called once every tick/turn
    void OnBoardTick()
    {
        move_and_attack();
        regenerate();
        update_graphic();
        set_header();
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
        this.physical_penetration = physical_penetration;
        this.magic_penetration = magic_penetration;
        this.damage_type = damage_type;

        update_graphic();
        set_header();
    }

    private void set_header() {
        header.text =
        "max_life: " + max_life + " | life: " + life + " | damage: " + damage + " | x: " + x + " | z: " + z + " | speed: " + speed + " | range: " + range + "\n" +
        "regen: " + regen + " | leach: " + leach + " | physical_defense: " + physical_defense + " | magic_defense: " + magic_defense + "\n" +
        "damage_type: " + damage_type + " | physical_penetration: " + physical_penetration + " | magic_penetration: " + magic_penetration;
    }

    //Collect the defenders into a set of references for tracking
    void collect_defenders() {
        defenders = new List<defender_script>(FindObjectsOfType<defender_script>()).ToArray();
        // defenders = new defender_script[] {
        //     GameObject.Find("defender_1").GetComponent<defender_script>(),
        //     GameObject.Find("defender_2").GetComponent<defender_script>(),
        //     GameObject.Find("defender_3").GetComponent<defender_script>(),
        //     GameObject.Find("defender_4").GetComponent<defender_script>()
        // };

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
            // Debug.Log($"spawn_script/move_and_attack closest_defender.attributes == null {closest_defender.roleAttributes == null}");
            doBulletAnimation(closest_defender);
            attack(closest_defender);
        }

    }

    private void doBulletAnimation(defender_script target) {
        // figure out the trajectory of the bullet
        Vector3 start = new Vector3(x, y, z);

        Vector3 end;
        // if the bullet does 
        if (target != null) {
            // bullet should end on impact
            end = target.transform.position;
        }
        else {
            // if the bullet does not reach its target, it will despawn after an arbitrary number of units
            end = start + (130 * Vector3.back);
        }

        // create a new instance of prefab
        GameObject newBullet = Instantiate(bulletPrefab, bulletParent);
        Bullet animator = newBullet.GetComponent<Bullet>();

        animator.configureBulletAnimation(start, end);
    }

    //Move torwards the end of the board. Move no closer to a defender than needed to be in range to attack
    void move(int distance) {
        int old_z = z;
        z = Math.Max(z - distance, -1);
        // if(z < 0 && old_z > 0) {
        //     print("GAME OVER, PASSED DEFENDERS");
        //     //reload scene 
        //     // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        // }
    }

    //Attack the nearest defender in the same lane
    void attack(defender_script defender) {
        // Debug.Log($"spawn_script/attack defender.attributes == null {defender.roleAttributes == null}");
        Debug.Log("SPAWN SHOOT");
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
    public (int, bool) take_damage(int damage_dealt, ArrayList spawns, int physical_penetration, int magic_penetration, string damage_type) {
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

        return (total_damage, life <= 0);
    }
}
