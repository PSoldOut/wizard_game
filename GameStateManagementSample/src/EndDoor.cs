using Microsoft.Xna.Framework;


namespace wizard_game
{
    public class EndDoor : Door
    {
        public bool front = false;
        public EndDoor(Point size, Color? color = null) : base(size, color) {clr = new Color(10,0,190);}

        public void SetFront(bool b)
        {
            front = b;
        }


    }

}