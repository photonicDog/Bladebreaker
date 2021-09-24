using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Components;
using Assets.Scripts.Types.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyStats : SerializedMonoBehaviour, IStats {
    public float health;
    [SerializeField] private Dictionary<WeaponType, float> dropTable;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0) {
            Die();
        }
    }

    public void ModifyHealth(float modify) {
        health += modify;
    }

    public void Die() {
        DropWeapon();
        //Enemy explosion effect
        Destroy(gameObject);
    }

    void DropWeapon() {
        foreach (KeyValuePair<WeaponType, float> entry in dropTable) {
            if (Random.Range(0f, 1f) <= entry.Value) {
                //Spawn weapon entity using dictionary stored in GameObject
                break;
            }
        }
    }
}
