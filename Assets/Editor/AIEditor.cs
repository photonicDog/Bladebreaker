using BladeBreaker.Gameplay.Entities;
using System;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(EnemyModel))]
    public class AIEditor : Editor {
        private void OnSceneGUI() {
            Handles.color = Color.yellow;
            EnemyModel myObj = (EnemyModel) target;
            Transform objTransform = myObj.transform;

            Handles.color = Color.magenta;
            for (int i = 0; i < myObj.wanderNodes.Count; i++) {
                Handles.DrawWireCube(myObj.wanderNodes[i], Vector3.one);
                myObj.wanderNodes[i] = Handles.DoPositionHandle(myObj.wanderNodes[i], Quaternion.identity);
                //TEST
            }
            
            //Leash
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
