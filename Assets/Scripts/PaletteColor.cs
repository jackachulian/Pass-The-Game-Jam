using UnityEngine;

public class PaletteColor : MonoBehaviour
{
    public TerrainManager.LandType landType;
 
    public bool hovered; // Set to true by paint drawer each frame when hovered over; set to false at thnd of this object's update

    private Renderer renderer;
    public Color matBaseColor { get; private set; }

    
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        matBaseColor = renderer.material.color;
    }


    private void Update()
    {
        if (hovered)
        {
            renderer.material.color = Color.Lerp(matBaseColor, Color.white, 0.5f);
        } else
        {
            renderer.material.color = matBaseColor;
        }

        hovered = false;
    }
}