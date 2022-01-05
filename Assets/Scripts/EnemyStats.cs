using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Components;
using Assets.Scripts.Controllers;
using Assets.Scripts.Types.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyStats : SerializedMonoBehaviour, IStats {
    public float health;
    public Dictionary<WeaponType, float> dropTable;
    public AudioClip explodeSound;
    private EntityAnimation _ea;
    private EntityMovement _em;
    
    // Start is called before the first frame update
    void Start() {
        _ea = GetComponent<EntityAnimation>();
        _em = GetComponent<EntityMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0) {
            Die();
        }
    }

    public void ModifyHealth(int modify) {
        health += modify;
    }

    public void Die() {
        DropWeapon();
        AudioController.Instance.PlayEnemySFX(explodeSound);
        GameManager.Instance.player.GetComponent<PlayerStatsController>().DefeatEnemy();
        GameManager.Instance.player.GetComponent<PlayerStatsController>().IncrementCombo();
        _ea.Die();
        Destroy(gameObject);
    }

    void DropWeapon() {
        foreach (KeyValuePair<WeaponType, float> entry in dropTable) {
            if (Random.Range(0f, 1f) <= entry.Value) {
                Weapon weapon = new Weapon(GameManager.Instance.WeaponData[entry.Key]);
                GameObject droppedWeapon = Instantiate(GameManager.Instance.WeaponDrops[entry.Key], transform.position, quaternion.identity);
                droppedWeapon.GetComponent<WeaponPickup>().weapon = weapon;
                droppedWeapon.GetComponent<EntityMovement>().PushEntity(new Vector2(0.2f, _em.midair?-0.5f:0.2f));
                
                break;
            }
        }
    }
}
