using BladeBreaker.Gameplay.Level;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts {
    [CustomEditor(typeof(Door))]
    public class DoorEditor : Editor {
        private void OnSceneGUI() {
            Handles.color = Color.red;
            
            Door myObj = (Door) target;
            
            Handles.DrawWireCube(myObj.Location, Vector3.one);
            myObj.Location = Handles.PositionHandle(myObj.Location, Quaternion.identity);
            myObj.LocationCameraBounds = CheckCollision(myObj.Location);

        }
        
        private Collider2D CheckCollision(Vector3 check) {
            List<Collider2D> bounds = new List<Collider2D>();
            Transform boundsParent = GameObject.Find("Camera Bounds").transform;
            foreach (Transform child in boundsParent) {
                Collider2D b;
                if (child.TryGetComponent(out b)) {
                    bounds.Add(b);
                }
            }
            
            foreach (Collider2D b in bounds) {
                if (b.bounds.Contains(check)) {
                    return b;
                }
            }

            return null;
        }
    }
}