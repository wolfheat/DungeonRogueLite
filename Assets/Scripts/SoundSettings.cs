
using System;

[Serializable]
public class SoundSettings
{
    public bool GlobalMaster { get; set; } = true;
    public bool UseMaster { get; set; } = true;
    public bool UseMusic { get; set; } = true;
    public float MasterVolume { get; set; } = 0.6f;
    public float MusicVolume { get; set; } = 0.1f;
    public bool UseSFX { get; set; } = true;
    public float SFXVolume { get; set; } = 0.3f;
}
