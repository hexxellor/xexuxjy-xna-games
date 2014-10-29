﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ModelNamer;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Color = Microsoft.Xna.Framework.Color;

namespace XNAViewer
{
    public class XNAPointViewer : Microsoft.Xna.Framework.Game
    {
        static int size = 5;
        Vector3 eyePos = new Vector3(size * 5, size * 5, 0);
        Vector3 eyeLookat = new Vector3(0, 0, 0);
        const float rotation_speed = 1.0f;
        float angle;
        SpriteFont viewerFont;
        SpriteBatch spriteBatch;
        private Matrix cameraMatrix;
        private Matrix rotationMatrix;
        private float[] mouseSpeed = new float[2];
        private Vector2 mouseDelta;
        private Vector3 location;
        private Vector3 up = Vector3.UnitY;
        private float pitch = 0.0f;
        private float facing = 0.0f;
        public String textureBasePath = @"D:\gladius-extracted-archive\gc-compressed\textures.jpg\";
        //public String textureBasePath = @"C:\temp\textures\";
        public Texture2D m_missingTexture;
        public List<String> m_fileNames = new List<string>();
        public Dictionary<String, WrappedModel> m_modelMap = new Dictionary<string, WrappedModel>();

        public GraphicsDeviceManager m_graphicsDeviceManager;

        InputState inputState;


        Matrix projectionMatrix;
        Matrix modelviewMatrix;

        BasicEffect m_basicEffect;
        Effect m_metalEffect;

        public XNAPointViewer()
        {
            m_graphicsDeviceManager = new GraphicsDeviceManager(this);
            m_graphicsDeviceManager.PreferredBackBufferWidth = 1024;
            m_graphicsDeviceManager.PreferredBackBufferHeight = 768;
            //.GraphicsDeviceManager.Globals.GraphicsDeviceManager.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
            Content.RootDirectory = "Content";
            inputState = new InputState();
            //CreateEmptyTexture();
            //m_textPrinter = new OpenTK.Graphics.TextPrinter();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            CreateShaders();

            //RasterizerState rs = new RasterizerState();
            //rs.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rs;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            viewerFont = Content.Load <SpriteFont> ("ViewerFont");


            m_missingTexture = GetTexture(Color.Pink);


            //this.VSync = VSyncMode.Off;
            //m_modelReader = new GCModelReader();
            m_modelReader = new XboxModelReader();
            string baseModelPath = m_modelReader is GCModelReader ? @"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\" : @"D:\gladius-extracted-archive\xbox-decompressed\ModelFilesRenamed\";

            
            //m_modelReader = new XboxModelReader();
            //m_fileNames.AddRange(Directory.GetFiles(@"C:\tmp\unpacking\gc-probable-models-renamed\probable-models-renamed", "*"));
            //m_fileNames.AddRange(Directory.GetFiles(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\weapons", "*"));
            //m_fileNames.Add(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\yeti.mdl");
            //m_xbModelReader = new XboxModelReader();
            //m_fileNames.Add(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\alpha_box.mdl");
            //m_fileNames.Add(@"D:\gladius-extracted-archive\xbox-decompressed\ModelFilesRenamed\alpha_box.mdl");



            m_fileNames.Add(baseModelPath+"anklet_queen.mdl");
            ChangeModelNext();




            //cameraMatrix = Matrix.Identity;
            //location = new Vector3(0f, 10f, 0f);
            //mouseDelta = new Vector2();

            //System.Windows.Forms.Cursor.Position = new Point(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);

            ////Mouse.Move += new EventHandler<MouseMoveEventArgs>(OnMouseMove);

        }


        protected override void Update(GameTime gameTime)
        {
            inputState.Update();
            HandleInput(inputState, gameTime);

            facing += mouseSpeed[0];
            pitch += mouseSpeed[1];

            Vector3 lookatPoint = new Vector3((float)Math.Cos(facing), (float)Math.Sin(pitch), (float)Math.Sin(facing));
            modelviewMatrix = Matrix.CreateLookAt(location, location + lookatPoint, up);

            //modelviewMatrix = Matrix.CreateLookAt(new Vector3(0,0,-10), Vector3.Zero, up);
        }

