using System;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(EntityAI))]
    public class AIEditor : Editor {
        private void OnSceneGUI() {
            Handles.color = Color.yellow;
            EntityAI myObj = (EntityAI) target;
            Transform objTransform = myObj.transform;
            
            //Wander
            Handles.color = Color.magenta;
            Handles.DrawWireCube(myObj.wanderRadiusCenter, Vector3.one*myObj.wanderRadiusBounds);
            Handles.PositionHandle(myObj.wanderRadiusCenter, quaternion.identity);
            myObj.wanderRadiusBounds = Handles.ScaleSlider(myObj.wanderRadiusBounds, 
                myObj.wanderRadiusCenter,
                Vector3.right, 
                Quaternion.identity, 
                4f, 0.25f);
            myObj.wanderRadiusCenter = Handles.DoPositionHandle(myObj.wanderRadiusCenter, quaternion.identity);

            Handles.color = Color.red;
            Handles.DrawWireCube(myObj.leashRadiusCenter, Vector3.one*myObj.leashRadiusBounds);
            Handles.PositionHandle(myObj.leashRadiusCenter, quaternion.identity);
            myObj.leashRadiusBounds = Handles.ScaleSlider(myObj.leashRadiusBounds, 
                myObj.leashRadiusCenter,
                Vector3.right, 
                Quaternion.identity, 
                4f, 0.25f);
            myObj.leashRadiusCenter = Handles.DoPositionHandle(myObj.leashRadiusCenter, quaternion.identity);
            
            Handles.color = Color.green;
            Handles.DrawWireDisc(objTransform.position, Vector3.forward, myObj.detectionRadius);
            myObj.detectionRadius = Handles.ScaleSlider(myObj.detectionRadius,
                objTransform.position,
                Vector3.left,
                Quaternion.identity,
                3f, 0.25f);
        }
    }
