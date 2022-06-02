using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BladeBreaker.Gameplay.Combat
{
    public class WeaponDropper : MonoBehaviour
    {
        public Dictionary<WeaponObject, float> dropTable;

        public void DropWeapon()
        {
            foreach (KeyValuePair<WeaponObject, float> entry in dropTable)
            {
                if (Random.Range(0f, 1f) <= entry.Value)
                {
                    //TODO: Move weapon object spawning into the weapon itself.
                    GameObject droppedWeapon = Instantiate(entry.Key.Drop, transform.position, Quaternion.identity);
                    droppedWeapon.GetComponent<WeaponPickup>().WeaponObject.Weapon = entry.Key.Weapon;
                    droppedWeapon.GetComponent<EntityMovement>().PushEntity(new Vector2(0.2f, 0.2f));

                    break;
                }
            }
        }
    }

}