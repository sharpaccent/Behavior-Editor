using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class StateManager : MonoBehaviour
    {
        public float health;
        
        public State currentState;


        [HideInInspector]
        public float delta;
        [HideInInspector]
        public Transform mTransform;

        private void Start()
        {
            mTransform = this.transform;
        }

        private void Update()
        {
            if(currentState != null)
            {
                currentState.Tick(this);
            }
        }
    }
}
