using System;
using System.Threading.Tasks;
using k514;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BDG
{
    public class MapImageMakerSetting : EditorWindow
    {
        private Vector3 _scanStartVector;
        private int _scanX, _scanY;

        public static CurrentWork Work;
        public static bool _isWalked;
        public static int _curX, _curY;

        private MapImageMaker _mapImageMaker = new MapImageMaker();

        // variable that need for sub thread access to main thread
        public static bool RayCastRequest;
        public static string SceneName;

        public enum CurrentWork
        {
            Ready,
            Scan,
            MakeHeader,
            PrintPixel,
            WriteFile
        }

        private void Update()
        {
            if (!RayCastRequest) return;
            
            for (var j = 0; j < _mapImageMaker.LineScan.Length; j++, _mapImageMaker.ScanPoint.x += MapImageMaker.ScanDensity)
            {
                _curX = j;
                
                _mapImageMaker.HitCount = Physics.RaycastNonAlloc(_mapImageMaker.ScanPoint, Vector3.down, _mapImageMaker.RaycastHits);

                var color = new Color32();
                if (_mapImageMaker.HitCount == 0)
                {
                    color.r = 1 << 7;
                }
                else
                {
                    var hit = _mapImageMaker.RaycastHits[0].point;
                    color.b = (byte)((_mapImageMaker.ScanPoint.y - hit.y) * 2);
                }

                _mapImageMaker.LineScan[j] = color;
            }
            RayCastRequest = false;
            _mapImageMaker.IsResponse = true;
        }

        [MenuItem("CustomSystem/MapBaking/MapDrawSetting")]
        private static void Init()
        {
            var windowInstance = GetWindow<MapImageMakerSetting>(true, "Setting", true);
            EditorWindowTool.PopWindowInCenter(windowInstance);
        }
        
        private async void OnGUI()
        {
            if (!_isWalked)
            {
                _scanStartVector = EditorGUILayout.Vector3Field("Scan Start Vector", _scanStartVector);
                _scanX = EditorGUILayout.IntField("scan x", _scanX);
                _scanY = EditorGUILayout.IntField("scan y", _scanY);
                var click = GUILayout.Button("Bake");
                
                if (!click) return;
                
                _isWalked = true;
                Work = CurrentWork.Scan;
                SceneName = SceneManager.GetActiveScene().name;
                _mapImageMaker.Setting(_scanStartVector,_scanX,_scanY);
                await Task.Run(_mapImageMaker.ScanMap);
                while (Work != CurrentWork.MakeHeader) await Task.Delay(1000);
                Debug.Log("MakeHeader");
                await Task.Run(_mapImageMaker.PrintBmpFile);
                Work = CurrentWork.Ready;
            }
            else
            {
                switch (Work)
                {
                    case CurrentWork.Ready:
                        _isWalked = false;
                        break;
                    case CurrentWork.Scan:
                        GUILayout.Label("[ON RAY CAST]");
                        GUILayout.Label($"[CURRENT WORK POINT] X:{_curX}/{_scanX}  Y:{_curY}/{_scanY}");
                        break;
                    case CurrentWork.MakeHeader:
                        GUILayout.Label("[Make Header]");
                        break;
                    case CurrentWork.PrintPixel:
                        GUILayout.Label("[Print pixel to bmp file...]");
                        GUILayout.Label($"[CURRENT WORK POINT] X:{_curX}/{_scanX}  Y:{_curY}/{_scanY}");
                        break;
                    case CurrentWork.WriteFile:
                        GUILayout.Label("[Write bmp file...]");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
        }
    }
}