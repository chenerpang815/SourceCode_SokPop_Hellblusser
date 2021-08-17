using UnityEngine;
using Npc;
using System.Collections.Generic;

public class NpcDatabase : MonoBehaviour
{
    // instance
    public static NpcDatabase instance;

    // data
    [Header("data")]
    public NpcDataAsset ratDataAsset;
    public NpcDataAsset batDataAsset;
    public NpcDataAsset goblinDataAsset;
    public NpcDataAsset slugDataAsset;
    public NpcDataAsset smallSpiderDataAsset;
    public NpcDataAsset ghostDataAsset;
    public NpcDataAsset ratWarriorDataAsset;
    public NpcDataAsset fireMageDataAsset;
    public NpcDataAsset ratKingDataAsset;
    public NpcDataAsset smallRatDataAsset;
    public NpcDataAsset magicSkullDataAsset;
    public NpcDataAsset faerieDataAsset;
    public NpcDataAsset hellLordDataAsset;
    public NpcDataAsset hellPopjeDataAsset;
    public NpcDataAsset hellFlyingFaceDataAsset;
    public NpcDataAsset skeletonDataAsset;
    public NpcDataAsset redSkeletonDataAsset;
    public NpcDataAsset blackSkeletonDataAsset;
    public NpcDataAsset fireDevilDataAsset;
    public NpcDataAsset fireBatDataAsset;
    public NpcDataAsset smallFireRatDataAsset;
    public NpcDataAsset orcWarriorDataAsset;
    public NpcDataAsset orcMageDataAsset;
    public NpcDataAsset mediumSpiderDataAsset;
    public NpcDataAsset bigSpiderDataAsset;
    public NpcDataAsset bigSlugDataAsset;
    public NpcDataAsset bigRatDataAsset;
    public NpcDataAsset goblinMageDataAsset;
    public NpcDataAsset goblinRangerDataAsset;
    public NpcDataAsset goblinChieftainDataAsset;
    public NpcDataAsset smallRockGolemDataAsset;
    public NpcDataAsset orcLeaderDataAsset;

    // bosses special behaviour
    public enum RatKingSpecialBehaviour { SpawnMinion, LightCannon };
    public enum MagicSkullSpecialBehaviour { ChargeAttack, WildAttack };

    // dictionary
    Dictionary<Type, Info> npcInfo;

    void Awake ()
    {
        instance = this;

        // store info
        npcInfo = new Dictionary<Type, Info>();
        for ( int i = 0; i < (int)(Type.Length); i ++ )
        {
            StoreInfo((Type)i);
        }
    }

    public void StoreInfo ( Type _type )
    {
        bool succeeded = false;
        Info info = new Info();
        switch (_type)
        {
            case Type.Rat: info = ratDataAsset.Load(); break;
            case Type.Bat: info = batDataAsset.Load(); break;
            case Type.Goblin: info = goblinDataAsset.Load(); break;
            case Type.Slug: info = slugDataAsset.Load(); break;
            case Type.SmallSpider: info = smallSpiderDataAsset.Load(); break;
            case Type.Ghost: info = ghostDataAsset.Load(); break;
            case Type.RatWarrior: info = ratWarriorDataAsset.Load(); break;
            case Type.FireMage: info = fireMageDataAsset.Load(); break;
            case Type.RatKing: info = ratKingDataAsset.Load(); break;
            case Type.SmallRat: info = smallRatDataAsset.Load(); break;
            case Type.MagicSkull: info = magicSkullDataAsset.Load(); break;
            case Type.Faerie: info = faerieDataAsset.Load(); break;
            case Type.HellLord: info = hellLordDataAsset.Load(); break;
            case Type.HellPopje: info = hellPopjeDataAsset.Load(); break;
            case Type.HellFlyingFace: info = hellFlyingFaceDataAsset.Load(); break;
            case Type.Skeleton: info = skeletonDataAsset.Load(); break;
            case Type.RedSkeleton: info = redSkeletonDataAsset.Load(); break;
            case Type.BlackSkeleton: info = blackSkeletonDataAsset.Load(); break;
            case Type.FireDevil: info = fireDevilDataAsset.Load(); break;
            case Type.FireBat: info = fireBatDataAsset.Load(); break;
            case Type.SmallFireRat: info = smallFireRatDataAsset.Load(); break;
            case Type.OrcWarrior: info = orcWarriorDataAsset.Load(); break;
            case Type.OrcMage: info = orcMageDataAsset.Load(); break;
            case Type.MediumSpider: info = mediumSpiderDataAsset.Load(); break;
            case Type.BigSpider: info = bigSpiderDataAsset.Load(); break;
            case Type.BigSlug: info = bigSlugDataAsset.Load(); break;
            case Type.BigRat: info = bigRatDataAsset.Load(); break;
            case Type.GoblinMage: info = goblinMageDataAsset.Load(); break;
            case Type.GoblinRanger: info = goblinRangerDataAsset.Load(); break;
            case Type.GoblinChieftain: info = goblinChieftainDataAsset.Load(); break;
            case Type.SmallRockGolem: info = smallRockGolemDataAsset.Load(); break;
            case Type.OrcLeader: info = orcLeaderDataAsset.Load(); break;
        }
        if ( info.prefab != null )
        {
            succeeded = true;
        }
        if (succeeded)
        {
            if (!npcInfo.ContainsKey(_type))
            {
                npcInfo.Add(_type, info);
            }
        }
    }

    public Info LoadInfo ( Type _type )
    {
        return npcInfo[_type];
    }

    public GameObject LoadPrefab ( Type _type )
    {
        return npcInfo[_type].prefab;
    }
}
