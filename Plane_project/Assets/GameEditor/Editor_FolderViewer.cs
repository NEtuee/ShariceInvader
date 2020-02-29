using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor_FolderViewer : MonoBehaviour
{
    public string topFolderPath = "Assets/Resources/Sprites/";
    public GameObject folderItemBase;
    public RectTransform contentPlace;
    
    private Editor_FolderItemBase selectedItem;

    void Start()
    {
        UpdateFolderList();
    }

    void Update()
    {
        
    }

    public void SelectButton()
    {
        if(Editor_FolderItemBase.selected != null)
        {
            Editor_FolderItemBase.selected.SubfolderButton();
            
        }
    }

    public void UpdateFolderList()
    {
        string[] lists = Directory.GetDirectories(IOManager.PathForDocumentsFile(topFolderPath));

        if(lists.Length == 0)
        {
            Debug.Log("root folder is empty");
            return;
        }
        
        Vector2 pos = new Vector2(5f,-5f);
        Editor_FolderItemBase item = UpdateFolderList(contentPlace,null,pos,lists[0]);;
        

        for(int i = 1; i < lists.Length; ++i)
        {
            pos.y -= item.rectTp.sizeDelta.y;
            var sub = UpdateFolderList(contentPlace,null,pos,lists[i]);

            item.child = sub;
            item = sub;
        }
    }

    public Editor_FolderItemBase UpdateFolderList(RectTransform parent, Editor_FolderItemBase root, Vector2 pos, string path)
    {
        Editor_FolderItemBase item = Instantiate(folderItemBase).GetComponent<Editor_FolderItemBase>();
        item.filePath = path;
        string[] s = path.Split('\\');
        item.nameText.text = s[s.Length - 1];
        item.subRoot = root;
        item.rectTp.SetParent(parent);

        item.rectTp.anchoredPosition = pos;

        string[] lists = Directory.GetDirectories(IOManager.PathForDocumentsFile(path));
        Vector2 subPos = pos;
        pos.x = 10;

        for(int i = 0; i < lists.Length; ++i)
        {
            pos.y = -item.rectTp.sizeDelta.y * (i + 1);
            var sub = UpdateFolderList(item.subFolderRoot,item,pos,lists[i]);

            item.subFolders.Add(sub);

            if(i > 0)
            {
                item.subFolders[i - 1].child = sub;
            }
        }

        return item;
    }
}
