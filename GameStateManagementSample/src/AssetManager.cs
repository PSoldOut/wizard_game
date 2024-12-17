using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Manager
{
    public class AssetManager
    {
        private static AssetManager instance;
        Dictionary<string, object> data =new Dictionary<string, object>(); 
        public AssetManager()
        {
            Debug.WriteLine("INIT MANAGER #####");

        }
        public static void Create()
        {
                if (instance == null) instance = new AssetManager();
                
        }
        public static object Get(string path)
        {
            Create();
            try{
                //Debug.WriteLine("Get Asset form "+path);
                return instance.data[path];
            }
            catch (KeyNotFoundException)
            {
                object asset = GameStateManagementGame.Get().Content.Load<object>(path);
                instance.data.Add(path,asset);
                return asset;
            }
            
            //return new Texture2D();
        }
        public static SoundEffect GetSound(string path)
        {
            return (SoundEffect)Get(path);
        }
        public static  SoundEffectInstance GetSoundInstance(string path)
        {
            return GetSound(path).CreateInstance();
        }
        public static Texture2D GetTexture(string path)
        {
            return (Texture2D)Get(path);
        }
        public static SpriteFont GetFont(string path)
        {
            return (SpriteFont)Get(path);
            
            
        }


    }
}