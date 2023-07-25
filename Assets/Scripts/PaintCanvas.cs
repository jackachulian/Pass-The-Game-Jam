using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintCanvas : MonoBehaviour
{
    [SerializeField] private Vector2Int _textureSize;
    public Vector2Int textureSize { get { return _textureSize; } }

    public Texture2D texture { get; private set; }
    private Renderer r;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Renderer>();

        texture = new Texture2D(_textureSize.x, _textureSize.y);

        r.material.mainTexture = texture;

        FindFirstObjectByType<Terrain>().materialTemplate = r.material;

        //TestingDraw();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
