using UnityEngine;

[System.Serializable]
public class Character
{
        public int AC { get; set; }
        public int Initiative { get; set; }
        public int Pass_Perc { get; set; }
        public string Hit_Dice { get; set; }
        public int Str { get; set; }
        public int Dex { get; set; }
        public int Con { get; set; }
        public int Int { get; set; }
        public int Wis { get; set; }
        public int Cha { get; set; }
        public int Curr_Pf { get; set; }
        public string Class { get; set; }
        public int Level { get; set; }
        public string Race { get; set; }
        public string Name { get; set; }

        public int Max_Pf { get; set; }

        public string Bio { get; set; }

        public bool Assigned {get; set; }
}