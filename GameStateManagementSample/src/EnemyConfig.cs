using wizard_game;

public class RoomEnemyConfig
{
    public EnemyType Type { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }

    public RoomEnemyConfig(EnemyType type, int min, int max)
    {
        Type = type;
        Min = min;
        Max = max;
    }


}
