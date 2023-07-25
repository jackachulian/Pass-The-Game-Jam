using System.Linq;
using UnityEngine;

public class PaintDrawer : MonoBehaviour
{
    [SerializeField] private int _penSize = 15;
    [SerializeField] private Color _drawColor;
    [SerializeField] private PaintCanvas _canvas;
    [SerializeField] private LayerMask canvasLayerMask;
    [SerializeField] private float maxPaintDistance = 10f;
    [SerializeField] private int interpolateSteps = 2;
    [SerializeField] private TerrainManager terrainManager;

    private Color[] _colors;

    private Vector2 _lastTouchPos;

    private bool _touchedLastFrame;

    private void Start()
    {
        _colors = Enumerable.Repeat(_drawColor, _penSize * _penSize).ToArray();
    }

    private void Update()
    {
        Draw();
    }

    private void Draw()
    {
        bool drawing = Input.GetMouseButton(0);

        if (drawing && Physics.Raycast(transform.position, transform.forward, out RaycastHit _paintTouch, maxPaintDistance, canvasLayerMask))
        {
            var touchPos = _paintTouch.textureCoord;

            // Get canvas-space int touch coordinates
            var x = (int)(touchPos.x * _canvas.textureSize.x - (_penSize / 2));
            var y = (int)(touchPos.y * _canvas.textureSize.y - (_penSize / 2));

            // stop if brush is outside of texture
            if (y < 0 || y > _canvas.textureSize.y - _penSize || x < 0 || x > _canvas.textureSize.x - _penSize) return;

            if (_touchedLastFrame)
            {
                _canvas.texture.SetPixels(x, y, _penSize, _penSize, _colors);
                terrainManager.SetLandTypeRegion(touchPos.x, touchPos.y, _penSize * 1f / _canvas.textureSize.x, TerrainManager.LandType.Water);

                // Interpolate between the two frames and draw, so that there aren't holes in the line.
                for (float t = 1f/interpolateSteps; t < 1f; t +=1f/interpolateSteps)
                {
                    int lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, t);
                    int lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, t);
                    _canvas.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);
                }

                _canvas.texture.Apply();
            }

            _lastTouchPos = new Vector2(x, y);
            _touchedLastFrame = true;
        } else
        {
            _touchedLastFrame = false;
        }
    }
}