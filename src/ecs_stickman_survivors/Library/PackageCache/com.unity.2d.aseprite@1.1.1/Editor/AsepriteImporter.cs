using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEditor.AssetImporters;
using UnityEditor.U2D.Aseprite.Common;
using UnityEditor.U2D.Sprites;
using UnityEngine.Serialization;

namespace UnityEditor.U2D.Aseprite
{
    /// <summary>
    /// ScriptedImporter to import Aseprite files
    /// </summary>
    // Version using unity release + 5 digit padding for future upgrade. Eg 2021.2 -> 21200000
    [ScriptedImporter(21300002, new string[] {"aseprite", "ase"}, AllowCaching = true)]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.aseprite@latest")]
    public partial class AsepriteImporter : ScriptedImporter, ISpriteEditorDataProvider
    {
        [SerializeField] TextureImporterSettings m_TextureImporterSettings = new TextureImporterSettings()
        {
            mipmapEnabled = false,
            mipmapFilter = TextureImporterMipFilter.BoxFilter,
            sRGBTexture = true,
            borderMipmap = false,
            mipMapsPreserveCoverage = false,
            alphaTestReferenceValue = 0.5f,
            readable = false,

#if ENABLE_TEXTURE_STREAMING
            streamingMipmaps = false,
            streamingMipmapsPriority = 0,
#endif

            fadeOut = false,
            mipmapFadeDistanceStart = 1,
            mipmapFadeDistanceEnd = 3,

            convertToNormalMap = false,
            heightmapScale = 0.25F,
            normalMapFilter = 0,

            generateCubemap = TextureImporterGenerateCubemap.AutoCubemap,
            cubemapConvolution = 0,

            seamlessCubemap = false,

            npotScale = TextureImporterNPOTScale.ToNearest,

            spriteMode = (int) SpriteImportMode.Multiple,
            spriteExtrude = 1,
            spriteMeshType = SpriteMeshType.Tight,
            spriteAlignment = (int) SpriteAlignment.Center,
            spritePivot = new Vector2(0.5f, 0.5f),
            spritePixelsPerUnit = 100.0f,
            spriteBorder = new Vector4(0.0f, 0.0f, 0.0f, 0.0f),

            alphaSource = TextureImporterAlphaSource.FromInput,
            alphaIsTransparency = true,
            spriteTessellationDetail = -1.0f,

            textureType = TextureImporterType.Sprite,
            textureShape = TextureImporterShape.Texture2D,

            filterMode = FilterMode.Point,
            aniso = 1,
            mipmapBias = 0.0f,
            wrapModeU = TextureWrapMode.Clamp,
            wrapModeV = TextureWrapMode.Clamp,
            wrapModeW = TextureWrapMode.Clamp
        };


        [SerializeField] AsepriteImporterSettings m_PreviousAsepriteImporterSettings;
        [SerializeField] AsepriteImporterSettings m_AsepriteImporterSettings = new AsepriteImporterSettings()
        {
            fileImportMode = FileImportModes.AnimatedSprite,
            importHiddenLayers = false,
            layerImportMode = LayerImportModes.MergeFrame,
            defaultPivotAlignment = SpriteAlignment.BottomCenter,
            defaultPivotSpace = PivotSpaces.Canvas,
            customPivotPosition = new Vector2(0.5f, 0.5f),
            mosaicPadding = 4,
            spritePadding = 0,
            generateAnimationClips = true,
            generateModelPrefab = true,
            addSortingGroup = true,
            addShadowCasters = false
        };
        
        // Use for inspector to check if the file node is checked
        [SerializeField]
#pragma warning disable 169, 414
        bool m_ImportFileNodeState = true;
        
        // Used by platform settings to mark it dirty so that it will trigger a reimport
        [SerializeField]
#pragma warning disable 169, 414
        long m_PlatformSettingsDirtyTick;        
        
        [SerializeField] string m_TextureAssetName = null;
        
        [SerializeField] List<SpriteMetaData> m_SingleSpriteImportData = new List<SpriteMetaData>(1) { new SpriteMetaData() };
        [FormerlySerializedAs("m_MultiSpriteImportData")] 
        [SerializeField] List<SpriteMetaData> m_AnimatedSpriteImportData = new List<SpriteMetaData>();
        [SerializeField] List<SpriteMetaData> m_SpriteSheetImportData = new List<SpriteMetaData>();
        
