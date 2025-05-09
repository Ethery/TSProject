using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static Game;

[CreateAssetMenu(menuName = "VisualDataSet")]
public class VisualDataSet : ScriptableObject
{
    public StoneDataSet<Sprite> Sprites;
    public StoneDataSet<Texture> Textures;

    private void OnValidate()
    {
        Sprites.OnValidate();
        Textures.OnValidate();
    }
}

[Serializable]
public class StoneDataSet<T> where T : class 
{
    [Serializable]
    public class Data
    {
        public EStone Stone;
        public T Value;
    }

    [SerializeField]
    private List<Data> DataList;

    public T this[EStone aStone] => GetDataFor(aStone);

    public bool Contains(EStone aStone)
    {
        foreach(Data data in DataList)
        {
            if (data.Stone == aStone)
                return true;
        }

        return false;
    }
    
    private T GetDataFor(EStone aStone)
    {
        foreach(Data data in DataList)
        {
            if (data.Stone == aStone)
                return data.Value;
        }
        return null;
    }

    public void OnValidate()
    {
        foreach(EStone aStone in Enum.GetValues(typeof(EStone)))
        {
            if (!Contains(aStone) && aStone != EStone.Count)
            {
                DataList.Add(new Data() { Stone = aStone, Value = null });
            }
        }
    }
}