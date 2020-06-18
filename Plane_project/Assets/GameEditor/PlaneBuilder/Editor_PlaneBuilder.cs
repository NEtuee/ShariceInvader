using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor_PlaneBuilder : MonoBehaviour
{
    public Dropdown         animationType;
    public Dropdown         objectType;
    
    public InputField       planeName;
    public InputField       spriteSet;

    public InputField       boostAni;
    public InputField       trailMaterial;

    public InputField       mass;
    public InputField       frictionFactor;
    public InputField       gravityScale;
    public InputField       maxSpeed;
    public InputField       speed;
    public InputField       dodgeDist;

    public InputField       trailTime;
    public InputField       trailStartWidth;
    public InputField       trailEndWidth;
    public InputField       trailSortingOrder;

    public Toggle           rotateLock;
    public Toggle           velocityFlip;
    public Toggle           directionAngle;
    public Toggle           trailEmmit;
    public Toggle           boostAniProgress;

    public InputField       hp;
    public InputField       bodyAttack;

    public InputField       boostCount;
    public InputField       trailCount;


    public Button saveButton;


    public Editor_AnimationKeyViewer animationKeyViewer;
    public Editor_CursorController cursorController;
    public Editor_PlaneInfoBase planeInfo;
    public Editor_UIBase uIBase;

    public Sprite trailCursorSprite;
    public Sprite boostCursorSprite;

    public Editor_Addon_DirectionWheel directionWheel;

    private List<Editor_CursorBase> _trailCursor = new List<Editor_CursorBase>();
    private List<Editor_CursorBase> _boostCursor = new List<Editor_CursorBase>();

    private List<AnimationControllEx> _boostAnimation = new List<AnimationControllEx>();
    private List<Editor_TrailRenderer> _trailRenderers = new List<Editor_TrailRenderer>();

    private GameObject _boostObjects;
    private GameObject _trailObjects;

    private int _trailCursorCount = 0;
    private int _boostCursorCount = 0;

    private bool _boostAniProgress = true;

    public void Start()
    {
        planeInfo = new Editor_PlaneInfoBase();

        cursorController.deleteCursorEvent += DeleteCursor;
        Editor_AnimationKeyViewer.keySelected += KeySelectEvent;
        Editor_CursorBase.cursorValueChangedEvent += CursorPositionUpdateEvent;
        Editor_CursorBase.cursorSelectedEvent += UpdateTrailSortingOrderData;
        Editor_CursorBase.cursorDeselectedEvent += UpdateTrailSortingOrderData;

        _boostObjects = new GameObject("BoostObjects");
        _trailObjects = new GameObject("TrailObjects");

        uIBase.ContentLock(true);

        AnimationDropdownListUpdate();
        ObjectTypeDropdownListUpdate();
    }

    public void Update()
    {
        if(_boostAniProgress)
        {
            foreach(var ani in _boostAnimation)
            {
                ani.AnimationProgress(Time.deltaTime);
                ani._sprRenderer.transform.eulerAngles = new Vector3(0f,0f,MathEx.directionToAngle(directionWheel.direction) - 180f);

                if(ani.isEnd)
                {
                    ani.ChangeAni("Loop",true);
                }
            }
        }

        foreach(var trail in _trailRenderers)
        {
            trail.direction = directionWheel.direction;
        }
    }

    public void ApplyAllCursorButton()
    {
        Editor_EventSystem.instance.ActiveMessageBox("ApplyAllCursor","Do you really want to apply it to every frame?",ApplyAllCursor);
    }

    public void ApplyAllCursor()
    {
        foreach(var cursor in Editor_CursorBase.selectedCursorList)
        {
            if(_trailCursor.Contains(cursor))
            {
                foreach(var item in planeInfo.trailPoint)
                {
                    item.Value[cursor.uniqueNumber] = planeInfo.trailPoint[Editor_AnimationKeyBase.selected.frame][cursor.uniqueNumber];
                }
            }
            else if(_boostCursor.Contains(cursor))
            {
                foreach(var item in planeInfo.boostPoint)
                {
                    item.Value[cursor.uniqueNumber] = planeInfo.boostPoint[Editor_AnimationKeyBase.selected.frame][cursor.uniqueNumber];
                }
            }
        }

        Editor_EventSystem.instance.ActiveNotice("Work Complete");

    }

    public void AddBoostPointButton()
    {
        AddBoostCursor(Vector2.zero);
    }

    public void AddTrailPointButton()
    {
        AddTrailCursor(Vector2.zero);
    }

    public void PlayBurstAnimationButton()
    {
        foreach(var ani in _boostAnimation)
        {
            ani.ChangeAni("Burst",false);
        }
    }

    public void DeleteAllTrailObjects()
    {
        foreach(var trail in _trailRenderers)
        {
            Destroy(trail.gameObject);
        }

        _trailRenderers.Clear();
    }

    public void DeleteAllBoostObjects()
    {
        int count = _boostAnimation.Count;
        while(count > 0)
            DeleteBoostAni(--count);

        _boostAnimation.Clear();
    }

    public void DeleteTrailRenderer(int target)
    {
        var t = _trailRenderers[target];
        _trailRenderers[target] = null;
        _trailRenderers.RemoveAt(target);

        Destroy(t.gameObject);
    }

    public void UpdateTrailData()
    {
        foreach(var trail in _trailRenderers)
        {
            trail.TrailDataUpdate();
        }
    }

    public void UpdateTrailSotringOrder()
    {
        int frame = Editor_AnimationKeyBase.selected.frame;

        for(int i = 0; i < _trailRenderers.Count; ++i)
        {
            _trailRenderers[i].trail.sortingOrder = planeInfo.trailSortingOredrs[frame][i];
        }
    }

    public void MoveTrailRendererPosition(Vector2 pos, int target)
    {
        _trailRenderers[target].targetPos = pos;
    }

    public void AddTrailRenderer(Vector2 pos)
    {
        Editor_TrailRenderer trail = new GameObject("trail").AddComponent<Editor_TrailRenderer>();
        _trailRenderers.Add(trail);

        trail.gameObject.transform.SetParent(_trailObjects.transform);
        trail.Init(planeInfo);
    }

    public void DeleteBoostAni(int target)
    {
        var t = _boostAnimation[target];
        _boostAnimation[target] = null;
        _boostAnimation.RemoveAt(target);

        Destroy(t._sprRenderer.gameObject);
    }

    public void MoveBoostPosition(Vector2 pos, int target)
    {
        _boostAnimation[target]._sprRenderer.transform.position = pos;
    }


    public void AddBoostAnimation(Vector2 pos, bool ani = false)
    {
        SpriteRenderer sprRenderer = new GameObject("boost").AddComponent<SpriteRenderer>();
        AnimationControllEx boost = new AnimationControllEx(sprRenderer);
        _boostAnimation.Add(boost);

        sprRenderer.transform.SetParent(_boostObjects.transform);

        if(ani)
        {
            string path = planeInfo.boostAni;
            Sprite[] spr = ResourceManager.GetInstance().GetSpriteSet(path);

            if(spr == null)
            {
                Editor_EventSystem.instance.ActiveNotice("Boost Animation Does Not Exist");
            }
            else
            {
                boost.ClearAnimationList();
                boost.AddAnimation("Loop",path + "/Loop");
                boost.AddAnimation("Burst",path + "/Burst");

                boost.ChangeAni("Loop",true);
            }
        }
    }

    public bool UpdateBoostAnimation()
    {
        string path = planeInfo.boostAni;
        Sprite[] spr = ResourceManager.GetInstance().GetSpriteSet(path + "/Loop");

        if(spr == null)
        {
            Editor_EventSystem.instance.ActiveNotice("Boost Animation Does Not Exist");
            return false;
        }
        else
        {
            foreach(var boost in _boostAnimation)
            {
                boost.ClearAnimationList();
                boost.AddAnimation("Loop",path + "/Loop");
                boost.AddAnimation("Burst",path + "/Burst");

                boost.ChangeAni("Loop",true);
            }

            return true;
        }
    }

    public void Save()
    {
        planeInfo.SaveData();

        Editor_EventSystem.instance.ActiveNotice("Save Complete");
    }

    public void PathSelectEvent()
    {
        Editor_FolderItemBase item = Editor_FolderItemBase.selected;
        string path = item.filePath;

        string [] p = Directory.GetDirectories(path);

        if(p.Length != 0)
        {
            Debug.Log("null");
            return;
        }

        Sprite [] spr = ResourceManager.GetInstance().GetSpriteAll(path);

        if(spr != null)
        {
            _trailCursorCount = 0;
            _boostCursorCount = 0;

            string file = path.Substring(path.LastIndexOf("Planes\\")).Replace("Planes\\",string.Empty);
            string file2 = path.Substring(path.LastIndexOf("\\"));
            string name = path + file2 + "_Plane"  + ".txt";
            Debug.Log(file);
            Debug.Log(name);

            planeInfo.SetPath(name);

            if(File.Exists(name))
            {
                planeInfo.LoadDataFile();
            }
            else
            {
                planeInfo.CreateDataFile("SpriteSet\\Planes\\" + file);
            }

            cursorController.DisableAllCursor();
            DeleteAllTrailObjects();
            DeleteAllBoostObjects();

            _trailCursor.Clear();
            _boostCursor.Clear();

            for(int i = 0; i < planeInfo.trailCount; ++i)
            {
                AddTrailCursor((planeInfo.trailPoint[0])[i],true);
            }

            for(int i = 0; i < planeInfo.boostCount; ++i)
            {
                AddBoostCursor((planeInfo.boostPoint[0])[i],true);
            }

            animationKeyViewer.KeySelectEvent(0);

            DataUpdateAll();
            saveButton.interactable = true;
            uIBase.ContentLock(false);
        }
    }

    public void KeySelectEvent(Editor_AnimationKeyBase key)
    {
        int frame = key.frame;

        foreach(var cursor in _trailCursor)
        {
            cursor.SetPosition(planeInfo.trailPoint[frame][cursor.uniqueNumber]);
        }

        foreach(var cursor in _boostCursor)
        {
            cursor.SetPosition(planeInfo.boostPoint[frame][cursor.uniqueNumber]);
        }

        Editor_CursorBase.UpdateCenterPosition();
        UpdateTrailSortingOrderData(null);
        UpdateTrailSotringOrder();
    }

    public void AddTrailCursor(Vector2 pos, bool load = false)
    {
        int count = _trailCursor.Count;

        var cursor = cursorController.AddCursor("TrailCursor_" + _trailCursorCount.ToString(),pos,trailCursorSprite);
        cursor.uniqueNumber = count;
        _trailCursorCount++;

        _trailCursor.Add(cursor);

        if(!load)
        {
            foreach(var item in planeInfo.trailPoint)
            {
                item.Value.Add(pos);
            }
            foreach(var item in planeInfo.trailSortingOredrs)
            {
                item.Value.Add(-1);
            }

            UpdateTrailCount();
        }

        AddTrailRenderer(pos);

    }

    public void AddBoostCursor(Vector2 pos, bool load = false)
    {
        int count = _boostCursor.Count;

        var cursor = cursorController.AddCursor("BoostCursor_" + _boostCursorCount.ToString(),pos,boostCursorSprite);
        cursor.uniqueNumber = count;
        _boostCursorCount++;

        _boostCursor.Add(cursor);

        if(!load)
        {
            foreach(var item in planeInfo.boostPoint)
            {
                item.Value.Add(pos);
            }
            UpdateBoostCount();
        }

        AddBoostAnimation(pos,true);
    }

    public void CursorPositionUpdateEvent(Editor_CursorBase cursor)
    {
        if(_trailCursor.Contains(cursor))
        {
            planeInfo.trailPoint[Editor_AnimationKeyBase.selected.frame][cursor.uniqueNumber] = cursor.GetPosition();

            MoveTrailRendererPosition(cursor.GetPosition(),cursor.uniqueNumber);
        }
        else if (_boostCursor.Contains(cursor))
        {
            planeInfo.boostPoint[Editor_AnimationKeyBase.selected.frame][cursor.uniqueNumber] = cursor.GetPosition();

            MoveBoostPosition(cursor.GetPosition(),cursor.uniqueNumber);
        }
    }

    public void UpdateTrailCount()
    {
        trailCount.text = _trailCursor.Count.ToString();
        DataSet_trailCount();
    }

    public void UpdateBoostCount()
    {
        boostCount.text = _boostCursor.Count.ToString();
        DataSet_boostCount();
    }

    public void DeleteCursor(Editor_CursorBase cursor)
    {
        if(_trailCursor.Contains(cursor))
        {
            DeleteCursor(ref _trailCursor,cursor.uniqueNumber);
            DeleteTrailRenderer(cursor.uniqueNumber);

            Debug.Log(cursor.uniqueNumber);

            foreach(var item in planeInfo.trailPoint)
            {
                item.Value.RemoveAt(cursor.uniqueNumber);
            }

            foreach(var item in planeInfo.trailSortingOredrs)
            {
                item.Value.RemoveAt(cursor.uniqueNumber);
            }
            
            UpdateTrailCount();

            //uIBase.ContentLock(true);
        }
        else if (_boostCursor.Contains(cursor))
        {
            DeleteCursor(ref _boostCursor,cursor.uniqueNumber);
            DeleteBoostAni(cursor.uniqueNumber);

            foreach(var item in planeInfo.boostPoint)
            {
                item.Value.RemoveAt(cursor.uniqueNumber);
            }

            UpdateBoostCount();

            //uIBase.ContentLock(true);
        }
    }

    public void DeleteCursor(ref List<Editor_CursorBase> list, int number)
    {
        for(int i = number + 1; i < list.Count; ++i)
        {
            list[i].uniqueNumber--;
        }

        list.RemoveAt(number);
    }

    public void AnimationDropdownListUpdate()
    {
        animationType.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        
        for(int i = 0; i < (int)PlaneBase.AnimationType.End; ++i)
        {
            Dropdown.OptionData option = new Dropdown.OptionData(((PlaneBase.AnimationType)i).ToString());
            options.Add(option);
        }

        animationType.AddOptions(options);
    }

    public void ObjectTypeDropdownListUpdate()
    {
        objectType.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        
        for(int i = 0; i < (int)Define.ObjectType.AutoProgressEnd; ++i)
        {
            Dropdown.OptionData option = new Dropdown.OptionData(((Define.ObjectType)i).ToString());
            options.Add(option);
        }

        objectType.AddOptions(options);
    }

    public void DataUpdateAll()
    {
        animationType.value = (int)planeInfo.animationType;
        objectType.value = (int)planeInfo.objectType;
        planeName.text = planeInfo.planeName;
        spriteSet.text = planeInfo.spriteSet;
        boostAni.text = planeInfo.boostAni;
        trailMaterial.text = planeInfo.trailInfo.trailMaterial;
        mass.text = planeInfo.mass.ToString();
        frictionFactor.text = planeInfo.frictionFactor.ToString();
        gravityScale.text = planeInfo.gravityScale.ToString();
        maxSpeed.text = planeInfo.maxSpeed.ToString();
        speed.text = planeInfo.speed.ToString();
        dodgeDist.text = planeInfo.dodgeDist.ToString();
        rotateLock.isOn = planeInfo.rotateLock;
        velocityFlip.isOn = planeInfo.velocityFlip;
        directionAngle.isOn = planeInfo.directionAngle;
        trailEmmit.isOn = planeInfo.trailEmmit;
        boostAniProgress.isOn = planeInfo.boostAniProgress;
        hp.text = planeInfo.hp.ToString();;
        bodyAttack.text = planeInfo.bodyAttack.ToString();;
        boostCount.text = planeInfo.boostCount.ToString();;
        trailCount.text = planeInfo.trailCount.ToString();;

        trailTime.text = planeInfo.trailInfo.time.ToString();
        trailStartWidth.text = planeInfo.trailInfo.startWidth.ToString();
        trailEndWidth.text = planeInfo.trailInfo.endWidth.ToString();

        UpdateTrailSortingOrderData(null);
    }

    public void UpdateTrailSortingOrderData(Editor_CursorBase c)
    {
        if(c != null)
        {
            if(c.selected && _boostCursor.Contains(c))
            {
                trailSortingOrder.interactable = false;
                return;
            }
        }

        int count = Editor_CursorBase.selectedCursorList.Count;
        if(count == 1)
        {
            int key = Editor_AnimationKeyBase.selected.frame;
            int cursor = Editor_CursorBase.selectedCursorList[0].uniqueNumber;

            trailSortingOrder.text = planeInfo.trailSortingOredrs[key][cursor].ToString();
        }
        else
        {
            trailSortingOrder.text = "";
        }

        if(count == 0)
        {
            trailSortingOrder.interactable = false;
        }
        else
        {
            trailSortingOrder.interactable = true;
        }
    }

    public void DataSet_animationType(){planeInfo.animationType = (PlaneBase.AnimationType)animationType.value;}
    public void DataSet_objectType(){planeInfo.objectType = (Define.ObjectType)objectType.value;}
    public void DataSet_planeName(){planeInfo.planeName = planeName.text;}
    public void DataSet_spriteSet(){planeInfo.spriteSet = spriteSet.text;}
    public void DataSet_boostAni()
    {
        string s = planeInfo.boostAni;
        planeInfo.boostAni = boostAni.text;

        if(!UpdateBoostAnimation())
        {
            planeInfo.boostAni = s;
            boostAni.text = s;
        }

    }
    public void DataSet_trailMaterial()
    {
        if(ResourceManager.GetInstance().GetMaterial(trailMaterial.text) == null)
        {
            trailMaterial.text = planeInfo.trailInfo.trailMaterial;
            Editor_EventSystem.instance.ActiveNotice("Trail Material does not exist");
        }
        else
        {
            planeInfo.trailInfo.trailMaterial = trailMaterial.text;
            UpdateTrailData();
        }
    }
    public void DataSet_mass(){planeInfo.mass = float.Parse(mass.text);}
    public void DataSet_frictionFactor(){planeInfo.frictionFactor = float.Parse(frictionFactor.text);}
    public void DataSet_gravityScale(){planeInfo.gravityScale = float.Parse(gravityScale.text);}
    public void DataSet_maxSpeed(){planeInfo.maxSpeed = float.Parse(maxSpeed.text); UpdateTrailData();}
    public void DataSet_speed(){planeInfo.speed = float.Parse(speed.text); UpdateTrailData();}
    public void DataSet_dodgeDist(){planeInfo.dodgeDist = float.Parse(dodgeDist.text);}
    public void DataSet_rotateLock(){planeInfo.rotateLock = rotateLock.isOn;}
    public void DataSet_velocityFlip(){planeInfo.velocityFlip = velocityFlip.isOn;}
    public void DataSet_directionAngle(){planeInfo.directionAngle = directionAngle.isOn;}
    public void DataSet_trailEmmit(){planeInfo.trailEmmit = trailEmmit.isOn; _trailObjects.SetActive(trailEmmit.isOn);}
    public void DataSet_boostAniProgress()
    {
        planeInfo.boostAniProgress = _boostAniProgress = boostAniProgress.isOn;

        _boostObjects.SetActive(_boostAniProgress);
    }
    public void DataSet_hp(){planeInfo.hp = int.Parse(hp.text);}
    public void DataSet_bodyAttack(){planeInfo.bodyAttack = int.Parse(bodyAttack.text);}
    public void DataSet_boostCount(){planeInfo.boostCount = int.Parse(boostCount.text);}
    public void DataSet_trailCount(){planeInfo.trailCount = int.Parse(trailCount.text);}

    public void DataSet_TrailTime(){planeInfo.trailInfo.time = float.Parse(trailTime.text); UpdateTrailData();}
    public void DataSet_TrailStartWidth(){planeInfo.trailInfo.startWidth = float.Parse(trailStartWidth.text); UpdateTrailData();}
    public void DataSet_TrailEndWidth(){planeInfo.trailInfo.endWidth = float.Parse(trailEndWidth.text); UpdateTrailData();}
    public void DataSet_TrailSortingOredr()
    {
        int key = Editor_AnimationKeyBase.selected.frame;

        foreach(var item in Editor_CursorBase.selectedCursorList)
        {
            planeInfo.trailSortingOredrs[key][item.uniqueNumber] = int.Parse(trailSortingOrder.text);
        }

        UpdateTrailSotringOrder();
    }
}
