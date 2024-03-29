using UnityEngine;
using System.Collections.Generic;
using System;
using Gladius;

//namespace Gladius

public class BaseActor : MonoBehaviour
{

    public float MovementSpeed = 2f;
    public float TurnTime = 0.5f;
    public String m_name;
    public Bounds m_bounds = new Bounds();

    private Animator m_animator;

    private string m_characterModelName;
    public string CharacterModelName
    {
        get
        { return m_characterModelName; }
        set
        {
            m_characterModelName = value;
        }

    }


    public GameObject m_healthBar;

    public string HeadModelName
    {
        get;
        set;
    }

    public string ShoulderModelName
    {
        get;
        set;
    }

    public string BodyModelName
    {
        get;
        set;
    }

    public string HandModelName
    {
        get;
        set;
    }

    public string ShoesModelName
    {
        get;
        set;
    }

    public string HelmetModelName
    {
        get;
        set;
    }


    public int CON
    { get { return m_characterData.CON; } set { m_characterData.CON = value; } }

    public int PWR
    { get { return m_characterData.PWR; } set { m_characterData.PWR = value; } }

    public int ACC
    { get { return m_characterData.ACC; } set { m_characterData.ACC = value; } }

    public int DEF
    { get { return m_characterData.DEF; } set { m_characterData.DEF = value; } }

    public int INI
    { 
        get 
        {
            return m_characterData.INI + INIAddition;
        } 
        set 
        { 
            m_characterData.INI = value; 
        } 
    }

    public ArenaStateCommon ArenaStateCommon
    {
        get { return GladiusGlobals.GameStateManager.ArenaStateCommon; }
    }



    public int INIAddition
    {
        get
        {
            if (ArenaStateCommon.Crowd.GetValueForSchool(m_characterData.School) > 75)
            {
                return 20;
            }
            return 0;
        }
    }


    public float MOVE
    { get { return m_characterData.MOV; } set { m_characterData.MOV = value; } }



    public BaseActor()
    {
        ModelHeight = 1f;
        m_characterData = new CharacterData();
        m_characterData.Name = "Test";

    }

    public void Setup()
    {
        gameObject.SetActive(true);

        GameObject playerUIRoot = GameObject.Find("PlayerUIRoot");

        GameObject healthBarPrefab = GameObject.Find("UIHealthBarPanelPrefab");

        m_healthBar = (GameObject)GameObject.Instantiate(healthBarPrefab);

        // parent it to the gui
        m_healthBar.transform.parent = playerUIRoot.transform;

        dfFollowObject follow = m_healthBar.GetComponent<dfFollowObject>();
        follow.attach = gameObject;
        // toggle enabled so the attachment changes are picked up .
        follow.enabled = false;
        follow.enabled = true;


        ArenaStateCommon.TurnManager.AddActor(this);

        Health = MaxHealth = CON * 10;

        SetupSkills();

        SetupAnimationData();

        QueueAnimation(AnimationEnum.Idle);


    }

    static bool setEvents = false;

    public void SetupAnimationData()
    {

        m_clipNameDictionary[AnimationEnum.Idle] = "w_idle-2";
        m_clipNameDictionary[AnimationEnum.Walk] = "walk";
        m_clipNameDictionary[AnimationEnum.Attack1] = IsTwoHanded ? "w_2h_attack-1" : "w_attack-1";
        m_clipNameDictionary[AnimationEnum.Attack2] = IsTwoHanded ? "w_2h_attack-2" : "w_attack-2";
        m_clipNameDictionary[AnimationEnum.Attack3] = IsTwoHanded ? "w_2h_attack-3" : "w_attack-3";

        m_clipNameDictionary[AnimationEnum.BowShot] = "w_bow_action";
        m_clipNameDictionary[AnimationEnum.Stagger] = "hit-2";
        m_clipNameDictionary[AnimationEnum.Die] = "death-1";

        m_clipNameDictionary[AnimationEnum.BlockStart] = IsTwoHanded ? "w_2h_block_start" : "w_bow_block_start";
        m_clipNameDictionary[AnimationEnum.BlockPose] = IsTwoHanded ? "w_2h_block_pose" : "w_bow_block_pose";
        m_clipNameDictionary[AnimationEnum.BlockStart] = IsTwoHanded ? "w_2h_block_end" : "w_bow_block_end";

        m_clipNameDictionary[AnimationEnum.KnockDown] = "w_death-2";
        m_clipNameDictionary[AnimationEnum.GetUp] = "w_get-up";



        m_clipNameDictionary[AnimationEnum.Cast] = "w_cast_spell-1";


        if (!setEvents)
        {
            setEvents = true;
            //AnimationClip animClip = null;
            //m_characterGameObject.animation.GetClip("w_attack-1");
            //if (animClip != null)
            //{
            //    AnimationEvent dpae = new AnimationEvent();
            //    dpae.functionName = "DamagePoint";
            //    dpae.time = 0.2f;
            //    animClip.AddEvent(dpae);
            //}

            //animClip = m_characterGameObject.animation.GetClip("w_bow_action");
            //if(animClip != null)
            //{
            //    AnimationEvent baae = new AnimationEvent();
            //    baae.functionName = "BowFirePoint";
            //    baae.time = 0.2f;
            //    animClip.AddEvent(baae);
            //}
        }

    }

