using System.Collections.Generic;
using UnityEngine;

public class GameEntity
{
    private int _ID;
    private int _color; 
    private int _current_pf;

    private bool _assigned;

    public GameEntity() { }

    public struct Action
    {
        public string actionName;
        public string description;
        public int range;
        public int damage;
    }

    private List<Action> _actions;
    public List<Action> actions { get { return _actions; } set { _actions = value; } }
    public int color
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
        }

    }

    public bool assigned
    {
        get
        {
            return this._assigned;
        }
        set
        {
            this._assigned = value;
        }

    }


    public int current_pf
    {
        get
        {
            return this._current_pf;
        }
        set
        {
            if (value < 0)
            {
                this._current_pf = 0;
            }
            else
            {
                this._current_pf = value;
            }
        }
    }


    public int ID
    {
        get
        {
            return this._ID;
        }
        set
        {
            this._ID = value;
        }

    }

}
