using UnityEngine;
using UnityEngine.UI;

public class PaintCrosshair : MonoBehaviour
{
    [SerializeField] private PaintDrawer paintDrawer;
    [SerializeField] private Texture canvasPaintTexture;
    [SerializeField] private RawImage crosshair;

    // This code will hide the cursor when not in range of the canvas.
    // Code is kind of weird, the next dev can probably find a better UI solution
    // but it's to show that the player can't interact with teh canvas when they're out of range
    private void Update()
    {
        if (paintDrawer.withinRange)
        {
            crosshair.texture = canvasPaintTexture;
            crosshair.color = Color.white;
        } else
        {
            crosshair.texture = null;
            crosshair.color = Color.clear;
        }
    }
}