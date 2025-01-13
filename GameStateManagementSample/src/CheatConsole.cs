using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using wizard_game;
using GameStateManagement;

public class CheatConsole
{
    private string currentInput = "";
    private List<string> cheatMessages = new List<string>();
    private KeyboardState previousState;
    private SpriteFont spriteFont;
    private bool IsActive = false;
    private bool actDown = false;
    public CheatConsole(SpriteFont font)
    {
        spriteFont = font;
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState currentState = Keyboard.GetState();
        if (currentState.IsKeyDown(Keys.D0) && !actDown)
        {
            actDown = true;
            SetIsActive(!GameplayScreen.cheat.GetIsActive());
            return;
        }
        if (currentState.IsKeyUp(Keys.D0) && actDown)
        {
            actDown = false;
            return;
        }
        if (!IsActive) return;

        foreach (var key in currentState.GetPressedKeys())
        {
            if (previousState.IsKeyUp(key))
            {
                if (key == Keys.Enter)
                {
                    ProcessCheatInput(currentInput);
                    currentInput = "";
                }
                else if (key == Keys.Back && currentInput.Length > 0)
                {
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
                else if (key >= Keys.A && key <= Keys.Z)
                {
                    currentInput += key.ToString().ToLower();
                }
                else if (key >= Keys.D0 && key <= Keys.D9)
                {
                    currentInput += ((int)key - (int)Keys.D0).ToString();
                }
            }
        }

        previousState = currentState;
    }


    public void SetIsActive(bool b)
    {
        IsActive = b;
    }

    public bool GetIsActive()
    {
        return IsActive;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;
        spriteBatch.DrawString(spriteFont, "Cheat: " + currentInput, new Vector2(10, 10), Color.White);

        int y = 30;
        foreach (var message in cheatMessages)
        {
            spriteBatch.DrawString(spriteFont, message, new Vector2(10, y), Color.Yellow);
            y += 20;
        }
    }

    private void ProcessCheatInput(string input)
    {
        if (input.Equals("gold"))
        {
            Player.Get().coins = 999;
            return;
        }
        int v = int.Parse(input);
        GameplayScreen.map.activeRoom.acteurs.Remove(Player.Get());
        GameplayScreen.map.activeRoom = GameplayScreen.map.rooms[v];
        GameplayScreen.map.activeRoom.acteurs.Add(Player.Get());
        Map.level = GameplayScreen.map.activeRoom.level;
        GameplayScreen.map.SameLevel();
    }

    private void EnableGodMode()
    {
        // Logik zum Aktivieren des God Mode
    }

    private void AddLife()
    {
        // Logik zum Hinzufügen eines Lebens
    }
}