        [SerializeField] List<Layer> m_AsepriteLayers = new List<Layer>();

        [SerializeField] List<TextureImporterPlatformSettings> m_PlatformSettings = new List<TextureImporterPlatformSettings>();
        
        [SerializeField] bool m_GeneratePhysicsShape = false;
        [SerializeField] SecondarySpriteTexture[] m_SecondarySpriteTextures;   
        [SerializeField] string m_SpritePackingTag = "";   
        
        SpriteImportMode spriteImportModeToUse => m_TextureImporterSettings.textureType != TextureImporterType.Sprite ? 
            SpriteImportMode.None : (SpriteImportMode)m_TextureImporterSettings.spriteMode;    
        
        AsepriteImportData m_ImportData;
        AsepriteFile m_AsepriteFile;
        List<Tag> m_Tags = new List<Tag>();
        List<Frame> m_Frames = new List<Frame>();
        
        [SerializeField] Vector2Int m_CanvasSize;

        GameObject m_RootGameObject;
        readonly Dictionary<int, GameObject> m_LayerIdToGameObject = new Dictionary<int, GameObject>(); 

        AsepriteImportData importData
        {
            get
            {
                var returnValue = m_ImportData;
                if (returnValue == null)
                    // Using LoadAllAssetsAtPath because AsepriteImportData is hidden
                    returnValue = AssetDatabase.LoadAllAssetsAtPath(assetPath).FirstOrDefault(x => x is AsepriteImportData) as AsepriteImportData;
                    
                if (returnValue == null)
                    returnValue = ScriptableObject.CreateInstance<AsepriteImportData>();
                    
                m_ImportData = returnValue;
                return returnValue;
            }
        }
        
        internal bool isNPOT => Mathf.IsPowerOfTwo(importData.textureActualWidth) && Mathf.IsPowerOfTwo(importData.textureActualHeight);
        
        internal int textureActualWidth
        {
            get => importData.textureActualWidth;
            private set => importData.textureActualWidth = value; 
        }

        internal int textureActualHeight
        {
            get => importData.textureActualHeight;
            private set => importData.textureActualHeight = value;
        }

        float definitionScale
        {
            get
            {
                var definitionScaleW = importData.importedTextureWidth / (float) textureActualWidth;
                var definitionScaleH = importData.importedTextureHeight / (float) textureActualHeight;
                return Mathf.Min(definitionScaleW, definitionScaleH);
            }
        }

