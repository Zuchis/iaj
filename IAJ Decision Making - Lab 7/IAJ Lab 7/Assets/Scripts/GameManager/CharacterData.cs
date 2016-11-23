﻿using UnityEngine;

namespace Assets.Scripts.GameManager
{
    public class CharacterData
    {
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Mana { get; set; }
        public int XP { get; set; }
        public float Time { get; set; }
        public int Money { get; set; }
        public int Level { get; set; }
        public GameObject CharacterGameObject { get; private set; }

        public CharacterData(GameObject gameObject)
        {
            this.CharacterGameObject = gameObject;
            this.HP = 100;
            this.MaxHP = 10;
            this.Mana = 100;
            this.Money = 0;
            this.Time = 0;
            this.XP = 0;
            this.Level = 1;
        }
    }
}
