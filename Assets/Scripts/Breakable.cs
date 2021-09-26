using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Breakable : MonoBehaviour
    {
        public int health;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (health <= 0)
            {
                Die();
            }
        }

        public void ModifyHealth(int modify)
        {
            health += modify;
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
