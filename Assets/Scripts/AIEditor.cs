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
            Handles.DrawWireCube(myObj.wanderRadiusCenterOffset + objTransform.position, Vector3.one*myObj.wanderRadiusBounds);
            myObj.wanderRadiusBounds = Handles.ScaleSlider(myObj.wanderRadiusBounds, 
                myObj.wanderRadiusCenterOffset + objTransform.position,
                Vector3.right, 
                Quaternion.identity, 
                4f, 0.25f);
            myObj.wanderRadiusCenterOffset = Handles.PositionHandle(myObj.wanderRadiusCenterOffset + objTransform.position, Quaternion.identity) - objTransform.position;


            Handles.color = Color.red;
            Handles.DrawWireCube(myObj.leashRadiusCenterOffset + objTransform.position, Vector3.one*myObj.leashRadiusBounds);
            myObj.leashRadiusBounds = Handles.ScaleSlider(myObj.leashRadiusBounds, 
                myObj.leashRadiusCenterOffset + objTransform.position,
                Vector3.right, 
                Quaternion.identity, 
                4f, 0.25f);
            myObj.leashRadiusCenterOffset = Handles.DoPositionHandle(myObj.leashRadiusCenterOffset + objTransform.position, quaternion.identity) - objTransform.position;

            
            Handles.color = Color.green;
            Handles.DrawWireDisc(objTransform.position, Vector3.forward, myObj.detectionRadius);
            myObj.detectionRadius = Handles.ScaleSlider(myObj.detectionRadius,
                objTransform.position,
                Vector3.left,
                Quaternion.identity,
                3f, 0.25f);
        }
    }