        internal SecondarySpriteTexture[] secondaryTextures
        {
            get => m_SecondarySpriteTextures;
            set => m_SecondarySpriteTextures = value;
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if(m_ImportData == null)
                m_ImportData = ScriptableObject.CreateInstance<AsepriteImportData>();
            m_ImportData.hideFlags = HideFlags.HideInHierarchy;
            
            try
            {
                m_AsepriteFile = AsepriteReader.ReadFile(ctx.assetPath);
                if (m_AsepriteFile == null)
                    return;
                
                m_CanvasSize = new Vector2Int(m_AsepriteFile.width, m_AsepriteFile.height);
                
                var newLayers = RestructureImportData(in m_AsepriteFile);
                FilterOutLayers(ref newLayers);
                UpdateCellNames(ref newLayers);
                m_Frames = ExtractFrameData(in m_AsepriteFile);
                m_Tags = ExtractTagsData(in m_AsepriteFile);

                if (newLayers.Count == 0)
                    return;

                var assetName = System.IO.Path.GetFileNameWithoutExtension(ctx.assetPath);
                
                List<NativeArray<Color32>> cellBuffers;
                List<int> cellWidth;
                List<int> cellHeight;
                if (layerImportMode == LayerImportModes.IndividualLayers)
                {
                    m_AsepriteLayers = UpdateLayers(in newLayers, in m_AsepriteLayers);
                    ImportLayers.Import(m_AsepriteLayers, out cellBuffers, out cellWidth, out cellHeight);
                }
                else
                {
                    ImportMergedLayers.Import(assetName, ref newLayers, out cellBuffers, out cellWidth, out cellHeight);
                    // Update layers after merged, since merged import creates new layers.
                    // The new layers should be compared and merged together with the ones existing in the meta file. 
                    m_AsepriteLayers = UpdateLayers(in newLayers, in m_AsepriteLayers);
                }

                var mosaicPad = m_AsepriteImporterSettings.mosaicPadding;
                var spritePad = m_AsepriteImporterSettings.fileImportMode == FileImportModes.AnimatedSprite ? m_AsepriteImporterSettings.spritePadding : 0;
                
                var requireSquarePotTexture = IsRequiringSquarePotTexture(ctx);
                ImagePacker.Pack(cellBuffers.ToArray(), cellWidth.ToArray(), cellHeight.ToArray(), (int)mosaicPad, spritePad, requireSquarePotTexture, out var outputImageBuffer, out var packedTextureWidth, out var packedTextureHeight, out var spriteRects, out var uvTransforms);
                
                var packOffsets = new Vector2Int[spriteRects.Length];
                for (var i = 0; i < packOffsets.Length; ++i)
                {
                    packOffsets[i] = new Vector2Int(uvTransforms[i].x - spriteRects[i].position.x, uvTransforms[i].y - spriteRects[i].position.y);
                    packOffsets[i] *= -1;
                }

                var spriteImportData = UpdateSpriteImportData(in m_AsepriteLayers, spriteRects, packOffsets, uvTransforms);

                importData.importedTextureHeight = textureActualHeight = packedTextureHeight;
                importData.importedTextureWidth = textureActualWidth = packedTextureWidth;
                
                var output = TextureGeneration.Generate(
                    ctx, 
                    outputImageBuffer, 
                    packedTextureWidth, 
                    packedTextureHeight, 
                    spriteImportData.ToArray(),
                    in m_PlatformSettings,
                    in m_TextureImporterSettings,
                    m_SpritePackingTag,
                    secondaryTextures);
                
                if (output.texture)
                {
                    importData.importedTextureHeight = output.texture.height;
                    importData.importedTextureWidth = output.texture.width;
                }

                if (output.texture != null && output.sprites != null)
                    SetPhysicsOutline(GetDataProvider<ISpritePhysicsOutlineDataProvider>(), output.sprites, definitionScale, pixelsPerUnit, m_GeneratePhysicsShape);
                
                RegisterAssets(ctx, output);
                OnPostAsepriteImport?.Invoke(new ImportEventArgs(this, ctx));
                
                outputImageBuffer.DisposeIfCreated();
                foreach (var cellBuffer in cellBuffers)
                    cellBuffer.DisposeIfCreated();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to import file {assetPath}. Error: {e.Message} \n{e.StackTrace}");
            }
            finally
            {
                m_PreviousAsepriteImporterSettings = m_AsepriteImporterSettings;
                EditorUtility.SetDirty(this);
                m_AsepriteFile?.Dispose();
            }
        }

