using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Drawable : ObjectBase {

	public SpriteRenderer sprRenderer{get{return _sprRenderer;}}
	protected SpriteRenderer _sprRenderer;
	protected SortingGroup _sortingGroup;

	public override void firstSetting(){AddSpriteRenderer();}
	public abstract override void initialize();
	public abstract override void progress(float deltaTime);
	public override void release(){}

	public void SetSprite(string name)
	{
		_sprRenderer.sprite = ResourceManager.GetInstance().GetSprite(name);
	}

	public void SetSprite(Sprite spr)
	{
		_sprRenderer.sprite = spr;
	}

	public Drawable SetSortingOrder(int index)
	{
		_sprRenderer.sortingOrder = index;
		//_sortingGroup.sortingOrder = index;
		return this;
	}

	public Drawable SetSortingGroupOrder(int index)
	{
		_sortingGroup.sortingOrder = index;
		return this;
	}

	protected void AddSpriteRenderer()
	{
		_sprRenderer = obj.AddComponent<SpriteRenderer>();
		_sprRenderer.material = ResourceManager.GetInstance().GetPixelSnapMaterial();
	}

	protected void AddSortingGroup()
	{
		_sortingGroup = obj.AddComponent<SortingGroup>();
	}
}
