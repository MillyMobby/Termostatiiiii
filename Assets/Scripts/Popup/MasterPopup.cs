using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MasterPopup : MonoBehaviour
{
    public class Entity
    {
        public Sprite icon;
        public string name;

        public Entity(string iconPath, string name)
        {
            icon = Resources.Load<Sprite>(iconPath);
            this.name = name;
        }
    }

    [SerializeField] private Transform canvas;
    [SerializeField] private EntityPopup entityPrefab;
    private List<Entity> entitties = new List<Entity>();




    public void Start()
    {
        string path = "Sprites/Icons/enemyIcon";
        entitties.Add(new Entity(path, "Goblin"));
        entitties.Add(new Entity(path, "Panizzi"));

        GenerateList();

    }

    public void GenerateList()
    {
        foreach (Entity e in entitties)
        {
            Debug.Log(e.name);
            EntityPopup newEntity = Instantiate(entityPrefab, canvas);
            newEntity.Init(e.icon, e.name);
        }
    }
}
