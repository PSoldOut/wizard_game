namespace wizard_game{
    public enum Gamestate{
        WALL,
        ENEMY,
        GOLD,
        ITEM,
        PLAYER,
        EMPTY
    }

    public enum EnemyAction{
        GO_EAST,
        GO_WEST,
        GO_SOUTH,
        GO_NORTH,
        NONE
    }

    public enum EnemyType{
        WIZARD,
        GUARD,
        SKELETON,
        PRISONER,
        KNIGHT,
        DOUBLER,
        MAGIE
    }
}