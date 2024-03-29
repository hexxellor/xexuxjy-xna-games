﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ModelNamer
{
    public class XboxModelReader : BaseModelReader
    {


        public void LoadModels(String sourceDirectory, String infoFile, int maxFiles = -1)
        {
            m_models.Clear();
            String[] files = Directory.GetFiles(sourceDirectory, "*");
            int counter = 0;

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (String file in files)
                {
                    try
                    {
                        FileInfo sourceFile = new FileInfo(file);
                        if (sourceFile.Name != "File 005496")
                        {
                            // 410, 1939

                            //continue;
                        }

                        XboxModel model = LoadSingleModel(sourceFile.FullName) as XboxModel;


                        m_models.Add(model);

                    }
                    catch (Exception e)
                    {
                    }
                    counter++;
                    if (maxFiles > 0 && counter > maxFiles)
                    {
                        break;
                    }

                }
            }
        }

        public override BaseModel LoadSingleModel(String modelPath, bool readDisplayLists = true)
        {
            XboxModel model = null;
            FileInfo sourceFile = new FileInfo(modelPath);

            using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
            {
                model = new XboxModel(sourceFile.Name);
                //model.LoadModelTags(binReader, Common.xboxTags);
                model.LoadData(binReader);

            }
            return model;
        }


        static void Main(string[] args)
        {
            String rootPath = @"d:\gladius-extracted-archive\xbox-decompressed\";
            //rootPath = @"c:\tmp\gladius-extracted-archive\gladius-extracted-archive\xbox-decompressed\";
            String modelPath = rootPath + "ModelFilesRenamed";
            String infoFile = rootPath + "ModelInfo.txt";
            XboxModelReader reader = new XboxModelReader();
            String objOutputPath = rootPath + @"ModelFilesRenamed-FBXA\";
            //objOutputPath = rootPath + @"ModelFilesRenamed-OBJ\";
            String texturePath = @"c:\tmp\gladius-extracted-archive\gladius-extracted-archive\gc-compressed\textures.jpg\";
            texturePath = @"C:\tmp\xbox-texture-output\";

            List<string> filenames = new List<string>();

            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "**"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*armor_all*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*carafe_decanter*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*animalsk*"));
            filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\characters\", "*PropPracticePost1*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\characters\", "*PropPracticePost*"));
            filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\characters\", "*ogre*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\characters\", "*bear*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\characters\", "*amazon*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\arenas\", "*thepit*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\arenas\", "*caltha*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\arenas\", "*exuros*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\arenas\", "*valenssc*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\arenas\", "*nordagh_w*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\weapons\", "*axe*"));
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\axeCS_declamatio.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\swordM_gladius.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\swordCS_unofan.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\bow_amazon.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\bow_black.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\armor_all.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\wheel.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\arcane_water_crown.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\characters\amazon.mdl")
            //filenames.Add(rootPath + @"ModelFilesRenamed\characters\bear.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\characters\urlancinematic.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\characters\yeti.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\armband_base.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\carafe_decanter.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\carafe_carafe.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\arenas\palaceibliis.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\arenas\darkgod.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\characters\barbarian.mdl");

            foreach (string name in filenames)
            {
                if (name.Contains("worldmap"))
                {
                    //continue;
                }

                reader.m_models.Add(reader.LoadSingleModel(name));
            }

            using (StreamWriter infoSW = new StreamWriter(rootPath + "submesh-details.txt"))
            {
                foreach (XboxModel model in reader.m_models)
                {
                    if (model.m_subMeshData3 != null)
                    {
                        model.m_subMeshData3.WriteInfo(infoSW);
                    }
                    if (model.m_textures.Count != model.m_materialDataList.Count)
                    {
                        int ibreak = 0;
                    }
                    foreach (TextureData td in model.m_textures)
                    {
                        infoSW.WriteLine(td.ToString());
                    }


                    infoSW.WriteLine("Total bones : " + model.m_bones.Count);
                    foreach (BoneNode bn in model.m_bones)
                    {
                        infoSW.WriteLine(bn.ToString());
                    }
                    if (model.m_skinned)
                    {
                        foreach (XboxVertexInstance vbi in model.m_allVertices)
                        {
                            float weightSum = 0f;
                            for (int i = 0; i < 4; ++i)
                            {
                                weightSum += vbi.Weight(i);
                            }
                            if (weightSum != 1.0f)
                            {
                                int ibreak = 0;
                            }

                            infoSW.WriteLine(vbi.DumpWeight());
                        }
                    }

                    List<SubmeshData> result = model.GetIndices(new List<int>());

                    foreach (SubmeshData smi in result)
                    {
                        infoSW.WriteLine("Submesh : " + smi.index);
                        foreach (int i in smi.indices)
                        {
                            XboxVertexInstance vbi = model.m_allVertices[i];
                            infoSW.WriteLine(vbi.DumpWeight());
                        }
                        infoSW.WriteLine("**************************************");
                    }

                    int startIndex = 0;

                }
            }

            objOutputPath = rootPath + @"ModelFilesRenamed-DAE\";

            foreach (XboxModel model in reader.m_models)
            {
                try
                {
                    //foreach (ShaderData sd in model.m_shaderData)
                    //{
                    //    shadernames.Add(sd.shaderName);
                    //}
                    ////model.DumpSections(tagOutputPath);
                    //using (StreamWriter objSw = new StreamWriter(objOutputPath + model.m_name + ".obj"))
                    //{
                    //    using (StreamWriter matSw = new StreamWriter(objOutputPath + model.m_name + ".mtl"))
                    //    {
                    //        model.WriteOBJ(objSw, matSw, texturePath, -1);
                    //    }
                    //}
                    bool skinned = true;
                    //for (int i = 0; i < model.NumMeshes; ++i)
                    //{
                    int bestSkinnedLod = (1 | 2 | 4 | 8 | 16 | 32);
                    //bestSkinnedLod = 512;
                    // 256 is main part of body at lower lod.
                    // 128 is main part of body at lower lod.
                    // 64 is main part of body at lower lod.
                    // 32 is main part of body at lower lod.
                    // 16 is ears and hair for remove with helmet.
                    // 8 is top of head?
                    // 4 is eyes and front of face
                    // 2 is rest of face and teeth
                    // 1 is main part of body, but also eyes and face and most of body apart from addons?
                    bestSkinnedLod = (1);

                    string extension = ".fbx";
                    extension = ".dae";
                    //using (StreamWriter objSw = new StreamWriter(objOutputPath + model.m_name + "-" + model.m_subMeshData1List[i].LodLevel+"-" + i + ".fbx"))
                    using (StreamWriter objSw = new StreamWriter(objOutputPath + model.m_name + extension))
                    {
                        List<int> excludeList = new List<int>();
                        List<int> includeList = new List<int>();

                        if (model.m_skinned)
                        {
                            for (int j = 0; j < model.NumMeshes; ++j)
                            {
                                //if (model.m_subMeshData1List[j].LodLevel < bestSkinnedLod)
                                if ((model.m_subMeshData1List[j].LodLevel & bestSkinnedLod) != 0)
                                {
                                    includeList.Add(j);
                                }
                            }
                        }

                        //for (int i = 0; i < 7; ++i)
                        //{
                        //    includeList.Add(i);
                        //}

                        if (includeList.Count > 0)
                        {
                            for (int j = 0; j < model.NumMeshes; ++j)
                            {
                                if (!includeList.Contains(j))
                                {
                                    excludeList.Add(j);
                                }
                            }
                        }

                        //excludeList.Remove(0);
                        //excludeList.Clear();
                        //model.WriteFBXA(objSw, null, texturePath, skinned, excludeList);
                        model.WriteCollada(objSw, null, texturePath, skinned, excludeList);
                    }
                    //}

                }
                catch (System.Exception ex)
                {

                }
            }



        }

        List<BaseModel> m_models = new List<BaseModel>();
    }

    /*
     * Model Amazon
     * 
     * mesh1 : teeth
     * 
    */

    public class XboxModel : BaseModel
    {
        public List<XboxVertexInstance> m_allVertices = new List<XboxVertexInstance>();
        public List<ushort> m_allIndices = new List<ushort>();
        public List<SubMeshData1> m_subMeshData1List = new List<SubMeshData1>();
        public List<SubMeshData2> m_subMeshData2List = new List<SubMeshData2>();
        public List<MaterialData> m_materialDataList = new List<MaterialData>();
        public List<MeshMaterial> m_meshMaterialList = new List<MeshMaterial>();

        public const int s_textureBlockSize = 64;
        public const int s_materialBlockSize = 44;

        public SubMeshData3 m_subMeshData3;
        public int NumMeshes = 0;
        public int m_avgVertex;
        public XboxModel(String name)
            : base(name)
        {

        }

        public override void LoadData(BinaryReader binReader)
        {
            binReader.BaseStream.Position = 0;
            ReadSELSSection(binReader);
            binReader.BaseStream.Position = 0;
            Common.ReadNullSeparatedNames(binReader, Common.nameTag, m_boneNames);
            binReader.BaseStream.Position = 0;
            ReadTextureSection(binReader);
            binReader.BaseStream.Position = 0;
            ReadSKELSection(binReader);
            binReader.BaseStream.Position = 0;
            ReadXRNDSection(binReader);
            LoadAnimationData();
        }


        public void LoadAnimationData()
        {
            if (m_skinned)
            {
                try
                {
                    string animFilename = Common.AnimFileForModel(m_name);
                    if (!String.IsNullOrEmpty(animFilename))
                    {
                        AnimationReader animationReader = new AnimationReader();

                        using (BinaryReader binReader = new BinaryReader(new FileStream(animFilename, FileMode.Open)))
                        {
                            animationReader.Read(binReader);
                        }

                        foreach (AnimationData animation in animationReader.animations)
                        {
                            AddAnimation(animation);
                        }
                    }
                }
                catch (System.Exception ex)
                {

                }
            }
        }



        public void ReadSELSSection(BinaryReader binReader)
        {
            Common.ReadNullSeparatedNames(binReader, Common.selsTag, m_selsInfo);

        }

        public void ReadXRNDSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.xrndTag))
            {
                int sectionLength = binReader.ReadInt32();
                int uk2a = binReader.ReadInt32();
                int numEntries = binReader.ReadInt32();
                int numSkip = binReader.ReadInt32();
                binReader.BaseStream.Position += numSkip * 4;
                //int uk2c = binReader.ReadInt32();

                //int uk2d = binReader.ReadInt32();
                byte[] doegStart = binReader.ReadBytes(4);
                Debug.Assert(doegStart[0] == 'd' && doegStart[3] == 'g');
                if (doegStart[0] == 'd' && doegStart[3] == 'g')
                {
                    int doegLength = binReader.ReadInt32();
                    Debug.Assert(doegLength == 0x7c);
                    int unk1a = binReader.ReadInt32();
                    Debug.Assert(unk1a == 0x01);
                    int unk1b = binReader.ReadInt32();
                    int unk1c = binReader.ReadInt32();
                    int unk1d = binReader.ReadInt32();
                    Debug.Assert(unk1d == 0x01);

                    NumMeshes = binReader.ReadInt32();
                    int unk1e = binReader.ReadInt32();
                    int numMeshCopy = binReader.ReadInt32();
                    Debug.Assert(NumMeshes == numMeshCopy);
                    int numTextures = binReader.ReadInt32();
                    int unk1g = binReader.ReadInt32();
                    int unk1h = binReader.ReadInt32();
                    Debug.Assert(unk1h == 0x00);


                    int blockStart1 = binReader.ReadInt32();
                    Debug.Assert(blockStart1 == 0x70);
                    int blockStart2 = binReader.ReadInt32();
                    Debug.Assert(blockStart2 - blockStart1 == (NumMeshes * 4));
                    int blockStart3 = binReader.ReadInt32();
                    Debug.Assert(blockStart3 - blockStart2 == (NumMeshes * 56));
                    int blockStart4 = binReader.ReadInt32();
                    Debug.Assert(blockStart4 - blockStart3 == (NumMeshes * 20));
                    int unk1m = binReader.ReadInt32();

                    // if this is not -1 then it's some information on skinning/anims?
                    int skinIndicator = binReader.ReadInt32();


                    int unk1o = binReader.ReadInt32();
                    int unk1p = binReader.ReadInt32();
                    int unk1q = binReader.ReadInt32();

                    int minus1a = binReader.ReadInt32();
                    int minus1b = binReader.ReadInt32();
                    int minus1c = binReader.ReadInt32();
                    int minus1d = binReader.ReadInt32();

                    Debug.Assert(minus1a == -1 && minus1b == -1 && minus1c == -1 && minus1d == -1);

                    int unk1r = binReader.ReadInt32();
                    int unk1s = binReader.ReadInt32();
                    int unk1t = binReader.ReadInt32();
                    int unk1u = binReader.ReadInt32();

                    int minus1e = binReader.ReadInt32();

                    Debug.Assert(minus1a == -1 && minus1b == -1 && minus1c == -1 && minus1d == -1 && minus1e == -1);

                    // this is the number of bytes from here to the beginning of the texture names if you include the end doeg section (4)

                    int doegToTextureSize = binReader.ReadInt32();

                    byte[] doegEnd = binReader.ReadBytes(4);
                    Debug.Assert(doegEnd[0] == 'd' && doegEnd[3] == 'g');
                    int doegEndVal = (int)(binReader.BaseStream.Position - 4);

                    binReader.BaseStream.Position = doegEndVal + blockStart1;

                    List<int> blockOneValues = new List<int>();
                    for (int i = 0; i < NumMeshes; ++i)
                    {
                        blockOneValues.Add(binReader.ReadInt32());
                    }


                    for (int i = 0; i < NumMeshes; ++i)
                    {
                        SubMeshData1 smd = SubMeshData1.FromStream(binReader);
                        m_subMeshData1List.Add(smd);
                    }

                    int TotalIndices = 0;


                    //NumMeshes += 1;

                    for (int i = 0; i < NumMeshes; ++i)
                    {
                        SubMeshData2 smd = SubMeshData2.FromStream(binReader);
                        string groupName = String.Format("{0}-submesh{1}", m_name, i);
                        smd.fbxNodeId = groupName;
                        m_subMeshData2List.Add(smd);

                        TotalIndices += smd.NumIndices;
                        int val1a = smd.NumIndices * 2;
                        smd.pad = val1a % 4;
                    }

                    binReader.BaseStream.Position += 4;
                    int TotalVertices = binReader.ReadInt32();

                    // do stuff...
                    m_subMeshData3 = SubMeshData3.FromStream(this, binReader, NumMeshes, unk1m, m_skinned);


                    binReader.BaseStream.Position = doegEndVal + doegToTextureSize;

                    Common.ReadNullSeparatedNames(binReader, binReader.BaseStream.Position, numTextures, m_textureNames);

                    for (int i = 0; i < m_textureNames.Count; ++i)
                    {
                        if (!m_textureNames[i].EndsWith(".tga"))
                        {
                            m_textureNames[i] += ".tga";
                        }
                    }

                    foreach (SubMeshData2 smd in m_subMeshData2List)
                    {
                        for (int i = 0; i < smd.NumIndices; ++i)
                        {
                            ushort val = binReader.ReadUInt16();
                            smd.indices.Add(val);
                        }
                        m_allIndices.AddRange(smd.indices);
                        smd.padBytes = binReader.ReadBytes(smd.pad);
                        smd.BuildMinMax();
                    }

                    long testPosition = binReader.BaseStream.Position;

                    int skygoldIndex = -1;
                    //foreach (MaterialData materialData in m_materialDataList)
                    for (int i = 0; i < m_materialDataList.Count; ++i)
                    {
                        MaterialData materialData = m_materialDataList[i];
                        //materialData.textureId = AdjustForModel(materialData.textureId);
                        int textureIndex = materialData.diffuseTextureId / s_textureBlockSize;
                        materialData.diffuseTextureData = m_textures[textureIndex];
                        if (materialData.diffuseTextureData.textureName.Contains("skygold"))
                        {
                            skygoldIndex = i;
                        }
                    }
                    // if we have skygold then make the specular of the following it and remove from list.
                    if (skygoldIndex != -1)
                    {
                        //Debug.Assert(skygoldIndex < m_materialDataList.Count - 1);
                        if (skygoldIndex < m_materialDataList.Count - 1)
                        {
                            MaterialData skyGoldMaterial = m_materialDataList[skygoldIndex];
                            MaterialData skyGoldFollowMaterial = m_materialDataList[skygoldIndex + 1];

                            skyGoldFollowMaterial.specularTextureId = skygoldIndex;
                            skyGoldFollowMaterial.specularTextureData = m_textures[skygoldIndex];
                            m_materialDataList.RemoveAt(skygoldIndex);
                        }
                    }

                    if (m_skinned)
                    {
                        ReadSkinnedVertexData28(binReader, m_allVertices, TotalVertices);
                        //ReadSkinnedVertexData32(binReader, m_allVertices, TotalVertices);

                        int maxWeight = -1;
                        foreach (XboxVertexInstance vbi in m_allVertices)
                        {
                            vbi.BoneIndices = new short[3];
                            vbi.BoneIndices[0] = binReader.ReadInt16();
                            vbi.BoneIndices[1] = binReader.ReadInt16();
                            vbi.BoneIndices[2] = binReader.ReadInt16();

                            DebugBoneWeights(vbi.BoneIndices[0]);
                            DebugBoneWeights(vbi.BoneIndices[1]);
                            DebugBoneWeights(vbi.BoneIndices[2]);

                            for (int z = 0; z < vbi.BoneIndices.Length; ++z)
                            {
                                if (z == 6)
                                {
                                    break;
                                }
                            }
                            //vbi.Weights = binReader.ReadBytes(6);
                            //if (vbi.Weights[0] / 3 >= m_bones.Count)
                            //{
                            //    int ibreak = 0;
                            //}
                            //if (vbi.Weights[2] / 3 >= m_bones.Count)
                            //{
                            //    int ibreak = 0;
                            //}

                            //if (vbi.Weights[0] != 255 && vbi.Weights[0] % 3 !=0)
                            //{
                            //    int ibreak = 0;
                            //}

                            //if (vbi.Weights[2] != 255 && vbi.Weights[2] % 3 != 0)
                            //{
                            //    int ibreak = 0;
                            //}

                            //if (vbi.Weights[0] != 255)
                            //{
                            //    maxWeight = Math.Max(vbi.Weights[0] / 3, maxWeight);
                            //}
                            //if (vbi.Weights[2] != 255)
                            //{
                            //    maxWeight = Math.Max(vbi.Weights[2] / 3, maxWeight);
                            //}
                            //if (vbi.Weights[4] != 255)
                            //{
                            //    maxWeight = Math.Max(vbi.Weights[4] / 3, maxWeight);
                            //}


                            //if (vbi.Weights[0] / 3 >= m_bones.Count)
                            //{
                            //    int ibreak = 0;
                            //}

                        }
                        int ibreak2 = 0;
                    }
                    else
                    {

                        if (Common.FindCharsInStream(binReader, Common.endTag))
                        {
                            m_avgVertex = ((int)binReader.BaseStream.Position - 4 - (int)testPosition) / TotalVertices;
                            binReader.BaseStream.Position = testPosition;

                            //ReadUnskinnedVertexData36(binReader, m_allVertices, TotalVertices);
                            switch (m_avgVertex)
                            {
                                case 24:
                                    ReadUnskinnedVertexData24(binReader, m_allVertices, TotalVertices);
                                    break;
                                case 28:
                                    ReadUnskinnedVertexData28(binReader, m_allVertices, TotalVertices);
                                    break;
                                case 36:
                                    ReadUnskinnedVertexData36(binReader, m_allVertices, TotalVertices);
                                    break;
                                default:
                                    int ibreak = 0;
                                    break;
                            }
                        }
                    }
                    BuildBB();

                    for (int i = 0; i < m_textureNames.Count; ++i)
                    {
                        ShaderData shaderData = new ShaderData();
                        shaderData.shaderName = "test";
                        shaderData.textureId1 = (byte)i;
                        shaderData.textureId2 = (byte)(i + 1);
                        m_shaderData.Add(shaderData);
                    }
                }
                else
                {
                    //infoStream.WriteLine("Doeg not at expected - multi file? : " + sourceFile.FullName);
                }

            }

        }

        public void DebugBoneWeights(short index)
        {
            XboxVertexInstance.sBoneIndices.Add(index);
            int count = 0;
            if(!XboxVertexInstance.sBoneIndicesCount.TryGetValue(index,out count))
            {
                XboxVertexInstance.sBoneIndicesCount[index] = count;
            }
            count++;
            XboxVertexInstance.sBoneIndicesCount[index] = count;
        }

        public bool IsEnd(BinaryReader binReader)
        {
            byte[] endBlock = binReader.ReadBytes(4);
            char[] endBlockChar = new char[endBlock.Length];
            for (int i = 0; i < endBlock.Length; ++i)
            {
                endBlockChar[i] = (char)endBlock[i];
            }
            if (endBlockChar[0] == 'E' && endBlockChar[1] == 'N' && endBlock[2] == 'D')
            {
                return true;
            }
            return false;
        }

        public bool ValidVertex(Vector3 v)
        {
            return Math.Abs(v.X) < 10000 && Math.Abs(v.Y) < 10000 && Math.Abs(v.Z) < 10000;
        }

        public void WriteOBJ(StreamWriter writer, StreamWriter materialWriter, String texturePath, int desiredLod = -1)
        {
            int vertexCountOffset = 0;
            int normalCountOffset = 0;
            int uvCountOffset = 0;

            //String materialName = null;

            foreach (TextureData textureData in m_textures)
            {
                String textureName = textureData.textureName + ".png";

                materialWriter.WriteLine("newmtl " + textureName);
                materialWriter.WriteLine("Ka 1.000 1.000 1.000");
                materialWriter.WriteLine("Kd 1.000 1.000 1.000");
                materialWriter.WriteLine("Ks 0.000 0.000 0.000");
                materialWriter.WriteLine("d 1.0");
                materialWriter.WriteLine("illum 2");

                //if (m_skinned)
                //{
                //    textureName = "colorpicker_texture.png";
                //}


                materialWriter.WriteLine("map_Ka " + texturePath + textureName);
                materialWriter.WriteLine("map_Kd " + texturePath + textureName);

                //materialWriter.WriteLine("refl -type sphere -mm 0 1 clouds.mpc");
            }

            writer.WriteLine("mtllib " + m_name + ".mtl");
            int submeshCount = 0;

            //writer.WriteLine("g allObjects");

            foreach (XboxVertexInstance vpnt in m_allVertices)
            {
                writer.WriteLine(String.Format("v {0:0.00000} {1:0.00000} {2:0.00000}", vpnt.Position.X, vpnt.Position.Y, vpnt.Position.Z));
            }

            foreach (XboxVertexInstance vpnt in m_allVertices)
            {
                Vector2 uv = vpnt.UV;
                if (m_skinned)
                {
                    uv = CalcUVForWeight(vpnt);
                }

                writer.WriteLine(String.Format("vt {0:0.00000} {1:0.00000} ", uv.X, 1.0f - uv.Y));
            }

            foreach (XboxVertexInstance vpnt in m_allVertices)
            {
                writer.WriteLine(String.Format("vn {0:0.00000} {1:0.00000} {2:0.00000}", vpnt.Normal.X, vpnt.Normal.Y, vpnt.Normal.Z));
            }

            if (m_skinned)
            {
                foreach (XboxVertexInstance vpnt in m_allVertices)
                {
                    writer.WriteLine(String.Format("# weights BI2[{0}] [{1}][{2}][{3}]", vpnt.BoneWeights, vpnt.BoneIndices[0], vpnt.BoneIndices[1], vpnt.BoneIndices[2]));
                }
            }


            int startIndex = 0;

            int max = -1;
            int indices = -1;

            for (int a = 0; a < m_subMeshData2List.Count; ++a)
            {
                if (m_subMeshData2List[a].NumIndices > indices)
                {
                    indices = m_subMeshData2List[a].NumIndices;
                    max = a;
                }
            }

            int meshTextureId = -1;
            int modelCount = 0;
            int matIndex = 0;
            int adjustedIndex = 0;

            int maxMatIndex = -1;
            for (int a = 0; a < m_subMeshData2List.Count; ++a)
            {
                maxMatIndex = Math.Max(maxMatIndex, m_meshMaterialList[a].val3 / s_materialBlockSize);
            }

            int lastMatIndex = -1;
            for (int a = 0; a < m_subMeshData2List.Count; ++a)
            {
                try
                {
                    matIndex = m_meshMaterialList[a].val3 / s_materialBlockSize;
                    if (lastMatIndex != matIndex)
                    {
                        lastMatIndex = matIndex;
                    }
                    MaterialData materialData = m_materialDataList[matIndex];
                    adjustedIndex = materialData.diffuseTextureId / s_textureBlockSize;
                    if (adjustedIndex == meshTextureId)
                    {
                        modelCount++;
                    }
                }
                catch (Exception e)
                {
                    int ibreak = 0;
                }
            }

            for (int a = 0; a < m_subMeshData2List.Count; ++a)
            {
                SubMeshData2 headerBlock = m_subMeshData2List[a];
                SubMeshData1 data1 = m_subMeshData1List[a];

                //FindSmallest mesh with max indixes to test against textures
                if (a != max)
                {

                    //inue;
                }

                try
                {

                    submeshCount++;

                    //if (data1.LodLevel != 0 && ((data1.LodLevel & desiredLod) == 0))
                    //{
                    //    continue;
                    //}
                    matIndex = m_meshMaterialList[a].val3 / s_materialBlockSize;
                    MaterialData materialData = m_materialDataList[matIndex];
                    adjustedIndex = materialData.diffuseTextureId / s_textureBlockSize;


                    if (meshTextureId != -1 && adjustedIndex != meshTextureId)
                    {
                        startIndex += headerBlock.NumIndices;
                        continue;
                    }

                    if (adjustedIndex > 14)
                    {
                        startIndex += headerBlock.NumIndices;
                        continue;
                    }


                    adjustedIndex = AdjustForModel(adjustedIndex);


                    string groupName = String.Format("{0}-submesh{1}-LOD{2}", m_name, submeshCount, data1.LodLevel);

                    writer.WriteLine("o " + groupName);

                    materialData.diffuseTextureData = m_textures[adjustedIndex];
                    string adjustedTexture = materialData.diffuseTextureData.textureName;
                    String materialName = adjustedTexture + ".png";

                    writer.WriteLine("usemtl " + materialName);
                    bool swap = false;
                    //for (int i = 0; i < headerBlock.Indices.Count - 2; i++)
                    //int start = headerBlock.StartOffset / 2;
                    int end = startIndex + headerBlock.NumIndices - 2;

                    for (int i = startIndex; i < end; i++)
                    {
                        int index1 = i;
                        int index2 = i + 1;
                        int index3 = i + 2;
                        if (index3 >= m_allIndices.Count)
                        {
                            index3 = index1;
                        }
                        if (index2 >= m_allIndices.Count)
                        {
                            index2 = index1;
                        }
                        if (i >= m_allIndices.Count)
                        {
                            int ibreak = 0;
                        }

                        int i1 = m_allIndices[index1];
                        int i2 = m_allIndices[index2];
                        int i3 = m_allIndices[index3];

                        // 1 based.
                        i1 += 1;
                        i2 += 1;
                        i3 += 1;

                        // alternate winding
                        if (swap)
                        {
                            writer.WriteLine(String.Format("f {0}/{1}/{2}  {3}/{4}/{5} {6}/{7}/{8}", i3, i3, i3, i2, i2, i2, i1, i1, i1));
                        }
                        else
                        {
                            writer.WriteLine(String.Format("f {0}/{1}/{2}  {3}/{4}/{5} {6}/{7}/{8}", i1, i1, i1, i2, i2, i2, i3, i3, i3));
                        }
                        swap = !swap;
                    }
                    startIndex += headerBlock.NumIndices;

                }
                catch (Exception e)
                {
                    int ibreak = 0;
                }
                //break;
            }
        }

        public List<SubmeshData> GetIndices(List<int> excludeList)
        {
            List<SubmeshData> result = new List<SubmeshData>();
            bool swap = false;
            int startIndex = 0;
            for (int a = 0; a < m_subMeshData2List.Count; ++a)
            {
                SubMeshData2 headerBlock = m_subMeshData2List[a];
                SubMeshData1 data1 = m_subMeshData1List[a];
                
                int end = startIndex + headerBlock.NumIndices - 2;
                if (excludeList.Contains(a))
                {
                    startIndex += headerBlock.NumIndices;
                    continue;
                }

                SubmeshData smi = new SubmeshData();
                smi.index = a;
                smi.subMeshData = data1;
                smi.indices = new List<int>();
                //smi.vertices = new List<XboxVertexInstance>();
                List<int> indexList = smi.indices;

                for (int i = startIndex; i < end; i++)
                {
                    int index1 = i;
                    int index2 = i + 1;
                    int index3 = i + 2;
                    if (index3 >= m_allIndices.Count)
                    {
                        index3 = index1;
                    }
                    if (index2 >= m_allIndices.Count)
                    {
                        index2 = index1;
                    }
                    if (i >= m_allIndices.Count)
                    {
                        int ibreak = 0;
                    }

                    int i1 = m_allIndices[swap ? index1 : index3];
                    int i2 = m_allIndices[index2];
                    int i3 = m_allIndices[swap ? index3 : index1];

                    indexList.Add(i1);
                    indexList.Add(i2);
                    indexList.Add(i3);

                    if (!smi.verticesInMesh.Contains(i1))
                    {
                        smi.verticesInMesh.Add(i1);
                    }

                    if (!smi.verticesInMesh.Contains(i2))
                    {
                        smi.verticesInMesh.Add(i2);
                    }

                    if (!smi.verticesInMesh.Contains(i3))
                    {
                        smi.verticesInMesh.Add(i3);
                    }

                    swap = !swap;
                }
                smi.verticesInMesh.Sort();
                result.Add(smi);
                startIndex += headerBlock.NumIndices;
            }
            return result;
        }


        public void WriteCollada(StreamWriter writer, StreamWriter materialWriter, String texturePath, bool skinned = false, List<int> excludeList = null)
        {
            writer.WriteLine("<COLLADA xmlns=\"http://www.collada.org/2005/11/COLLADASchema\" version=\"1.4.1\">");
            writer.WriteLine("<asset>");
            writer.WriteLine("<unit meter=\"1.0\" name=\"meter\"></unit><up_axis>Y_UP</up_axis>");
            writer.WriteLine("</asset>");


            string materialId = "";

            writer.WriteLine("<library_materials>");
            foreach (TextureData texture in m_textures)
            {
                materialId = texture.textureName;


                writer.WriteLine(String.Format("<material id=\"{0}\" name=\"{1}\">", materialId, materialId));
                writer.WriteLine(String.Format("<instance_effect url=\"#{0}\" />", materialId + "-fx"));
                writer.WriteLine("</material>");
            }

            writer.WriteLine("</library_materials>");
            writer.WriteLine("<library_images>");
            foreach (TextureData texture in m_textures)
            {
                materialId = texture.textureName;

                writer.WriteLine(String.Format("<image id=\"{0}\" name=\"{1}\">", materialId + "-img", materialId));
                writer.WriteLine(String.Format("<init_from>file://{0}</init_from>", texturePath + materialId + ".png"));
                writer.WriteLine("</image>");
            }

            writer.WriteLine("</library_images>");

            writer.WriteLine("<library_effects>");

            //string positionTag = "_Position";
            //string normalTag = "_Normal";
            //string uvTag = "_UV";

            //string geometryLibId = meshName + "-lib";
            string firstMesh = "Mesh0";

            foreach (TextureData texture in m_textures)
            {
                materialId = texture.textureName;

                writer.WriteLine(String.Format("<effect id=\"{0}\" name=\"{1}\">", materialId + "-fx", texture.textureName));

                using (StreamReader sr = new StreamReader("ColladaMaterialDefinition.txt"))
                {
                    string line = sr.ReadToEnd();
                    line = line.Replace("REPLACEME-TEXTURE", materialId + "-img");
                    line = line.Replace("REPLACEME-SAMPLER", materialId + "-sampler");
                    line = line.Replace("REPLACEME-SURFACE", materialId + "-surface");
                    line = line.Replace("REPLACEME-UVSET", firstMesh+"-map1");
                    writer.Write(line);
                }
                writer.WriteLine("</effect>");
            }

            writer.WriteLine("</library_effects>");
            writer.WriteLine("<library_geometries>");

            List<SubmeshData> indexList = GetIndices(excludeList);

            for (int i = 0; i < indexList.Count; ++i)
            {
                string meshName = "Mesh" + i;

                writer.WriteLine(String.Format("<geometry id=\"{0}\" name=\"{1}\">", meshName, meshName));
                writer.WriteLine("<mesh>");

                //if (i == 0)
                {

                    writer.WriteLine(String.Format("<source id=\"{0}-positions\" name=\"position\">", meshName));
                    writer.WriteLine(String.Format("<float_array id=\"{0}-positions-array\" count=\"{1}\">", meshName, indexList[i].verticesInMesh.Count * 3));
                    //foreach (XboxVertexInstance vpnt in m_allVertices)
                    for(int j=0;j<indexList[i].verticesInMesh.Count;++j)
                    {
                        XboxVertexInstance vpnt = m_allVertices[indexList[i].verticesInMesh[j]];
                        writer.WriteLine(String.Format("{0:0.00000} {1:0.00000} {2:0.00000}", vpnt.Position.X, vpnt.Position.Y, vpnt.Position.Z));
                    }
                    writer.WriteLine("</float_array>");
                    WriteCommonTechnique(writer, "#" + meshName + "-positions-array", indexList[i].verticesInMesh.Count, 3);

                    writer.WriteLine("</source>");
                    writer.WriteLine(String.Format("<source id=\"{0}-normals\">", meshName));
                    writer.WriteLine(String.Format("<float_array id=\"{0}-normals-array\" count=\"{1}\">", meshName, indexList[i].verticesInMesh.Count * 3));
                    for (int j = 0; j < indexList[i].verticesInMesh.Count; ++j)
                    {
                        XboxVertexInstance vpnt = m_allVertices[indexList[i].verticesInMesh[j]];
                        writer.WriteLine(String.Format("{0:0.00000} {1:0.00000} {2:0.00000}", vpnt.Normal.X, vpnt.Normal.Y, vpnt.Normal.Z));
                    }
                    writer.WriteLine("</float_array>");
                    WriteCommonTechnique(writer, "#" + meshName + "-normals-array", indexList[i].verticesInMesh.Count, 3);
                    writer.WriteLine("</source>");

                    writer.WriteLine("<source id=\"{0}-map1\">", meshName);
                    writer.WriteLine("<float_array id=\"{0}-map1-array\" count=\"{1}\">", meshName, indexList[i].verticesInMesh.Count* 2);
                    for (int j = 0; j < indexList[i].verticesInMesh.Count; ++j)
                    {
                        XboxVertexInstance vpnt = m_allVertices[indexList[i].verticesInMesh[j]];
                        writer.WriteLine("{0:0.00000} {1:0.00000}", vpnt.UV.X, 1.0f - vpnt.UV.Y);
                    }
                    writer.WriteLine("</float_array>");
                    WriteCommonTechnique(writer, "#" + meshName + "-map1-array", indexList[i].verticesInMesh.Count, 2);
                    writer.WriteLine("</source>");
                }

                writer.WriteLine("<vertices id=\"{0}-vertices\">", meshName);
                writer.WriteLine("<input semantic=\"POSITION\" source=\"#{0}-positions\"/>", meshName);
                writer.WriteLine("<input semantic=\"NORMAL\" source=\"#{0}-normals\"/>", meshName);
                writer.WriteLine("<input semantic=\"TEXCOORD\" source=\"#{0}-map1\"/>", meshName);
                writer.WriteLine("</vertices>");

                //WriteColladaTriStrips(writer, materialId, meshName);
                int matIndex = 0;
                int lastMatIndex = 0;
                int modelCount = 0;
                int meshTextureId = 0;
                //List<int> indexList = BuildIndexList2L(false, excludeList);

                //for (int a = 0; a < m_subMeshData2List.Count; ++a)
                {
                    try
                    {
                        //int adjustedIndex = GetTextureId(indexList[i].index);
                        int adjustedIndex = TextureForSubmesh(indexList[i].index);
                        if (adjustedIndex >= m_textureNames.Count)
                        {
                            int ibreak = 0;
                        }
                        string textureName = m_textureNames[adjustedIndex];

                        WriteColladaTriangles(writer, textureName, meshName, indexList[i]);

                    }
                    catch (Exception e)
                    {
                        int ibreak = 0;
                    }
                }


                writer.WriteLine("</mesh>");

                writer.WriteLine("</geometry>");


            }
            writer.WriteLine("</library_geometries>");

            writer.WriteLine("<library_controllers>");
            for (int i = 0; i < indexList.Count; ++i)
            {
                string meshName = "Mesh" + i;

                writer.WriteLine("<controller id=\"{0}-skin\" name=\"skinCluster1\">", meshName);
                writer.WriteLine("<skin source=\"#{0}\">", meshName);
                WriteMatrix(writer, "bind_shape_matrix", "",Matrix.Identity);


                WriteJointsAndPoses(writer, m_rootBone, meshName);
                WriteJointVertexWeight(writer, m_rootBone, meshName, indexList[i]);
                writer.WriteLine("</skin>");
                writer.WriteLine("</controller>");
            }

            writer.WriteLine("</library_controllers>");

            writer.WriteLine("<library_visual_scenes>");
            writer.WriteLine("<visual_scene id=\"VisualSceneNode\" name=\"bind_sample\">");
            
            if (m_skinned)
            {
                WriteColladaSkeleton(writer, m_rootBone);
            }

            for (int i = 0; i < indexList.Count; ++i)
            {
                string meshName = "Mesh" + i;
                writer.WriteLine(String.Format("<node name=\"{0}Mesh\" id=\"{1}Mesh\" type=\"NODE\">", meshName, meshName));

                //writer.WriteLine("<matrix sid=\"matrix\">1.000000 0.000000 0.000000 0.000000 0.000000 1.000000 0.000000 0.000000 0.000000 0.000000 1.000000 0.000000 0.000000 0.000000 0.000000 1.000000</matrix>");
                //WriteMatrix(writer, "matrix", Matrix.Identity);
                if (m_skinned)
                {

                    writer.WriteLine("<instance_controller url=\"#" + meshName + "-skin\">");
                    writer.WriteLine("<skeleton>#{0}</skeleton>", m_rootBone.name);
                }
                else
                {
                    writer.WriteLine("<instance_geometry url=\"#" + meshName + "\">");
                }

                writer.WriteLine("<bind_material>");
                writer.WriteLine("<technique_common>");
                foreach (string textureId in m_textureNames)
                {
                    writer.WriteLine(String.Format("<instance_material symbol=\"{0}\" target=\"#{1}\"/>", textureId, textureId));
                }
                writer.WriteLine("</technique_common>");
                writer.WriteLine("</bind_material>");

                if (m_skinned)
                {
                    writer.WriteLine("</instance_controller>");
                }
                else
                {
                    writer.WriteLine("</instance_geometry>");
                }


                writer.WriteLine("</node>");
            }
            writer.WriteLine("</visual_scene>");

            writer.WriteLine("</library_visual_scenes>");
            if (m_animationsList.Count > 0)
            {
                writer.WriteLine("<library_animations>");
                foreach (AnimationData animationData in m_animationsList)
                {
                    WriteAnimation(writer, animationData);
                    break;
                }
                writer.WriteLine("</library_animations>");
            }

            writer.WriteLine("<scene>");
            writer.WriteLine("<instance_visual_scene url=\"#VisualSceneNode\"></instance_visual_scene>");
            writer.WriteLine("</scene>");

            writer.WriteLine("</COLLADA>");
        }

        public void WriteMatrix(StreamWriter writer,string name,string sid,Matrix matrix)
        {
            if (!String.IsNullOrEmpty(sid))
            {
                writer.Write("<{0} sid=\"matrix\"> ", name);
            }
            else
            {
                writer.Write("<{0}> ", name);
            }
            WriteMatrixData(writer, matrix);
            writer.WriteLine("</{0}>",name);
        }

        public void WriteMatrixData(StreamWriter writer, Matrix matrix)
        {
            //writer.Write("{0:0.0000} {1:0.0000} {2:0.0000} 0.0000 ", matrix.Left.X, matrix.Left.Y, matrix.Left.Z);
            //writer.Write("{0:0.0000} {1:0.0000} {2:0.0000} 0.0000 ", matrix.Up.X, matrix.Up.Y, matrix.Up.Z);
            //writer.Write("{0:0.0000} {1:0.0000} {2:0.0000} 0.0000 ", matrix.Forward.X, matrix.Forward.Y, matrix.Forward.Z);
            //writer.Write("{0:0.0000} {1:0.0000} {2:0.0000} 0.0000 ", matrix.Translation.X, matrix.Translation.Y, matrix.Translation.Z);

            writer.WriteLine("{0:0.0000} {1:0.0000} {2:0.0000} {3:0.0000} ", matrix.Right.X, matrix.Up.X, matrix.Backward.X, matrix.Translation.X);
            writer.WriteLine("{0:0.0000} {1:0.0000} {2:0.0000} {3:0.0000} ", matrix.Right.Y, matrix.Up.Y, matrix.Backward.Y, matrix.Translation.Y);
            writer.WriteLine("{0:0.0000} {1:0.0000} {2:0.0000} {3:0.0000} ", matrix.Right.Z, matrix.Up.Z, matrix.Backward.Z, matrix.Translation.Z);
            writer.WriteLine("{0:0.0000} {1:0.0000} {2:0.0000} {3:0.0000} ", 0.0f, 0.0f, 0.0f, 1.0f);

        }

        public void WriteJointsAndPoses(StreamWriter writer, BoneNode boneNode, String meshId)
        {
            writer.WriteLine("<source id=\"{0}-skin-joints\">", meshId);
            writer.WriteLine("<Name_array id=\"{0}-skin-joints-array\" count=\"{1}\">", meshId, m_bones.Count);
            foreach (BoneNode node in m_bones)
            {
                writer.Write("{0} ", node.name);
            }
            writer.WriteLine("</Name_array>");
            writer.WriteLine("<technique_common>");
            writer.WriteLine("<accessor source=\"#{0}-skin-joints-array\" count=\"{1}\" stride=\"1\">", meshId, m_bones.Count);
            writer.WriteLine("<param name=\"JOINT\" type=\"Name\"/>");
            writer.WriteLine("</accessor>");
            writer.WriteLine("</technique_common>");

            writer.WriteLine("</source>");
            writer.WriteLine("<source id=\"{0}-skin-bind_poses\">", meshId);
            writer.WriteLine("<float_array id=\"{0}-skin-bind_poses-array\" count=\"{1}\">",meshId,m_bones.Count * 16);
            foreach (BoneNode node in m_bones)
            {
                // FIXME - write a converter so that weights are taken from appropriate bone in skeleton?
                // need to check again correlation between skeleton bone names, and anim ones.
                // still doesn't explain why i have to swap parts for breastplate and helmet..
                WriteMatrixData(writer, Matrix.Invert(node.finalMatrix));
                //WriteMatrixData(writer, node.finalMatrix);
                //WriteMatrixData(writer, Matrix.Identity);
                writer.WriteLine();
            }
            writer.WriteLine("</float_array>");
            writer.WriteLine("<technique_common>");
            writer.WriteLine("<accessor source=\"#{0}-skin-bind_poses-array\" count=\"{1}\" stride=\"16\">", meshId, m_bones.Count);
            writer.WriteLine("<param name=\"TRANSFORM\" type=\"float4x4\"/>");
            writer.WriteLine("</accessor>");
            writer.WriteLine("</technique_common>");

            writer.WriteLine("</source>");


        }


        public void WriteJointVertexWeight(StreamWriter writer, BoneNode boneNode,String meshId,SubmeshData smd)
        {
            //if (first)
            {
                writer.WriteLine("<source id=\"{0}-skin-weights\">", meshId);
                int count = 0;
                for (int i = 0; i < smd.verticesInMesh.Count; ++i)
                {
                    for (int j = 0; j < m_allVertices[smd.verticesInMesh[i]].ActiveWeights(); ++j)
                    {
                        count++;
                    }
                }


                writer.WriteLine("<float_array id=\"{0}-skin-weights-array\" count=\"{1}\">", meshId, count);
                for (int i = 0; i < smd.verticesInMesh.Count; ++i)
                {
                    for (int j = 0; j < m_allVertices[smd.verticesInMesh[i]].ActiveWeights(); ++j)
                    {
                           writer.Write("{0:0.0000} ", m_allVertices[smd.verticesInMesh[i]].Weight(j));
                    }
                }
                writer.WriteLine("</float_array>");

                writer.WriteLine("<technique_common>");
                writer.WriteLine("<accessor source=\"#{0}-skin-weights-array\" count=\"{1}\" stride=\"1\">", meshId, count);
                writer.WriteLine("<param name=\"WEIGHT\" type=\"float\"/>");
                writer.WriteLine("</accessor>");
                writer.WriteLine("</technique_common>");
                writer.WriteLine("</source>");
            }

            String referenceMeshId = meshId;// "Mesh0";

            writer.WriteLine("<joints>");
            writer.WriteLine("<input semantic=\"JOINT\" source=\"#{0}-skin-joints\"/>", referenceMeshId);
            writer.WriteLine("<input semantic=\"INV_BIND_MATRIX\" source=\"#{0}-skin-bind_poses\"/>", referenceMeshId);
            writer.WriteLine("</joints>");

            writer.WriteLine("<vertex_weights count=\"{0}\">", smd.verticesInMesh.Count);
            writer.WriteLine("<input semantic=\"JOINT\" offset=\"0\" source=\"#{0}-skin-joints\"/>", referenceMeshId);
            writer.WriteLine("<input semantic=\"WEIGHT\" offset=\"1\" source=\"#{0}-skin-weights\"/>", referenceMeshId);
            writer.WriteLine("<vcount>");
            // up to ?3 weights per vertex
            for (int i = 0; i < smd.verticesInMesh.Count; ++i)
            {
                writer.Write(m_allVertices[smd.verticesInMesh[i]].ActiveWeights() + " ");
            }

            writer.WriteLine("</vcount>");
            writer.WriteLine("<v>");
            int weightCount = 0;
            for (int i = 0; i < smd.verticesInMesh.Count; ++i)
            {
                for (int j = 0; j < m_allVertices[smd.verticesInMesh[i]].ActiveWeights(); ++j)
                {
                    int boneIndex = XboxVertexInstance.AdjustBone(m_allVertices[smd.verticesInMesh[i]].BoneIndices[j]);
                    //if (meshIndex == 0)
                    //{
                    //    if (boneIndex == 2)
                    //    {
                    //        boneIndex = 1;
                    //    }
                    //    else
                    //    {
                    //        boneIndex = 2;
                    //    }
                    //}
                    writer.Write("{0} {1} ", boneIndex, weightCount);
                    weightCount++;
                }
            }
            writer.WriteLine("</v>");
            writer.WriteLine("</vertex_weights>");

        }

        public void WriteColladaSkeleton(StreamWriter writer, BoneNode boneNode)
        {
            writer.WriteLine("<node id=\"{0}\" name=\"{0}\" sid=\"{0}\" type=\"JOINT\">",boneNode.name);
            WriteMatrix(writer, "matrix","matrix",boneNode.bindPoseMatrix);
            foreach (BoneNode child in boneNode.children)
            {
                WriteColladaSkeleton(writer, child);
            }
            writer.WriteLine("</node>");
        }

        public void WriteColladaTriStrips(StreamWriter writer, String materialId, String meshName)
        {
            writer.WriteLine(String.Format("<tristrips count=\"{0}\" material=\"{1}\">", m_allIndices.Count, materialId));
            writer.WriteLine(String.Format("<input semantic=\"VERTEX\" offset=\"0\" source=\"#{0}\"/>", meshName + "VERTEX"));
            writer.WriteLine("<p>");
            foreach (int index in m_allIndices)
            {
                writer.Write("" + index + " ");
            }
            writer.WriteLine("</p>");
            writer.WriteLine("</tristrips>");

        }

        public void WriteColladaTriangles(StreamWriter writer, String materialId, String meshName, SubmeshData smi)
        {
            List<int> indices = smi.indices;

            writer.WriteLine(String.Format("<triangles count=\"{0}\" material=\"{1}\">", indices.Count/3, materialId));
            writer.WriteLine(String.Format("<input semantic=\"VERTEX\" offset=\"0\" source=\"#{0}-vertices\"/>", meshName));
            writer.WriteLine("<p>");
            int index1 = 0;
            int index2 = 0;
            int index3 = 0;
            int i1 = 0;
            int i2 = 0;
            int i3 = 0;
            try
            {
                int startIndex = 0;
                int lowestIndex = Math.Min(indices[0], Math.Min(indices[1], indices[2]));

                //int endIndex = indices.Count - 2;
                int endIndex = indices.Count;
                for (int i = startIndex; i < endIndex; i+=3)
                {
                    index1 = i;
                    index2 = i + 1;
                    index3 = i + 2;
                    index1 = Math.Min(index1, m_allIndices.Count - 1);
                    index2 = Math.Min(index2, m_allIndices.Count - 1);
                    index3 = Math.Min(index3, m_allIndices.Count - 1);


                    i1 = indices[index1];
                    i2 = indices[index2];
                    i3 = indices[index3];

                    if (i1 - lowestIndex < 0)
                    {
                        int ibreak = 0;
                    }

                    if (i2 - lowestIndex < 0)
                    {
                        int ibreak = 0;
                    }
                    if (i3 - lowestIndex < 0)
                    {
                        int ibreak = 0;
                    }

                    i1 -= lowestIndex;
                    i2 -= lowestIndex;
                    i3 -= lowestIndex;

                    writer.WriteLine(String.Format("{0} {1} {2}", i1, i2, i3));
                }
            }
            catch (Exception e)
            {
                int ibreak = 0;
            }
            finally
            {
                writer.WriteLine("</p>");
                writer.WriteLine("</triangles>");
            }

        }

        public void WriteCommonTechnique(StreamWriter writer, string source, int count, int stride)
        {
            writer.WriteLine("<technique_common>");
            writer.WriteLine(String.Format("<accessor source=\"{0}\" count=\"{1}\" stride=\"{2}\">", source, count, stride));
            if (stride == 2)
            {
                writer.WriteLine("<param name=\"S\" type=\"float\"/>");
                writer.WriteLine("<param name=\"T\" type=\"float\"/>");
            }
            else if (stride == 3)
            {
                writer.WriteLine("<param name=\"X\" type=\"float\"/>");
                writer.WriteLine("<param name=\"Y\" type=\"float\"/>");
                writer.WriteLine("<param name=\"Z\" type=\"float\"/>");
            }
            writer.WriteLine("</accessor>");
            writer.WriteLine("</technique_common>");
        }

        public void WriteEffect(StreamWriter writer)
        {

        }


        public void WriteAnimation(StreamWriter writer, AnimationData animation)
        {
            writer.WriteLine("<animation name=\"{0}\" id=\"{1}\">",animation.animationName,animation.animationName);

            // ugly way of building all the anim data.
            foreach (BoneNode bone in m_bones)
            {
                bone.ResetFrameValues();
            }

            m_currentAnimation = animation;
            
            while (m_animTime < animation.TrueLength())
            {
                UpdateAnimation();
            }


            int numBonesToUse = m_bones.Count;
            for(int i=0;i<m_bones.Count;++i)
            {
                BoneNode bone = m_bones[i];
                if (i > numBonesToUse)
                {
                    continue;
                }
                // input timestep
                writer.WriteLine("<source id=\"{0}-matrix-input\">", bone.name);
                writer.WriteLine("<float_array id=\"{0}-matrix-input-array\" count=\"{1}\">", bone.name, animation.timeStepList.Count());
                foreach (float f in animation.timeStepList)
                {
                    writer.Write("{0:0.0000} ", f);
                }
                writer.WriteLine("</float_array>");
                writer.WriteLine("<technique_common>");
                writer.WriteLine(String.Format("<accessor source=\"#{0}-matrix-input-array\" count=\"{1}\" stride=\"{2}\">", bone.name, animation.timeStepList.Count, 1));
                writer.WriteLine("<param name=\"TIME\" type=\"float\"/>");
                writer.WriteLine("</accessor>");
                writer.WriteLine("</technique_common>");
                writer.WriteLine("</source>");

                writer.WriteLine("<source id=\"{0}-matrix-output\">", bone.name);
                writer.WriteLine("<float_array id=\"{0}-matrix-output-array\" count=\"{1}\">", bone.name, animation.timeStepList.Count()*16);
                //foreach (Matrix m in bone.timeMatrices)
                foreach (Matrix m in bone.timeMatrices)
                {
                    //WriteMatrixData(writer, m);
                    WriteMatrixData(writer, Matrix.Invert(m));
                }

                writer.WriteLine("</float_array>");
                writer.WriteLine("<technique_common>");
                writer.WriteLine(String.Format("<accessor source=\"#{0}-matrix-output-array\" count=\"{1}\" stride=\"{2}\">", bone.name, animation.timeStepList.Count, 16));

                writer.WriteLine("<param name=\"TRANSFORM\" type=\"float4x4\"/>");
                writer.WriteLine("</accessor>");
                writer.WriteLine("</technique_common>");
                writer.WriteLine("</source>");

                writer.WriteLine("<source id=\"{0}-matrix-interpolation\">", bone.name);
                writer.WriteLine("<Name_array id=\"{0}-matrix-interpolation-array\" count=\"{1}\">", bone.name, animation.timeStepList.Count());
                foreach (float f in animation.timeStepList)
                {
                    writer.Write("LINEAR ");    
                }

                writer.WriteLine("</Name_array>");
                writer.WriteLine("<technique_common>");
                writer.WriteLine(String.Format("<accessor source=\"#{0}-matrix-interpolation-array\" count=\"{1}\" stride=\"{2}\">", bone.name, animation.timeStepList.Count, 1));
                writer.WriteLine("<param name=\"INTERPOLATION\" type=\"name\"/>");
                writer.WriteLine("</accessor>");
                writer.WriteLine("</technique_common>");
                writer.WriteLine("</source>");


            }

            for (int i = 0; i < m_bones.Count; ++i)
            {
                BoneNode bone = m_bones[i];
                if (i > numBonesToUse)
                {
                    continue;
                }
                writer.WriteLine("<sampler id=\"{0}-matrix-animation-transform\">", bone.name);
                writer.WriteLine("<input semantic=\"INPUT\" source=\"#{0}-matrix-input\"/>",bone.name);
                writer.WriteLine("<input semantic=\"OUTPUT\" source=\"#{0}-matrix-output\"/>", bone.name);
                writer.WriteLine("<input semantic=\"INTERPOLATION\" source=\"#{0}-matrix-interpolation\"/>", bone.name);
                writer.WriteLine("</sampler>");

            }

            for (int i = 0; i < m_bones.Count; ++i)
            {
                BoneNode bone = m_bones[i];
                if (i > numBonesToUse)
                {
                    continue;
                }
                writer.WriteLine("<channel source=\"#{0}-matrix-animation-transform\" target=\"{0}/matrix\"/>", bone.name);
            }            
            
            writer.WriteLine("</animation>");
        }



        public Vector2 CalcUVForWeight(XboxVertexInstance vbi)
        {
            Vector2 result = new Vector2();
            if (vbi.BoneIndices[0] == 0)
            {
                result.X = 0.25f;
            }
            if (vbi.BoneIndices[0] == 3)
            {
                result.X = 0.5f;
            }
            if (vbi.BoneIndices[0] == 6)
            {
                result.X = 0.75f;
            }
            if (vbi.BoneIndices[0] == 9)
            {
                result.X = 1.0f;
            }


            if (vbi.BoneIndices[1] == 0)
            {
                result.Y = 0.25f;
            }
            if (vbi.BoneIndices[1] == 3)
            {
                result.Y = 0.5f;
            }
            if (vbi.BoneIndices[1] == 6)
            {
                result.Y = 0.75f;
            }
            if (vbi.BoneIndices[1] == 9)
            {
                result.Y = 1.0f;
            }


            return result;

        }


        public void ReadUnskinnedVertexData24(BinaryReader binReader, List<XboxVertexInstance> allVertices, int numVertices)
        {
            for (int i = 0; i < numVertices; ++i)
            //while(true)
            {
                try
                {
                    if (IsEnd(binReader))
                    {
                        break;
                    }
                    else
                    {
                        binReader.BaseStream.Position -= 4;
                    }

                    // 24 bytes per entry? , or 28...
                    Vector3 p = Common.FromStreamVector3(binReader);
                    int normal = binReader.ReadInt32();
                    Vector3 normV = UncompressNormal(normal);
                    Vector2 u = Common.FromStreamVector2(binReader);
                    XboxVertexInstance vpnt = new XboxVertexInstance();
                    vpnt.Position = p;
                    vpnt.UV = u;
                    vpnt.Normal = normV;
                    allVertices.Add(vpnt);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }
            int ibreak2 = 0;
        }


        public void ReadUnskinnedVertexData28(BinaryReader binReader, List<XboxVertexInstance> allVertices, int numVertices)
        {
            for (int i = 0; i < numVertices; ++i)
            //while(true)
            {
                try
                {
                    if (IsEnd(binReader))
                    {
                        break;
                    }
                    else
                    {
                        binReader.BaseStream.Position -= 4;
                    }

                    // 24 bytes per entry? , or 28...
                    Vector3 p = Common.FromStreamVector3(binReader);
                    Vector3 normV = UncompressNormal(binReader.ReadInt32());
                    //Vector3 normU = UncompressNormal(binReader.ReadInt32());
                    int unk1 = binReader.ReadInt32();
                    Vector2 u = Common.FromStreamVector2(binReader);
                    XboxVertexInstance vpnt = new XboxVertexInstance();
                    vpnt.Position = p;
                    vpnt.UV = u;
                    vpnt.Normal = normV;
                    vpnt.ExtraData = -1;
                    allVertices.Add(vpnt);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }
            int ibreak2 = 0;
        }

        public void ReadUnskinnedVertexData36(BinaryReader binReader, List<XboxVertexInstance> allVertices, int numVertices)
        {
            for (int i = 0; i < numVertices; ++i)
            //while(true)
            {
                try
                {
                    if (IsEnd(binReader))
                    {
                        break;
                    }
                    else
                    {
                        binReader.BaseStream.Position -= 4;
                    }

                    // 24 bytes per entry? , or 28...
                    Vector3 p = Common.FromStreamVector3(binReader);
                    Vector3 normV = UncompressNormal(binReader.ReadInt32());
                    int unk1 = binReader.ReadInt32();
                    Vector2 u = Common.FromStreamVector2(binReader);
                    Vector2 u2 = Common.FromStreamVector2(binReader);
                    XboxVertexInstance vpnt = new XboxVertexInstance();
                    vpnt.Position = p;
                    vpnt.UV = u;
                    vpnt.UV2 = u2;
                    vpnt.Normal = normV;
                    vpnt.ExtraData = unk1;
                    allVertices.Add(vpnt);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }
            int ibreak2 = 0;
        }


        public void ReadSkinnedVertexData28(BinaryReader binReader, List<XboxVertexInstance> allVertices, int numVertices)
        {
            for (int i = 0; i < numVertices; ++i)
            {
                try
                {
                    XboxVertexInstance vpnt = new XboxVertexInstance();
                    vpnt.Position = Common.FromStreamVector3(binReader);
                    //vpnt.BoneInfo1 = binReader.ReadInt32();
                    vpnt.Normal = UncompressNormal(binReader.ReadInt32());
                    //int unk1 = binReader.ReadInt32();
                    vpnt.UV = Common.FromStreamVector2(binReader);
                    vpnt.BoneWeights = binReader.ReadInt32();




                    allVertices.Add(vpnt);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }
            int ibreak2 = 0;
        }

        public void ReadSkinnedVertexData32(BinaryReader binReader, List<XboxVertexInstance> allVertices, int numVertices)
        {
            for (int i = 0; i < numVertices; ++i)
            {
                try
                {
                    // 24 bytes per entry? , or 28...
                    Vector3 p = Common.FromStreamVector3(binReader);
                    int boneInfo1 = binReader.ReadInt32();
                    int boneInfo2 = binReader.ReadInt32();
                    Vector2 u = Common.FromStreamVector2(binReader);
                    int boneInfo3 = binReader.ReadInt32();
                    XboxVertexInstance vpnt = new XboxVertexInstance();
                    vpnt.Position = p;
                    vpnt.UV = u;
                    //vpnt.Normal = normV;
                    //vpnt.ExtraData = unk1;
                    allVertices.Add(vpnt);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }
            int ibreak2 = 0;
        }



        // taken from old sol code and it seems to work. wow!
        public Vector3 UncompressNormal(int cv)
        {
            Vector3 v = new Vector3();
            int x = ((int)(cv & 0x7ff) << 21) >> 21,
                    y = ((int)(cv & 0x3ffe00) << 10) >> 21,
                    z = (int)(cv & 0xffc00000) >> 22;

            v.X = ((float)x) / (float)((1 << 10) - 1);
            v.Y = ((float)y) / (float)((1 << 10) - 1);
            v.Z = ((float)z) / (float)((1 << 9) - 1);
            return v;
        }

        public int TextureForSubmesh(int subMeshId)
        {
            if (m_name == "barbarian.mdl")
            {
                if (subMeshId == 0)
                {
                    return 0;
                }
                if (subMeshId == 1)
                {
                    return 1;
                }
                
                return 2;

            }
            return 0;
        }

        public int AdjustForModel(int adjustedIndex)
        {
            if (m_name == "calthaArena")
            {
                if (adjustedIndex == 10)
                {
                    adjustedIndex = 9;
                }
                else if (adjustedIndex == 13)
                {
                    adjustedIndex = 12;
                }
            }
            else if (m_name.Contains("onon"))
            {
                if (adjustedIndex == 5)
                {
                    adjustedIndex = 6;
                }
                else if (adjustedIndex > 5)
                {
                    adjustedIndex += 2;
                }
                //if (adjustedIndex == 5)
                //{
                //    adjustedIndex = 6;
                //}
                //else if (adjustedIndex == 6)
                //{
                //    adjustedIndex = 8;
                //}
                //else if (adjustedIndex == 8)
                //{
                //    adjustedIndex = 10;
                //}
                //else if (adjustedIndex == 9)
                //{
                //    adjustedIndex = 11;
                //}

            }
            else if (m_name.Contains("galdr"))
            {
                if (adjustedIndex == 8)
                {
                    adjustedIndex = 7;
                }
                else if (adjustedIndex == 9)
                {
                    adjustedIndex = 8;
                }
                else if (adjustedIndex == 12)
                {
                    adjustedIndex = 11;
                }

            }
            else if (m_name.Contains("wandering"))
            {
                if (adjustedIndex >= 15)
                {
                    adjustedIndex--;
                }
            }
            else if (m_name.Contains("valenssc"))
            {
                if (adjustedIndex >= 12)
                {
                    adjustedIndex++;
                }
            }
            else if (m_name == "barbarian.mdl")
            {
         
            }
            return adjustedIndex;
        }


        public override void BuildBB()
        {
            MinBB = new Vector3(float.MaxValue);
            MaxBB = new Vector3(float.MinValue);
            foreach (ModelSubMesh subMesh in m_modelMeshes)
            {
                MinBB = Vector3.Min(MinBB, subMesh.MinBB);
                MaxBB = Vector3.Max(MaxBB, subMesh.MaxBB);
            }
        }

        public void ReadSKELSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.skelTag))
            {
                m_hasSkeleton = true;
                m_skinned = true;
                int blockSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int pad2 = binReader.ReadInt32();
                int numBones = (blockSize - 16) / 32;

                for (int i = 0; i < numBones; ++i)
                {
                    BoneNode node = BoneNode.FromStream(binReader);
                    node.name = m_boneNames[i];
                    List<string> names;
                    if (!BoneNode.pad1ByteNames.TryGetValue(node.pad1, out names))
                    {
                        names = new List<string>();
                        BoneNode.pad1ByteNames[node.pad1] = names;
                    }
                    names.Add(node.name);
                    m_bones.Add(node);
                }
                ConstructSkeleton();
            }

        }


        //void CalcBindFinalMatrix(BoneNode bone, Matrix parentMatrix)
        //{
        //    bone.combinedMatrix = bone.localMatrix * parentMatrix;
        //    //bone.finalMatrix = bone.offsetMatrix * bone.combinedMatrix;
        //    bone.finalMatrix = bone.combinedMatrix;

        //    foreach (BoneNode child in bone.children)
        //    {
        //        CalcBindFinalMatrix(child, bone.combinedMatrix);
        //    }
        //}

        //public List<VertexPositionNormalTexture> m_points = new List<VertexPositionNormalTexture>();

        public void WriteFBXAHeader(StreamWriter writer)
        {

            int total = m_subMeshData2List.Count + (3 * m_textures.Count) + 2;
            using (StreamReader sr = new StreamReader("FBXAHeader.txt"))
            {
                string line = sr.ReadToEnd();
                writer.Write(line);
            }
            using (StreamReader sr = new StreamReader("FBXADocumentDescription.txt"))
            {
                string line = sr.ReadToEnd();
                writer.Write(line);
            }
            using (StreamReader sr = new StreamReader("FBXADefinitions.txt"))
            {
                string line = sr.ReadToEnd();
                writer.Write(line);
            }


        }

        public string m_geometryId = "9999";

        public void WriteFBXA(StreamWriter writer, StreamWriter materialWriter, String texturePath, bool skinned = false, List<int> excludeList = null)
        {
            if (excludeList == null)
            {
                excludeList = new List<int>();
            }
            WriteFBXAHeader(writer);
            writer.WriteLine("Objects:  {");

            // loop here?

            BuildClusterInfo();

            WriteGeometryStart(writer, m_subMeshData2List[0]);

            WriteVertices(writer);
            WriteIndices(writer, excludeList);
            WriteNormals(writer, excludeList);
            WriteUVs(writer, excludeList);
            WriteLayerElementMaterial(writer, excludeList);
            WriteGeometryEnd(writer);
            WriteModels(writer);

            WriteMaterials(writer, m_subMeshData2List[0], texturePath);
            WriteTexturesAndVideos(writer, texturePath);
            if (skinned)
            {
                WriteSkeleton(writer);
                WriteDeformers(writer);
            }

            WriteGlobals(writer);
            writer.WriteLine("}");
            if (skinned)
            {
                WritePose(writer);
            }
            WriteConnections(writer, skinned);


            //writer.WriteLine("}");
        }



        public void WriteGeometryStart(StreamWriter writer, SubMeshData2 smd2)
        {
            writer.WriteLine("Geometry: " + m_geometryId + ",\"Geometry::\", \"Mesh\" {");
            writer.WriteLine("Version: 232");

        }

        public void WriteGeometryEnd(StreamWriter writer)
        {
            writer.WriteLine("Layer: 0 {");
            writer.WriteLine("Version: 100");
            writer.WriteLine("LayerElement:  {");
            writer.WriteLine("	Type: \"LayerElementNormal\"");
            writer.WriteLine("	TypedIndex: 0");
            writer.WriteLine("}");
            //writer.WriteLine("LayerElement:  {");
            //writer.WriteLine("	Type: "LayerElementSmoothing"";
            //writer.WriteLine("	TypedIndex: 0";
            //writer.WriteLine("}";
            writer.WriteLine("LayerElement:  {");
            writer.WriteLine("	Type: \"LayerElementUV\"");
            writer.WriteLine("	TypedIndex: 0");
            writer.WriteLine("}");
            writer.WriteLine("LayerElement:  {");
            writer.WriteLine("	Type: \"LayerElementTexture\"");
            writer.WriteLine("	TypedIndex: 0");
            writer.WriteLine("}");
            writer.WriteLine("LayerElement:  {");
            writer.WriteLine("	Type: \"LayerElementMaterial\"");
            writer.WriteLine("	TypedIndex: 0");
            writer.WriteLine("}");
            writer.WriteLine("}");
            writer.WriteLine("}");
        }


        public string m_fbxPoseNodeId;
        public void WritePose(StreamWriter writer)
        {
            m_fbxPoseNodeId = GenerateNodeId();
            writer.WriteLine(String.Format("Pose: {0}, \"Pose::BinePose\", \"BindPose\" {{", m_fbxPoseNodeId));
            writer.WriteLine("  Type: \"BindPose\"");
            writer.WriteLine("  Version: 100");
            writer.WriteLine("  NbPoseNodes: " + m_bones.Count);
            foreach (BoneNode boneNode in m_bones)
            {
                Matrix m = Matrix.Identity;

                writer.WriteLine("  PoseNode:  {");
                writer.WriteLine(String.Format("Node: {0}", boneNode.fbxLimbNodeId));
                writer.WriteLine(String.Format("  Matrix: *{0} {{", 16));
                writer.WriteLine("a: " + Common.ToStringC(m));
                writer.WriteLine(" }");
                writer.WriteLine(" }");

            }


            writer.WriteLine("}");
        }

        public void WriteGlobals(StreamWriter writer)
        {
        }



        public void WriteVertices(StreamWriter writer)
        {
            // write vertices

            int startMesh = 0;
            int numMesh = m_subMeshData2List.Count;
            int endIndex = 0;
            int startIndex = 0;

            for (int a = 0; a < startMesh + numMesh; ++a)
            {
                if (a < startMesh)
                {
                    startIndex = endIndex;
                }
                SubMeshData2 headerBlock = m_subMeshData2List[a];
                endIndex += headerBlock.NumIndices;
            }


            int minVertex = int.MaxValue;
            int maxVertex = int.MinValue;

            for (int i = startIndex; i < endIndex; ++i)
            {
                minVertex = Math.Min(minVertex, m_allIndices[i]);
                maxVertex = Math.Max(maxVertex, m_allIndices[i]);
            }

            maxVertex++;
            writer.WriteLine(String.Format("Vertices: *{0} {{ ", (maxVertex - minVertex) * 3));
            for (int i = minVertex; i < maxVertex; ++i)
            {
                XboxVertexInstance vpnt = m_allVertices[i];
                writer.Write(String.Format("{0:0.00000},{1:0.00000},{2:0.00000}", vpnt.Position.X, vpnt.Position.Y, vpnt.Position.Z));
                if (i < maxVertex - 1)
                {
                    writer.Write(",");
                }
            }
            writer.WriteLine();
            writer.WriteLine("}");
        }


        public List<int> BuildIndexList2L(bool adjust, List<int> excludeList)
        {
            List<int> result = new List<int>();
            bool swap = false;
            int startIndex = 0;
            int endIndex = m_allIndices.Count - 2;

            StringBuilder sb = new StringBuilder();


            for (int a = 0; a < m_subMeshData2List.Count; ++a)
            {
                bool includeSubMesh = !excludeList.Contains(a);

                SubMeshData2 headerBlock = m_subMeshData2List[a];
                {
                    int end = startIndex + headerBlock.NumIndices - 2;

                    for (int i = startIndex; i < end; i++)
                    {
                        int index1 = i;
                        int index2 = i + 1;
                        int index3 = i + 2;
                        if (index3 >= m_allIndices.Count)
                        {
                            index3 = index1;
                        }
                        if (index2 >= m_allIndices.Count)
                        {
                            index2 = index1;
                        }
                        if (i >= m_allIndices.Count)
                        {
                            int ibreak = 0;
                        }

                        int i1 = m_allIndices[index1];
                        int i2 = m_allIndices[index2];
                        int i3 = m_allIndices[index3];


                        // alternate winding
                        if (includeSubMesh)
                        {
                            if (swap)
                            {
                                if (adjust)
                                {
                                    result.Add(i3);
                                    result.Add(i2);
                                    result.Add((i1 + 1) * -1);

                                }
                                else
                                {
                                    result.Add(i3);
                                    result.Add(i2);
                                    result.Add(i1);
                                }
                            }
                            else
                            {
                                if (adjust)
                                {
                                    result.Add(i1);
                                    result.Add(i2);
                                    result.Add((i3 + 1) * -1);

                                }
                                else
                                {
                                    result.Add(i1);
                                    result.Add(i2);
                                    result.Add(i3);
                                }
                            }
                        }
                        swap = !swap;
                    }
                }
                startIndex += headerBlock.NumIndices;
            }

            return result;
        }


        public void WriteIndices(StreamWriter writer, List<int> excludeList)
        {
            // write vertices
            int startIndex = 0;
            List<int> indexList = BuildIndexList2L(true, excludeList);
            int endIndex = indexList.Count;
            writer.WriteLine(String.Format("PolygonVertexIndex: *{0} {{ ", endIndex));
            for (int i = 0; i < indexList.Count; ++i)
            {
                writer.Write(indexList[i]);
                if (i < indexList.Count - 1)
                {
                    writer.Write(",");
                }
            }

            //String indices = BuildIndexList2(true);
            //startIndex += headerBlock.NumIndices;
            //writer.WriteLine(indices);
            writer.WriteLine();
            writer.WriteLine("}");
        }


        public void WriteNormals(StreamWriter writer, List<int> excludeList)
        {
            writer.WriteLine("LayerElementNormal: 0 {");
            writer.WriteLine("  Version: 101");
            writer.WriteLine("  Name: \"\"");
            writer.WriteLine("  MappingInformationType: \"ByPolygonVertex\"");
            writer.WriteLine("  ReferenceInformationType: \"IndexToDirect\"");
            writer.WriteLine(String.Format("  Normals: *{0} {{", m_allVertices.Count * 3));
            writer.Write("  a: ");
            bool swap = false;
            for (int i = 0; i < m_allVertices.Count; ++i)
            {
                XboxVertexInstance vpnt = m_allVertices[i];
                Vector3 norm = vpnt.Normal;
                swap = !swap;
                if (swap)
                {
                    //norm *= -1f;
                }
                writer.Write(String.Format("{0:0.00000},{1:0.00000},{2:0.00000}", norm.X, norm.Y, norm.Z));
                if (i < m_allVertices.Count - 1)
                {
                    writer.Write(",");
                }
            }
            writer.WriteLine();
            writer.WriteLine("}");
            writer.WriteLine();
            List<int> indexList = BuildIndexList2L(false, excludeList);
            writer.WriteLine(String.Format("NormalIndex: *{0} {{ ", indexList.Count));
            for (int i = 0; i < indexList.Count; ++i)
            {
                writer.Write(indexList[i]);
                if (i < indexList.Count - 1)
                {
                    writer.Write(",");
                }
            }
            writer.WriteLine();
            writer.WriteLine("}");


            writer.WriteLine("}");
        }
        public void WriteUVs(StreamWriter writer, List<int> excludeList)
        {
            writer.WriteLine("LayerElementUV: 0 {");
            writer.WriteLine("  Version: 101");
            writer.WriteLine("  Name: \"UVSet0\"");
            writer.WriteLine("  MappingInformationType: \"ByPolygonVertex\"");
            writer.WriteLine("  ReferenceInformationType: \"IndexToDirect\"");
            writer.WriteLine(String.Format("  UV: *{0} {{", m_allVertices.Count * 2));
            writer.Write("  a: ");
            for (int i = 0; i < m_allVertices.Count; ++i)
            {
                XboxVertexInstance vpnt = m_allVertices[i];
                writer.Write(String.Format("{0:0.00000},{1:0.00000}", vpnt.UV.X, 1.0f - vpnt.UV.Y));
                if (i < m_allVertices.Count - 1)
                {
                    writer.Write(",");
                }
            }
            writer.WriteLine();
            writer.WriteLine("}");

            int startIndex = 0;
            int endIndex = m_allIndices.Count - 2;
            List<int> indexList = BuildIndexList2L(false, excludeList);
            writer.WriteLine(String.Format("UVIndex: *{0} {{ ", indexList.Count));
            for (int i = 0; i < indexList.Count; ++i)
            {
                writer.Write(indexList[i]);
                if (i < indexList.Count - 1)
                {
                    writer.Write(",");
                }
            }
            writer.WriteLine();
            writer.WriteLine("}");


            writer.WriteLine();
            writer.WriteLine("}");
        }

        public void WriteLayerElementTexture(StreamWriter writer)
        {
            writer.WriteLine("LayerElementTexture: 0 {");
            writer.WriteLine("	Version: 101");
            writer.WriteLine("	Name: \"\" ");
            writer.WriteLine("	MappingInformationType: \"ByVertice\"");
            writer.WriteLine("	ReferenceInformationType: \"Direct\"");
            writer.WriteLine("	BlendMode: \"Translucent\"");
            writer.WriteLine("	TextureAlpha: 1");
            writer.WriteLine("	TextureId: ");
            writer.WriteLine("}");

        }

        public string m_mainModelId;
        public void WriteModels(StreamWriter writer)
        {
            m_mainModelId = GenerateNodeId();
            writer.WriteLine(String.Format("Model: {0}, \"Model::Main\", \"Mesh\" {{", m_mainModelId));
            writer.WriteLine("  Version: 232");
            writer.WriteLine("  Properties70:  {");
            writer.WriteLine("  P: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            writer.WriteLine("  P: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",0");
            writer.WriteLine("  }");
            writer.WriteLine("  Shading: Y");
            writer.WriteLine("  Culling: \"CullingOff\"");
            writer.WriteLine("}");
        }

        static int s_nodeCount = 10;
        public string GenerateNodeId()
        {
            return "" + s_nodeCount++;
        }

        public void BuildClusterInfo()
        {
            if (m_skinned)
            {
                Dictionary<short, BoneNode> boneMap = new Dictionary<short, BoneNode>();
                foreach (BoneNode node in m_bones)
                {
                    boneMap[node.id] = node;
                }

                for (int i = 0; i < m_allVertices.Count; ++i)
                {
                    XboxVertexInstance xbi = m_allVertices[i];
                    for (int a = 0; a < 3; ++a)
                    {
                        short index1 = (short)XboxVertexInstance.AdjustBone(xbi.BoneIndices[a]);
                        if (index1 != -1)
                        {
                            boneMap[index1].AddIndexAndWeight(i, xbi.Weight(a));
                        }
                    }
                }
                foreach (BoneNode node in m_bones)
                {
                    if (node.weightVertexIndices.Count > 0)
                    {
                        int ibreak2 = 0;
                    }
                }

            }

            int ibreak = 0;
        }


        public void WriteSkeleton(StreamWriter writer)
        {
            if (m_skinned)
            {
                //writer.WriteLine("; Object properties");
                //writer.WriteLine(";------------------------------------------------------------------");

                //writer.WriteLine("Objects: {");

                foreach (BoneNode boneNode in m_bones)
                {
                    boneNode.fbxBoneAttributeId = GenerateNodeId();
                    writer.WriteLine(String.Format("NodeAttribute: {0}, \"NodeAttribute::{1}\", \"LimbNode\" {{", boneNode.fbxBoneAttributeId, boneNode.name));
                    writer.WriteLine("  Properties70: {");
                    writer.WriteLine(String.Format("    P: \"Size\", \"double\", \"Number\",\"\",{0}", 1.0f));
                    writer.WriteLine("  }");
                    writer.WriteLine("  TypeFlags: \"Skeleton\"");
                    writer.WriteLine("}");
                }
                //writer.WriteLine("}");

                foreach (BoneNode boneNode in m_bones)
                {
                    boneNode.fbxLimbNodeId = GenerateNodeId();

                    writer.WriteLine(String.Format("Model: {0}, \"Model::{1}\", \"LimbNode\" {{", boneNode.fbxLimbNodeId, boneNode.name));
                    writer.WriteLine("  Properties70: {");
                    writer.WriteLine(String.Format("    P: \"Size\", \"double\", \"Number\",\"\",{0}", 1.0f));
                    writer.WriteLine("	P: \"RotationActive\", \"bool\", \"\", \"\",1");
                    writer.WriteLine("	P: \"InheritType\", \"enum\", \"\", \"\",1");
                    writer.WriteLine("	P: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
                    writer.WriteLine("	P: \"PreferedAngleX\", \"double\", \"Number\", \"\",0.0");
                    writer.WriteLine("	P: \"PreferedAngleY\", \"double\", \"Number\", \"\",0.0");
                    writer.WriteLine("	P: \"PreferedAngleZ\", \"double\", \"Number\", \"\",0.0");
                    writer.WriteLine("	P: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",0");
                    writer.WriteLine("	P: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A+\", " + Common.ToStringC(boneNode.offset));
                    writer.WriteLine("	P: \"Lcl Rotation\", \"Lcl Rotation\", \"\", \"A+\",0.0,0.0,0.0");
                    writer.WriteLine("	P: \"liw\", \"Bool\", \"\", \"A+U\",0");
                    writer.WriteLine("  }");
                    writer.WriteLine("  TypeFlags: \"Skeleton\"");
                    writer.WriteLine("}");
                }

            }


        }

        public int GetTextureId(int val)
        {
            SubMeshData1 data1 = m_subMeshData1List[val];


            int matIndex = 0;
            if (val < m_meshMaterialList.Count)
            {
                val = m_meshMaterialList[val].val3 / s_materialBlockSize;
            }

            int adjustment = 0;
            matIndex = Math.Min(matIndex, m_materialDataList.Count);

            for (int i = 0; i < matIndex; ++i)
            {
                if (m_materialDataList[i].specularTextureData != null)
                {
                    adjustment = 1;
                }
            }
            matIndex -= adjustment;

            //adjustedIndex = materialData.diffuseTextureId / s_textureBlockSize;
            //adjustedIndex -= adjustment;
            int adjustedIndex = AdjustForModel(matIndex);

            if (adjustedIndex >= m_textureNames.Count)
            {
                adjustedIndex = m_textureNames.Count - 1;
            }
            return adjustedIndex;

        }

        public List<int> BuildLayerElementMaterial(List<int> excludeList)
        {
            int startIndex = 0;
            int endIndex = 0;
            int submeshCount = 0;
            int matIndex;
            int adjustedIndex;

            List<int> materialList = new List<int>();
            for (int a = 0; a < m_subMeshData2List.Count; ++a)
            {
                SubMeshData2 headerBlock = m_subMeshData2List[a];
                int adjustment = 0;

                if (!excludeList.Contains(a))
                {
                    SubMeshData1 data1 = m_subMeshData1List[a];

                    submeshCount++;
                    matIndex = m_meshMaterialList[a].val3 / s_materialBlockSize;

                    matIndex = Math.Min(matIndex, m_materialDataList.Count);

                    for (int i = 0; i < matIndex; ++i)
                    {
                        if (m_materialDataList[i].specularTextureData != null)
                        {
                            adjustment = 1;
                        }
                    }
                    matIndex -= adjustment;

                    //adjustedIndex = materialData.diffuseTextureId / s_textureBlockSize;
                    //adjustedIndex -= adjustment;
                    adjustedIndex = AdjustForModel(matIndex);

                    if (adjustedIndex >= m_materialDataList.Count)
                    {
                        int ibreak = 0;
                    }

                    if (adjustedIndex < 0)
                    {
                        int ibreak = 0;
                    }
                    int end = startIndex + (headerBlock.NumIndices);

                    //end -= 1;
                    for (int i = startIndex; i < end; i++)
                    {
                        materialList.Add(adjustedIndex);
                    }
                }
                startIndex += headerBlock.NumIndices;
                //startIndex -= 1;
                //break;
            }
            return materialList;
        }


        public void WriteLayerElementMaterial(StreamWriter writer, List<int> excludeList)
        {
            List<int> materialList = BuildLayerElementMaterial(excludeList);
            writer.WriteLine("LayerElementMaterial: 0 {");
            writer.WriteLine("  Version: 101");
            writer.WriteLine("  Name: \"\"");
            writer.WriteLine("  MappingInformationType: \"ByPolygon\"");
            writer.WriteLine("  ReferenceInformationType: \"IndexToDirect\"");
            writer.WriteLine(String.Format("  Materials: *{0} {{", materialList.Count));
            writer.Write("      a:");
            for (int i = 0; i < materialList.Count; ++i)
            {
                writer.Write(materialList[i]);
                if (i < materialList.Count - 1)
                {
                    writer.Write(",");
                }
            }
            writer.WriteLine();
            writer.WriteLine("}");
            writer.WriteLine("}");

        }

        public void WriteRelations(StreamWriter writer)
        {
            writer.WriteLine("; Object relations");
            writer.WriteLine(";------------------------------------------------------------------");

            writer.WriteLine("Relations: {");
            foreach (SubMeshData2 headerBlock in m_subMeshData2List)
            {
                writer.WriteLine(String.Format("    Model: \"{0}\", \"Mesh\" {{", headerBlock.fbxNodeId));
                writer.WriteLine("}");
                // fixme . find the texture name here and link to the model as well...
            }
            foreach (TextureData material in m_textures)
            {
                string line = String.Format("   Material: \"{0}\" , \"\" {{", material.textureFbxNodeId);

                //String line = String.Format("Texture: {0}","foo");
                writer.WriteLine(line);
                writer.WriteLine("}");
            }
            writer.WriteLine("}");
        }

        public void WriteConnections(StreamWriter writer, bool skinned)
        {

            writer.WriteLine("; Object connections");
            writer.WriteLine(";------------------------------------------------------------------");

            writer.WriteLine("Connections: {");

            int count = 0;
            string endModelId = "-1";

            writer.WriteLine(String.Format(";Model::{0}, Model::RootNode", m_mainModelId));
            writer.WriteLine(String.Format("    C: \"OO\",{0}, 0", m_mainModelId));


            writer.WriteLine(String.Format(";   Geometry:: ,MainModel"));
            writer.WriteLine(String.Format("    C: \"OO\",{0}, {1}", m_geometryId, m_mainModelId));



            //foreach (SubMeshData2 headerBlock in m_subMeshData2List)
            //{
            //    if (count == 0)
            //    {
            //        writer.WriteLine(String.Format(";Model::{0}, Model::RootNode", headerBlock.fbxNodeName));
            //        writer.WriteLine(String.Format("    C: \"OO\",{0}, 0", headerBlock.fbxNodeId));
            //    }
            //    else
            //    {
            //        //;Model::default, Model::carafe_decanter.mdl_root
            //        writer.WriteLine(String.Format(";   Model::{0} ,Model::{1}", headerBlock.fbxNodeName, m_subMeshData2List[count - 1].fbxNodeName));
            //        writer.WriteLine(String.Format("    C: \"OO\",{0}, {1}", headerBlock.fbxNodeId, m_subMeshData2List[count-1].fbxNodeId));
            //    }

            //    if(count == m_subMeshData1List.Count-1)
            //    {
            //        writer.WriteLine(String.Format(";   Geometry:: ,Model::{0}", headerBlock.fbxNodeName));
            //        writer.WriteLine(String.Format("    C: \"OO\",{0}, {1}",m_geometryId, headerBlock.fbxNodeId));
            //        endModelId = headerBlock.fbxNodeId;
            //    }

            //    count++;
            //}




            // Connect material to the object...
            //writer.WriteLine(String.Format("    Connect: \"OO\",\"{0}\", \"{1}\"", m_baseMaterialName,m_subMeshData2List[0].fbxNodeId));
            //writer.WriteLine(String.Format("    Connect: \"OP\",\"{0}\", \"{1}\",\"DiffuseColor\"", m_textures[1].textureFbxNodeId, m_baseMaterialName));
            count = 0;
            int i = 0;
            //foreach (TextureData material in m_textures)
            foreach (MaterialData material in m_materialDataList)
            {
                //if (!material.textureName.Contains("skygold"))
                {
                    //writer.WriteLine(String.Format(";    \"Material::{0}\", \"Model::{1}\"", material.textureName, m_subMeshData2List[count].fbxNodeId));
                    writer.WriteLine(String.Format(";    \"Material::{0}\", \"Model::{1}\"", material.diffuseTextureData.textureName, m_mainModelId));
                    writer.WriteLine(String.Format("    C: \"OO\",{0}, {1}", material.fbxNodeId, m_mainModelId));

                    writer.WriteLine(String.Format(";    \"Texture::{0}\", \"Material::{1}\"", material.diffuseTextureData.textureName, material.diffuseTextureData.textureName));
                    writer.WriteLine(String.Format("    C: \"OP\",{0}, {1},\"DiffuseColor\"", material.diffuseTextureData.textureFbxNodeId, material.fbxNodeId));
                    if (material.specularTextureData != null)
                    {
                        writer.WriteLine(String.Format(";    \"Texture::{0}\", \"Material::{1}\"", material.specularTextureData.textureName, material.specularTextureData.textureName));
                        writer.WriteLine(String.Format("    C: \"OP\",{0}, {1},\"SpecularColor\"", material.specularTextureData.textureFbxNodeId, material.fbxNodeId));
                    }

                    writer.WriteLine(String.Format(";    Video::{0}, Texture::{1}", material.diffuseTextureData.textureName, material.diffuseTextureData.textureName));
                    writer.WriteLine(String.Format("    C: \"OO\",{0}, {1}", material.diffuseTextureData.videoFbxNodeId, material.diffuseTextureData.textureFbxNodeId));
                    count++;
                }
            }



            if (skinned)
            {
                writer.WriteLine(String.Format(";    TopLevelDeformer , Geometry "));
                writer.WriteLine(String.Format("    C: \"OO\",{0}, {1}", m_topLevelDeformerId, m_geometryId));

                foreach (BoneNode boneNode in m_bones)
                {
                    writer.WriteLine(String.Format(";  {0} , TopLevelDeformer", boneNode.name));
                    writer.WriteLine(String.Format("    C: \"OO\",{0},{1}", boneNode.fbxDeformerNodeId, m_topLevelDeformerId));
                    writer.WriteLine();
                }

                foreach (BoneNode boneNode in m_bones)
                {
                    writer.WriteLine(String.Format(";  NodeAttribute::{0} , LimbModel::{1}", boneNode.name, boneNode.name));
                    writer.WriteLine(String.Format("    C: \"OO\",{0},{1}", boneNode.fbxBoneAttributeId, boneNode.fbxDeformerNodeId));
                    writer.WriteLine();
                }

                foreach (BoneNode boneNode in m_bones)
                {
                    if (boneNode.parent != null)
                    {
                        writer.WriteLine(String.Format(";  LimbModel::{0} , LimbModel::{1}", boneNode.name, boneNode.parent.name));
                        writer.WriteLine(String.Format("    C: \"OO\",{0},{1}", boneNode.fbxLimbNodeId, boneNode.parent.fbxLimbNodeId));
                        writer.WriteLine();
                    }
                }

            }



            //foreach (BoneNode boneNode in m_bones)
            //{
            //    foreach (BoneNode childNode in boneNode.children)
            //    {
            //        writer.WriteLine(String.Format(";  {0}::{1}", boneNode.name, childNode.name));
            //        writer.WriteLine(String.Format("    C: \"OO\",{0},{1}", childNode.fbxDeformerNodeId,boneNode.fbxDeformerNodeId ));
            //        writer.WriteLine();
            //    }
            //}

            writer.WriteLine("}");
        }

        //public string m_baseMaterialName = "Material::BaseMaterial";

        public void WriteMaterials(StreamWriter writer, SubMeshData2 headerBlock, String texturePath)
        {
            //foreach (TextureData texture in m_textures)
            foreach (MaterialData materialData in m_materialDataList)
            {
                //if (!texture.textureName.Contains("skygold"))
                {

                    materialData.fbxNodeId = GenerateNodeId();
                    writer.WriteLine(String.Format("Material: {0}, \"Material::{1}\" , \"\" {{", materialData.fbxNodeId, materialData.fbxNodeId));
                    writer.WriteLine("    Version: 102");
                    writer.WriteLine("    ShadingModel: \"phong\"");
                    writer.WriteLine("    MultiLayer: 0");
                    writer.WriteLine("    Properties70:  {");
                    writer.WriteLine("    P: \"AmbientColor\", \"Color\", \"\", \"A\",1,1,1");
                    writer.WriteLine("    P: \"DiffuseColor\", \"Color\", \"\", \"A\",1,1,1");
                    writer.WriteLine("    P: \"TransparentColor\", \"Color\", \"\", \"A\",1,1,1");
                    writer.WriteLine("    P: \"SpecularColor\", \"Color\", \"\", \"A\",1,1,1");
                    writer.WriteLine("    P: \"Emissive\", \"Vector3D\", \"Vector\", \"\",0,0,0");
                    writer.WriteLine("    P: \"Ambient\", \"Vector3D\", \"Vector\", \"\",1,1,1");
                    writer.WriteLine("    P: \"Diffuse\", \"Vector3D\", \"Vector\", \"\",1,1,1");
                    writer.WriteLine("    P: \"Specular\", \"Vector3D\", \"Vector\", \"\",1,1,1");
                    writer.WriteLine("    P: \"Shininess\", \"double\", \"Number\", \"\",20");
                    writer.WriteLine("    P: \"Opacity\", \"double\", \"Number\", \"\",1");
                    writer.WriteLine("    P: \"Reflectivity\", \"double\", \"Number\", \"\",0");


                    writer.WriteLine("  }");
                    writer.WriteLine("}");
                }
            }
        }

        public void WriteTexturesAndVideos(StreamWriter writer, String texturePath)
        {
            foreach (TextureData texture in m_textures)
            {
                //if (!texture.textureName.Contains("skygold"))
                {

                    String fullPath = texturePath + texture.textureName + ".png";
                    texture.videoFbxNodeId = GenerateNodeId();
                    string line = String.Format("Video: {0} , \"Video::{1}\",\"Clip\" {{", texture.videoFbxNodeId, texture.textureName);
                    writer.WriteLine(line);
                    writer.WriteLine("Type: \"Clip\"");
                    writer.WriteLine("Properties70:  {");
                    writer.WriteLine("P: \"Path\", \"KString\", \"XRefUrl\", \"\", \"" + fullPath + "\"");
                    writer.WriteLine("}");
                    writer.WriteLine("UseMipMap: 0");
                    writer.WriteLine("Filename: \"" + fullPath + "\"");
                    writer.WriteLine("RelativeFilename: \"" + fullPath + "\"");
                    writer.WriteLine("}");
                }
            }

            foreach (TextureData texture in m_textures)
            {
                //if (!texture.textureName.Contains("skygold"))
                {

                    texture.textureFbxNodeId = GenerateNodeId();
                    //String line = String.Format("Texture: {0}","foo");
                    String fullPath = texturePath + texture.textureName + ".png";
                    writer.WriteLine(String.Format("Texture: {0}, \"Texture::{1}\",\"\" {{", texture.textureFbxNodeId, texture.textureName));
                    writer.WriteLine("	Type: \"TextureVideoClip\"");
                    writer.WriteLine(String.Format("	TextureName: \"Texture::{0}\"", texture.textureName));
                    writer.WriteLine("	Properties70:  {");
                    writer.WriteLine("		P: \"UVSet\", \"KString\", \"\", \"\", \"UVSet0\"");
                    writer.WriteLine("	}");
                    writer.WriteLine(String.Format("	Media: \"Video::{0}\"", texture.textureName));
                    writer.WriteLine(String.Format("	FileName: \"{0}\"", fullPath));
                    writer.WriteLine(String.Format("	RelativeFilename: \"{0}\"", fullPath));
                    writer.WriteLine("	ModelUVTranslation: 0,0");
                    writer.WriteLine("	ModelUVScaling: 1,1");
                    writer.WriteLine("	Texture_Alpha_Source: \"None\"");
                    writer.WriteLine("	Cropping: 0,0,0,0");
                    writer.WriteLine("}");
                }
            }


        }

        public string m_topLevelDeformerId;
        public void WriteDeformers(StreamWriter writer)
        {
            m_topLevelDeformerId = GenerateNodeId();
            //return;
            writer.WriteLine(String.Format("Deformer: {0}, \"Deformer::Skin\" , \"Skin\" {{", m_topLevelDeformerId));
            writer.WriteLine("  Version: 101");
            writer.WriteLine("  Link_DeformAccuracy: 50");
            writer.WriteLine(" }");


            foreach (BoneNode boneNode in m_bones)
            {
                boneNode.fbxSubDeformerNodeId = GenerateNodeId();
                writer.WriteLine(String.Format("; {0}", boneNode.name));
                writer.WriteLine(String.Format("Deformer: {0}, \"SubDeformer::\" , \"Cluster\" {{", boneNode.fbxSubDeformerNodeId));
                writer.WriteLine("  Version: 100");
                writer.WriteLine("  UserData: \"\", \"\"");
                // to do, 
                writer.WriteLine(String.Format("  Indexes: *{0} {{", boneNode.weightVertexIndices.Count));
                writer.Write("a: ");
                for (int i = 0; i < boneNode.weightVertexIndices.Count; ++i)
                {
                    writer.Write("" + boneNode.weightVertexIndices[i]);
                    if (i < boneNode.weightVertexIndices.Count - 1)
                    {
                        writer.Write(",");
                    }
                }
                writer.WriteLine();
                writer.WriteLine(" }");
                writer.WriteLine(String.Format("  Weights: *{0} {{", boneNode.weightVertexWeights.Count));
                writer.Write("a: ");
                for (int i = 0; i < boneNode.weightVertexWeights.Count; ++i)
                {
                    writer.Write(String.Format("{0:0.00000}", boneNode.weightVertexWeights[i]));
                    if (i < boneNode.weightVertexWeights.Count - 1)
                    {
                        writer.Write(",");
                    }
                }
                writer.WriteLine();
                writer.WriteLine(" }");

                // local offset as matrix.
                writer.WriteLine(String.Format("  Transform: *{0} {{", 16));
                writer.WriteLine("a: " + Common.ToStringC(Matrix.Identity));
                writer.WriteLine(" }");

                // parent link as matrix
                writer.WriteLine(String.Format("  TransformLink: *{0} {{", 16));
                writer.WriteLine("a: " + Common.ToStringC(boneNode.finalMatrix));
                writer.WriteLine(" }");

                writer.WriteLine(" }");
            }


        }


    }





    public class SubMeshData1
    {
        public int minus1;
        public int zero1;
        public int zero2;
        public int counter1;
        //byte[] name1;
        //byte f1a;
        //byte f1b;
        //byte f1c;
        //byte f1d;

        //float f1;
        //float f2;
        //float f3;
        //float f4;

        int f1;
        int f2;
        int f3;
        int f4;
        //byte[] usefulBytes;
        //byte[] padBytes;

        public int pad1;
        public int pad2;
        public int pad3;
        public int pad4;
        public int pad5;
        //public byte pad5a;
        //public byte pad5b;
        //public byte pad5c;
        //public byte pad5d;
        public int LodLevel;
        public static HashSet<int> s_lodLevels = new HashSet<int>();


        public static SubMeshData1 FromStream(BinaryReader binReader)
        {
            SubMeshData1 smd = new SubMeshData1();
            smd.zero1 = binReader.ReadInt32();
            smd.zero2 = binReader.ReadInt32();
            smd.counter1 = binReader.ReadInt32();
            //smd.name1 = binReader.ReadBytes(16);
            //smd.f1 = binReader.ReadSingle();



            //smd.f1a = binReader.ReadByte();
            //smd.f1b = binReader.ReadByte();
            //smd.f1c = binReader.ReadByte();
            //smd.f1d = binReader.ReadByte();

            //smd.f1 = binReader.ReadSingle();
            //smd.f2 = binReader.ReadSingle();
            //smd.f3 = binReader.ReadSingle();
            //smd.f4 = binReader.ReadSingle();

            //smd.usefulBytes = binReader.ReadBytes(16);
            //smd.padBytes = binReader.ReadBytes(20);
            smd.f1 = binReader.ReadInt32();
            smd.f2 = binReader.ReadInt32();
            smd.f3 = binReader.ReadInt32();
            smd.f4 = binReader.ReadInt32();


            smd.pad1 = binReader.ReadInt32();
            smd.pad2 = binReader.ReadInt32();
            smd.pad3 = binReader.ReadInt32();
            smd.pad4 = binReader.ReadInt32();
            smd.pad5 = binReader.ReadInt32();
            //smd.pad5a = binReader.ReadByte();
            //smd.pad5b = binReader.ReadByte();
            //smd.pad5c = binReader.ReadByte();
            //smd.pad5d = binReader.ReadByte();
            smd.LodLevel = binReader.ReadInt32();
            smd.minus1 = binReader.ReadInt32();
            s_lodLevels.Add(smd.LodLevel);
            return smd;
        }

        public String PrintTidy(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; ++i)
            {
                sb.Append("[" + data[i] + "]\t");
            }
            return sb.ToString();
        }


        public void WriteInfo(StreamWriter sw)
        {
            //sw.WriteLine(String.Format("[{0}][{1}][{2}][{3}][{4}][{5}][{6}][{7}][{8}][{9}][{10}] a[{11}] b[{12}] c[{13}] d[{14}] lod[{15}]", zero1, zero2, counter1, , pad1, pad2, pad3, pad4, pad5a, pad5b, pad5c, pad5d, LodLevel));
            sw.WriteLine(String.Format("[{0}][{1}][{2}][{3}][{4}][{5}][{6}][{7}][{8}][{9}][{10}] a[{11}] lod[{12}]", zero1, zero2, counter1, f1, f2, f3, f4, pad1, pad2, pad3, pad4, pad5, LodLevel));
            //sw.WriteLine(String.Format("[{0}][{1}][{2}][{3}][{4}]", zero1, zero2, counter1,PrintTidy(usefulBytes),PrintTidy(padBytes),LodLevel));
            //sw.WriteLine(String.Format("[{0}][{1}][{2}][{3}]",f1a,f1b,f1c,f1d));
        }


    }

    public class SubMeshData2
    {
        public float val1;
        public int StartOffset;
        public int val2;
        public int NumIndices;

        public int val3;
        public int pad;
        public byte[] padBytes;
        public List<ushort> indices = new List<ushort>();
        public int MinVertex = int.MaxValue;
        public int MaxVertex = int.MinValue;
        public string fbxNodeId;
        public string fbxNodeName;

        public static SubMeshData2 FromStream(BinaryReader binReader)
        {
            SubMeshData2 smd = new SubMeshData2();
            smd.val1 = binReader.ReadSingle();
            smd.StartOffset = binReader.ReadInt32();
            smd.val1 = binReader.ReadInt32();
            smd.NumIndices = binReader.ReadInt32();
            //smd.NumIndices *= 2;
            smd.val2 = binReader.ReadInt32();
            return smd;
        }
        public void WriteInfo(StreamWriter sw)
        {
            sw.WriteLine(String.Format("SO[{0}]NI[{1}]V1[{2}]V2[{3}]V3[{4}]", StartOffset, NumIndices, val1, val2, val3));
        }

        public void BuildMinMax()
        {
            for (int i = 0; i < indices.Count; ++i)
            {
                MinVertex = Math.Min(MinVertex, indices[i]);
                MaxVertex = Math.Max(MaxVertex, indices[i]);
            }

        }

    }

    public class SubMeshData3
    {
        public List<int> initialValsList = new List<int>();
        public List<int> list1 = new List<int>();
        public List<MaterialBlock> materialBlockList = new List<MaterialBlock>();
        public float val7;

        public int lastElementOffset;
        public int headerEnd2;
        public int headerEnd3;
        public int headerEnd4;
        public int headerEnd5;
        public float headerEnd6;
        public int[] headerEndZero = new int[8];
        public int[] headerEndZero2 = new int[3];

        public XboxModel model;

        public void WriteInfo(StreamWriter sw)
        {
            sw.WriteLine("**********************************************************************");
            sw.WriteLine(model.m_name);
            sw.WriteLine();
            foreach (int val in initialValsList)
            {
                sw.Write(val);
                sw.Write(",");
            }
            sw.WriteLine();
            //foreach (int[] ia in model.m_meshMaterialList)
            //{
            //    sw.WriteLine(String.Format("{0,10} {1,10} {2,10}", ia[0], ia[1], ia[2]));
            //}
            sw.WriteLine("**********************************************************************");
            foreach (MaterialData smd4 in model.m_materialDataList)
            {
                smd4.WriteInfo(sw);
            }
        }

        //1073742880 is 2.0 (ish)
        //1065353216 is 1.0 (ish)
        // read those vals to sensible floats.


        public static SubMeshData3 FromStream(XboxModel model, BinaryReader binReader, int numMeshes, int endOffset, bool skinned)
        {
            SubMeshData3 smd3 = new SubMeshData3();
            smd3.model = model;
            int maxOffset = 0;

            if (skinned)
            {
                byte[] searchBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x0c, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x00 };
                if (Common.FindCharsInStream(binReader, searchBytes))
                {
                    binReader.BaseStream.Position -= searchBytes.Length;
                    for (int i = 0; i < numMeshes; ++i)
                    {
                        smd3.list1.Add(binReader.ReadInt32());
                    }
                }
            }
            else
            {
                int val1 = binReader.ReadInt32();
                int val2 = binReader.ReadInt32();
                Debug.Assert(val1 == 0);
                //Debug.Assert(val2 == -1);

                int numElements = binReader.ReadInt32();
                smd3.initialValsList.Add(numElements);
                int count = (numElements - 4) / 4;
                count = 3;
                for (int i = 0; i < count; ++i)
                {
                    smd3.initialValsList.Add(binReader.ReadInt32());
                }
                //smd3.val3 = binReader.ReadInt32();
                //smd3.val4 = binReader.ReadInt32();
                //smd3.val5 = binReader.ReadInt32();
                //smd3.val6 = binReader.ReadInt32();
                // only -1 on unskinned?
                //Debug.Assert(smd3.val2 == -1);
                //Debug.Assert(smd3.val4 == -1);
                //Debug.Assert(smd3.val6 == -1);
                for (int i = 0; i < numMeshes; ++i)
                {
                    smd3.list1.Add(binReader.ReadInt32());
                }
            }

            MeshMaterial mm = null;
            do
            {
                mm = MeshMaterial.FromStream(binReader);
                if (mm != null)
                {
                    model.m_meshMaterialList.Add(mm);
                    maxOffset = Math.Max(maxOffset, mm.val3);
                }
            }
            while (mm != null);

            Debug.Assert(maxOffset % XboxModel.s_materialBlockSize == 0);
            maxOffset /= XboxModel.s_materialBlockSize;
            maxOffset += 1;
            int ibreak2 = 0;

            for (int i = 0; i < maxOffset; ++i)
            {
                smd3.materialBlockList.Add(MaterialBlock.FromStream(binReader));
            }

            smd3.lastElementOffset = binReader.ReadInt32();
            smd3.headerEnd2 = binReader.ReadInt32();
            smd3.headerEnd3 = binReader.ReadInt32();
            smd3.headerEnd4 = binReader.ReadInt32();
            smd3.headerEnd5 = binReader.ReadInt32();
            smd3.headerEnd6 = binReader.ReadSingle();
            //Debug.Assert(smd3.headerEnd6 == 1.0f || smd3.headerEnd6 == 0.0f);
            for (int i = 0; i < smd3.headerEndZero.Length; ++i)
            {
                smd3.headerEndZero[i] = binReader.ReadInt32();
                //Debug.Assert(smd3.headerEndZero[i] == 0);
            }
            //maxOffset -= 1;

            float[] toFind = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            int index = 0;
            while (Common.PositionAtFloats(binReader, toFind))
            {
                int startVal = smd3.materialBlockList[index].Offset;
                int endVal = index < smd3.materialBlockList.Count - 1 ? smd3.materialBlockList[index + 1].Offset : startVal + (28 * 4);//100;
                int sectionLength = endVal - startVal;
                MaterialBlock materialBlock = smd3.materialBlockList[index];

                model.m_materialDataList.Add(MaterialData.FromStream(binReader, numMeshes, sectionLength, materialBlock));
                ++index;
            }
            //for (int i = 0; i < smd3.headerEndZero2.Length; ++i)
            //{
            //    smd3.headerEndZero2[i] = binReader.ReadInt32();
            //    Debug.Assert(smd3.headerEndZero2[i] == 0);
            //}

            return smd3;
        }
    }

    public class MaterialData
    {
        public float[] array0 = new float[4];
        public int header1;
        public int header2;
        public int header3;
        public int diffuseTextureId = -1;
        public int specularTextureId = -1;
        public int header4;
        public float startVal;
        public int header5;
        public int header6;

        public float endVal;
        public int[] endBlock2 = new int[8];
        public List<int[]> m_data = new List<int[]>();

        public TextureData diffuseTextureData;
        public TextureData specularTextureData;
        public string fbxNodeId;

        public static MaterialData FromStream(BinaryReader binReader, int numMeshes, int sectionLength, MaterialBlock materialBlock)
        {
            MaterialData smd = new MaterialData();
            for (int i = 0; i < smd.array0.Length; ++i)
            {
                smd.array0[i] = binReader.ReadSingle();
                Debug.Assert(smd.array0[i] == 1.0f);
            }

            smd.header1 = binReader.ReadInt32();

            int count1 = (sectionLength - 112) / 12;

            int val1 = -1;
            do
            {
                val1 = binReader.ReadInt32();
                if (val1 == 3073)
                {
                    int val2 = binReader.ReadInt32();
                    int val3 = binReader.ReadInt32();
                    int[] tdata = new int[] { val1, val2, val3 };
                    smd.m_data.Add(tdata);
                }
                else
                {
                    binReader.BaseStream.Position -= 4;
                }
            }
            while (val1 == 3073);

            int val4 = binReader.ReadInt32();
            Debug.Assert(val4 == 330761);

            //smd.header3 = binReader.ReadInt32();
            //smd.diffuseTextureId = binReader.ReadInt32();
            //smd.header4 = binReader.ReadInt32();
            //Debug.Assert(smd.header4 == 330761);
            smd.startVal = binReader.ReadSingle();
            smd.header5 = binReader.ReadInt32();
            //Debug.Assert(smd.header5 == 3 || smd.header5 == 8);
            smd.header6 = binReader.ReadInt32();
            //Debug.Assert(smd.header6 == 1036 smd.header6 == 1036);

            int offset = (smd.array0.Length * 4);
            int count = (sectionLength - offset) / 4;


            int toFind = 1036;
            Common.PositionAtInt(binReader, toFind);

            bool keepGoing = true;
            int[] data = null;
            List<int[]> iaList = new List<int[]>();
            while (keepGoing)
            {

                data = new int[7];//new int[count - 17];

                for (int i = 0; i < data.Length; ++i)
                {
                    data[i] = binReader.ReadInt32();
                    if (i == 0 && data[i] != toFind)
                    {
                        binReader.BaseStream.Position -= 4;
                        keepGoing = false;
                        break;
                    }
                    else
                    {
                        iaList.Add(data);
                    }
                }

            }
            //if (Common.PositionAtFloat(binReader, 1.0f) || Common.PositionAtFloat(binReader, 0.0f))
            if (Common.PositionAtFloat(binReader, 1.0f))
            {
                smd.endVal = binReader.ReadSingle();
                for (int i = 0; i < smd.endBlock2.Length; ++i)
                {
                    smd.endBlock2[i] = binReader.ReadInt32();
                    //Debug.Assert(smd.endBlock2[i] == 0);
                }
            }
            else
            {
                int ibreak = 0;
            }

            return smd;
        }

        public void WriteInfo(StreamWriter sw)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("T{0} {1} {2} {3} {4} {5:0.00000} {6} {7}", diffuseTextureId / XboxModel.s_textureBlockSize, header1, header2, header3, header4, startVal, header5, header6);
            sb.AppendLine();
            foreach (int[] db in m_data)
            {
                foreach (int i in db)
                {
                    sb.Append(String.Format("{0,10} ", i));
                }
                sb.AppendLine();
            }
            sb.AppendLine();
            sb.AppendFormat("{0:0.000000}", endVal);
            sb.AppendLine();
            sb.AppendLine("*******************************************");
            sw.WriteLine(sb.ToString());
        }

    }



    public class XboxVertexInstance
    {


        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
        public Vector2 UV2;
        public int ExtraData;
        //public byte[] Weights;
        public short[] BoneIndices;
        public int BoneInfo1;
        public int BoneWeights;
        public int BoneInfo3;

        public static HashSet<short> sBoneIndices = new HashSet<short>();
        public static Dictionary<short,int> sBoneIndicesCount = new Dictionary<short,int>();

        public override string ToString()
        {
            return String.Format("P {0}\tN {1}\tUV {2}\tE {3}", Common.ToString(Position), Common.ToString(Normal), Common.ToString(UV), ExtraData);
        }
        public string DumpWeight()
        {
            //return String.Format("P {0}\tN {1}\tUV {2}\tBI1 {3} BI2 {4} BI3 {5} W{6}", Common.ToString(Position), Common.ToString(Normal), Common.ToString(UV), BoneInfo1, BoneInfo2, BoneInfo3, Common.ByteArrayToString(Weights));
            //return String.Format("BI1 {0}\t BI2 {1}\t BI3 {2}\t W{3}", BoneInfo1, BoneInfo2, BoneInfo3, Common.ByteArrayToString(Weights));
            return String.Format("W1 {0:0.0000}\t W2 {1:0.0000}\t W3 {2:0.0000} {3},{4},{5}", Weight(0), Weight(1), Weight(2), AdjustBone(BoneIndices[0]), AdjustBone(BoneIndices[1]), AdjustBone(BoneIndices[2]));
        }

        public static int AdjustBone(int index)
        {
            if (index != -1)
            {
                Debug.Assert(index % 3 == 0);
                index /= 3;
            }
            return index+1;
        }

        public int ActiveWeights()
        {
            int result = 0;
            for (int i = 0; i < 3; ++i)
            {
                if (Weight(i) > 0.0f)
                {
                    result++;
                }
            }
            return result;
        }

        public float Weight(int index)
        {
            uint[] masks = { 0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000 };
            uint mask = masks[index];
            int a = (int)(BoneWeights & mask);
            a = a >> (index * 8);
            return (float)a / (float)255;
        }

    }

    public class MeshMaterial
    {
        public float val1;
        public int val2;
        public int val3;

        public static MeshMaterial FromStream(BinaryReader binReader)
        {
            MeshMaterial meshMaterial = new MeshMaterial();
            meshMaterial.val1 = binReader.ReadSingle();
            if (meshMaterial.val1 == 1.0f)
            {
                meshMaterial.val2 = binReader.ReadInt32();
                meshMaterial.val3 = binReader.ReadInt32();
            }
            else
            {
                binReader.BaseStream.Position -= 4;
                meshMaterial = null;
            }

            return meshMaterial;
        }
    }

    public class MaterialBlock
    {
        public int[] blockData = new int[11];

        public int Offset
        {
            get { return blockData[5]; }
        }

        public int Lod
        {
            get { return blockData[4]; }
        }


        public static MaterialBlock FromStream(BinaryReader binReader)
        {
            MaterialBlock materialBlock = new MaterialBlock();
            for (int i = 0; i < materialBlock.blockData.Length; ++i)
            {
                materialBlock.blockData[i] = binReader.ReadInt32();
            }
            return materialBlock;
        }
    }

    public class SubmeshData
    {
        public int index = 0;
        public SubMeshData1 subMeshData;
        public List<int> indices;
        //public XboxVertexInstance[] vertices;
        public List<int> verticesInMesh = new List<int>();

    }

}

