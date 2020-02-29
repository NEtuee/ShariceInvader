using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor_FolderItemBase : MonoBehaviour
{
    public delegate void selectEventBase(Editor_FolderItemBase item);

    public static Editor_FolderItemBase selected = null;
    public static selectEventBase selectEvent = new selectEventBase((Editor_FolderItemBase t)=>{});
    
    public Editor_FolderItemBase subRoot = null;
    public Editor_FolderItemBase child = null;
    public RectTransform rectTp;
    public RectTransform subFolderRoot;

    public List<Editor_FolderItemBase> subFolders = new List<Editor_FolderItemBase>();
    public Text downText;
    public Text nameText;

    public Button pathButton;
    public float subFolderHeight;

    public string filePath;

    protected bool _subfolder;

    public void ShowSubfolder()
    {
        downText.text = "▶";
        subFolderRoot.gameObject.SetActive(true);

        _subfolder = true;
    }

    public void HideSubfolder()
    {
        downText.text = "▼";
        subFolderRoot.gameObject.SetActive(false);

        _subfolder = false;
    }

    public void SubfolderButton()
    {
        if(subFolders.Count == 0)
            return;

        if(_subfolder)
            HideSubfolder();
        else
            ShowSubfolder();
        
        subFolderHeight = CalculateSubFolderHeight();
        
        if(child != null)
        {
            child.UpdatePosition(rectTp.anchoredPosition.y,subFolderHeight + rectTp.sizeDelta.y);
        }
        else
        {
            UpdateSubRootPosition(rectTp.anchoredPosition.y,rectTp.sizeDelta.y + subFolderHeight);
        }
    }
    
    public float CalculateSubFolderHeight()
    {
        float height = 0f;
        
        if(_subfolder)
        {
            foreach(var folder in subFolders)
            {
                height += folder.CalculateSubFolderHeight() + rectTp.sizeDelta.y;
            }
        }

        return height;
    }

    public void UpdatePosition(float parentPos, float folderHeight)
    {
        Vector3 pos = rectTp.anchoredPosition;
        pos.y = parentPos - folderHeight;

        rectTp.anchoredPosition = pos;

        if(child != null)
        {
            child.UpdatePosition(pos.y, subFolderHeight + rectTp.sizeDelta.y);
        }
        else
        {
            UpdateSubRootPosition(rectTp.anchoredPosition.y,subFolderHeight + rectTp.sizeDelta.y);
        }

    }

    public void UpdateSubRootPosition(float pos, float height)
    {
        if(subRoot != null && subRoot.child != null)
        {
            subRoot.child.UpdatePosition(subRoot.rectTp.anchoredPosition.y + pos,height);
        }
    }

    public void PathSelect()
    {
        if(selected != null)
            selected.pathButton.interactable = true;

        selected = this;
        pathButton.interactable = false;
        selectEvent(this);
    }
}
