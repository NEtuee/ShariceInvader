using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceMapper
{
    public class Place
    {
        public LinkedList<ObjectBase> list;
        public Dictionary<int,LinkBase<ObjectBase>> links;
        public Vector2 leftBottom;

        public int placeCount;


        public void EnterPlace(ObjectBase obj)
        {
            obj.place = this;
            links.Add(obj.uniqueNumber, list.Add(obj));
        }

        public void ExitPlace(ObjectBase obj)
        {
            if(links.ContainsKey(obj.uniqueNumber))
            {
                list.DisconnectLink(links[obj.uniqueNumber]);
                links.Remove(obj.uniqueNumber);
            }
        }

        public void UpdatePosition(float leftPoint)
        {
            float l = (leftPoint - leftBottom.x);

            LinkBase<ObjectBase> link = list.front;
            while(link != null)
            {
                Vector3 pos = link.target.position;
                pos.x = pos.x + l;
                link.target.SetPosition(pos);
                link.target.beforeUpdateTransform();
                link = link.next;
            }

            leftBottom.x = leftPoint;
        }

        public Place(int count)
        {
            placeCount = count; 
            list = new LinkedList<ObjectBase>();
            links = new Dictionary<int, LinkBase<ObjectBase>>();
        }
    }

    public ObjectBase mainObject = null;
    public Place mainPlace = null;

    public Place [] _places;

    public Place _left;
    public Place _right;

    public float _mapWidth;
    public float _mapHeight;
    public float _placeWidth;
    public int _placeCount;
    public int _centerCount;

    private LineRenderer alertLine;
    private LineRenderer groundLine;
    private LineRenderer topLine;

    public void InitPlace(float mapWidth, float mapHeight, int placeCount)
    {
        _mapWidth = mapWidth;
        _mapHeight = mapHeight;
        _placeCount = placeCount;
        _placeWidth = _mapWidth / (float)placeCount;

        _places = new Place[_placeCount];

        for(int i = 0; i < _placeCount; ++i)
        {
            Place p = new Place(i);
            p.leftBottom = new Vector2(i * _placeWidth,0);
            _places[i] = p;
        }

        _left = _places[0];
        _right = _places[_placeCount - 1];

        _centerCount = _placeCount / 2;

        alertLine = GameObject.Find("AlertLine").GetComponent<LineRenderer>();
        groundLine = GameObject.Find("GroundLine").GetComponent<LineRenderer>();
        topLine = GameObject.Find("TopLine").GetComponent<LineRenderer>();
    }

    public void UpdatePlaceOrder()
    {
        if(mainObject == null)
            return;
        
        int mainCount = PlacePosCheck(mainObject);
        int mainPlacePos = PlaceCheck(mainObject);
        if(mainPlacePos == -1)
            return;
        
        mainPlace = _places[mainPlacePos];

        if(_centerCount == mainCount)
            return;
        
        if(_centerCount < mainCount)
        {
            int diff = mainCount - _centerCount;
            for(int i = 0; i < diff; ++i)
            {
                Place l = _left;
                l.UpdatePosition(_right.leftBottom.x + _placeWidth);
                _left = _places[l.placeCount + 1 >= _placeCount ? 0 : l.placeCount + 1];
                _right = l;
            }
        }
        else if(_centerCount > mainCount)
        {
            int diff = _centerCount - mainCount;
            for(int i = 0; i < diff; ++i)
            {
                Place r = _right;

                r.UpdatePosition(_left.leftBottom.x - _placeWidth);
                _right = _places[r.placeCount - 1 < 0 ? _placeCount - 1 : r.placeCount - 1];
                _left = r;
                
            }
        }

        Vector3 pos = _left.leftBottom;
        pos.y = pos.y + 3f;

        alertLine.SetPosition(0,pos);
        pos.x = _right.leftBottom.x;
        alertLine.SetPosition(1,pos);

        groundLine.SetPosition(0,_left.leftBottom);
        groundLine.SetPosition(1,_right.leftBottom);
        
        topLine.SetPosition(0,new Vector2(_left.leftBottom.x,_mapHeight));
        topLine.SetPosition(1,new Vector2(_right.leftBottom.x,_mapHeight));
    }

    public void SetMainObject(ObjectBase obj)
    {
        mainObject = obj;
        UpdatePlaceOrder();
    }

    public Place GetPlace(ObjectBase obj)
    {
        int i = PlaceCheck(obj);

        if(i == -1)
        {
            return null;
        }
        else
            return _places[i];
    }

    public Vector2 MapPosToWorldPos(Vector2 mapPos)
    {
        float d = mapPos.x - (float)((int)(mapPos.x / _placeWidth)) * _placeWidth;
        int p = (int)(mapPos.x / _placeWidth);

        Vector2 pos = new Vector2(_places[p].leftBottom.x + d,mapPos.y);

        return pos;
    }

    public Vector2 WorldPosToMapPos(Vector2 worldPos)
    {
        int place = PlaceCheck(worldPos.x);

        if(place != -1)
        {
            return new Vector2(place * _placeWidth + worldPos.x - _places[place].leftBottom.x,worldPos.y);
        }

        return Vector2.zero;
    }

    public Vector2 GetPosPercentage(Vector3 obj)
    {
        int place = PlaceCheck(obj);

        if(place != -1)
        {
            float dist = obj.x - _places[place].leftBottom.x;
            return new Vector2((dist + (float)place * _placeWidth) / _mapWidth,obj.y / _mapHeight);
        }

        return new Vector2();
    }

    public Vector2 GetPosPercentage(ObjectBase obj)
    {
        int place = PlaceCheck(obj);

        if(place != -1)
        {
            float dist = obj.position.x - _places[place].leftBottom.x;
            return new Vector2((dist + (float)place * _placeWidth) / _mapWidth,obj.position.y / _mapHeight);
        }

        return new Vector2();
    }

    public int PlacePosCheck(ObjectBase obj)
    {
        float x = obj.position.x;

        if(x >= _left.leftBottom.x && x <= _right.leftBottom.x + _placeWidth)
        {
            
            float dist = x - _left.leftBottom.x;
            int p = (int)(MathEx.abs(dist) / _placeWidth);

            return p;
        }
        else
            return -1;
    }

    public int PlaceCheck(Vector3 obj)
    {
        float x = obj.x;

        if(x >= _left.leftBottom.x && x <= _right.leftBottom.x + _placeWidth)
        {
            
            float dist = x - _left.leftBottom.x;
            int p = (int)(MathEx.abs(dist) / _placeWidth);

            p = _left.placeCount + p;
            p = p >= _placeCount ? p - _placeCount : p;

            return p;
        }
        else
            return -1;
    }

    public int PlaceCheck(ObjectBase obj)
    {
        float x = obj.position.x;

        if(x >= _left.leftBottom.x && x <= _right.leftBottom.x + _placeWidth)
        {
            
            float dist = x - _left.leftBottom.x;
            int p = (int)(MathEx.abs(dist) / _placeWidth);

            p = _left.placeCount + p;
            p = p >= _placeCount ? p - _placeCount : p;

            return p;
        }
        else
            return -1;
    }

    public int PlaceCheck(float x)
    {
        if(x >= _left.leftBottom.x && x <= _right.leftBottom.x + _placeWidth)
        {
            
            float dist = x - _left.leftBottom.x;
            int p = (int)(MathEx.abs(dist) / _placeWidth);

            p = _left.placeCount + p;
            p = p >= _placeCount ? p - _placeCount : p;

            return p;
        }
        else
            return -1;
    }

    public bool IsInPlaceMap(Vector3 pos)
    {
        return pos.x >= _left.leftBottom.x && pos.x <= _right.leftBottom.x + _placeWidth;
    }
    
}