        List<Layer> RestructureImportData(in AsepriteFile file)
        {
            var frameData = file.frameData;

            var nameGenerator = new UniqueNameGenerator();
            var layers = new List<Layer>();
            var parentTable = new Dictionary<int, Layer>();
            for (var i = 0; i < frameData.Count; ++i)
            {
                var chunks = frameData[i].chunks;
                for (var m = 0; m < chunks.Count; ++m)
                {
                    if (chunks[m].chunkType == ChunkTypes.Layer)
                    {
                        var layerChunk = chunks[m] as LayerChunk;

                        var layer = new Layer();
                        var childLevel = layerChunk.childLevel;
                        parentTable[childLevel] = layer;
                        
                        layer.parentIndex = childLevel == 0 ? -1 : parentTable[childLevel - 1].index;
                        
                        layer.name = nameGenerator.GetUniqueName(layerChunk.name, layer.parentIndex);
                        layer.layerFlags = layerChunk.flags;
                        layer.layerType = layerChunk.layerType;
                        layer.blendMode = layerChunk.blendMode;
                        layer.opacity = layerChunk.opacity / 255f;
                        layer.index = layers.Count;
                        layer.guid = Layer.GenerateGuid(layer);

                        layers.Add(layer);
                    }
                }
            }
            
            for (var i = 0; i < frameData.Count; ++i)
            {
                var chunks = frameData[i].chunks;
                for (var m = 0; m < chunks.Count; ++m)
                {
                    if (chunks[m].chunkType == ChunkTypes.Cell)
                    {
                        var cellChunk = chunks[m] as CellChunk;
                        var layer = layers.Find(x => x.index == cellChunk.layerIndex);
                        if (layer == null)
                        {
                            Debug.LogWarning($"Could not find the layer for one of the cells. Frame Index={i}, Chunk Index={m}.");
                            continue;
                        }

                        var cellType = cellChunk.cellType;
                        if (cellType == CellTypes.LinkedCell)
                        {
                            var cell = new LinkedCell();
                            cell.frameIndex = i;
                            cell.linkedToFrame = cellChunk.linkedToFrame;
                            layer.linkedCells.Add(cell);   
                        }
                        else
                        {
                            var cell = new Cell();
                            cell.frameIndex = i;
                            cell.updatedCellRect = false;

                            // Flip Y. Aseprite 0,0 is at Top Left. Unity 0,0 is at Bottom Left. 
                            var cellY = (m_CanvasSize.y - cellChunk.posY) - cellChunk.height;
                            cell.cellRect = new RectInt(cellChunk.posX, cellY, cellChunk.width, cellChunk.height);
                            cell.opacity = cellChunk.opacity / 255f;
                            cell.blendMode = layer.blendMode;
                            cell.image = cellChunk.image;
                            cell.name = layer.name;
                            cell.spriteId = GUID.Generate();

                            var opacity = cell.opacity * layer.opacity;
                            if ((1f - opacity) > Mathf.Epsilon)
                                TextureTasks.AddOpacity(ref cell.image, opacity);
                            
                            layer.cells.Add(cell);   
                        }
                    }
                }
            }

            return layers;
        }

        void FilterOutLayers(ref List<Layer> layers)
        {
            for (var i = layers.Count - 1; i >= 0; --i)
            {
                var layer = layers[i];
                if (!includeHiddenLayers && !ImportUtilities.IsLayerVisible(layer.index, in layers))
                {
                    DisposeCellsInLayer(layer);
                    layers.RemoveAt(i);
                    continue;
                }
                
                var cells = layer.cells;
                for (var m = cells.Count - 1; m >= 0; --m)
                {
                    var width = cells[m].cellRect.width;
                    var height = cells[m].cellRect.width;
                    if (width == 0 || height == 0)
                        cells.RemoveAt(m);
                    else if (cells[m].image == default || !cells[m].image.IsCreated)
                        cells.RemoveAt(m);
                }
            }
        }

        static void DisposeCellsInLayer(Layer layer)
        {
            foreach (var cell in layer.cells)
            {
                var image = cell.image;
                image.DisposeIfCreated();
            }
        }

        static void UpdateCellNames(ref List<Layer> layers)
        {
            for (var i = 0; i < layers.Count; ++i)
            {
                var cells = layers[i].cells;
                for (var m = 0; m < cells.Count; ++m)
                {
                    var cell = cells[m];
                    cell.name = ImportUtilities.GetCellName(cell.name, cell.frameIndex, cells.Count);
                    cells[m] = cell;
                }
            }
        }

        static List<Layer> UpdateLayers(in List<Layer> newLayers, in List<Layer> oldLayers)
        {
            if (oldLayers.Count == 0)
                return new List<Layer>(newLayers);

            var finalLayers = new List<Layer>(oldLayers);
            
            // Remove old layers
            for (var i = 0; i < oldLayers.Count; ++i)
            {
                var oldLayer = oldLayers[i];
                if (newLayers.FindIndex(x => x.guid == oldLayer.guid) == -1)
                    finalLayers.Remove(oldLayer);
            }
            
            // Add new layers
            for (var i = 0; i < newLayers.Count; ++i)
            {
                var newLayer = newLayers[i];
                var layerIndex = finalLayers.FindIndex(x => x.guid == newLayer.guid);
                if (layerIndex == -1)
                    finalLayers.Add(newLayer);
            }
            
            // Update layer data
            for (var i = 0; i < finalLayers.Count; ++i)
            {
                var finalLayer = finalLayers[i];
                var layerIndex = newLayers.FindIndex(x => x.guid == finalLayer.guid);
                if (layerIndex != -1)
                {
                    var oldCells = finalLayer.cells;
                    var newCells = newLayers[layerIndex].cells;
                    for (var m = 0; m < newCells.Count; ++m)
                    {
                        if (m < oldCells.Count)
                        {
                            var oldCell = oldCells[m];
                            var newCell = newCells[m];
                            newCell.spriteId = oldCell.spriteId;
#if UNITY_2023_1_OR_NEWER                            
                            newCell.updatedCellRect = newCell.cellRect != oldCell.cellRect;
#else
                            newCell.updatedCellRect = !newCell.cellRect.IsEqual(oldCell.cellRect);
#endif
                            newCells[m] = newCell;
                        }
                    }
                    finalLayer.cells = new List<Cell>(newCells);
                    finalLayer.linkedCells = new List<LinkedCell>(newLayers[layerIndex].linkedCells);
                    finalLayer.index = newLayers[layerIndex].index;
                    finalLayer.opacity = newLayers[layerIndex].opacity;
                    finalLayer.parentIndex = newLayers[layerIndex].parentIndex;
                }
            }

            return finalLayers;
        }

