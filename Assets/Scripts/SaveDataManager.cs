﻿using Assets.Scripts.Types;
using Assets.Scripts.Types.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class SaveDataManager : MonoBehaviour
    {
        private FileStream _saveFileStream;
        private string _savePath;
        private BinaryFormatter _bf;

        private static SaveDataManager _instance;
        public static SaveDataManager Instance { get { return _instance; } }

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            _savePath = Application.persistentDataPath + "\\game.dat";
            _bf = new BinaryFormatter();
        }

        public bool SaveData(int levelIndex, Ranking ranking, int score, float time, int deaths, int maxCombo, int[] secrets)
        {
            try
            {
                SaveData save = LoadData();

                save.Rankings[levelIndex] = ranking;
                save.Scores[levelIndex] = score;
                save.Times[levelIndex] = time;
                save.Deaths[levelIndex] = deaths;
                save.MaxCombos[levelIndex] = maxCombo;
                save.Secrets[levelIndex] = secrets;

                OverwriteSave(save);
                return true;
            } catch (Exception ex)
            {
                Debug.Log($"Error saving data! {ex.Message}");
                return false;
            }
        }

        public SaveData LoadData()
        {
            SaveData save;

            if (!File.Exists(_savePath))
            {
                save = CreateNewSave();
            } else
            {
                _saveFileStream = File.Open(_savePath, FileMode.Open);
                save = (SaveData)_bf.Deserialize(_saveFileStream);
                _saveFileStream.Close();
            }

            return save;
        }

        private SaveData CreateNewSave()
        {
            SaveData save = new SaveData();

            OverwriteSave(save);

            return save;
        }

        private void OverwriteSave(SaveData save)
        {
            _saveFileStream = File.Create(_savePath);
            _bf.Serialize(_saveFileStream, save);
            _saveFileStream.Close();
        }
    }
}