        public void HandleInput(InputState inputState, GameTime gameTime)
        {
            GenerateKeyEvents(ref inputState.LastKeyboardStates[0], ref inputState.CurrentKeyboardStates[0], gameTime);
        }


        void CreateShaders()
        {
            m_basicEffect = new BasicEffect(m_graphicsDeviceManager.GraphicsDevice);
            m_basicEffect.TextureEnabled = true;
            //m_basicEffect.VertexColorEnabled = false;
            m_basicEffect.EnableDefaultLighting();

            //projectionMatrix = Matrix.CreatePerspective(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 1, 500);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1.0f, 300.0f);

            m_basicEffect.Projection = projectionMatrix;


            //m_metalEffect = Content.Load<Effect>("Effect1");
            m_metalEffect = Content.Load<Effect>("metal");

        }



        public static Enum[] GetEnumValues(Type enumType)
        {

            if (enumType.BaseType == typeof(Enum))
            {
                FieldInfo[] info = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
                Enum[] values = new Enum[info.Length];
                for (int i = 0; i < values.Length; ++i)
                {
                    values[i] = (Enum)info[i].GetValue(null);
                }
                return values;
            }
            else
            {
                throw new Exception("Given type is not an Enum type");
            }
        }

        static Enum[] keysEnumValues = GetEnumValues(typeof(Keys));
        private void GenerateKeyEvents(ref KeyboardState old, ref KeyboardState current, GameTime gameTime)
        {
            foreach (Keys key in keysEnumValues)
            {
                bool released = WasReleased(ref old, ref current, key);
                if (released || IsHeldKey(ref current, key))
                {
                    KeyboardCallback(key, released, ref current, ref old, gameTime);
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        // This is a way of generating 'pressed' events for keys that we want to hold down
        private bool IsHeldKey(ref KeyboardState current, Keys key)
        {
            return (current.IsKeyDown(key) && ((key == Keys.Left || key == Keys.Right || key == Keys.Up ||
                key == Keys.Down || key == Keys.PageUp || key == Keys.PageDown || key == Keys.A ||
                key == Keys.W || key == Keys.S || key == Keys.D || key == Keys.Q || key == Keys.Z)));
        }
        //----------------------------------------------------------------------------------------------

        private bool WasReleased(ref KeyboardState old, ref KeyboardState current, Keys key)
        {
            // figure out if the key was released between states.
            return old.IsKeyDown(key) && !current.IsKeyDown(key);
        }


        public virtual void KeyboardCallback(Keys key, bool released, ref KeyboardState newState, ref KeyboardState oldState, GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (key == Keys.O)
                this.ChangeModelPrev();
            if (key == Keys.P)
                this.ChangeModelNext();
            if (key == Keys.K)
                this.ChangeTexturePrev();
            if (key == Keys.L)
                this.ChangeTextureNext();
            if (key == Keys.N)
                this.ChangeSubModelPrev();
            if (key == Keys.M)
                this.ChangeSubModelNext();
            if (key == Keys.F)
                this.rotateX = !this.rotateX;
            if (key == Keys.G)
                this.rotateY = !this.rotateY;
            if (key == Keys.H)
                this.rotateZ = !this.rotateZ;
            if (key == Keys.Escape)
                Exit();

            if (IsHeldKey(ref  inputState.CurrentKeyboardStates[0], Keys.W))
            {
                location.X += (float)Math.Cos(facing) * 0.1f;
                location.Z += (float)Math.Sin(facing) * 0.1f;
            }

            if (IsHeldKey(ref  inputState.CurrentKeyboardStates[0], Keys.S))
            {
                location.X -= (float)Math.Cos(facing) * 0.1f;
                location.Z -= (float)Math.Sin(facing) * 0.1f;
            }

            if (IsHeldKey(ref  inputState.CurrentKeyboardStates[0], Keys.A))
            {
                location.X -= (float)Math.Cos(facing + Math.PI / 2) * 0.1f;
                location.Z -= (float)Math.Sin(facing + Math.PI / 2) * 0.1f;
            }

            if (IsHeldKey(ref  inputState.CurrentKeyboardStates[0], Keys.D))
            {
                location.X += (float)Math.Cos(facing + Math.PI / 2) * 0.1f;
                location.Z += (float)Math.Sin(facing + Math.PI / 2) * 0.1f;
            }



        }

        public void ChangeTextureNext()
        {
            if (m_currentModel.m_model.m_textures.Count > 1)
            {
                m_currentTextureIndex++;
                m_currentTextureIndex %= m_currentModel.m_model.m_textures.Count;
            }
        }

        public void ChangeTexturePrev()
        {
            if (m_currentModel.m_model.m_textures.Count > 1)
            {
                m_currentTextureIndex--;
                if (m_currentTextureIndex < 0)
                {
                    m_currentTextureIndex += m_currentModel.m_model.m_textures.Count;
                }
            }
        }



        void CalcBindFinalMatrix(BoneNode bone, Matrix parentMatrix)
        {
            bone.combinedMatrix = bone.localMatrix * parentMatrix;
            //bone.finalMatrix = bone.offsetMatrix * bone.combinedMatrix;
            bone.finalMatrix = bone.combinedMatrix;

            foreach (BoneNode child in bone.children)
            {
                CalcBindFinalMatrix(child, bone.combinedMatrix);
            }
        }

        public void DrawSkeleton()
        {
            //GCModel model = m_currentModel;
            //GL.Begin(PrimitiveType.Points);

            //BoneNode start = model.m_rootBone;
            //CalcBindFinalMatrix(start, Matrix.Identity);

            //foreach (BoneNode node in model.m_bones)
            //{
            //    GL.Vertex3(node.finalMatrix.ExtractTranslation());
            //}

            //GL.End();
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            m_basicEffect.Projection = projectionMatrix;

            if (m_currentModel != null)
            {
                //this.Title = "Name: " + m_currentModel.m_model.m_name + "  Loc: " + string.Format("{0:F}.{1:F}.{2:f}", location.X, location.Y, location.Z);

                //GL.Enable(EnableCap.Texture2D);
                //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                //GL.PointSize(10f);

                angle += rotation_speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                rotationMatrix = Matrix.Identity;
                if (rotateX)
                {
                    rotationMatrix *= Matrix.CreateRotationX(angle);
                }
                if (rotateY)
                {
                    rotationMatrix *= Matrix.CreateRotationY(angle);
                }
                if (rotateZ)
                {
                    rotationMatrix *= Matrix.CreateRotationZ(angle);
                }

                //rotationMatrix *= modelviewMatrix;
                Matrix final = modelviewMatrix * rotationMatrix;

                //GL.Begin(PrimitiveType.Points);

                //m_textPrinter.Begin();

                //m_textPrinter.Print(BuildDebugString(), font, Color.White);
                //m_textPrinter.End();

                spriteBatch.Begin();

                spriteBatch.DrawString(viewerFont, BuildDebugString(), Vector2.Zero, Color.White);

                spriteBatch.End();

                bool drawSkeleton = false;

                if (drawSkeleton)
                {
                 //   DrawSkeleon();
                }
                else
                {
                    DrawModel();
                }
            }
            else
            {
                //this.Title = "Null Model";
            }


            base.Draw(gameTime);

        }


        public void DrawModel()
        {
            int counter = -1;
            foreach (WrappedDisplayListHeader header in m_currentModel.m_wrappedHeaderList)
            {
                counter++;
                if (displayAll == false && counter != m_currentModelSubIndex)
                {
                    continue;
                }

                if (m_currentModel.m_model.m_skinned)
                {
                    if (header.m_modelSubMesh.MeshId != 4)
                    {
                        //continue;
                    }

                    if (header.m_modelSubMesh.LodLevel != 0x01)
                    {
                        continue;
                    }
                }



                ShaderData sd = m_currentModel.m_model.m_shaderData[header.m_modelSubMesh.MeshId];


                //SetTexture(sd.textureId1);

                //header.Render();

                GraphicsDevice.SetVertexBuffer(header.m_vertexBuffer);
                GraphicsDevice.Indices = header.m_indexBuffer;
                //m_basicEffect.Texture = m_missingTexture;

                Effect effect;
                SetShaderData(sd, out effect);

                if (effect != null)
                {
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        //m_graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_header.entries.Count, 0, m_header.entries.Count / 3);
                        //GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, header.m_modelSubMesh.NumIndices, 0, header.m_modelSubMesh.NumIndices / 3);
                    }
                }



            }
        }

        public void SetShaderData(ShaderData sd, out Effect effect)
        {
            Texture2D tex1 = SetTexture(sd.textureId1);
            Texture2D tex2 = SetTexture(sd.textureId2);


            if (sd.shaderName == "metal")
            {
                m_metalEffect.Parameters["Texture1"].SetValue(tex1);
                m_metalEffect.Parameters["Texture2"].SetValue(tex2);
                m_metalEffect.Parameters["World"].SetValue(rotationMatrix);
                m_metalEffect.Parameters["View"].SetValue(modelviewMatrix);
                m_metalEffect.Parameters["Projection"].SetValue(projectionMatrix);
                
                //ApplyLightToEffect(m_metalEffect);
                effect = m_metalEffect;
            }
            else
            {
                m_basicEffect.Texture = tex1;
                m_basicEffect.World = rotationMatrix;
                m_basicEffect.View = modelviewMatrix;


                effect = m_basicEffect;
            }
        }


        public void ChangeModelNext()
        {
            if (m_fileNames.Count > 0)
            {
                int attempts = 0;
                while (attempts < m_fileNames.Count)
                {
                    attempts++;

                    m_currentModelIndex++;
                    m_currentModelIndex %= m_fileNames.Count;
                    if (ChangeModel())
                    {
                        m_currentModel = m_modelMap[m_fileNames[m_currentModelIndex]];
                        break;
                    }
                }
            }
        }

        public void ChangeModelPrev()
        {
            if (m_fileNames.Count > 0)
            {
                int attempts = 0;
                while (attempts < m_fileNames.Count)
                {
                    attempts++;

                    m_currentModelIndex--;
                    if (m_currentModelIndex < 0)
                            {
                        m_currentModelIndex += m_fileNames.Count;
                    }
                    if (ChangeModel())
                    {
                        m_currentModel = m_modelMap[m_fileNames[m_currentModelIndex]];
                        break;
                    }
                }


            }

        }


        public void ChangeSubModelNext()
        {
            if (!displayAll)
            {
                if (m_currentModel.m_model.m_modelMeshes.Count > 1)
                {
                    m_currentModelSubIndex++;
                    m_currentModelSubIndex %= m_currentModel.m_model.m_modelMeshes.Count;
                    ChangeSubModel();
                }
            }
        }

        public void ChangeSubModelPrev()
        {
            if (!displayAll)
            {
                if (m_currentModel.m_model.m_modelMeshes.Count > 1)
                {
                    m_currentModelSubIndex--;
                    if (m_currentModelSubIndex < 0)
                    {
                        m_currentModelSubIndex += m_currentModel.m_model.m_modelMeshes.Count;
                        ChangeSubModel();
                    }
                }
            }

        }

        public void ChangeSubModel()
        {
            ModelSubMesh header = m_currentModel.m_model.m_modelMeshes[m_currentModelSubIndex];
            Lookat(header.MinBB, header.MaxBB);
        }



        public bool ChangeModel()
        {
            bool valid;
            BaseModel model = null;
            WrappedModel wrappedModel = null;
            if (!m_modelMap.ContainsKey(m_fileNames[m_currentModelIndex]))
            {
                try
                {
                    model = m_modelReader.LoadSingleModel(m_fileNames[m_currentModelIndex]);
                }
                catch (System.Exception ex)
                {

                }
                if (model != null)
                {
                    wrappedModel = new WrappedModel(model, m_graphicsDeviceManager.GraphicsDevice, m_basicEffect);
                    m_modelMap[m_fileNames[m_currentModelIndex]] = wrappedModel;
                }
            }
            else
            {
                wrappedModel = m_modelMap[m_fileNames[m_currentModelIndex]];
            }

            if (wrappedModel.m_model != null && wrappedModel.m_model.Valid)
            {

                //m_currentModel = m_modelReader.m_models[m_currentModelIndex];
                m_currentTextureIndex = 0;
                m_currentModelSubIndex = 0;

                //GCModel currentModel = m_currentModel;

                Lookat(wrappedModel.m_model.MinBB, wrappedModel.m_model.MaxBB);


                m_currentTextureIndex = wrappedModel.m_model.m_textures.Count - 1;
            }


            return wrappedModel.m_model != null ? wrappedModel.m_model.Valid : false;
        }


        public void Lookat(Vector3 min, Vector3 max)
        {
            Vector3 mid = new Vector3();
            //if (readDisplayLists)
            {
                mid = (max - min) / 2f;
            }

            float longest = Math.Max(mid.X, Math.Max(mid.Y, mid.Z));

            //location = model.Center;

            float val = longest * 3.5f;
            if (val < 1f)
            {
                val = 1f;
            }

            location = min + mid;

            location = location - (Vector3.UnitX * val);

            facing = 0;
            pitch = 0;


        }

        public String BuildDebugString()
        {
            BaseModel currentModel = m_currentModel.m_model;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Model : " + currentModel.m_name);
            sb.AppendLine("Bones : " + currentModel.m_bones.Count);
            sb.AppendLine("MaxVertex : " + currentModel.m_maxVertex);

            sb.AppendLine(String.Format("BB : {0:0.00000000} {1:0.00000000} {2:0.00000000}][{3:0.00000000} {4:0.00000000} {5:0.00000000}]", currentModel.MinBB.X, currentModel.MinBB.Y, currentModel.MinBB.Z, currentModel.MaxBB.X, currentModel.MaxBB.Y, currentModel.MaxBB.Z));
            sb.AppendLine(String.Format("DSL [{0}/{1}] Length [{2}] Valid[{3}] ", m_currentModelSubIndex, currentModel.m_modelMeshes.Count, currentModel.m_modelMeshes[m_currentModelSubIndex].NumIndices, currentModel.m_modelMeshes[m_currentModelSubIndex].Valid));

            sb.AppendLine("Textures : ");
            int counter = 0;
            foreach (TextureData textureData in currentModel.m_textures)
            {
                //if (!m_textureDictionary.ContainsKey(textureData.textureName))
                //{
                //    Texture2D texture;
                //    if (LoadTexture(textureData.textureName, out texture))
                //    {
                //        m_textureDictionary[textureData.textureName] = texture;
                //    }
                //}
                sb.AppendLine(textureData.textureName);
            }
            sb.AppendLine();
            sb.AppendFormat("Loc [{0:0.00000000} {1:0.00000000} {2:0.00000000}]", location.X, location.Y, location.Z);
            return sb.ToString();

        }

        public Texture2D GetTexture(Color color)
        {
            Texture2D newTexture = new Texture2D(m_graphicsDeviceManager.GraphicsDevice, 1, 1);
            Color[] colorData = new Color[1];
            colorData[0] = color;
            newTexture.SetData(colorData);
            return newTexture;
        }


        public bool LoadTexture(string filename, out Texture2D texture)
            {
                String textureFileName = textureBasePath + filename + ".jpg";
                FileInfo fileInfo = new FileInfo(textureFileName);
                if (fileInfo.Exists)
                {
                    Bitmap  i = Bitmap.FromFile(fileInfo.FullName, true) as Bitmap;
                    texture = BitmapToTexture2D(GraphicsDevice, i);
                    return true;
                }
                texture = m_missingTexture;
                return true;
            }

        public static Texture2D BitmapToTexture2D(GraphicsDevice GraphicsDevice, System.Drawing.Bitmap image)
        {
            // Buffer size is size of color array multiplied by 4 because   
            // each pixel has four color bytes  
            int bufferSize = image.Height * image.Width * 4;

            // Create new memory stream and save image to stream so   
            // we don't have to save and read file  
            System.IO.MemoryStream memoryStream =
                new System.IO.MemoryStream(bufferSize);
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Creates a texture from IO.Stream - our memory stream  
            Texture2D texture = Texture2D.FromStream(GraphicsDevice, memoryStream);

            return texture;
        }


        public Texture2D SetTexture(int index)
        {
            Texture2D texture = null;
            if (m_currentModel.m_model.m_textures.Count > 0 && index < m_currentModel.m_model.m_textures.Count)
            {
                String textureName = m_currentModel.m_model.m_textures[index].textureName;
                if (!m_textureDictionary.ContainsKey(textureName))
                {
                    LoadTexture(textureName, out texture);
                    m_textureDictionary[textureName] = texture;
                }

                texture = m_textureDictionary[textureName];

                
            }
            return texture;
        }

        public void ApplyLightToEffect(Effect effect)
        {
            Vector3 ambientLightColor = new Vector3(1f);
            float ambientLightIntensity = 0.1f;
            Vector3 directionalLightColor = new Vector3(1f);
            float directionalLightIntensity = 1f;

            Vector3 specularLightColor = new Vector3(1f);
            float specularLightIntensity = 1f;

            //effect.Parameters["AmbientLightColor"].SetValue(ambientLightColor);
            //effect.Parameters["AmbientLightIntensity"].SetValue(ambientLightIntensity);

            //effect.Parameters["DirectionalLightColor"].SetValue(directionalLightColor);
            //effect.Parameters["DirectionalLightIntensity"].SetValue(directionalLightIntensity);

            //effect.Parameters["SpecularLightColor"].SetValue(specularLightColor);
            //effect.Parameters["SpecularLightIntensity"].SetValue(specularLightIntensity);

            //effect.Parameters["LightPosition"].SetValue(location);
            //effect.Parameters["LightDirection"].SetValue(Globals.Player.Forward);
            Vector3 lightDir = new Vector3(0, 1, -1);
            lightDir.Normalize();
            //effect.Parameters["LightDirection"].SetValue(lightDir);
            //effect.Parameters["LightPosition"].SetValue(new Vector3(0, 40, 0));
            //Vector3 lightDirection = new Vector3(10, -10, 0);
            //lightDirection.Normalize();

            //effect.Parameters["LightDirection"].SetValue(lightDirection);

        }



        bool rotateX = false;
        bool rotateY = false;
        bool rotateZ = false;
        bool displayAll = true;

        int m_currentModelIndex = 0;
        int m_currentTextureIndex = 0;
        int m_currentModelSubIndex = 0;
        WrappedModel m_currentModel;

        
        BaseModelReader m_modelReader;

        //OpenTK.Graphics.TextPrinter m_textPrinter;
        String m_debugLine;

        //private readonly Font font = new Font("Verdana", 10);

        public Dictionary<String, Texture2D> m_textureDictionary = new Dictionary<String, Texture2D>();


        List<Vector3> m_points = new List<Vector3>();
    }


    public class WrappedModel
    {
        public BaseModel m_model;
        public List<WrappedDisplayListHeader> m_wrappedHeaderList = new List<WrappedDisplayListHeader>();

        public WrappedModel(BaseModel model, GraphicsDevice graphicsDevice, Effect effect)
        {
            m_model = model;
            int counter = 0;
            foreach (ModelSubMesh modelMesh in model.m_modelMeshes)
            {
                WrappedDisplayListHeader wrappedHeader = new WrappedDisplayListHeader(modelMesh, graphicsDevice, effect);
                m_wrappedHeaderList.Add(wrappedHeader);
                counter++;
            }
        }

        public void Draw()
        {

        }

    }


    public class WrappedDisplayListHeader 
    {
        public ModelSubMesh m_modelSubMesh;
        public VertexBuffer m_vertexBuffer;
        public IndexBuffer m_indexBuffer;
        public GraphicsDevice m_graphicsDevice;
        public Effect m_effect;

        //public WrappedDisplayListHeader(XboxModel model, GraphicsDevice graphicsDevice, Effect effect)
        //{
        //    VertexPositionNormalTexture[] vertexData = model.m_points.ToArray();
        //    m_vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexData.Length, BufferUsage.None);
        //    m_vertexBuffer.SetData(vertexData);

        //    ushort[] indices = model.m_indices.ToArray();
        //    m_indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Count(), BufferUsage.None);
        //    m_indexBuffer.SetData(indices);

        //}

        public WrappedDisplayListHeader(ModelSubMesh modelSubMesh, GraphicsDevice graphicsDevice, Effect effect)
        {
            m_modelSubMesh = modelSubMesh;
            m_graphicsDevice = graphicsDevice;
            m_effect = effect;

            if (modelSubMesh is DisplayListHeader)
            {
                DisplayListHeader displayListHeader = modelSubMesh as DisplayListHeader;

                VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[displayListHeader.entries.Count];
                for (int i = 0; i < displayListHeader.entries.Count; ++i)
                {
                    vertexData[i].Position = displayListHeader.Vertices[displayListHeader.entries[i].PosIndex];
                    Vector3 v = Vector3.Up;
                    if (displayListHeader.entries[i].NormIndex < displayListHeader.Normals.Count)
                    {
                        v = displayListHeader.Normals[displayListHeader.entries[i].NormIndex];
                    }
                    vertexData[i].Normal = v;
                    vertexData[i].TextureCoordinate = displayListHeader.UVs[displayListHeader.entries[i].UVIndex];
                }

                m_vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexData.Length, BufferUsage.None);
                m_vertexBuffer.SetData(vertexData);


                ushort[] indexData = new ushort[displayListHeader.entries.Count];
                for (int i = 0; i < indexData.Length; i += 3)
                {
                    indexData[i + 0] = (ushort)(i + 0);
                    indexData[i + 1] = (ushort)(i + 2);
                    indexData[i + 2] = (ushort)(i + 1);
                }
                m_indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indexData.Count(), BufferUsage.None);
                m_indexBuffer.SetData(indexData);
            }
            else if (modelSubMesh is XBoxSubMesh)
            {
                XBoxSubMesh doegSection = modelSubMesh as XBoxSubMesh;
                VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[modelSubMesh.Vertices.Count];
                for (int i = 0; i < vertexData.Length; ++i)
                {
                    vertexData[i].Position = modelSubMesh.Vertices[i];
                    vertexData[i].TextureCoordinate= modelSubMesh.UVs[i];
                    vertexData[i].Normal= modelSubMesh.Normals[i];
                }
                m_vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexData.Length, BufferUsage.None);
                m_vertexBuffer.SetData(vertexData);


                ushort[] indexData = new ushort[modelSubMesh.Indices.Count];
                for (int i = 0; i < indexData.Length; i += 3)
                {
                    //indexData[i + 0] = modelSubMesh.Indices[(i + 0)];
                    //indexData[i + 1] = modelSubMesh.Indices[(i + 1)];
                    //indexData[i + 2] = modelSubMesh.Indices[(i + 2)];
                }

                int adjustedIndexLength = (indexData.Length / 3) * 3;

                for (int i = 0; i < adjustedIndexLength; i += 3)
                {
                    indexData[i + 0] = (ushort)(i + 0);
                    indexData[i + 1] = (ushort)(i + 2);
                    indexData[i + 2] = (ushort)(i + 1);
                }



                m_indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indexData.Length, BufferUsage.None);
                m_indexBuffer.SetData(indexData);

            }
      }


    }
}
