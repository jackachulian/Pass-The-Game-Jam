using System.Linq;
using UnityEngine;

public class PaintDrawer : MonoBehaviour
{
    [SerializeField] private int _penSize = 15;
    [SerializeField] private PaintCanvas _canvas;
    [SerializeField] private LayerMask _canvasLayerMask;
    [SerializeField] private LayerMask _paletteColorLayerMask;
    [SerializeField] private float _maxPaintDistance = 10f;
    [SerializeField] private int _interpolateSteps = 10;
    [SerializeField] private TerrainManager _terrainManager;

    private TerrainManager.LandType _currentLandTypeDraw = TerrainManager.LandType.Empty;

    private Color[] _colors;

    private Vector2 _lastTouchPos;
    private Vector2 _lastTouchTextureCoord;

    private bool _touchedLastFrame;

    private bool _withinRange; // if player is within painting distance
    public bool withinRange { get { return _withinRange; } }

    private bool _hoveringOverCanvas; // if within range and cursor is pointed towards the canvas
    public bool hoveringOverCanvas { get { return _hoveringOverCanvas; } }

    private void Start()
    {
        
    }

    private void Update()
    {
        CheckSelectColor();
        Draw();
    }

    private void CheckSelectColor()
    {
        // Check for hovered palette color
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit paletteColorTouch, 10f, _paletteColorLayerMask))
        {
            Debug.Log("TOUCH " + paletteColorTouch.collider.gameObject.name);
            PaletteColor paletteColorObj = paletteColorTouch.collider.gameObject.GetComponent<PaletteColor>();
            if (!paletteColorObj) return;

            // set hover to true - this will make the color lit up this frame, after which palettecolor will set this back to false
            paletteColorObj.hovered = true;

            // only select if clicked this frame
            if (!Input.GetMouseButtonDown(0)) return;

            Color drawColor = paletteColorObj.matBaseColor;
            SetLandType(paletteColorObj.landType, drawColor);

            Debug.Log("set color to " + paletteColorObj);
        }
    }

    private void SetLandType(TerrainManager.LandType landType, Color canvasDrawColor)
    {
        _currentLandTypeDraw = landType;
        
        _colors = Enumerable.Repeat(canvasDrawColor, _penSize * _penSize).ToArray();
    }

    private void Draw()
    {
        float distanceFromCanvas = (_canvas.transform.position - transform.position).magnitude;
        _withinRange = distanceFromCanvas < _maxPaintDistance;
        if (!_withinRange) return;

        if (_currentLandTypeDraw == TerrainManager.LandType.Empty) return;

        bool drawing = Input.GetMouseButton(0);
        _hoveringOverCanvas = Physics.Raycast(transform.position, transform.forward, out RaycastHit _paintTouch, _maxPaintDistance + 5f, _canvasLayerMask);

        if (drawing && _hoveringOverCanvas)
        {
            var touchPosTexCoord = _paintTouch.textureCoord;

            // Get canvas-space int touch coordinates
            var x = (int)(touchPosTexCoord.x * _canvas.textureSize.x - (_penSize / 2));
            var y = (int)(touchPosTexCoord.y * _canvas.textureSize.y - (_penSize / 2));

            // stop if brush is outside of texture
            if (y < 0 || y > _canvas.textureSize.y - _penSize || x < 0 || x > _canvas.textureSize.x - _penSize) return;

            if (_touchedLastFrame)
            {
                _canvas.texture.SetPixels(x, y, _penSize, _penSize, _colors);
                _terrainManager.SetLandTypeRegion(touchPosTexCoord.x, touchPosTexCoord.y, _penSize * 1f / _canvas.textureSize.x, _currentLandTypeDraw);

                // Interpolate between the two frames and draw, so that there aren't holes in the line.
                for (float t = 1f/_interpolateSteps; t < 1f; t +=1f/_interpolateSteps)
                {
                    int lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, t);
                    int lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, t);
                    _canvas.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);

                    float lerpTexX = Mathf.Lerp(_lastTouchTextureCoord.x, touchPosTexCoord.x, t);
                    float lerpTexY = Mathf.Lerp(_lastTouchTextureCoord.y, touchPosTexCoord.y, t);
                    _terrainManager.SetLandTypeRegion(lerpTexX, lerpTexY, _penSize * 1f / _canvas.textureSize.x, _currentLandTypeDraw);
                }

                _canvas.texture.Apply();
            }

            _lastTouchPos = new Vector2(x, y);
            _lastTouchTextureCoord = touchPosTexCoord;
            _touchedLastFrame = true;
        } else
        {
            _touchedLastFrame = false;
        }
    }
}