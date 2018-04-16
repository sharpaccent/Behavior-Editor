using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Execute();
    }
}
