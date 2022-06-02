using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace BladeBreaker.Gameplay.Level
{
    public class EntitySpawner : MonoBehaviour
    {
        public GameObject fx;
        public GameObject spawnableEntity;

        public bool AssociatedEntitiesDestroyed;

        [ShowInInspector] private List<GameObject> aliveEntities;
        // Start is called before the first frame update
        void Start()
        {
            aliveEntities = new List<GameObject>();
        }

        public void SpawnEntity()
        {
            GameObject currentEntity = Instantiate(spawnableEntity, transform.position, quaternion.identity);
            Instantiate(fx, transform.position, quaternion.identity);

            aliveEntities.Add(currentEntity);

            AssociatedEntitiesDestroyed = false;
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < aliveEntities.Count; i++)
            {
                if (aliveEntities[i] == null)
                {
                    aliveEntities.RemoveAt(i);
                    i--;
                }
            }

            if (aliveEntities.Count <= 0)
            {
                AssociatedEntitiesDestroyed = true;
            }
        }

        public void KillAll()
        {
            aliveEntities.ForEach(Destroy);
        }
    }
}

