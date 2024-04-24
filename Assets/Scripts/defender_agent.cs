using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using Unity.MLAgents.Policies;

public class defender_agent : defender_script
{

    // private int role 
    // public int pos;
    // private  BehaviorParameters bp;
    public override void OnEpisodeBegin()
    {
        initialize();
    }

    void Update(){
        // Debug.Log(roleAttributes == null);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //int[] known_information =
        Set_known_information(sensor);
        
        //foreach (int info in known_information) {
        //    sensor.AddObservation(info);
        //}
        // sensor.AddObservation(x);
        // foreach (Vector3 spawn_pos in spawn_positions) {
        //     sensor.AddObservation(spawn_pos.x);
        //     sensor.AddObservation(spawn_pos.y);
        //     sensor.AddObservation(spawn_pos.z);
        // }
        // base.CollectObservations(sensor);
    }
    protected override void requestAction()    {
        AddReward(REWARD_ALIVE_PER_TICK);
        RequestDecision();
    }
    protected override void take_action(int action) {
        // int[] known_information = get_known_information();
        // int action = get_action(known_information);
        // RequestDecision();
        // Debug.Log("Agent take Action: " + action);
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
                roleAttributes.roleAction(this);
                break;
            case 5:
                do_nothing();
                break;
        }
    }
    private void Set_known_information(VectorSensor sensor) {
        const int maxSpawns = 16;
        const int maxDefenders = 4;
        int expected_size = 6 + maxDefenders*2 + maxSpawns*7; // 126
        // bp.BrainParameters.VectorObservationSize = expected_size;
        // int vectorSize = bp.BrainParameters.VectorObservationSize;
        sensor.AddObservation(this.Role2int());
        sensor.AddObservation(x);
        sensor.AddObservation(energy);
        sensor.AddObservation(roleAttributes.damage);
        sensor.AddObservation(roleAttributes.physical_defence);
        sensor.AddObservation(roleAttributes.physical_penetration);
        sensor.AddObservation(roleAttributes.magic_defence);
        sensor.AddObservation(roleAttributes.magic_penetration);

        for (int i = 0; i < otherDefenders.Count; i++) {
            defender_script defender = otherDefenders[i];
            if(defender == this) continue;
            sensor.AddObservation(defender.Role2int());
            sensor.AddObservation(defender.x);
        }

        for (int i = otherDefenders.Count; i < maxDefenders - 1; i++) {
            sensor.AddObservation(-1);
            sensor.AddObservation(-1);
        }

        int howManySpawns = 0;
        if (attacker.spawns == null) howManySpawns = 0;
        else howManySpawns = attacker.spawns.Count;
        howManySpawns = howManySpawns > maxSpawns ? maxSpawns : howManySpawns;
        for(int i = 0; i < howManySpawns; i++)
        {
            var spawn = (spawn_script) attacker.spawns[i];
            sensor.AddObservation(spawn.x);
            sensor.AddObservation(spawn.z);
            sensor.AddObservation(spawn.damage);
            sensor.AddObservation(spawn.physical_defense);
            sensor.AddObservation(spawn.physical_penetration);
            sensor.AddObservation(spawn.magic_defense);
            sensor.AddObservation(spawn.magic_penetration);
        }
        
        Debug.Log(otherDefenders.Count);
        int current_size = 6 + maxDefenders*2 + 7*howManySpawns;
        int space_left = expected_size - current_size;
        for(int i = 0; i < space_left; i++) {
            sensor.AddObservation(-1);
        }

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Debug.Log("OnActionReceived" + actions.DiscreteActions[0]);
        take_action(actions.DiscreteActions[0]);
    }

    public void game_over() {
        // print("Reward");
        AddReward(-1.0f);
        EndEpisode();
    }
}
