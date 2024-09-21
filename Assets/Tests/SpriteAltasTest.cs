using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAltasTest : MonoBehaviour
{
    void Start()
    {
        sprRenderer.sprite = altas.GetSprite(spriteName);
    }

    public SpriteRenderer sprRenderer;
    public SpriteAtlas altas;
    public string spriteName;
}
