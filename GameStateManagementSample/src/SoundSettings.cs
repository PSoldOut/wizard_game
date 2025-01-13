using System.Collections.Generic;
using wizard_game;

public class SoundSettings
        {
            private int musicVolume;
            private int soundVolume;
            private int masterVolume;
            private List<GameEntity> listeners;

            public SoundSettings()
            {
                listeners = new List<GameEntity>();
                musicVolume = 1;
                soundVolume = 1;
                masterVolume = 1;
            }
            public void AddListener(GameEntity listener)
            {
                listeners.Add(listener);
            }
            public void SetMusicVolume(int v)
            {
                musicVolume = v;
                foreach (GameEntity l in listeners)
                {
                    l.RefreshVolume(GetVolumeForSound(), GetVolumeForMusic());
                }
            }
            public int GetMusicVolume()
            {
                return musicVolume;
            }
            public void SetSoundVolume(int v)
            {
                soundVolume = v;
                foreach (GameEntity l in listeners)
                {
                    l.RefreshVolume(GetVolumeForSound(), GetVolumeForMusic());
                }
            }
            public int GetSoundVolume()
            {
                return soundVolume;
            }
            public void SetMasterVolume(int v)
            {
                masterVolume = v;
                foreach (GameEntity l in listeners)
                {
                    l.RefreshVolume(GetVolumeForSound(), GetVolumeForMusic());
                }
            }
            public int GetMasterVolume()
            {
                return masterVolume;
            }

            public float GetVolumeForSound()
            {
                if (soundVolume == 0 || masterVolume == 0) return 0.0f;
                else return soundVolume/8.0f + masterVolume/8.0f;
            }

            public float GetVolumeForMusic()
            {
                if (musicVolume == 0 || masterVolume == 0) return 0.0f;
                else return musicVolume/8.0f + masterVolume/8.0f;
            }


        }