    public void SetMeshData()
    {

        if (ModelName != null)
        {
            GameObject prefab = Resources.Load(ModelName) as GameObject; 
            GameObject load = Instantiate(prefab)as GameObject;
            if (load == null)
            {
                int ibreak=0;
            }
            m_characterGameObject = load;
            m_characterGameObject.transform.parent = transform;

            m_animator = m_characterGameObject.GetComponent<Animator>();
            if (m_animator == null)
            {
                int ibreak = 0;
            }
            Animator goAnimator = gameObject.GetComponent<Animator>();
            if (goAnimator != null)
            {
                m_animator.runtimeAnimatorController = goAnimator.runtimeAnimatorController;
            }


            //if (m_characterGameObject.animation == null)
            //{
            //    m_characterGameObject.AddComponent<Animation>();
            //    int ibreak = 0;
            //}

        }


        if (m_characterGameObject != null)
        {
            Setup();


            SkinnedMeshRenderer[] childMeshes = m_characterGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer m in childMeshes)
            {
                //Debug.Log("Mesh : " + m.name);
                CheckMesh(m, "w_helmet", HelmetModelName);
                CheckMesh(m, "w_head", HeadModelName);
                CheckMesh(m, "w_body", BodyModelName);
                CheckMesh(m, "w_hand", HandModelName);
                CheckMesh(m, "w_shoes", ShoesModelName);
                CheckMesh(m, "w_shoulder", ShoulderModelName);

            }


            SkinnedMeshRenderer[] smra = m_characterGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (smra.Length > 0)
            {
                foreach (SkinnedMeshRenderer smr in smra)
                {
                    m_bounds.min = Vector3.Min(m_bounds.min, smr.bounds.min);
                    m_bounds.max = Vector3.Max(m_bounds.max, smr.bounds.max);

                    if (HandModelName.Equals(smr.name))
                    {
                        Transform[] boneTransforms = smr.bones;
                        foreach (Transform t in boneTransforms)
                        {
                            if (t.name.Equals("Bip01 L Hand"))
                            {
                                m_projectileHandTransform = t;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                // can't find skinned version, look for normal one
                MeshFilter[] mra = m_characterGameObject.GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter mr in mra)
                {
                    Vector3[] verts = mr.mesh.vertices;
                    foreach (Vector3 vert in verts)
                    {
                        m_bounds.min = Vector3.Min(m_bounds.min, vert);
                        m_bounds.max = Vector3.Max(m_bounds.max, vert);
                    }
                }
            }

            // align imported character models.
            if (ModelName.Contains(GladiusGlobals.ModelsRoot))
            {
                m_characterGameObject.transform.localRotation = Quaternion.Euler(new Vector3(270, 180, 0));
                // set scale up.

                float desiredHeight = 0.8f;
                float scalingFactor = desiredHeight / m_bounds.extents.z;

                m_characterGameObject.transform.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
            }






            SetupRagdollItems();

        }
    }

    private void SetupRagdollItems()
    {
        Item item = m_characterData.GetItemAtLocation(ItemLocation.Weapon);
        if (item != null)
        {
            LoadAndAttachModel("RightHandAttach", item.ModelMeshName);
        }
        item = m_characterData.GetItemAtLocation(ItemLocation.Shield);
        if (item != null)
        {
            LoadAndAttachModel("LeftHandAttach", item.ModelMeshName);
        }
        item = m_characterData.GetItemAtLocation(ItemLocation.Helmet);
        if (item != null)
        {
            LoadAndAttachModel("Bip01 Head", item.ModelMeshName);
        }
    }


    private void CheckMesh(SkinnedMeshRenderer m, String matchingName, String variableName)
    {
        if (m.name.StartsWith(matchingName))
        {
            m.enabled = (m.name.Equals(variableName));
        }
    }



    public void SetupCharacterData(CharacterData characterData)
    {
        m_characterData = characterData;

        if (characterData.StartPosition.HasValue)
        {
            ArenaPoint = characterData.StartPosition.Value;
        }
    }




    public DamageType WeaponAffinityType
    {
        get
        {
            Item item = m_characterData.GetItemAtLocation(ItemLocation.Weapon);
            return item != null ? item.DamageType : DamageType.None;
        }
    }

    public DamageType ArmourAffinityType
    {
        get
        {
            Item item = m_characterData.GetItemAtLocation(ItemLocation.Armor);
            return item != null ? item.DamageType : DamageType.None;
        }
    }

    private void DamagePoint()
    {
        // only do this at the animation hitpoint.
        ArenaStateCommon.CombatEngine.ResolveAttack(this, m_currentTarget, CurrentAttackSkill);
    }

    private void BowFirePoint(String name)
    {
        //only do this at the animation hitpoint.
        Projectile projectile = GetProjectile();

        //find hand position
        //Matrix handPos;

        //if (m_animatedModel.FindAbsoluteMatrixForBone("Bip01 L Hand", out handPos))
        //{
        //    Vector3 scaledModelVal = handPos.Translation;
        //    scaledModelVal = Vector3.Transform(scaledModelVal, World);
        //    Matrix4x4
        //    projectile.Position = scaledModelVal;
        //}
        if (m_projectileHandTransform != null)
        {
            Debug.Log("Projectile start pos : " + m_projectileHandTransform.position);
            projectile.Position = m_projectileHandTransform.position;
        }
        projectile.Target = m_currentTarget;
        projectile.AttackSKill = CurrentAttackSkill;
        projectile.gameObject.SetActive(true);
        Debug.Log("BowFirePoint");
    }

    public Projectile GetProjectile()
    {
        if (m_projectile == null)
        {
            GameObject arrowPrefab = (GameObject)Instantiate(Resources.Load("Prefabs/ArrowPrefab"));
            m_projectile = arrowPrefab.GetComponent<Projectile>();
            m_projectile.Owner = this;
        }
        return m_projectile;
    }

    public bool FiringProjectile
    {
        get
        {
            return m_projectile != null && m_projectile.gameObject.activeSelf;
        }
    }

    public float ModelHeight
    {
        get;
        set;
    }

    //public void SetupAttributes()
    //{

    //    m_attributeDictionary[GameObjectAttributeType.Health] = new BoundedAttribute(100);
    //    m_attributeDictionary[GameObjectAttributeType.Accuracy] = new BoundedAttribute(10);
    //    m_attributeDictionary[GameObjectAttributeType.Power] = new BoundedAttribute(10);
    //    m_attributeDictionary[GameObjectAttributeType.Defense] = new BoundedAttribute(10);
    //    m_attributeDictionary[GameObjectAttributeType.Constitution] = new BoundedAttribute(10);

    //    m_attributeDictionary[GameObjectAttributeType.ArenaSkillPoints] = new BoundedAttribute(0, 5, 5);
    //    m_attributeDictionary[GameObjectAttributeType.Affinity] = new BoundedAttribute(0, 0, 10);

    //}

    void AnimationStarted(AnimationEnum anim)
    {
        switch (anim)
        {
            case (AnimationEnum.Attack1):
                {
                    break;
                }
            case (AnimationEnum.Die):
                {
                    break;
                }
        }
    }

    void AnimationStopped(AnimationEnum anim)
    {
        switch (anim)
        {
            case (AnimationEnum.Attack1):
            case (AnimationEnum.Attack2):
            case (AnimationEnum.Attack3):
                {
                    StopAttack();
                    break;
                }
            case (AnimationEnum.Die):
                {
                    StopDeath();
                    break;
                }
            default:
                break;
        }
        m_currentAnimEnum = AnimationEnum.None;
    }

    public void Block(BaseActor actor)
    {
        SnapToFace(actor);
        QueueAnimation(AnimationEnum.BlockStart);
        QueueAnimation(AnimationEnum.BlockPose);
        QueueAnimation(AnimationEnum.BlockEnd);
    }

    public String Name
    {
        get
        {
            return m_characterData.Name;
        }
    }


    public Arena Arena
    {
        get;
        set;
    }

    //public int GetAttributeValue(GameObjectAttributeType attributeType)
    //{
    //    return m_attributeDictionary[attributeType].CurrentValue;
    //}

    //public void SetAttributeValue(GameObjectAttributeType attributeType, int val)
    //{
    //    m_attributeDictionary[attributeType].CurrentValue = val;
    //}

    public void PlayAnimation(AnimationEnum animEnum)
    {
        if (animEnum != m_currentAnimEnum)
        {
            if (m_currentAnimEnum != AnimationEnum.None)
            {
                AnimationStopped(m_currentAnimEnum);
            }
            m_currentAnimEnum = animEnum;

            try
            {
                String key = m_clipNameDictionary[animEnum];

                //m_characterGameObject.animation.Play(key);
                //RuntimeAnimatorController runtimeAnimController = animator.runtimeAnimatorController;
                m_animator.Play(key);

                AnimationStarted(m_currentAnimEnum);
            }
            catch (Exception e)
            {
                Debug.LogWarning("PlayAnimation : " + animEnum);

            }
        }
    }

    private List<AnimationEnum> m_animationQueue = new List<AnimationEnum>();
    public void QueueAnimation(AnimationEnum animEnum)
    {
        if (m_animationQueue.Count == 0 || m_animationQueue[m_animationQueue.Count - 1] != animEnum)
        {
            m_animationQueue.Add(animEnum);
        }
    }


    public void LoadContent()
    {
        QueueAnimation(AnimationEnum.Idle);
    }

    Point m_arenaPoint;

    public Point ArenaPoint
    {
        get
        {
            return m_arenaPoint;
        }

        set
        {
            m_arenaPoint = value;
            Position = Arena.ArenaToWorld(ArenaPoint);
        }


    }

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }

    //        private Transform m_world;
    //        public Transform World
    //        {
    //            get
    //            {
    //                return m_world;
    //            }
    //        }

    public int Health
    {
        get;
        set;
        //get
        //{
        //    return m_attributeDictionary[GameObjectAttributeType.Health].CurrentValue;
        //}
        //set
        //{
        //    m_attributeDictionary[GameObjectAttributeType.Health].CurrentValue = value;
        //}
    }

    public int MaxHealth
    {
        get;
        set;
        //{
        //    return m_attributeDictionary[GameObjectAttributeType.Health].MaxValue;
        //}
    }

    //public int CharacterSkillPoints
    //{
    //    get
    //    {
    //        return m_attributeDictionary[GameObjectAttributeType.CharacterSkillPoints].CurrentValue;
    //    }
    //    set
    //    {
    //        m_attributeDictionary[GameObjectAttributeType.CharacterSkillPoints].CurrentValue = value;
    //    }
    //}

    public int ArenaSkillPoints
    {
        get;
        set;
    }

    public int MaxArenaSkillPoints
    {
        get { return 5; }
    }

    private int _affinity = 2;
    public int Affinity
    {
        get
        {
            return _affinity;
        }
        private set { _affinity = Math.Max(0, Math.Min(value, MaxAffinity)); }
    }


    public void AddAffinity(int val)
    {
        float newVal = val * AffinityPointMultiplier;
        Affinity += val;
    }


    //public float AffinityMultiplier
    //{


    public int MaxAffinity
    {
        get
        {
            //return m_attributeDictionary[GameObjectAttributeType.Affinity].MaxValue;
            return 5;
        }
    }

    public Bounds BoundingBox
    {
        get;
        set;
    }

    public String TeamName
    {
        get { return m_characterData.TeamName; }
    }

    public bool HasShield
    {
        get;
        set;
    }

    public bool IsTwoHanded
    { get; set; }


    public ActorClass ActorClass
    {
        get;
        set;
    }

    public void TakeDamage(AttackResult attackResult)
    {
        if (attackResult.resultType == AttackResultType.Blocked)
        {
            Block(attackResult.damageCauser);
        }
        else if (attackResult.resultType == AttackResultType.Miss)
        {
            StartMiss(attackResult.damageCauser);
        }
        else if (attackResult.resultType != AttackResultType.Miss)
        {
            Health -= attackResult.damageDone;
            UpdateThreatList(attackResult.damageCauser);
            QueueAnimation(AnimationEnum.Stagger);
        }
    }

    public void LoadAndAttachModel(String boneName, String modelName)
    {
        Transform boneTransform = GladiusGlobals.FindChild(boneName, m_characterGameObject.transform);
        if (boneTransform != null)
        {
            GameObject load = Instantiate(Resources.Load(GladiusGlobals.ModelsRoot + modelName)) as GameObject;
            if (load != null)
            {
                load.transform.parent = boneTransform;
                load.transform.localPosition = Vector3.zero;
                load.transform.localRotation = Quaternion.Euler(new Vector3(90, 90, 0));
                if(modelName.Contains("bow"))
                {
                    load.transform.localScale = new Vector3(10f, 10f, 10f);
                }
                else
                {
                    load.transform.localScale = new Vector3(100f, 100f, 100f);
                }
                
                //load.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        else
        {
            Debug.LogWarning("Can't find boneName : " + boneName);
        }
    }



    private void UpdateThreatList(BaseActor actor)
    {
        // todo
    }

    public void Update()
    {

        CheckAnimationEnd();

        if (UnitActive)
        {
            if (m_knockedDown && m_knockDownTurns > 0)
            {
                m_knockDownTurns--;
                if (m_knockDownTurns == 0)
                {
                    GetUp();
                }
            }


            //TurnComplete = CheckTurnComplete();
            if (!TurnComplete)
            {
                UpdateMovement();
                UpdateAttackSkill();
            }
            TurnComplete = CheckTurnComplete();

        }

        if (m_knockedBack)
        {
            UpdateKnockBack();
        }

        CheckState();
        //            }
        //            catch (Exception e)
        //            {
        //                //int ibreak = 0;
        //			throw e;
        //            }
    }

    public void CheckAnimationEnd()
    {
        // if the current anim loops, and we have others queued , or anim has finished then jump to next
        //AnimationClip animClip = m_characterGameObject.animation.clip;

        AnimatorClipInfo[] clipState = m_animator.GetCurrentAnimatorClipInfo(0);
        if (clipState.Length == 0)
        {
            return;
        }

        AnimationClip animClip = clipState[0].clip;
        AnimatorStateInfo animStateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        

        if ((animClip != null && animClip.wrapMode == WrapMode.Loop && m_animationQueue.Count > 0) || animStateInfo.normalizedTime > 1f)
        {
            AnimationStopped(m_currentAnimEnum);
        }

        if (m_currentAnimEnum == AnimationEnum.None)
        {
            if (m_animationQueue.Count > 0)
            {
                AnimationEnum nextAnim = m_animationQueue[0];
                m_animationQueue.RemoveAt(0);
                PlayAnimation(nextAnim);
            }
            else
            {
                //m_currentAnimEnum = AnimationEnum.None;
                QueueAnimation(AnimationEnum.Idle); // or dead?
            }
        }
    }

    public bool CheckTurnComplete()
    {
        if (UnitActive && PlayerControlled && TurnManager.WaitingOnPlayerControl)
        {
            return false;
        }

        if (FollowingWayPoints && m_currentMovePoints > 0)
        {
            return false;
        }

        if (Attacking)
        {
            return false;
        }

        //if (FiringProjectile)
        //{
        //    return false;
        //}

        return true;

    }

    public void Think()
    {
        // Debug.Log("Think");
        if (!PlayerControlled)
        {

            if (!FollowingWayPoints)
            {

                ChooseTargetAndSkill();

                // Are we next to an enemy
                BaseActor enemy = Arena.NextToEnemy(this);
                if (enemy != null)
                {
                    Target = enemy;
                }
                else
                {
                    // pick random spot on arena and pathfind for now.

                    // todo - be a bit cleverer and try and flank enemy, or aim for highground.

                    Point result;
                    BaseActor target = Arena.FindNearestEnemy(this);
                    if (target != null)
                    {
                        Point nearestPoint = Arena.PointNearestLocation(ArenaPoint, target.ArenaPoint, false);
                        if (Arena.FindPath(ArenaPoint, nearestPoint, WayPointList))
                        {
                            ConfirmMove();
                        }
                    }
                }
            }
            ConfirmAttackSkill();
        }
    }

    private void UpdateKnockBack()
    {
        Vector3 targetPoint = Arena.ArenaToWorld(ArenaPoint);
        Vector3 diff = Position - targetPoint;
        diff.Normalize();
        float closeEnough = 0.01f;
        if (diff.sqrMagnitude < closeEnough)
        {
            m_knockedBack = false;
        }
        else
        {
            Position += diff * (float)Time.deltaTime * MovementSpeed;
        }

    }


    private void UpdateMovement()
    {

        if (Turning)
        {
            TurnTimer += Time.deltaTime;
            Rotation = Quaternion.Slerp(Rotation, TargetRotation, (TurnTimer / TurnTime));
            // close enough now to stop?
            //if (QuaternionHelper.FuzzyEquals(Rotation, TargetRotation))
            if (TurnTimer >= TurnTime)
            {
                Rotation = TargetRotation;
                Turning = false;
            }
        }
        else
        {
            if (FollowingWayPoints)
            {
                // mvoe towards the next point.
                if (WayPointList.Count > 0)
                {
                    // this is called every update and animation system doesn't reset if it's current anim
                    // check and see if next point is a ledge/box etc and jump if so.
                    QueueAnimation(AnimationEnum.Walk);
                    //ChooseWalkSkill();

                    Vector3 target = Arena.ArenaToWorld(WayPointList[0]);
                    Vector3 diff = target - Position;
                    float closeEnough = 0.01f;
                    // check that nothings blocked us since this was set.
                    if (Arena.CanMoveActor(this, WayPointList[0]))
                    {
                        if (diff.sqrMagnitude < closeEnough)
                        {
                            // check that nothings blocked us since this was set.
                            if (Arena.CanMoveActor(this, WayPointList[0]))
                            {
                                Arena.MoveActor(this, WayPointList[0]);
                                //Debug.Log ("MoveActor ["+gameObject.name+"] "+WayPointList[0]);
                                diff.Normalize();

                                Quaternion currentHeading = Quaternion.LookRotation(diff);
                                ArenaPoint = WayPointList[0];

                                WayPointList.RemoveAt(0);

                                // rebuild this now we've reached a point.
                                ArenaStateCommon.MovementGrid.RebuildMesh = true;

                                // we've moved one step so reduce our movement.
                                m_currentMovePoints--;

                                // check and see if we need to turn
                                if (WayPointList.Count > 0)
                                {
                                    Vector3 nextTarget = Arena.ArenaToWorld(WayPointList[0]);
                                    Vector3 nextDiff = nextTarget - Position;
                                    nextDiff.Normalize();
                                    Quaternion newHeading = Quaternion.LookRotation(nextDiff);
                                    if (newHeading != currentHeading)
                                    {
                                        FaceDirection(newHeading, TurnTime);
                                    }
                                }
                            }
                        }
                        else
                        {
                            diff.Normalize();
                            {
                                Position += diff * (float)Time.deltaTime * MovementSpeed;
                            }
                        }


                    }
                    else
                    {
                        // we've been blocked from where we were hoping for.
                        // clear state and force a rethink.
                        // pop our character 'back' to last square.
                        ArenaPoint = ArenaPoint;
                        WayPointList.Clear();
                        FollowingWayPoints = false;
                    }

                }
                else
                {
                    // finished moving.
                    FollowingWayPoints = false;
                }
            }
        }
    }

    public void SnapToFace(BaseActor actor)
    {
        if (actor != null)
        {
            Vector3 nextDiff = actor.Position - Position;
            nextDiff.y = 0;
            nextDiff.Normalize();
            OriginalRotation = Rotation;
            Rotation = Quaternion.LookRotation(nextDiff);
        }
    }

    public void FaceDirection(Quaternion newDirection, float turnSpeed)
    {
        if (!Turning)
        {
            Turning = true;
            OriginalRotation = Rotation;
            TargetRotation = newDirection;
            TurnTimer = 0f;
        }
        else
        {
            int ibreak = 0;
        }

    }

    public BaseActor Target
    {
        get
        {
            return m_currentTarget;
        }
        set
        {
            m_currentTarget = value;
        }
    }

    //private void ChooseAttackSkill()
    //{
    //    if (CurrentAttackSkill == null)
    //    {
    //        foreach (AttackSkill skill in m_knownAttacks)
    //        {
    //            if (skill.Name == "Strike")
    //            {
    //                CurrentAttackSkill = skill;
    //                break;
    //            }
    //        }
    //    }
    //}

    //private void ChooseWalkSkill()
    //{
    //    //CurrentAttackSkill = m_knownAttacks.FirstOrDefault(a => a.Name == "Strike");
    //    if (CurrentAttackSkill == null)
    //    {
    //        foreach (AttackSkill skill in m_knownAttacks)
    //        {
    //            if (skill.Name == "Strike")
    //            {
    //                CurrentAttackSkill = skill;
    //                break;
    //            }
    //        }
    //    }
    //}

    public void StartAttack()
    {
        //ChooseAttackSkill();
        // use the combat engine to display skill choice
        ArenaStateCommon.CombatEngineUI.DrawFloatingText(this.Position, Color.yellow, CurrentAttackSkill.Name, 1f);

        GladiusGlobals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Attack started on [{1}] Skill[{2}].", Name, m_currentTarget != null ? m_currentTarget.Name : "NoActorTarget", CurrentAttackSkill.Name));
        AnimationEnum attackAnim = CurrentAttackSkill.Animation != AnimationEnum.None ? CurrentAttackSkill.Animation : AnimationEnum.Attack1;
        QueueAnimation(attackAnim);
        Attacking = true;
    }

    public void StopAttack()
    {
        GladiusGlobals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Attack stopped.", Name));
        m_currentTarget = null;
        Attacking = false;
        CurrentAttackSkill = null;
        // FIXME - need to worry about out of turn attacks (ripostes, groups etc)
    }

    public void UpdateAttackSkill()
    {
        // eventually this will be anim driven for all command anims (except maybe idle...)
        if (CurrentAttackSkill != null && CurrentAttackSkill.Type == "Command")
        {
            if (!Attacking)
            {
                StartAttack();
            }
            else
            {
                StopAttack();
            }
        }
        else
        {
            if (!Attacking && !FollowingWayPoints)
            {



                if (ArenaStateCommon.CombatEngine.IsAttackerInRange(this, m_currentTarget))
                {
                    if (CurrentAttackSkill.FaceOnAttack)
                    {
                        SnapToFace(m_currentTarget);
                    }
                    m_currentTarget.SnapToFace(this);
                    StartAttack();
                }
            }
        }

    }

    private bool m_attacking;

    public bool Attacking
    {
        get
        {
            return m_attacking;
        }
        set
        {
            m_attacking = value;
        }
    }

    public void StartDeath()
    {
        Dead = true;
        GladiusGlobals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Death started.", Name));
        QueueAnimation(AnimationEnum.Die);
        //TurnManager.RemoveActor(this);
    }

    public void StopDeath()
    {
        GladiusGlobals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Death stopped.", Name));
        Arena.RemoveActor(this);

        // For now - we're invisible
        //Visible = false;
        //Enabled = false;
        //prefabGun.SetActiveRecursively(true);
    }

    public virtual void CheckState()
    {
        if (Health <= 0f && !Dead)
        {
            StartDeath();
        }
    }


    public void StartMiss(BaseActor attacker)
    {
        SnapToFace(attacker);
        QueueAnimation(AnimationEnum.Miss);
    }

    //public virtual void StartAction(ActionTypes actionType)
    //{
    //    Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] [{1}] started.", DebugName, actionType));
    //}

    //public virtual void StopAction(ActionTypes actionType)
    //{
    //    Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] [{1}] stopped.", DebugName, actionType));
    //}

    //public override void Draw(GameTime gameTime)
    //{
    //    Draw(Game.GraphicsDevice, Globals.Camera, gameTime);
    //}

    //public virtual void Draw(GraphicsDevice device, ICamera camera, GameTime gameTime)
    //{
    //    if (m_animatedModel != null)
    //    {
    //        m_animatedModel.Draw(device, camera, gameTime);
    //    }
    //}

    public List<Point> WayPointList
    {
        get { return m_wayPointList; }

    }

    private List<Point> OldWayPointList
    {
        get { return m_oldWayPointList; }

    }

    // 
    public void ConfirmMove()
    {
        FollowingWayPoints = true;
    }

    public bool UnitActive
    {
        get;
        set;
    }

    bool Turning
    {
        get;
        set;
    }

    bool FollowingWayPoints
    {
        get;
        set;
    }

    public float TurnTimer
    {
        get;
        set;
    }

    public Quaternion Rotation
    {
        get
        {
            return transform.rotation;
        }
        set
        {
            transform.rotation = value;
        }
    }

    private Quaternion OriginalRotation
    {
        get;
        set;
    }

    private Quaternion TargetRotation
    {
        get;
        set;
    }

    public Vector3 CameraFocusPoint
    {
        get
        {
            return Position + new Vector3(0, 0.5f, 0);
        }
    }

    private bool TurnStarted
    { get; set; }

    public void ActorTurnStarted(BaseActor baseActor)
    {

    }

    public bool MovedThisRound
    {
        get;
        set;
    }

    public void RoundStarted()
    {
        MovedThisRound = false;   
    }

    public void RoundEnded()
    {

    }


    public void StartTurn()
    {

        UnitActive = true;
        TurnComplete = false;
        TurnStarted = true;
        MovedThisRound = true;

        ArenaStateCommon.MovementGrid.RebuildMesh = true;
        m_currentMovePoints = m_totalMovePoints;

        ArenaSkillPoints++;

        if (!PlayerControlled)
        {
            Think();
        }
    }

    // we'll only get here after immunity checks and the like so don't re-check
    public void CauseStatus(String statusName, SkillStatus skillStatus)
    {





    }

    public bool ImmuneToStatus(String name)
    {
        // check all active and passive skills.
        foreach (AttackSkill skill in m_activeSkills)
        {
            if (skill.IsImmuneTo(name))
            {
                return true;
            }
        }
        foreach (AttackSkill skill in m_innateSkills)
        {
            if (skill.IsImmuneTo(name))
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateOngoingSkillStatuses(BaseActor source)
    {
        foreach (OngoingSkillStatus status in m_ongoingStatuses)
        {
            status.UpdateForSource(source);
        }
    }

    public void CancelSkillStatus(string statusName)
    {
        // go through all active skills.
        // if they're not permanent then remove the affected status..
        foreach (OngoingSkillStatus oss in m_ongoingStatuses)
        {
            if (!oss.Permanent)
            {
            }

        }


    }

    public bool ImmuneToDamageType(DamageType damageType)
    {
        return false;
    }

    public void ConfirmAttackSkill()
    {
        if (CurrentAttackSkill != null)
        {
            // adjust our skill points.
            ArenaSkillPoints -= CurrentAttackSkill.UseCost;
            //ArenaSkillPoints += CurrentAttackSkill.UseGain;

            if (UnitActive && PlayerControlled && TurnManager.WaitingOnPlayerControl)
            {
                TurnManager.WaitingOnPlayerControl = false;
            }
            if (CurrentAttackSkill.IsMoveToAttack)
            {
                ConfirmMove();
            }

            GladiusGlobals.GameStateManager.CurrentStateData.CameraManager.ReparentTarget(gameObject);

            StartAttackSkill();
        }
    }

    public void StartAttackSkill()
    {
        //if (CurrentAttackSkill.HasModifiers())
        //{
        //    ApplyModifiers(CurrentAttackSkill);
        //}
        //else if (CurrentAttackSkill.AttackType == AttackType.EndTurn)
        //{
        //    TurnComplete = true;
        //}
    }

    //public void EndAttackSkill()
    //{
    //    CurrentAttackSkill = null;
    //}


    public void EndTurn()
    {
        StopAttack();
        FaceCardinal();
        UnitActive = false;
        if (!Dead)
        {
            QueueAnimation(AnimationEnum.Idle);
        }
        else
        {

        }
    }

    bool tc;

    public bool TurnComplete
    {
        get { return tc; }
        set
        {
            tc = value;
        }
    }

    public bool PlayerControlled
    {
        get;
        set;
    }

    public TurnManager TurnManager
    {
        get;
        set;
    }

    public void SetupSkills()
    {
        // simple for now.
        m_knownAttacks.Clear();
        foreach (string skillname in m_characterData.m_skillList)
        {
            if (AttackSkillDictionary.Data.ContainsKey(skillname))
            {
                m_knownAttacks.Add(AttackSkillDictionary.Data[skillname]);
            }
            else
            {
                Debug.LogWarning("Can't find key : " + skillname);
            }
        }

    }

    public List<AttackSkill> AttackSkills
    {
        get
        {
            return m_knownAttacks;
        }
    }

    public void ApplyModifiers(AttackSkill skill)
    {
        //need to change the implementation of this so its based on turns not elapsed time
        //foreach (GameObjectAttributeModifier modifier in skill.StatModifiers)
        //{
        //    GameObjectAttributeModifier modifierCopy = modifier.Copy();
        //    modifierCopy.ApplyTo(this);
        //    m_attributeModifiers.Add(modifierCopy);
        //}
        // different blocks may have different bonuses.
        TurnComplete = true;
    }

    public AttackSkill CurrentAttackSkill
    {
        get;
        set;
    }

    public void FilterAttackSkills()
    {
        m_availableAttacks.Clear();
        foreach (AttackSkill skill in m_knownAttacks)
        {
            if (skill.UseCost <= ArenaSkillPoints)
            {
                m_availableAttacks.Add(skill);
            }
        }
    }

    public bool HaveAvailableSkillForRange(int range)
    {
        foreach (AttackSkill skill in m_availableAttacks)
        {
            if (skill.InRange(range) && skill.Available(this))
            {
                return true;
            }
        }
        return false;
    }

    public void ChooseTargetAndSkill()
    {
        // check for pass only skill.
        if (m_knownAttacks.Count == 1 && m_knownAttacks[0].Name == "Pass")
        {
            CurrentAttackSkill = m_knownAttacks[0];
            return;
        }


        // if theres an actor nearby with low health , prioritise that target to finish it off.
        
        bool foundTargetInRange = false;

        foreach (BaseActor actor in TurnManager.AllActors)
        {
            // reset threats
            UpdateThreat(actor, 0, false);
            if (ArenaStateCommon.CombatEngine.IsValidTarget(this, actor, null))
            {
                int weighting = 0;
                int distance = GladiusGlobals.PointDist2(this.ArenaPoint, actor.ArenaPoint);
                // weight score based on how far away. (may need to confirm via pathfind as well).
                int distanceAdjust = 10 - distance;
                UpdateThreat(actor, distanceAdjust);

                // look and see if we have an advantage from class (light v heavy, heavy v medium etc)
                int categoryBonus = ArenaStateCommon.CombatEngine.GetClassAdvantage(this, actor);
                if (categoryBonus < 0)
                {
                    UpdateThreat(actor, -2);
                }
                else if (categoryBonus > 0)
                {
                    UpdateThreat(actor, 2);
                }


                // do we have a skill 
                if (HaveAvailableSkillForRange(distance))
                {
                    foundTargetInRange = true;


                    // if we can't get there. then knock it down a lot.
                    // FIXME - this should be updated to take into account ranged skills...
                    if (!GladiusGlobals.GameStateManager.ArenaStateCommon.Arena.DoesPathExist(ArenaPoint, actor.ArenaPoint))
                    {
                        UpdateThreat(actor, -10);
                    }
                    // and if it is a ranged skill then we need to do an arena los check.


                    // more tests on self/team buffing skills

                    // test on AoE skills.


                    // tests on affinity bonus to see if we can damage opponent with a particular skill?



                    // prioritise finishing off targets.
                    if (ArenaStateCommon.CombatEngine.IsNearDeath(actor))
                    {
                        UpdateThreat(actor, 5);
                    }
                    else
                    {
                        UpdateThreat(actor, 2);
                    }
                }
            }

        }

        // maybe adjust for height differnces as well? trickier as we need to check squares next to target (or not for ranged)
        // and look at moving to the square that gives the biggest advantage.


        // now choose one with highest score. (may want to randomise this slightly if a few are 'close'
        BaseActor result = null;
        int bestScore = 0;
        foreach (BaseActor actor in m_threatMap.Keys)
        {
            int score = m_threatMap[actor];
            if (score > bestScore)
            {
                bestScore = score;
                result = actor;
            }
        }




        ChooseAttackSkillForTarget(result);
        Target = result;
    }

    public void UpdateThreat(BaseActor actor, int value, bool modify = true)
    {
        int val = 0;
        if (modify)
        {
            m_threatMap.TryGetValue(actor, out val);
            val += value;
        }
        else
        {
            val = value;
        }
        m_threatMap[actor] = val;
    }


    public bool Dead
    {
        get;
        set;
    }

    // called after we've chosen a target.
    private AttackSkill ChooseAttackSkillForTarget(BaseActor target)
    {
        AttackSkill result = null;
        if (target != null)
        {
            int distance = GladiusGlobals.PointDist2(this.ArenaPoint, target.ArenaPoint);
            float bestDamage = 0;
            foreach (AttackSkill skill in AttackSkills.FindAll(skill => HaveAvailableSkillForRange(distance) && skill.Available(this)))
            {
                // decide which one will do the most.
                float expectedDamage = ArenaStateCommon.CombatEngine.CalculateExpectedDamage(this, target, skill);
                if (expectedDamage > bestDamage)
                {
                    bestDamage = expectedDamage;
                    result = skill;
                }
                //if(skill.CanDamageTarget(target))
                //{
                //    result = skill;
                //}


            }
        }
        return result;
    }

    //public Dictionary<GameObjectAttributeType, BoundedAttribute> AttributeDictionary
    //{
    //    get { return m_attributeDictionary; }
    //}

    public int CurrentMovePoints
    {
        get { return m_currentMovePoints; }

    }

    public float MovePointMultiplier
    {
        get
        {
            if (GladiusGlobals.GameStateManager.ArenaStateCommon.Crowd.GetValueForSchool(m_characterData.School) > 25)
            {
                return 1.3f;
            }
            return 1.0f;
        }

    }

    public GladiatorSchool School
    {
        get { return m_characterData.School; }
    }

    public float AffinityPointMultiplier
    {
        get
        {
            if (GladiusGlobals.GameStateManager.ArenaStateCommon.Crowd.GetValueForSchool(m_characterData.School) > 50)
            {
                return 1.5f;
            }
            return 1.0f;
        }
    }



    public int TotalMovePoints
    {
        get { return (int)(m_totalMovePoints * MovePointMultiplier); }
    }

    public string ModelName
    {
        get
        {
            return CharacterModelName != null ? CharacterModelName : GladiusGlobals.CharacterModelsRoot + ActorClassData.MeshName;
        }
    }


    public void FaceCardinal()
    {
        return;

        float minDot = float.MaxValue;
        Quaternion bestFace = Quaternion.identity;
        for (int i = 0; i < CardinalDirs.Length; ++i)
        {
            float dot = Quaternion.Dot(Rotation, CardinalDirs[i]);
            if (dot < minDot)
            {
                minDot = dot;
                bestFace = CardinalDirs[i];
            }
        }
        Rotation = bestFace;
    }

    public void BreakShield()
    {
        m_characterData.AddItemByNameAndLoc(null, ItemLocation.Shield);
        // update model
    }

    public void RestoreShield()
    {

    }

    public void BreakHelmet()
    {
        m_characterData.AddItemByNameAndLoc(null, ItemLocation.Helmet);
        // update model
    }

    public void QueueCounterAttack(BaseActor attacker)
    {

    }


    public void Knockdown(int numTurns)
    {
        m_knockedDown = true;
        m_knockDownTurns = numTurns;
    }

    public void GetUp()
    {
        m_knockedDown = false;
        // queue getup anim
        QueueAnimation(AnimationEnum.GetUp);
    }

    public void Knockback(BaseActor attacker)
    {
        Point direction = GladiusGlobals.Subtract(attacker.ArenaPoint, ArenaPoint);
        direction = GladiusGlobals.CardinalNormalize(direction);
        Point newSquare = GladiusGlobals.Add(ArenaPoint, direction);
        if (!Arena.IsPointOccupied(newSquare))
        {
            m_knockedBack = true;
            m_knockBackArenaPoint = newSquare;

        }
    }

    public void Retreat(BaseActor attacker)
    {

    }

    public void BackupWaypoints()
    {
        OldWayPointList.Clear();
        OldWayPointList.AddRange(WayPointList);
    }

    public void RestoreWaypoints()
    {
        WayPointList.Clear();
        WayPointList.AddRange(OldWayPointList);
    }


    public ActorClassData ActorClassData
    {
        get { return m_characterData.ActorClassData; }
    }

    private CharacterData m_characterData;

    private Projectile m_projectile;

    private Dictionary<BaseActor, int> m_threatMap = new Dictionary<BaseActor, int>();
    private BaseActor m_currentTarget = null;
    private List<Point> m_wayPointList = new List<Point>();
	private List<Point> m_oldWayPointList = new List<Point>();
	
    private List<AttackSkill> m_knownAttacks = new List<AttackSkill>();
    private List<AttackSkill> m_availableAttacks = new List<AttackSkill>();

    public List<AttackSkill> m_innateSkills = new List<AttackSkill>();
    public List<AttackSkill> m_activeSkills = new List<AttackSkill>();

    public List<OngoingSkillStatus> m_ongoingStatuses = new List<OngoingSkillStatus>();


    private int m_currentMovePoints;
    private int m_totalMovePoints = 10;

    public Transform m_leftHandTransforms;
    public Transform m_rightHandTransforms;

    public GameObject m_projectileGameObject;
    public GameObject m_characterGameObject;


    private Transform m_projectileHandTransform;
    private Point m_knockBackArenaPoint = new Point();
    private bool m_knockedBack;

    private bool m_knockedDown;
    private int m_knockDownTurns = 0;

    private AnimationEnum m_currentAnimEnum = AnimationEnum.None;
    private Dictionary<AnimationEnum, String> m_clipNameDictionary = new Dictionary<AnimationEnum, string>();
    public const int MinLevel = 1;
    public const int MaxLevel = 15;
    static Quaternion[] CardinalDirs = new Quaternion[] {
				Quaternion.AngleAxis (0, Vector3.up),
				Quaternion.AngleAxis (180, Vector3.up),
				Quaternion.AngleAxis (90, Vector3.up),
				Quaternion.AngleAxis (270, Vector3.up)
		};

}



public class OngoingSkillStatus
{
    private int m_duration = 0;
    private bool m_permanent;
    private SkillStatus m_status;
    private BaseActor m_source;
    private BaseActor m_target;
    public String m_name;

    public OngoingSkillStatus(SkillStatus status, BaseActor target)
    {
        if (m_status.statusDurationType == "Permanent")
        {
            Permanent = true;
        }
        else if (m_status.statusDurationType == "Turns Source")
        {

        }
        else if (m_status.statusDurationType == "Turns Target")
        {

        }
        else if (m_status.statusDurationType == "Until Negated")
        {

        }
        else if (m_status.statusDurationType == "Lapse on Failed Condition")
        {

        }
        else if (m_status.statusDurationType == "Lapse with order")
        {

        }
        else if (m_status.statusDurationType == "Stack Absolute")
        {

        }
        else if (m_status.statusDurationType == "Stack Source Mult")
        {

        }
        else if (m_status.statusDurationType == "Stack Target Mult")
        {

        }
    }
    
    

    public bool Permanent
    { get; set; }

    public void UpdateForSource(BaseActor actor)
    {
        if (m_status.statusDurationType == "Turns Source" && actor == m_source)
        {
            m_duration = -1;
        }
        else if (m_status.statusDurationType == "Turns Target" && actor == m_target)
        {
            m_duration = -1;

        }
    }

    public void Clear()
    {

    }

    public bool Expired()
    {
        return !m_permanent || m_duration > 0;
    }
}