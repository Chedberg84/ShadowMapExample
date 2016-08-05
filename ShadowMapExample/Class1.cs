#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

using System;

using OpenTK;
//using Glu = OpenTK.Graphics.Glu;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace Examples.Tutorial
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    public class SimpleWindow : GameWindow
    {
        float[] ambientLight = new float[4] { 0.2f, 0.2f, 0.2f, 1.0f };
        float[] diffuseLight = new float[4] { 0.7f, 0.7f, 0.7f, 1.0f };
        float[] noLight = new float[4] { 0.0f, 0.0f, 0.0f, 1.0f };
        float[] lightPos = new float[4] { 200.0f, 300.0f, 100.0f, 1.0f };
        float[] cameraPos = new float[4] { 100.0f, 150.0f, 200.0f, 1.0f };
        
        int shadowSize = 512;                 // set based on window size
        uint shadowTextureID;
        float[] sPlane = new float[4] { 1.0f, 0.0f, 0.0f, 0.0f };
        float[] tPlane = new float[4] { 0.0f, 1.0f, 0.0f, 0.0f };
        float[] rPlane = new float[4] { 0.0f, 0.0f, 1.0f, 0.0f };
        float[] qPlane = new float[4] { 0.0f, 0.0f, 0.0f, 1.0f };
        float factor = 5.0f;

        float[] lightModelview = new float[16], lightProjection = new float[16];

        int windowWidth = 512;                // window size
        int windowHeight = 512;

        //IntPtr esfera, cono;

        float rot = 0;

        public SimpleWindow()
            : base(800, 600)
        {
            
            //esfera = Glu.NewQuadric();
            //cono = Glu.NewQuadric();
            Keyboard.KeyDown += Keyboard_KeyDown;
        }

        #region Keyboard_KeyDown

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The KeyboardDevice which generated this event.</param>
        /// <param name="key">The key that was pressed.</param>
        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs key)
        {
            if (key.Key == Key.Escape)
                this.Exit();
            if (key.Key == Key.Left)
                lightPos[2] += 5f;
            if (key.Key == Key.Right)
                lightPos[2] -= 5f;
            if (key.Key == Key.Up)
                lightPos[0] += 5f;
            if (key.Key == Key.Down)
                lightPos[0] -= 5f;

            if ((key.Key == Key.AltLeft || key.Key == Key.AltRight) &&
                (key.Key == Key.Enter || key.Key == Key.KeypadEnter))
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;
        }

        #endregion

        #region DrawModels()

        void DrawModels()
        {
            float width = 30;
            float height = 30;
            float length = 30;

            width /= 2.0f;
            height /= 2.0f;
            length /= 2.0f;

            // Draw plane that the objects rest on
            GL.Color3(0.0f, 0.0f, 0.90f); // Blue
            GL.Normal3(0.0f, 1.0f, 0.0f);
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(-100.0f, -25.0f, -100.0f);
            GL.Vertex3(-100.0f, -25.0f, 100.0f);
            GL.Vertex3(100.0f, -25.0f, 100.0f);
            GL.Vertex3(100.0f, -25.0f, -100.0f);
            GL.End();

            // Draw red cube
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.PushMatrix();
            GL.Translate(0.0f, 30.0f, 0.0f);
            GL.Begin(BeginMode.Quads);
            GL.Normal3(0.0f, 0.0f, -1.0f);
            GL.Vertex3(-width, height, -length); //0
            GL.Vertex3(width, height, -length);  //1
            GL.Vertex3(width, -height, -length);//2
            GL.Vertex3(-width, -height, -length); //3
                                                  //
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(-width, height, length);  //4
            GL.Vertex3(-width, -height, length); //5
            GL.Vertex3(width, -height, length);   //6
            GL.Vertex3(width, height, length);  //7
                                                //
            GL.Normal3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(-width, height, length);  //8 (4)
            GL.Vertex3(width, height, length);   //9 (6)
            GL.Vertex3(width, height, -length); //10 (0)
            GL.Vertex3(-width, height, -length);  //11 (1)
                                                  //
            GL.Normal3(0.0f, -1.0f, 0.0f);
            GL.Vertex3(-width, -height, length); //12 (5)
            GL.Vertex3(-width, -height, -length);//13 (2)
            GL.Vertex3(width, -height, -length);  //14 (7)
            GL.Vertex3(width, -height, length); //15 (3)
                                                //
            GL.Normal3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(width, height, -length);  //16 (1)
            GL.Vertex3(width, height, length);   //17 (6)
            GL.Vertex3(width, -height, length); //18 (3)
            GL.Vertex3(width, -height, -length);  //19 (7)
                                                  //
            GL.Normal3(-1.0f, 0.0f, 0.0f);
            GL.Vertex3(-width, height, -length); //20 (0)
            GL.Vertex3(-width, -height, -length);//21 (2)
            GL.Vertex3(-width, -height, length);  //22 (4)
            GL.Vertex3(-width, height, length); //23 (5)
                                                //
            GL.End();
            GL.PopMatrix();

            //// Draw green sphere
            //GL.Color3(0.0f, 1.0f, 0.0f);
            //GL.PushMatrix();
            //GL.Translate(-60.0f, 0.0f, 0.0f);
            ////Glu.Sphere(esfera, 25.0f, 50, 50);
            //GL.PopMatrix();

            //// Draw yellow cone
            //GL.Color3(1.0f, 1.0f, 0.0f);
            //GL.PushMatrix();
            //GL.Rotate(-90.0f, 1.0f, 0.0f, 0.0f);
            //GL.Translate(60.0f, 0.0f, -24.0f);
            ////Glu.Cylinder(cono, 25.0f, 0.0f, 50.0f, 50, 50);
            //GL.PopMatrix();

            //// Draw magenta torus
            //GL.Color3(1.0f, 0.0f, 1.0f);
            //GL.PushMatrix();
            //GL.Translate(0.0f, 0.0f, 60.0f);
            ////glutSolidTorus(8.0f, 16.0f, 50, 50);
            //GL.PopMatrix();

            //// Draw cyan octahedron
            //GL.Color3(0.0f, 1.0f, 1.0f);
            //GL.PushMatrix();
            //GL.Translate(0.0f, 0.0f, -60.0f);
            //GL.Scale(25.0f, 25.0f, 25.0f);
            ////glutSolidOctahedron();
            //GL.PopMatrix();
        }
        #endregion DrawModels()

        #region RegenerateShadowMap()

        void RegenerateShadowMap()
        {
            float lightToSceneDistance, nearPlane, fieldOfView;

            // Save the depth precision for where it's useful
            lightToSceneDistance = (float)Math.Sqrt(lightPos[0] * lightPos[0] +
                                                    lightPos[1] * lightPos[1] +
                                                    lightPos[2] * lightPos[2]);
            nearPlane = lightToSceneDistance - 150.0f;
            if (nearPlane < 50.0f)
                nearPlane = 50.0f;
            // Keep the scene filling the depth texture
            fieldOfView = 17000.0f / lightToSceneDistance;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            
            perspecitveGL(fieldOfView, 1.0f, nearPlane, nearPlane + 300.0f);
            //Glu.Perspective(fieldOfView, 1.0f, nearPlane, nearPlane + 300.0f);

            GL.GetFloat(GetPName.ProjectionMatrix, lightProjection);
            // Switch to light's point of view
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Matrix4 v = Matrix4.LookAt(lightPos[0], lightPos[1], lightPos[2],
                                        0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
            GL.MultMatrix(ref v);
            //Glu.LookAt(lightPos[0], lightPos[1], lightPos[2],
            //            0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);

            GL.GetFloat(GetPName.ModelviewMatrix, lightModelview);
            GL.Viewport(0, 0, shadowSize, shadowSize);

            // Clear the window with current clearing color
            GL.Clear(ClearBufferMask.DepthBufferBit);

            // All we care about here is resulting depth values
            GL.ShadeModel(ShadingModel.Flat);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.Normalize);
            GL.ColorMask(false, false, false, false);

            // Overcome imprecision
            GL.Enable(EnableCap.PolygonOffsetFill);

            DrawModels();

            // Copy depth values into depth texture
            GL.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent,
                     0, 0, shadowSize, shadowSize, 0);

            // Restore normal drawing state
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Normalize);
            GL.ColorMask(true, true, true, true);
            GL.Disable(EnableCap.PolygonOffsetFill);

            // Set up texture matrix for shadow map projection
            GL.MatrixMode(MatrixMode.Texture);
            GL.LoadIdentity();
            GL.Translate(0.5f, 0.5f, 0.5f);
            GL.Scale(0.5f, 0.5f, 0.5f);
            GL.MultMatrix(lightProjection);
            GL.MultMatrix(lightModelview);
        }
        #endregion RegenerateShadowMap()

        private void perspecitveGL(double fovY, double aspect, double zNear, double zFar)
        {
            const double pi = 3.14159;
            double fW, fH;

            fH = Math.Tan(fovY / 360 * pi) * zNear;
            fW = fH * aspect;

            GL.Frustum(-fW, fW, -fH, fH, zNear, zFar);
        }

        #region OnLoad

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            // Black background
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            // Hidden surface removal
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.PolygonOffset(factor, 0.0f);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            //GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0, 0, 0, 1 });
            // Set up some lighting state that never changes
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Normalize);
            GL.Enable(EnableCap.Light0);

            // Set up some texture state that never changes
            GL.GenTextures(1, out shadowTextureID);
            GL.BindTexture(TextureTarget.Texture2D, shadowTextureID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)(int)All.DepthTextureModeArb, (int)All.Intensity);
            //if (ambientShadowAvailable)
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)(int)All.TextureCompareFailValue, 0.5f);
            GL.TexGen(TextureCoordName.S, TextureGenParameter.TextureGenMode, (int)TextureGenMode.EyeLinear);
            GL.TexGen(TextureCoordName.T, TextureGenParameter.TextureGenMode, (int)TextureGenMode.EyeLinear);
            GL.TexGen(TextureCoordName.R, TextureGenParameter.TextureGenMode, (int)TextureGenMode.EyeLinear);
            GL.TexGen(TextureCoordName.Q, TextureGenParameter.TextureGenMode, (int)TextureGenMode.EyeLinear);

            RegenerateShadowMap();
        }

        #endregion

        #region OnResize

        /// <summary>
        /// Respond to resize events here.
        /// </summary>
        /// <param name="e">Contains information on the new GameWindow size.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnResize(EventArgs e)
        {
            int i;

            windowWidth = Width;
            windowHeight = Height;

            // Find the largest power of two that will fit in window
            if (Width > Height)
                shadowSize = Height;
            else
                shadowSize = Width;

            // Try each size until we get one that's too big
            i = 0;
            while ((1 << i) <= shadowSize)
                i++;
            shadowSize = (1 << (i - 1));
        }

        #endregion

        #region OnUpdateFrame

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            rot += 0.5f;

            //RegenerateShadowMap();
        }

        #endregion

        #region OnRenderFrame

        /// <summary>
        /// Add your game rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            RegenerateShadowMap();

            double aspect = (double)windowWidth / (double)windowHeight;
            // Track camera angle
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            perspecitveGL(45.0f, aspect, 1.0f, 1000.0f);
            //Glu.Perspective(45.0f, aspect, 1.0f, 1000.0f);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Matrix4 v = Matrix4.LookAt(cameraPos[0], cameraPos[1], cameraPos[2],
                                      0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
            GL.MultMatrix(ref v);
            //Glu.LookAt(cameraPos[0], cameraPos[1], cameraPos[2],
            //          0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);

            GL.Viewport(0, 0, windowWidth, windowHeight);

            GL.Rotate(rot, 0, 1, 0);

            // Track light position
            GL.Light(LightName.Light0, LightParameter.Position, lightPos);

            // Clear the window with current clearing color
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            // Set up shadow comparison
            GL.Enable(EnableCap.Texture2D);
            
            //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (float)TextureCompareMode.CompareRToTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMinFilter.Nearest);

            // Set up the eye plane for projecting the shadow map on the scene
            GL.Enable(EnableCap.TextureGenS);
            GL.Enable(EnableCap.TextureGenT);
            GL.Enable(EnableCap.TextureGenR);
            GL.Enable(EnableCap.TextureGenQ);
            GL.TexGen(TextureCoordName.S, TextureGenParameter.EyePlane, sPlane);
            GL.TexGen(TextureCoordName.T, TextureGenParameter.EyePlane, tPlane);
            GL.TexGen(TextureCoordName.R, TextureGenParameter.EyePlane, rPlane);
            GL.TexGen(TextureCoordName.Q, TextureGenParameter.EyePlane, qPlane);

            // Draw objects in the scene
            DrawModels();

            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.TextureGenS);
            GL.Disable(EnableCap.TextureGenT);
            GL.Disable(EnableCap.TextureGenR);
            GL.Disable(EnableCap.TextureGenQ);

            this.SwapBuffers();
        }

        #endregion

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);

            //Glu.DeleteQuadric(cono);
            //Glu.DeleteQuadric(esfera);
        }

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (SimpleWindow example = new SimpleWindow())
            {
                // Get the title and category  of this example using reflection.
                example.Title = "Shadow2Pass";
                example.Run(30.0, 0.0);
            }
        }

        #endregion
    }
}