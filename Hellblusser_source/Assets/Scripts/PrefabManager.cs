using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    // weapons
    [Header("weapons")]
    public GameObject ironSwordPrefab;

    // magic
    [Header("magic")]
    public GameObject fireBallPrefab;
    public GameObject fireCollectPrefab;

    // projectiles
    [Header("projectiles")]
    public GameObject[] boneThrowPrefab;

    // lava
    [Header("lava")]
    public GameObject[] lavaProjectilePrefab;
    public GameObject[] lavaBallPrefab;

    // impact
    [Header("impact")]
    public GameObject[] magicImpactParticlesPrefab;
    public GameObject[] magicImpactParticlesLocalPrefab;
    public GameObject fireImpactParticlesPrefab;
    public GameObject[] dustImpactParticlesPrefab;
    public GameObject[] lavaProjectileImpactParticlesPrefab;
    public GameObject[] lavaBallImpactParticlesPrefab;

    // effects
    [Header("effects")]
    public GameObject firePrefab;
    public GameObject fireChargePrefab;
    public GameObject magicCollectPrefab;

    // damage deal
    [Header("damage deal")]
    public GameObject damageDealPrefab;

    // ui
    [Header("UI")]
    public GameObject[] healthIndicatorPrefab;
    public GameObject[] damageIndicatorPrefab;
    public GameObject[] alertIndicatorPrefab;
    public GameObject[] blessingIconPrefab;
    public GameObject[] shopBrowseItemPrefab;
    public GameObject[] locationSlotPrefab;
    public GameObject[] settingsTextPrefab;

    // misc
    [Header("misc")]
    public GameObject whiteOrbPrefab;
    public GameObject whiteOrbPermanentPrefab;
    public GameObject[] coinPrefab;
    public GameObject[] tearPrefab;
    public GameObject[] keyPrefab;
    public GameObject[] donutPrefab;
    public GameObject[] flyingEyeBallPrefab;
    public GameObject bossExplosionPrefab;
    public GameObject[] whiteBonePrefab;
    public GameObject[] whiteSkullPrefab;
    public GameObject[] redBonePrefab;
    public GameObject[] redSkullPrefab;
    public GameObject[] blackBonePrefab;
    public GameObject[] blackSkullPrefab;

    // props
    [Header("props")]
    public GameObject[] potPrefab;
    public GameObject[] urnPrefab;
    public GameObject[] chestPrefab;
    public GameObject[] cratePrefab;
    public GameObject[] keyChestPrefab;
    public GameObject[] barrelPrefab;
    public GameObject[] sewerRubblePrefab;
    public GameObject[] sewerPlantPrefab;
    public GameObject[] cagePrefab;
    public GameObject[] dungeonRubblePrefab;
    public GameObject[] dungeonPlantPrefab;
    public GameObject[] bonePilePrefab;
    public GameObject[] eyeBallPrefab;
    public GameObject[] hellPlantPrefab;
    public GameObject[] hellGeyserPrefab;
    public GameObject[] moonStatuePrefab;
    public GameObject[] hellPotPrefab;

    // audio
    [Header("audio")]
    public GameObject audioSource2D;
    public GameObject audioSource3D;

    void Awake ()
    {
        instance = this;
    }

    public void SpawnPrefab ( GameObject _o, Vector3 _p, Quaternion _r, float _s )
    {
        if ( _o != null )
        {
            GameObject o = Instantiate(_o, _p, _r);
            Transform tr = o.transform;
            tr.localScale = Vector3.one * _s;
        }
    }

    public GameObject SpawnPrefabAsGameObject ( GameObject _o, Vector3 _p, Quaternion _r, float _s )
    {
        GameObject oRet = null;
        if (_o != null)
        {
            oRet = Instantiate(_o, _p, _r);
            Transform tr = oRet.transform;
            tr.localScale = Vector3.one * _s;
        }
        return oRet;
    }

    public void SpawnDamageDeal ( Vector3 _pos, float _radius, int _amount, Npc.AttackData.DamageType _type, int _duration, Transform _createdBy, float _impactForce, bool _createdByPlayer, DamageDeal.Target _target, NpcCore _npcCoreBy, bool _putOutOfSleep, bool _isKick )
    {
        GameObject o = SpawnPrefabAsGameObject(damageDealPrefab,_pos,Quaternion.identity,1f);
        DamageDeal damageDealScript = o.GetComponent<DamageDeal>();
        damageDealScript.position = _pos;
        damageDealScript.radius = _radius;
        damageDealScript.info = new DamageDeal.Info { amount = _amount, damageType = _type };
        damageDealScript.clearDur = _duration;
        damageDealScript.SetState(DamageDeal.State.Active);
        damageDealScript.createdByTransform = _createdBy;
        damageDealScript.impactForce = _impactForce;
        damageDealScript.createdByPlayer = _createdByPlayer;
        damageDealScript.myTarget = _target;
        damageDealScript.npcCoreBy = _npcCoreBy;
        damageDealScript.putOutOfSleep = _putOutOfSleep;
        damageDealScript.isKick = _isKick;

        // log
        //Debug.Log("spawn damage deal met target: " + _target + " || " + Time.time.ToString());
    }
}
