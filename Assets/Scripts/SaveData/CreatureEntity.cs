using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CreatureEntity : WorldEntity
{
    public int AC { get; set; }
    public int Initiative { get; set; }

    public struct Action
    {
        public string actionName;
        public string description;
        public int range;
        public int damage;
    }

    private List<Action> Actions { get; set; };
}