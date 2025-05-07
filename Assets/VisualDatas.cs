using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Game;

[CreateAssetMenu(menuName = "VisualDatas")]
public class VisualDatas : ScriptableObject
{
    [Serializable]
    public class SpriteData
    {
        public EStone Stone;
        public Sprite Sprite;
	}

    public List<SpriteData> Sprites;

    public Sprite GetSpriteFor(EStone stone)
    {
        foreach(SpriteData spriteData in Sprites)
        {
            if (spriteData.Stone == stone)
                return spriteData.Sprite;
        }
        return null;
    }
}
