using Android.Graphics;
using Android.Media.Effect;
using Android.Opengl;
using Android.Util;
using Matrix = Android.Graphics.Matrix;

using Java.Lang;
using Object = Java.Lang.Object;

namespace PILSharp
{
    class OutputSurface : Object
    {
        int[] _textures = new int[2];
        TextureRenderer _textureRenderer;

        EGLDisplay _eglDisplay = EGL14.EglNoDisplay;
        EGLContext _eglContext = EGL14.EglNoContext;
        EGLSurface _eglSurface = EGL14.EglNoSurface;
        int _width;
        int _height;

        // Creates an OutputSurface with the specified dimensions.
        // The new EGL context and surface will be made current.
        public OutputSurface(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new IllegalArgumentException();
            }
            _width = width;
            _height = height;

            EGLSetup();
            MakeCurrent();

            _textureRenderer = new TextureRenderer();
            _textureRenderer.Init();
        }

        // Prepares EGL. We want a GLES 2.0 context and a surface that supports pbuffer.
        void EGLSetup()
        {
            _eglDisplay = EGL14.EglGetDisplay(EGL14.EglDefaultDisplay);
            if (_eglDisplay == EGL14.EglNoDisplay)
            {
                throw new RuntimeException("unable to get EGL14 display");
            }
            int[] version = new int[2];
            if (!EGL14.EglInitialize(_eglDisplay, version, 0, version, 1))
            {
                _eglDisplay = null;
                throw new RuntimeException("unable to initialize EGL14");
            }

            // Configure EGL for pbuffer and OpenGL ES 2.0, 24-bit RGB.
            int[] attribList =
            {
                    EGL14.EglRedSize, 8,
                    EGL14.EglGreenSize, 8,
                    EGL14.EglBlueSize, 8,
                  //EGL14.EglAlphaSize, 8, //arg
                    EGL14.EglRenderableType, EGL14.EglOpenglEs2Bit,
                    EGL14.EglSurfaceType, EGL14.EglPbufferBit,
                    EGL14.EglNone
            };
            EGLConfig[] configs = new EGLConfig[1];
            int[] numConfigs = new int[1];
            if (!EGL14.EglChooseConfig(_eglDisplay, attribList, 0, configs, 0, configs.Length, numConfigs, 0))
            {
                throw new RuntimeException("unable to find RGB888+recordable ES2 EGL config");
            }

            // Configure context for OpenGL ES 2.0.
            int[] attrib_list =
            {
                    EGL14.EglContextClientVersion, 2,
                    EGL14.EglNone
            };
            _eglContext = EGL14.EglCreateContext(_eglDisplay, configs[0], EGL14.EglNoContext, attrib_list, 0);
            GLToolbox.CheckGLError("eglCreateContext");
            if (_eglContext == null)
            {
                throw new RuntimeException("null context");
            }

            // Create a pbuffer surface.
            int[] surfaceAttribs =
            {
                EGL14.EglWidth, _width,
                EGL14.EglHeight, _height,
                EGL14.EglNone
            };
            _eglSurface = EGL14.EglCreatePbufferSurface(_eglDisplay, configs[0], surfaceAttribs, 0);
            GLToolbox.CheckGLError("eglCreatePbufferSurface");
            if (_eglSurface == null)
            {
                throw new RuntimeException("surface was null");
            }
        }

        // Makes our EGL context and surface current.
        public void MakeCurrent()
        {
            if (!EGL14.EglMakeCurrent(_eglDisplay, _eglSurface, _eglSurface, _eglContext))
            {
                throw new RuntimeException("eglMakeCurrent failed");
            }
        }

        // Discard all resources held by this class, notably the EGL context.
        public void Release()
        {
            if (_eglDisplay != EGL14.EglNoDisplay)
            {
                EGL14.EglDestroySurface(_eglDisplay, _eglSurface);
                EGL14.EglDestroyContext(_eglDisplay, _eglContext);
                EGL14.EglReleaseThread();
                EGL14.EglTerminate(_eglDisplay);
            }
            _eglDisplay = EGL14.EglNoDisplay;
            _eglContext = EGL14.EglNoContext;
            _eglSurface = EGL14.EglNoSurface;

            _textureRenderer.TearDown();
            _textureRenderer = null;
        }

        public byte[] DrawImage(byte[] imageData)
        {
            // Load input bitmap
            var bitmap = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            // Flip input bitmap
            bitmap = Flip(bitmap);

            var _effectContext = EffectContext.CreateWithCurrentGlContext();
            LoadTextures(bitmap);

            // InitialiseEffect
            EffectFactory effectFactory = _effectContext.Factory;
            var _effect = effectFactory.CreateEffect(EffectFactory.EffectAutofix);
            _effect.SetParameter("scale", 1.0f);

            // ApplyEffect
            _effect.Apply(_textures[0], bitmap.Width, bitmap.Height, _textures[1]);

            // Draws the data from SurfaceTexture onto the current EGL surface.
            _textureRenderer.RenderTexture(_textures[1]);

            return _textureRenderer.GetImageBytes();
        }

        public new void Dispose()
        {
            this.Release();
            base.Dispose();
        }

        void LoadTextures(Bitmap bitmap)
        {
            // Generate textures
            GLES20.GlGenTextures(2, _textures, 0);

            _textureRenderer.UpdateTextureSize(bitmap.Width, bitmap.Height);

            // Upload to texture
            GLES20.GlBindTexture(GLES20.GlTexture2d, _textures[0]);
            GLUtils.TexImage2D(GLES20.GlTexture2d, 0, bitmap, 0);

            // Set texture parameters
            GLToolbox.InitTexParams();
        }

        Bitmap Flip(Bitmap source)
        {
            return Reflector(source, 1, -1);
        }

        Bitmap Mirror(Bitmap source)
        {
            return Reflector(source, -1, 1);
        }

        Bitmap Reflector(Bitmap source, int sx, int sy)
        {
            using (Matrix matrix = new Matrix())
            {
                matrix.PreScale(sx, sy);
                Bitmap output = Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, false);
                output.Density = (int)DisplayMetricsDensity.Default;
                return output;
            }
        }
    }
}