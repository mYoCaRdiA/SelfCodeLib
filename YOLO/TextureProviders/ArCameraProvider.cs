using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


namespace Assets.Scripts.TextureProviders
{
    [Serializable]
    public class ArCameraProvider : TextureProvider
    {
        [Tooltip("Leave empty for automatic selection.")]
        [SerializeField]
        private string cameraName;

        private Texture2D texture2d;
        

        public ArCameraProvider() : base()
        {

        }
        public ArCameraProvider(ARCameraManager arCameraManager, int width, int height, TextureFormat format = TextureFormat.RGB24) : base(width, height, format)
        {
            arCameraManager.frameReceived += (eventArgs) => {
                if (arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
                {
                    using (image)
                    {
                        var conversionParams = new XRCpuImage.ConversionParams
                        {
                            inputRect = new RectInt(0, 0, image.width, image.height),
                            outputDimensions = new Vector2Int(image.width, image.height),
                            outputFormat = TextureFormat.RGBA32,
                            transformation = XRCpuImage.Transformation.None
                        };

                        int size = image.GetConvertedDataSize(conversionParams);
                        var buffer = new NativeArray<byte>(size, Allocator.Temp);

                        image.Convert(conversionParams, buffer);

                        if (texture2d == null)
                        {
                            texture2d = new Texture2D(image.width, image.height, conversionParams.outputFormat, false);
                            InputTexture = texture2d;
                        }

                        texture2d.LoadRawTextureData(buffer);
                        texture2d.Apply();

                        buffer.Dispose();

                        
                    }
                }
            };
        }

        

        public override void Start()
        {
        
        }

        public override void Stop()
        {
           
        }

        public override TextureProviderType.ProviderType TypeEnum()
        {
            return TextureProviderType.ProviderType.ArCamera;
        }

       
    }
}