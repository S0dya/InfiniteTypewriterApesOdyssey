public interface ISaveable
{
    GameObjectSave GameObjectSave { get; set; }
    void ISaveableRegister();
    void ISaveableDeregister();
    GameObjectSave ISaveableSave();
    void ISaveableLoad(GameObjectSave gameObjectSave);
}