        bool IsRequiringSquarePotTexture(AssetImportContext ctx)
        {
            var platformSettings = TextureImporterUtilities.GetPlatformTextureSettings(ctx.selectedBuildTarget, in m_PlatformSettings);
            return (TextureImporterFormat.PVRTC_RGB2 <= platformSettings.format && platformSettings.format <= TextureImporterFormat.PVRTC_RGBA4);            
        }

        static List<Frame> ExtractFrameData(in AsepriteFile file)
        {
            var noOfFrames = file.noOfFrames;
            var frames = new List<Frame>(noOfFrames);
            for (var i = 0; i < noOfFrames; ++i)
            {
                var frameData = file.frameData[i];
                var eventStrings = ExtractEventStringFromCells(frameData);
                
                var frame = new Frame()
                {
                    duration = frameData.frameDuration,
                    eventStrings = eventStrings
                };
                frames.Add(frame);
            }

            return frames;
        }

        static string[] ExtractEventStringFromCells(FrameData frameData)
        {
            var chunks = frameData.chunks;
            var eventStrings = new HashSet<string>();
            for (var i = 0; i < chunks.Count; ++i)
            {
                if (chunks[i].chunkType != ChunkTypes.Cell)
                    continue;
                var cellChunk = (CellChunk)chunks[i];
                if (cellChunk.dataChunk == null)
                    continue;
                var dataText = cellChunk.dataChunk.text;
                if (string.IsNullOrEmpty(dataText) || !dataText.StartsWith("event:"))
                    continue;
                var eventString = dataText.Remove(0, "event:".Length);
                eventString = eventString.Trim(' ');
                eventStrings.Add(eventString);
            }
            return eventStrings.ToArray();
        }

        static List<Tag> ExtractTagsData(in AsepriteFile file)
        {
            var tags = new List<Tag>();
            
            var noOfFrames = file.noOfFrames;
            for (var i = 0; i < noOfFrames; ++i)
            {
                var frame = file.frameData[i];
                var noOfChunks = frame.chunkCount;
                for (var m = 0; m < noOfChunks; ++m)
                {
                    var chunk = frame.chunks[m];
                    if (chunk.chunkType != ChunkTypes.Tags)
                        continue;
                    
                    var tagChunk = chunk as TagsChunk;
                    var noOfTags = tagChunk.noOfTags;
                    for (var n = 0; n < noOfTags; ++n)
                    {
                        var data = tagChunk.tagData[n];
                        var tag = new Tag();
                        tag.name = data.name;
                        tag.noOfRepeats = data.noOfRepeats;
                        tag.fromFrame = data.fromFrame;
                        // Adding one more frame as Aseprite's tags seems to always be 1 short.
                        tag.toFrame = data.toFrame + 1;
                        
                        tags.Add(tag);
                    }
                }
            }

            return tags;
        }

