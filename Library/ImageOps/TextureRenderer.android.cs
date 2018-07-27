using System;
using System.IO;

using Android.Graphics;
using Android.Opengl;
using Java.Nio;

namespace PILSharp
{
    // Code for rendering a texture onto a surface using OpenGL ES 2.0.
    class TextureRenderer : Java.Lang.Object
    {
        public void Init()
        {
            // Create program
            _program = GLToolbox.CreateProgram(VertexShader, FragmentShader);

            // Bind attributes and uniforms
            _texSamplerHandle = GLES20.GlGetUniformLocation(_program, "tex_sampler");
            _texCoordHandle = GLES20.GlGetAttribLocation(_program, "a_texcoord");
            _posCoordHandle = GLES20.GlGetAttribLocation(_program, "a_position");

            // Setup coordinate buffers
            _texVertices = ByteBuffer.AllocateDirect(
                TexVertices.Length * FloatSizeBytes)
                    .Order(ByteOrder.NativeOrder()).AsFloatBuffer();

            _texVertices.Put(TexVertices).Position(0);

            _posVertices = ByteBuffer.AllocateDirect(
                PosVertices.Length * FloatSizeBytes)
                    .Order(ByteOrder.NativeOrder()).AsFloatBuffer();

            _posVertices.Put(PosVertices).Position(0);
        }

        public void TearDown()
        {
            GLES20.GlDeleteProgram(_program);
        }

        // Draws the external texture in SurfaceTexture onto the current EGL surface.
        public void RenderTexture(int texId)
        {
            // Bind default FBO
            GLES20.GlBindFramebuffer(GLES20.GlFramebuffer, 0);

            // Use our shader program
            GLES20.GlUseProgram(_program);
            GLToolbox.CheckGLError("glUseProgram");

            // Set viewport
            GLES20.GlViewport(0, 0, _texWidth, _texHeight);
            GLToolbox.CheckGLError("glViewport");

            // Disable blending
            GLES20.GlDisable(GLES20.GlBlend);

            // Set the vertex attributes
            GLES20.GlVertexAttribPointer(_texCoordHandle, 2, GLES20.GlFloat, false, 0, _texVertices);
            GLES20.GlEnableVertexAttribArray(_texCoordHandle);
            GLES20.GlVertexAttribPointer(_posCoordHandle, 2, GLES20.GlFloat, false, 0, _posVertices);
            GLES20.GlEnableVertexAttribArray(_posCoordHandle);
            GLToolbox.CheckGLError("vertex attribute setup");

            // Set the input texture
            GLES20.GlActiveTexture(GLES20.GlTexture0);
            GLToolbox.CheckGLError("glActiveTexture");
            GLES20.GlBindTexture(GLES20.GlTexture2d, texId);
            GLToolbox.CheckGLError("glBindTexture");
            GLES20.GlUniform1i(_texSamplerHandle, 0);

            // Draw
            GLES20.GlClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GLES20.GlClear(GLES20.GlColorBufferBit);
            GLES20.GlDrawArrays(GLES20.GlTriangleStrip, 0, 4);
        }

        public void UpdateTextureSize(int texWidth, int texHeight)
        {
            _texWidth = texWidth;
            _texHeight = texHeight;

            ComputeOutputVertices();
        }

        // Get's the image bytes of the current frame as a JPEG image.
        public byte[] GetImageBytes(Format imageFormat)
        {
            // glReadPixels gives us a ByteBuffer filled with what is essentially big-endian RGBA
            // data. To use the Bitmap constructor that takes an int[] array with pixel data, 
            // we need an int[] filled with little-endian ARGB data.
            //
            // It turns out that the Bitmap#copyPixelsFromBuffer() method wants RGBA pixels, not ARGB, 
            // so if we create an empty bitmap and then copy pixel data in we can just copy straight
            // into the Bitmap from the ByteBuffer.
            //
            // Our output will look upside-down relative to what appears on screen if the
            // typical GL conventions are used. (We avoid the issue by inverting the image
            // before callling GLUtils.TexImage2D.)

            byte[] imageBytes = Array.Empty<byte>();

            // Allocating large buffers is expensive, so we really want pixelBuf to be allocated ahead of time if possible.
            using (var pixelBuf = ByteBuffer.AllocateDirect(_texWidth * _texHeight * 4))
            {
                pixelBuf.Order(ByteOrder.LittleEndian);
                pixelBuf.Rewind();
                GLES20.GlReadPixels(0, 0, _texWidth, _texHeight, GLES20.GlRgba, GLES20.GlUnsignedByte, pixelBuf);

                using (var bitmap = Bitmap.CreateBitmap(_texWidth, _texHeight, Bitmap.Config.Argb8888))
                {
                    pixelBuf.Rewind();
                    bitmap.CopyPixelsFromBuffer(pixelBuf);
                    imageBytes = bitmap.ToByteArray(imageFormat);

                    bitmap.Recycle();
                }
            }

            return imageBytes;
        }

        void ComputeOutputVertices()
        {
            if (_posVertices != null)
            {
                float x0, y0, x1, y1;
                x0 = -1.0f;
                y0 = -1.0f;
                x1 = 1.0f;
                y1 = 1.0f;

                var coords = new[] { x0, y0, x1, y0, x0, y1, x1, y1 };
                _posVertices.Put(coords).Position(0);
            }
        }

        FloatBuffer _texVertices;
        FloatBuffer _posVertices;

        int _program;
        int _texSamplerHandle;
        int _texCoordHandle;
        int _posCoordHandle;
        int _texWidth;
        int _texHeight;

        const string VertexShader =
            "attribute vec4 a_position;\n" +
            "attribute vec2 a_texcoord;\n" +
            "varying vec2 v_texcoord;\n" +
            "void main() {\n" +
            "  gl_Position = a_position;\n" +
            "  v_texcoord = a_texcoord;\n" +
            "}\n";
        const string FragmentShader =
            "precision mediump float;\n" +
            "uniform sampler2D tex_sampler;\n" +
            "varying vec2 v_texcoord;\n" +
            "void main() {\n" +
            "  gl_FragColor = texture2D(tex_sampler, v_texcoord);\n" +
            "}\n";
        static readonly float[] TexVertices = {
            0.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f
        };
        static readonly float[] PosVertices = {
            -1.0f, -1.0f, 1.0f, -1.0f, -1.0f, 1.0f, 1.0f, 1.0f
        };

        const int FloatSizeBytes = 4;
    }
}