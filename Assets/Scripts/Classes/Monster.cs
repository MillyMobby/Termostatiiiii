using System.Drawing;

[System.Serializable]
public class Monster
{
   public static int color = 2;
   public string Name { get; set; }
   public int Max_Pf { get; set; }
   public int AC { get; set; }
   public int Initiative { get; set; }
   public float? CR { get; set; }
   public string Type { get; set; }
}