        List<SpriteMetaData> UpdateSpriteImportData(in List<Layer> layers, RectInt[] spriteRects, Vector2Int[] packOffsets, Vector2Int[] uvTransforms)
        {
            if (m_AsepriteImporterSettings.fileImportMode == FileImportModes.SpriteSheet)
            {
                return GetSpriteImportData();
            }
            
            var cellLookup = new List<Cell>();
            for (var i = 0; i < layers.Count; ++i)
                cellLookup.AddRange(layers[i].cells);

            var spriteImportData = GetSpriteImportData();
            if (spriteImportData.Count <= 0)
            {
                var newSpriteMeta = new List<SpriteMetaData>();

                for (var i = 0; i < spriteRects.Length; ++i)
                {
                    var cell = cellLookup[i];
                    var spriteData = CreateNewSpriteMetaData(in cell, in spriteRects[i], packOffsets[i], in uvTransforms[i]);
                    newSpriteMeta.Add(spriteData);
                }
                spriteImportData.Clear();
                spriteImportData.AddRange(newSpriteMeta);
            }
            else
            {
                // Remove old cells
                for (var i = spriteImportData.Count - 1; i >= 0; --i)
                {
                    var spriteData = spriteImportData[i];
                    if (cellLookup.FindIndex(x => x.spriteId == spriteData.spriteID) == -1)
                        spriteImportData.Remove(spriteData);
                }                
                
                // Add new cells
                for (var i = 0; i < cellLookup.Count; ++i)
                {
                    var cell = cellLookup[i];
                    if (spriteImportData.FindIndex(x => x.spriteID == cell.spriteId) == -1)
                    {
                        var spriteData = CreateNewSpriteMetaData(in cell, spriteRects[i], packOffsets[i], uvTransforms[i]);
                        spriteImportData.Add(spriteData);
                    }
                }
                
                // Update with new pack data
                for (var i = 0; i < cellLookup.Count; ++i)
                {
                    var cell = cellLookup[i];
                    var spriteData = spriteImportData.Find(x => x.spriteID == cell.spriteId);
                    if (spriteData != null)
                    {
                        var areSettingsUpdated = !m_PreviousAsepriteImporterSettings.IsDefault() &&
                                                 (pivotAlignment != m_PreviousAsepriteImporterSettings.defaultPivotAlignment ||
                                                  pivotSpace != m_PreviousAsepriteImporterSettings.defaultPivotSpace ||
                                                  customPivotPosition != m_PreviousAsepriteImporterSettings.customPivotPosition);
                        
                        // Update pivot if either the importer settings are updated
                        // or the source files rect has been changed (Only for Canvas, as rect position doesn't matter in local). 
                        if (pivotSpace == PivotSpaces.Canvas && 
                            (areSettingsUpdated || cell.updatedCellRect))
                        {
                            spriteData.alignment = SpriteAlignment.Custom;

                            var cellRect = cell.cellRect;
                            cellRect.x += packOffsets[i].x;
                            cellRect.y += packOffsets[i].y;
                            cellRect.width = spriteRects[i].width;
                            cellRect.height = spriteRects[i].height;
                            
                            spriteData.pivot = ImportUtilities.CalculateCellPivot(cellRect, m_CanvasSize, pivotAlignment, customPivotPosition);
                        }
                        else if (pivotSpace == PivotSpaces.Local && areSettingsUpdated)
                        {
                            spriteData.alignment = pivotAlignment;
                            spriteData.pivot = customPivotPosition;
                        }

                        spriteData.rect = new Rect(spriteRects[i].x, spriteRects[i].y, spriteRects[i].width, spriteRects[i].height);
                        spriteData.uvTransform = uvTransforms[i];
                    }
                }                
            }

            return spriteImportData;
        }

        SpriteMetaData CreateNewSpriteMetaData(in Cell cell, in RectInt spriteRect, in Vector2Int packOffset, in Vector2Int uvTransform)
        {
            var spriteData = new SpriteMetaData();
            spriteData.border = Vector4.zero;

            if (pivotSpace == PivotSpaces.Canvas)
            {
                spriteData.alignment = SpriteAlignment.Custom;
                
                var cellRect = cell.cellRect;
                cellRect.x += packOffset.x;
                cellRect.y += packOffset.y;
                cellRect.width = spriteRect.width;
                cellRect.height = spriteRect.height;

                spriteData.pivot = ImportUtilities.CalculateCellPivot(cellRect, m_CanvasSize, pivotAlignment, customPivotPosition);
            }
            else
            {
                spriteData.alignment = pivotAlignment;
                spriteData.pivot = customPivotPosition;
            }

            spriteData.rect = new Rect(spriteRect.x, spriteRect.y, spriteRect.width, spriteRect.height);
            spriteData.spriteID = cell.spriteId;
            spriteData.name = cell.name;
            spriteData.uvTransform = uvTransform;
            return spriteData;
        }
        
