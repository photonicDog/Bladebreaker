using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerChoreographer : SerializedMonoBehaviour {
    public List<EntitySpawner> spawners;
    [NonSerialized][OdinSerialize]public List<SpawnerChoreographyStep> steps;
    public bool loop;
    private SpawnerChoreographyStep currentStep;
    [ShowInInspector] private int stepIndex = 0;

    private bool activated;

    public UnityEvent finishedEvent;


    public void BeginChoreography() {
        activated = true;
        Step();
    }
    
    public void Step() {
        currentStep = steps[stepIndex];
        currentStep.ConstructCommands(spawners);
        currentStep.Do();
    }

    void Update() {
        if (!activated) return;
        if (currentStep == null) return;
        
        if (currentStep.CheckDone()) {
            stepIndex++;
            if (stepIndex > steps.Count-1) {
                if (loop) {
                    stepIndex = 0;
                }
                else {
                    finishedEvent.Invoke();
                    return;
                }
            }
            Step();
        }
    }

    public void KillEverything() {
        spawners.ForEach(a => a.KillAll());
        stepIndex = 0;
    }

    public void EndChoreography() {
        activated = false;
    }
}

[Serializable]
public class SpawnerChoreographyStep {
    [NonSerialized][OdinSerialize]public List<SpawnerCommand> spawnerCommands;

    public void ConstructCommands(List<EntitySpawner> esList) {
        foreach (SpawnerCommand command in spawnerCommands) {
            command.Construct(esList[command.spawnerIndex]);
        }
    }

    public void Do() {
        foreach (SpawnerCommand command in spawnerCommands) {
            command.Spawn();
        }
    }

    public bool CheckDone() {
        return spawnerCommands.All(a => a.Done());
    }
}

[Serializable]
public class SpawnerCommand {
    public int spawnerIndex;

    [HideInInspector] public EntitySpawner spawner;

    public void Construct(EntitySpawner spawner) {
        this.spawner = spawner;
    }
    
    public virtual void Spawn() {
        spawner.SpawnEntity();
    }

    public virtual bool Done() {
        return spawner.AssociatedEntitiesDestroyed;
    }
}

public class SpawnerCommandTrickle : SpawnerCommand {
    public int time;
    public int count;

    private bool DoneSpawning;

    public override void Spawn() {
        SpawnLoop(count);
    }

    private async void SpawnLoop(int ct) {
        for (int i = 0; i < ct; i++) {
            spawner.SpawnEntity();
            
            float ctime = Time.realtimeSinceStartup;

            while (ctime + time > Time.realtimeSinceStartup) {
                await Task.Yield();
            }
        }
        DoneSpawning = true;
    }

    public override bool Done() {
        return DoneSpawning && spawner.AssociatedEntitiesDestroyed;
    }
}

public class SpawnerCommandPureTime : SpawnerCommand {
    public int time;
    public int count;

    private bool DoneSpawning;

    public override void Spawn() {
        SpawnLoop(count);
    }

    private async void SpawnLoop(int ct) {
        for (int i = 0; i < ct; i++) {
            spawner.SpawnEntity();
            
            float ctime = Time.realtimeSinceStartup;

            while (ctime + time > Time.realtimeSinceStartup) {
                await Task.Yield();
            }
        }
        DoneSpawning = true;
    }

    public override bool Done() {
        return DoneSpawning;
    }
}