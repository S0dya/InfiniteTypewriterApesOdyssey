using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : SingletonMonobehaviour<AudioManager>
{
    List<EventInstance> eventInstances;
    List<StudioEventEmitter> eventEmitters;

    [HideInInspector] public Dictionary<string, EventInstance> EventInstancesDict;

    [field: SerializeField] public EventReference Music { get; private set; }
    [field: SerializeField] public EventReference Button { get; private set; }
    [field: SerializeField] public EventReference Dialoge { get; private set; }
    [field: SerializeField] public EventReference Finished { get; private set; }

    bool calmMusicIsCurrentlyPlaying;

    Coroutine fadeOutCoroutine;
    Coroutine randomSFXCor;

    protected override void Awake()
    {
        base.Awake();

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
        EventInstancesDict = new Dictionary<string, EventInstance>();
    }

    void Start()
    {
        EventInstancesDict.Add("Music", CreateInstance(Music));
        EventInstancesDict.Add("Button", CreateInstance(Button));
        EventInstancesDict.Add("Dialoge", CreateInstance(Dialoge));
        EventInstancesDict.Add("Finished", CreateInstance(Finished));

        EventInstancesDict["Music"].start();
    }
    public EventInstance CreateInstance(EventReference sound)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(sound);
        eventInstances.Add(eventInstance);

        return eventInstance;
    }

    public void PlayOneShot(string sound)
    {
        EventInstancesDict[sound].start();
    }

    public void ToggleSFX(bool val)
    {
        if (val)
        {
            RuntimeManager.GetBus("bus:/").setVolume(1);
        }
        else
        {
            RuntimeManager.GetBus("bus:/").setVolume(0);
        }
    }
}