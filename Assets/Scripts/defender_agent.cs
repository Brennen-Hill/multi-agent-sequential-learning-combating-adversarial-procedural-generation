using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;

public class defender_agent : defender_script
{
    // private int role 
    // public int pos;
    public override void OnEpisodeBegin()
    {
        initialize();
    }

    void Update(){
        // Debug.Log(roleAttributes == null);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        int[] known_information = Get_known_information();
        
        foreach (int info in known_information) {
            sensor.AddObservation(info);
        }
        // sensor.AddObservation(x);
        // foreach (Vector3 spawn_pos in spawn_positions) {
        //     sensor.AddObservation(spawn_pos.x);
        //     sensor.AddObservation(spawn_pos.y);
        //     sensor.AddObservation(spawn_pos.z);
        // }
        // base.CollectObservations(sensor);
    }
    protected override void requestAction()    {
        AddReward(0.01f);
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
                if(shoot()) {
                    AddReward(0.01f);
                }
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
    private int[] Get_known_information() {
        int input_size = 40;
        if(attacker.spawns ==null) {Debug.Log($"Attacker.spawn is NULL!!"); return new int[input_size];}
        int[] rtn = new int[input_size];
        rtn[0] = this.Role2int();
        rtn[1] = x;
        for(int i = 0; i < otherDefenders.Count; i++) {
            defender_script defender = otherDefenders[i];
            if(defender == this) continue;
            rtn[i*2+2] = defender.Role2int();
            rtn[i*2+3] = defender.x;
        }
        int current_size = otherDefenders.Count*2;
        int space_left = input_size - current_size;
        for(int i = 0; i + 1 < space_left; i+=2) {
            if(i/2 < attacker.spawns.Count){
                spawn_script spawn = (spawn_script)attacker.spawns[i/2];
                rtn[current_size + i] = spawn.x;
                rtn[current_size + i+1] = spawn.z;
            }else{
                rtn[current_size + i] = -1;
                rtn[current_size + i+1] = -1;
            }
        }
        return rtn;
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
