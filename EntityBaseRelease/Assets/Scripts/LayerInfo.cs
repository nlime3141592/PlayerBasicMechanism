using UnityEngine;

public static class LayerInfo
{
    public static int ground { get; private set; }
    public static int groundMask { get; private set; }

    public static int entity { get; private set; }
    public static int entityMask { get; private set; }

    static LayerInfo()
    {
        ground = LayerMask.NameToLayer("Ground");
        groundMask = 1 << ground;

        entity = LayerMask.NameToLayer("Entity");
        entityMask = 1 << entity;
    }
}