       static void SetPhysicsOutline(ISpritePhysicsOutlineDataProvider physicsOutlineDataProvider, Sprite[] sprites, float definitionScale, float pixelsPerUnit, bool generatePhysicsShape)
        {
            foreach (var sprite in sprites)
            {
                var guid = sprite.GetSpriteID();
                var outline = physicsOutlineDataProvider.GetOutlines(guid);

                var outlineOffset = sprite.pivot;
                var generated = false;
                if ((outline == null || outline.Count == 0) && generatePhysicsShape)
                {
                    InternalEditorBridge.GenerateOutlineFromSprite(sprite, 0.25f, 200, true, out var defaultOutline);
                    outline = new List<Vector2[]>(defaultOutline.Length);
                    for (var i = 0; i < defaultOutline.Length; ++i)
                    {
                        outline.Add(defaultOutline[i]);
                    }

                    generated = true;
                }
                if (outline != null && outline.Count > 0)
                {
                    // Ensure that outlines are all valid.
                    var validOutlineCount = 0;
                    for (var i = 0; i < outline.Count; ++i)
                        validOutlineCount += ( (outline[i].Length > 2) ? 1 : 0 );

                    var index = 0;
                    var convertedOutline = new Vector2[validOutlineCount][];
                    var useScale = generated ? pixelsPerUnit * definitionScale : definitionScale;

                    for (var i = 0; i < outline.Count; ++i)
                    {
                        if (outline[i].Length > 2)
                        {
                            convertedOutline[index] = new Vector2[outline[i].Length];
                            for (var j = 0; j < outline[i].Length; ++j)
                            {
                                convertedOutline[index][j] = outline[i][j] * useScale + outlineOffset;
                            }
                            index++;
                        }
                    }
                    sprite.OverridePhysicsShape(convertedOutline);
                }
            }
        }

        void RegisterAssets(AssetImportContext ctx, TextureGenerationOutput output)
        {
            if ((output.sprites == null || output.sprites.Length == 0) && output.texture == null)
            {
                Debug.LogWarning(TextContent.noSpriteOrTextureImportWarning, this);
                return;
            }

            var assetNameGenerator = new UniqueNameGenerator();
            if (!string.IsNullOrEmpty(output.importInspectorWarnings))
            {
                Debug.LogWarning(output.importInspectorWarnings);
            }
            if (output.importWarnings != null && output.importWarnings.Length != 0)
            {
                foreach (var warning in output.importWarnings)
                    Debug.LogWarning(warning);
            }
            if (output.thumbNail == null)
                Debug.LogWarning("Thumbnail generation fail");
            if (output.texture == null)
            {
                throw new Exception("Texture import fail");
            }

            var assetName = assetNameGenerator.GetUniqueName(System.IO.Path.GetFileNameWithoutExtension(ctx.assetPath), -1, true, this);
            UnityEngine.Object mainAsset = null;

            RegisterTextureAsset(ctx, output, assetName, ref mainAsset);
            RegisterSprites(ctx, output, assetNameGenerator);
            RegisterGameObjects(ctx, output, ref mainAsset);
            RegisterAnimationClip(ctx, assetName, output);
            RegisterAnimatorController(ctx, assetName);

            ctx.AddObjectToAsset("AsepriteImportData", m_ImportData);
            ctx.SetMainObject(mainAsset);
        }
        
        void RegisterTextureAsset(AssetImportContext ctx, TextureGenerationOutput output, string assetName, ref UnityEngine.Object mainAsset)
        {
            var registerTextureNameId = string.IsNullOrEmpty(m_TextureAssetName) ? "Texture" : m_TextureAssetName;

            output.texture.name = assetName;
            ctx.AddObjectToAsset(registerTextureNameId, output.texture, output.thumbNail);
            mainAsset = output.texture;
        }

        static void RegisterSprites(AssetImportContext ctx, TextureGenerationOutput output, UniqueNameGenerator assetNameGenerator)
        {
            if (output.sprites == null)
                return;
            
            foreach (var sprite in output.sprites)
            {
                var spriteGuid = sprite.GetSpriteID().ToString();
                var spriteAssetName = assetNameGenerator.GetUniqueName(spriteGuid, -1, false, sprite);
                ctx.AddObjectToAsset(spriteAssetName, sprite);
            }            
        }

