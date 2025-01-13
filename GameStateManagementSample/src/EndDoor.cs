using Microsoft.Xna.Framework;


namespace wizard_game
{
    public class EndDoor : Door
    {
        public bool front = false;
        public int goldNeeded;
        public EndDoor(Point size, Color? color = null) : base(size, color) 
        {
            clr = new Color(14,24,150);
            goldNeeded = 20;
            text = goldNeeded + " gold";
        }

        public void SetFront(bool b)
        {
            front = b;
        }


    }

}