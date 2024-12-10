using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;
using wizard_game;


//sprite ist zuständig für animationen. ist so gedacht dass der spieler, gegner usw eine Sprite variable kriegen und darüber ihre animationen darstellen können
//man kann auch ein einzelnes bild darstellen, dafür isAnimating einfach auf false setzen


public class Sprite
{
    public Texture2D texture;
    public float animationSpeed { get; set; }
    public float scale;


    double currentTime;
    int currentFrame;
    int hFrames;
    int vFrames;
    public int frameWidth { get; }                             //width in pixels
    public int frameHeight { get; }                            //height in pixels
    int[][] frameAnimations;                                   //int array for animations. an animation is just a sequenze of indices refering to the frame in the spritesheet
    int animationCount;
    int maxAnimations;
    int animationFrameIndex;
    int animationIndex;
    bool isAnimated;                                           //is the sprite animated in general?
    bool isPlaying;
    public float rotation;
    public Vector2 origin;
    private bool flipped;
    Dictionary<String, int> nameToIndexDic;                    //dictionary for mapping name of the anomation to the index in the 2d array
    Color[] currentFrameData;

    public float layerDepth = 0.5f;

    public Sprite(Texture2D texture, int hFrames, int vFrames, float scale, bool isAnimated)
    {
        this.texture = texture;
        this.hFrames = hFrames;
        this.vFrames = vFrames;
        frameWidth = texture.Width / hFrames;
        frameHeight = texture.Height / vFrames;
        this.scale = scale;
        currentFrame = 0;
        animationSpeed = 200;
        animationCount = 0;
        maxAnimations = 10;
        animationFrameIndex = 0;
        animationIndex = 0;
        frameAnimations = new int[maxAnimations][];
        nameToIndexDic = new Dictionary<string, int>();
        currentFrameData = new Color[frameWidth * frameHeight];
        this.isAnimated = isAnimated;
        this.origin = new Vector2(0, 0);
        this.rotation = 0;
        flipped = false;
        this.isPlaying = false;
    }

    public Sprite(Texture2D texture, int hFrames, int vFrames, float scale) : this(texture, hFrames, vFrames, scale, false) { }


    public void Update(GameTime gameTime)
    {
        if (isAnimated && isPlaying)
        {
            currentTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            //setting the next frame of the current animation (animationIndex) after some time.
            if (currentTime >= animationSpeed)
            {
                currentTime -= animationSpeed;
                int animLength = frameAnimations[animationIndex].Length;
                animationFrameIndex = (animationFrameIndex + 1) % animLength;
                if (animationFrameIndex == 0) isPlaying = false;
                currentFrame = frameAnimations[animationIndex][animationFrameIndex];
            }
        }

    }




    public void Draw(int x, int y)
    {
        if (flipped)
            GameStateManagementGame._spriteBatch.Draw(texture, new Rectangle((int)x, (int)y, (int)(frameWidth * scale), (int)(frameHeight * scale)),
                new Rectangle((currentFrame * frameWidth) % texture.Width, ((currentFrame * frameWidth) / texture.Width) * frameHeight, frameWidth, frameHeight), Color.White, rotation, origin, SpriteEffects.FlipHorizontally, layerDepth);
        else
            GameStateManagementGame._spriteBatch.Draw(texture, new Rectangle((int)x, (int)y, (int)(frameWidth * scale), (int)(frameHeight * scale)),
            new Rectangle((currentFrame * frameWidth) % texture.Width, ((currentFrame * frameWidth) / texture.Width) * frameHeight, frameWidth, frameHeight), Color.White, rotation, origin, SpriteEffects.None, layerDepth);
    }




    public int addAnimtaion(int[] animation, String name)
    {
        if (animationCount < maxAnimations)
        {
            frameAnimations[animationCount] = animation;
            nameToIndexDic.Add(name, animationCount);
            animationCount++;
        }
        return animationCount;
    }


    //set the current animation but not restartin it when it was already the current animation
    public void setAnimation(int animationIndex)
    {
        isPlaying = true;
        if (this.animationIndex == animationIndex) return;
        this.animationIndex = animationIndex;
        this.animationFrameIndex = 0;
        currentFrame = frameAnimations[animationIndex][animationFrameIndex];



        texture.GetData(
            0,
            new Rectangle((currentFrame * frameWidth) % texture.Width, ((currentFrame * frameWidth) / texture.Width) * frameHeight, frameWidth, frameHeight),
            currentFrameData,
            0,
            frameHeight * frameWidth

            );

    }

    //set the current animation but not restartin it when it was already the current animation
    public void setAnimation(String name)
    {
        isPlaying = true;
        int animationIndex = nameToIndexDic.GetValueOrDefault(name, -1);
        if (this.animationIndex == animationIndex || animationIndex == -1) return;
        setAnimation(animationIndex);
    }





    public void Flip()
    {
        flipped = !flipped;
    }


    public void setFlipped(bool flipped)
    {
        this.flipped = flipped;
    }

    public void SetScale(float scale)
    {
        this.scale = scale;
    }
    public Color[] GetCurrentColorData()
    {
        return currentFrameData;
    }

}