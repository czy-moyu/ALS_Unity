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
        private string guid = Guid.NewGuid().ToString();
        
        public string GetGuid => guid;
    }
}