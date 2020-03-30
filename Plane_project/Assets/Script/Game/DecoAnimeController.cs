using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoAnimeController
{
    public List<AnimationControllEx> aniList = new List<AnimationControllEx>();
    public Transform parent;

    public AnimationControllEx AddDeco(Vector2 pos)
    {
        GameObject deco = new GameObject("deco");
        deco.transform.SetParent(parent);
        deco.transform.position = pos;

        var spr = deco.AddComponent<SpriteRenderer>();
        spr.sortingOrder = 1;
        AnimationControllEx anicon = new AnimationControllEx(spr);

        aniList.Add(anicon);

        return anicon;
    }

    public void DecoAniProgress(float deltaTime)
    {
        foreach(var ani in aniList)
        {
            ani.AnimationProgress(deltaTime);
        }
    }

    public void DestroyAll()
    {
        for(int i = 0; i < aniList.Count; ++i)
        {
            GameObject.Destroy(aniList[i]._sprRenderer.gameObject);
        }

        aniList.Clear();
    }
    
    public DecoAnimeController(Transform p)
    {
        parent = p;
    }
}
