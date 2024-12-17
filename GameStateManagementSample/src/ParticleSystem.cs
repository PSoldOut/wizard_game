using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



public class Particle
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float TimeToLive; // Lebenszeit in Sekunden
    public Color Color;

    public Particle(Vector2 position, Vector2 velocity, float timeToLive, Color color)
    {
        Position = position;
        Velocity = velocity;
        TimeToLive = timeToLive;
        Color = color;
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

            float timeToLive = (float)(random.NextDouble()*0.125 + 0.125); // Lebenszeit: 0.5 - 1 Sekunde
            particles.Add(new Particle(position, velocity, timeToLive, Color.Red));
        }
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        for (int i = particles.Count - 1; i >= 0; i--)
        {
            var particle = particles[i];
            particle.Velocity.Y += 5.0f;   //graviation
            particle.Position += particle.Velocity * deltaTime; // Position aktualisieren
            particle.TimeToLive -= deltaTime; // Lebenszeit verringern

            if (particle.TimeToLive <= 0)
                particles.RemoveAt(i); // Partikel entfernen
        }
    }

    public void Draw(Texture2D pixelTexture)
    {
        foreach (var particle in particles)
        {
            GameStateManagementGame._spriteBatch.Draw(pixelTexture, particle.Position, null, particle.Color, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.0f);
        }
    }
}
