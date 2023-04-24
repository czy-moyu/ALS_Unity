using System;
using UnityEngine;
using SGuid = System.Guid;

namespace Moyu.Anim
{
    [Serializable]
    public class BaseGraph
    {
        [SerializeField]
        [ReadOnly]
        private string name;
    
        public string Name {
            get => name;
            set => name = value;
        }
        
        [SerializeField]
        [ReadOnly]
        private string guid = SGuid.NewGuid().ToString();
        
        public string Guid => guid;
    }
}