

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using Microsoft.Xna.Framework.Audio;
using Manager;



public class Particle
{
    public Vector2 Position;
    public Vector2 Velocity;
    public Vector2 acceleration;
    public float TimeToLive; // Lebenszeit in Sekunden
    public Color Color;
    public static Texture2D texture;
    public Particle(Vector2 position, Vector2 velocity, Vector2 acceleration, float timeToLive, Color color)
    {
        this.Position = position;
        this.Velocity = velocity;
        this.acceleration = acceleration;
        TimeToLive = timeToLive;
        Color = color;
        texture = new Texture2D(GameStateManagementGame.Get().GraphicsDevice, 1, 1);
        texture.SetData(new Color[] { Color.White });
    }
}

public class ParticleSystem
{
    private List<Particle> particles = new List<Particle>();
    private Random random = GameplayScreen.rand;
    public void AddBloodEffect(Vector2 position, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // Zufällige Geschwindigkeit für Blutspritzer
            Vector2 velocity = new Vector2(
                (float)(random.NextDouble() * 2 - 1), // X-Richtung: -1 bis 1
                (float)(random.NextDouble() * 2 - 1)  // Y-Richtung: -1 bis 1
            ) * 250f; // Geschwindigkeit skalieren

            Vector2 acceleration = new Vector2(0, 5);

            float timeToLive = (float)(random.NextDouble()*0.125 + 0.125); // Lebenszeit: 0.5 - 1 Sekunde
            particles.Add(new Particle(position, velocity, acceleration, timeToLive, Color.Red));
        }
    }

    public void AddMagicEffect(Vector2 position, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 velocity = new Vector2(
                (float)(random.NextDouble() * 2 - 1), -5 + ((float)(random.NextDouble() * 2 - 1))) * 50f; // Geschwindigkeit skalieren

            Vector2 acceleration = new Vector2(0, -2);

            float timeToLive = (float)(random.NextDouble()*1 + 1);
            Vector2 nPosition = new Vector2((float)(position.X + random.NextDouble() * 20 -10), (float)(position.Y + random.NextDouble() * 10 -5));
            particles.Add(new Particle(nPosition, velocity, acceleration, timeToLive, Color.AliceBlue));
        }
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        for (int i = particles.Count - 1; i >= 0; i--)
        {
            var particle = particles[i];
            particle.Velocity += particle.acceleration;
            particle.Position += particle.Velocity * deltaTime; // Position aktualisieren
            particle.TimeToLive -= deltaTime; // Lebenszeit verringern

            if (particle.TimeToLive <= 0)
                particles.RemoveAt(i); // Partikel entfernen
        }
    }

    public void Draw()
    {
        foreach (var particle in particles)
        {
            GameStateManagementGame._spriteBatch.Draw(Particle.texture, particle.Position, null, particle.Color, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.0f);
        }
    }
}