        void RegisterGameObjects(AssetImportContext ctx, TextureGenerationOutput output, ref UnityEngine.Object mainAsset)
        {
            if (output.sprites.Length == 0)
                return;
            if (m_AsepriteImporterSettings.fileImportMode != FileImportModes.AnimatedSprite)
                return;
            
            PrefabGeneration.Generate(
                ctx, 
                output, 
                m_AsepriteLayers, 
                m_LayerIdToGameObject,
                m_CanvasSize,
                m_AsepriteImporterSettings,
                ref mainAsset, 
                out m_RootGameObject);
        }

        void RegisterAnimationClip(AssetImportContext ctx, string assetName, TextureGenerationOutput output)
        {
            if (output.sprites.Length == 0)
                return;
            if (m_AsepriteImporterSettings.fileImportMode != FileImportModes.AnimatedSprite)
                return;            
            if (!generateAnimationClips)
                return;
            var noOfFrames = m_AsepriteFile.noOfFrames;
            if (noOfFrames == 1)
                return;

            var sprites = output.sprites;
            var clips = AnimationClipGeneration.Generate(
                assetName, 
                sprites, 
                m_AsepriteFile, 
                m_AsepriteLayers, 
                m_Frames,
                m_Tags, 
                m_LayerIdToGameObject);
            
            for (var i = 0; i < clips.Length; ++i)
                ctx.AddObjectToAsset(clips[i].name, clips[i]);
        }

        void RegisterAnimatorController(AssetImportContext ctx, string assetName)
        {
            if (m_AsepriteImporterSettings.fileImportMode != FileImportModes.AnimatedSprite)
                return;
            
            AnimatorControllerGeneration.Generate(ctx, assetName, m_RootGameObject, generateModelPrefab);
        }

        internal void Apply()
        {
            // Do this so that asset change save dialog will not show
            var originalValue = EditorPrefs.GetBool("VerifySavingAssets", false);
            EditorPrefs.SetBool("VerifySavingAssets", false);
            AssetDatabase.ForceReserializeAssets(new string[] { assetPath }, ForceReserializeAssetsOptions.ReserializeMetadata);
            EditorPrefs.SetBool("VerifySavingAssets", originalValue);
        }

        public override bool SupportsRemappedAssetType(Type type)
        {
            if (type == typeof(AnimationClip))
                return true;
            return base.SupportsRemappedAssetType(type);
        }

        void SetPlatformTextureSettings(TextureImporterPlatformSettings platformSettings)
        {
            var index = m_PlatformSettings.FindIndex(x => x.name == platformSettings.name);
            if(index < 0)
                m_PlatformSettings.Add(platformSettings);
            else
                m_PlatformSettings[index] = platformSettings;
        }
        
        void SetDirty()
        {
            EditorUtility.SetDirty(this);
        }

        List<SpriteMetaData> GetSpriteImportData()
        {
            if (spriteImportModeToUse == SpriteImportMode.Multiple)
            {
                switch (m_AsepriteImporterSettings.fileImportMode)
                {
                    case FileImportModes.SpriteSheet:
                        return m_SpriteSheetImportData;
                    case FileImportModes.AnimatedSprite:
                    default:
                        return m_AnimatedSpriteImportData;
                }
            }
            return m_SingleSpriteImportData;
        }

        internal SpriteRect GetSpriteData(GUID guid)
        {
            if (spriteImportModeToUse == SpriteImportMode.Multiple)
            {
                switch (m_AsepriteImporterSettings.fileImportMode)
                {
                    case FileImportModes.SpriteSheet:
                        return m_SpriteSheetImportData.FirstOrDefault(x => x.spriteID == guid);
                    case FileImportModes.AnimatedSprite:
                    default:
                        return m_AnimatedSpriteImportData.FirstOrDefault(x => x.spriteID == guid);
                }
            }
            return m_SingleSpriteImportData[0];
        }
        
        internal TextureImporterPlatformSettings[] GetAllPlatformSettings()
        {
            return m_PlatformSettings.ToArray();
        }
        
        internal void ReadTextureSettings(TextureImporterSettings dest)
        {
            m_TextureImporterSettings.CopyTo(dest);
        }        
    }
}