
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace BDG
{
    using UnityEditor;
    using UnityEngine;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    // bmp Header struct 그대로 넣으면 데이터 주소 제약 때문에 크기가 늘어남으로 버퍼에 넣을 때는 각각 넣어줘야 한다.
    struct bmpHeader
    {
        public ushort magicNumber;   // 2byte + margin 2byte 0x42 0x4d
        public uint fileSize;        // 4byte
        public uint temp;            // 4byte 0
        public uint dataStartOffset; // 4byte 0x36 0x00 0x00 0x00
    }

    // DIBHeader struct
    struct DIBHeader
    {
        public uint DIBHeaderSize;                  // 0x28 0x00 0x00 0x00
        public uint bmpWidth;                       // 
        public uint bmpHeight;                      // 
        public ushort colorPalne;                   // 0x01 0x00
        public ushort bitCount;                     // 0x18 0x00
        public uint compression;                    // 0
        public uint pictureSize;                    // 
        public uint widthDensity;                   // 
        public uint heightDensity;                  // 
        public uint nOfColorPalette;                // 0
        public uint nOfImportantColor;              // 0
    }
    
    public class MapImageMaker
    {
        private const int HEADER_SIZE = 54;    // header size (byte)
        private const int BMP_HEADER_SIZE = 14;
        private const int DIB_HEADER_SIZE = 40;
        
        private bmpHeader _bmpHeader;
        private DIBHeader _dibHeader;
        private Color32[][] _image;

        public const float ScanDensity = 0.3f; 

        private Vector3 _scanStartPosition;
        private int _mapSizeX, _mapSizeY;
        private byte _xPadding;

        public bool IsResponse;
        public Vector3 ScanPoint;
        public int HitCount;
        public RaycastHit[] RaycastHits = new RaycastHit[10];
        public Color32[] LineScan;

        public void Setting(Vector3 start, int x, int y)
        {
            _scanStartPosition = start;
            _mapSizeX = x;
            _mapSizeY = y;
        }
        
        public async void ScanMap()
        {
            _dibHeader.bmpWidth = (uint) (_mapSizeX / ScanDensity);
            _dibHeader.bmpHeight = (uint) (_mapSizeY / ScanDensity);

            _xPadding = (byte) ((4 - (_dibHeader.bmpWidth & 0b11)) & 0b11);
            
            _bmpHeader.magicNumber = 0x4d42;
            _bmpHeader.fileSize = HEADER_SIZE + 3 * _dibHeader.bmpWidth * _dibHeader.bmpHeight + _xPadding * _dibHeader.bmpHeight + 2;
            _bmpHeader.temp = 0;
            _bmpHeader.dataStartOffset = 0x36;
            _dibHeader.DIBHeaderSize = 0x28;
            _dibHeader.colorPalne = 0x01;
            _dibHeader.bitCount = 0x18;
            _dibHeader.compression = 0;
            _dibHeader.pictureSize = _bmpHeader.fileSize - HEADER_SIZE;
            _dibHeader.widthDensity = 2834;
            _dibHeader.heightDensity = 2834;
            _dibHeader.nOfColorPalette = 0;
            _dibHeader.nOfImportantColor = 0;
            
            ScanPoint = _scanStartPosition;

            Debug.Log($"[width : {_dibHeader.bmpWidth}] [height : {_dibHeader.bmpHeight}] [file size : {_bmpHeader.fileSize}]");
            _image = new Color32[_dibHeader.bmpHeight][];
            LineScan = new Color32[_dibHeader.bmpWidth];
            for (var i = 0; i < _dibHeader.bmpHeight; i++, ScanPoint.z += ScanDensity)
            {
                MapImageMakerSetting._curY = i;
                
                ScanPoint.x = _scanStartPosition.x;
                _image[i] = new Color32[_dibHeader.bmpWidth];
                
                MapImageMakerSetting.RayCastRequest = true;
                while (!IsResponse);
                IsResponse = false;
                
                for (var j = 0; j < _dibHeader.bmpWidth; j++)
                {
                    _image[i][j] = LineScan[j];
                }
            }
            
            // complete signal
            MapImageMakerSetting.Work = MapImageMakerSetting.CurrentWork.MakeHeader;
        }

        public void PrintBmpFile()
        {   
            if (_bmpHeader.fileSize < HEADER_SIZE) return;
            
            var path = $"{Environment.CurrentDirectory}/mapImage/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            FileStream fileStream;
            var fileNum = 0;
            var fileName = $"{MapImageMakerSetting.SceneName}.bmp"; 
            while(File.Exists(path + fileName))
            {
                fileName = $"{MapImageMakerSetting.SceneName} ({fileNum++}).bmp";
            }
            fileStream = File.Create(path + fileName, 1024, FileOptions.None);

            Debug.Log(path + fileName);
            
            var buffer = new byte[_bmpHeader.fileSize];

            //// BMP Header ////
            // magic number
            buffer[0] = 0x42;
            buffer[1] = 0x4D;
            // file size(byte)
            for (var i = 2; i < 6; i++)
            {
                buffer[i] = (byte)(_bmpHeader.fileSize & 0xff);
                _bmpHeader.fileSize >>= 8;
            }
            // temp
            for (var i = 6; i < 10; i++)
            {
                buffer[i] = (byte)(_bmpHeader.temp & 0xff);
                _bmpHeader.temp >>= 8;
            }
            //data start offset
            for (var i = 10; i < 14; i++)
            {
                buffer[i] = (byte)(_bmpHeader.dataStartOffset & 0xff);
                _bmpHeader.dataStartOffset >>= 8;
            }
            
            //// DIB Header ////
            IntPtr ptr = Marshal.AllocHGlobal(DIB_HEADER_SIZE);
            Marshal.StructureToPtr(_dibHeader, ptr, true);
            Marshal.Copy(ptr, buffer, BMP_HEADER_SIZE, DIB_HEADER_SIZE);
            Marshal.FreeHGlobal(ptr);
            
            MapImageMakerSetting.Work = MapImageMakerSetting.CurrentWork.PrintPixel;
            //// Image Data ////
            var offset = HEADER_SIZE;
            for (var i = 0; i < _dibHeader.bmpHeight; i++)
            {
                for (var j = 0; j < _dibHeader.bmpWidth; j++)
                {
                    MapImageMakerSetting._curX = j;
                    MapImageMakerSetting._curY = i;
                    
                    buffer[offset + 2] = _image[i][j].r;
                    buffer[offset + 1] = _image[i][j].g;
                    buffer[offset] = _image[i][j].b;
                    offset += 3;
                }
                offset += _xPadding;
            }

            Debug.Log($"Write File {buffer.Length}");
            MapImageMakerSetting.Work = MapImageMakerSetting.CurrentWork.WriteFile;
            fileStream.Write(buffer, 0, buffer.Length);
            fileStream.Close();
        }
    }
}