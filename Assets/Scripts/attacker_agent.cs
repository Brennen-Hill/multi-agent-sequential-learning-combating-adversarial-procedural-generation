using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;

public class attacker_agent : attacker_script
{
    public override void OnEpisodeBegin()
    {
        initialize();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        const int maxSpawns = 16;
        int expected_size = 2 + 4*3 + maxSpawns*15; // 254

        sensor.AddObservation(energy);
        sensor.AddObservation(max_energy);

        for (int i = 0; i < defenders.Count; i++) {
            defender_script defender = defenders[i];
            sensor.AddObservation(defenders[i].Role2int());
            sensor.AddObservation(defenders[i].x);
            sensor.AddObservation(defenders[i].z);
        }

        int howManySpawns = 0;
        if (spawns == null) howManySpawns = 0;
        else howManySpawns = spawns.Count;
        howManySpawns = howManySpawns > maxSpawns ? maxSpawns : howManySpawns;

        for(int i = 0; i < howManySpawns; i++)
        {
            var spawn = (spawn_script) spawns[i];
            sensor.AddObservation(spawn.x);
            sensor.AddObservation(spawn.z);
            sensor.AddObservation(spawn.speed);
            sensor.AddObservation(spawn.range);
            sensor.AddObservation(spawn.damage);
            sensor.AddObservation(spawn.max_life);
            sensor.AddObservation(spawn.life);
            sensor.AddObservation(spawn.regen);
            sensor.AddObservation(spawn.leach);
            sensor.AddObservation(spawn.damage_type == "physical" ? 0 : 1);
            sensor.AddObservation(spawn.physical_defense);
            sensor.AddObservation(spawn.physical_penetration);
            sensor.AddObservation(spawn.magic_defense);
            sensor.AddObservation(spawn.magic_penetration);
            sensor.AddObservation(spawn.magic_penetration);
        }

        while (sensor.ObservationSize() <= expected_size)
        {
            sensor.AddObservation(-1);
        }        
    }
    /*
        actions.DiscreteActions format:
        a list of 12 integers
        0: 0-10
        1: 1-5
        2: 1-25
        3: 1-5
        4: 1-15
        5: 0-3
        6: 0-5
        7: 0-5
        8: 0-5
        9: 0-5
        10: 0-5
        11: 0-5
        12: 0-1
    */
    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-0.01f); //Decrement reward for not winning yet

        //spawn_p is set to 1 if the attacker wants to spawn, otherwise set to 0
        bool spawn_p = 1 == actions.DiscreteActions[actions.DiscreteActions.Length - 1];

        if(spawn_p) {
            int trait_count = 11;
            int[] traits = new int[trait_count];
            for(int i = 0; i < trait_count; i ++) {
                traits[i] = actions.DiscreteActions[i];
            }

            bool spawn_success = spawn(traits);
            if(!spawn_success) AddReward(-1.0f);
        }
    }

    public void game_over() {
        AddReward(100.0f);
        EndEpisode();
    }
}