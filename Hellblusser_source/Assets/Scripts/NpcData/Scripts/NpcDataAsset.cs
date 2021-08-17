using UnityEngine;
using Npc;

[CreateAssetMenu(fileName = "NpcData", menuName = "ScriptableObjects/Npc/NpcDataAsset", order = 1)]
public class NpcDataAsset : ScriptableObject
{
    public GameObject prefab;
    public Stats stats;
    public Npc.Graphics graphics;
    public Audio audio;
    public AttackData[] attacks; 

    public Info Load ()
    {
        return new Info { prefab = prefab, stats = stats, graphics = graphics, audio = audio, attacks = attacks };
    }
}
