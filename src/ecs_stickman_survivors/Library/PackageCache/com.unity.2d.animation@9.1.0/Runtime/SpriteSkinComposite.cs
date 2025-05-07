using System;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.U2D.Common;
using Unity.Burst;
using Unity.Profiling;
using UnityEngine.Assertions;

namespace UnityEngine.U2D.Animation
{
    internal struct PerSkinJobData
    {
        public int deformVerticesStartPos;
        public int2 bindPosesIndex;
        public int2 verticesIndex;
    }

    internal struct SpriteSkinData
    {
        public NativeCustomSlice<Vector3> vertices;
        public NativeCustomSlice<BoneWeight> boneWeights;
        public NativeCustomSlice<Matrix4x4> bindPoses;
        public NativeCustomSlice<Vector4> tangents;
        public bool hasTangents;
        public int spriteVertexStreamSize;
        public int spriteVertexCount;
        public int tangentVertexOffset;
        public int deformVerticesStartPos;
        public int transformId;
        public NativeCustomSlice<int> boneTransformId;
    }
    
    [BurstCompile]
    internal struct PrepareDeformJob :IJob
    {
        [ReadOnly]
        public NativeArray<PerSkinJobData> perSkinJobData;
        [ReadOnly]
        public int batchDataSize;
        [WriteOnly]
        public NativeArray<int2> boneLookupData;
        [WriteOnly]
        public NativeArray<int2> vertexLookupData;

        public void Execute()
        {
            for (var i = 0; i < batchDataSize; ++i)
            {
                var jobData = perSkinJobData[i];
                for (int k = 0, j = jobData.bindPosesIndex.x; j < jobData.bindPosesIndex.y; ++j, ++k)
                {
                    boneLookupData[j] = new int2(i, k);
                }
                for (int k = 0, j = jobData.verticesIndex.x; j < jobData.verticesIndex.y; ++j, ++k)
                {
                    vertexLookupData[j] = new int2(i, k);
                }
            }
        }
    }
    
    [BurstCompile]
    internal struct BoneDeformBatchedJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float4x4> boneTransform;
        [ReadOnly]
        public NativeArray<float4x4> rootTransform;
        [ReadOnly]
        public NativeArray<int2> boneLookupData;
        [ReadOnly]
        public NativeArray<SpriteSkinData> spriteSkinData;
        [ReadOnly]
        public NativeHashMap<int, TransformAccessJob.TransformData> rootTransformIndex;
        [ReadOnly]
        public NativeHashMap<int, TransformAccessJob.TransformData> boneTransformIndex;
        [WriteOnly]
        public NativeArray<float4x4> finalBoneTransforms;

        public void Execute(int i)
        {
            var x = boneLookupData[i].x;
            var y = boneLookupData[i].y;
            var ssd = spriteSkinData[x];
            var v = ssd.boneTransformId[y];
            var index = boneTransformIndex[v].transformIndex;
            if (index < 0)
                return;
            var aa = boneTransform[index];
            var bb = ssd.bindPoses[y];
            var cc = rootTransformIndex[ssd.transformId].transformIndex;
            finalBoneTransforms[i] = math.mul(rootTransform[cc], math.mul(aa, bb));
        }
    }
    
    [BurstCompile]
    internal struct SkinDeformBatchedJob : IJobParallelFor
    {
        public NativeSlice<byte> vertices;

        [ReadOnly]
        public NativeArray<float4x4> finalBoneTransforms;
        [ReadOnly]
        public NativeArray<PerSkinJobData> perSkinJobData;
        [ReadOnly]
        public NativeArray<SpriteSkinData> spriteSkinData;
        [ReadOnly]
        public NativeArray<int2> vertexLookupData;

        public unsafe void Execute(int i)
        {
            var j = vertexLookupData[i].x;
            var k = vertexLookupData[i].y;

            var perSkinData = perSkinJobData[j];
            var spriteSkin = spriteSkinData[j];
            var srcVertex = (float3)spriteSkin.vertices[k];
            var tangents = (float4)spriteSkin.tangents[k];
            var influence = spriteSkin.boneWeights[k];

            var bone0 = influence.boneIndex0 + perSkinData.bindPosesIndex.x;
            var bone1 = influence.boneIndex1 + perSkinData.bindPosesIndex.x;
            var bone2 = influence.boneIndex2 + perSkinData.bindPosesIndex.x;
            var bone3 = influence.boneIndex3 + perSkinData.bindPosesIndex.x;

            var deformedPosOffset = (byte*)vertices.GetUnsafePtr();
            var deformedPosStart = deformedPosOffset + spriteSkin.deformVerticesStartPos;
            var deformableVerticesFloat3 = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float3>(deformedPosStart, spriteSkin.spriteVertexStreamSize, spriteSkin.spriteVertexCount);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref deformableVerticesFloat3, NativeSliceUnsafeUtility.GetAtomicSafetyHandle(vertices));
#endif
            if (spriteSkin.hasTangents)
            {
                var deformedTanOffset = deformedPosStart + spriteSkin.tangentVertexOffset;
                var deformableTangentsFloat4 = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float4>(deformedTanOffset, spriteSkin.spriteVertexStreamSize, spriteSkin.spriteVertexCount);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref deformableTangentsFloat4, NativeSliceUnsafeUtility.GetAtomicSafetyHandle(vertices));
#endif
                var tangent = new float4(tangents.xyz, 0.0f);
                tangent =
                    math.mul(finalBoneTransforms[bone0], tangent) * influence.weight0 +
                    math.mul(finalBoneTransforms[bone1], tangent) * influence.weight1 +
                    math.mul(finalBoneTransforms[bone2], tangent) * influence.weight2 +
                    math.mul(finalBoneTransforms[bone3], tangent) * influence.weight3;
                deformableTangentsFloat4[k] = new float4(math.normalize(tangent.xyz), tangents.w);
            }
            
            deformableVerticesFloat3[k] =
                math.transform(finalBoneTransforms[bone0], srcVertex) * influence.weight0 +
                math.transform(finalBoneTransforms[bone1], srcVertex) * influence.weight1 +
                math.transform(finalBoneTransforms[bone2], srcVertex) * influence.weight2 +
                math.transform(finalBoneTransforms[bone3], srcVertex) * influence.weight3;
        }
    }
    
    [BurstCompile]
    internal struct CalculateSpriteSkinAABBJob : IJobParallelFor
    {
        public NativeSlice<byte> vertices;
        [ReadOnly] 
        public NativeArray<bool> isSpriteSkinValidForDeformArray;
        [ReadOnly]
        public NativeArray<SpriteSkinData> spriteSkinData;

        [WriteOnly]
        public NativeArray<Bounds> bounds;

        public unsafe void Execute(int i)
        {
            if (!isSpriteSkinValidForDeformArray[i])
                return;

            var spriteSkin = spriteSkinData[i];
            var deformedPosOffset = (byte*)vertices.GetUnsafePtr();
            var deformableVerticesFloat3 = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float3>(deformedPosOffset + spriteSkin.deformVerticesStartPos, spriteSkin.spriteVertexStreamSize, spriteSkin.spriteVertexCount);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref deformableVerticesFloat3, NativeSliceUnsafeUtility.GetAtomicSafetyHandle(vertices));
#endif

            bounds[i] = SpriteSkinUtility.CalculateSpriteSkinBounds(deformableVerticesFloat3);
        }
    }
    
    [BurstCompile]
    internal struct FillPerSkinJobSingleThread : IJob
    {
        public PerSkinJobData combinedSkinBatch;

        [ReadOnly]
        public NativeArray<bool> isSpriteSkinValidForDeformArray;

        public NativeArray<SpriteSkinData> spriteSkinDataArray;
        public NativeArray<PerSkinJobData> perSkinJobDataArray;

        public NativeArray<PerSkinJobData> combinedSkinBatchArray;

        public void Execute()
        {
            var startIndex = 0;
            var endIndex = spriteSkinDataArray.Length;
            for (var index = startIndex; index < endIndex; ++index)
            {
                var spriteSkinData = spriteSkinDataArray[index];
                spriteSkinData.deformVerticesStartPos = -1;
                var vertexBufferSize = 0;
                var vertexCount = 0;
                var bindPoseCount = 0;
                if (isSpriteSkinValidForDeformArray[index])
                {
                    spriteSkinData.deformVerticesStartPos = combinedSkinBatch.deformVerticesStartPos;
                    vertexBufferSize = spriteSkinData.spriteVertexCount * spriteSkinData.spriteVertexStreamSize;
                    vertexCount = spriteSkinData.spriteVertexCount;
                    bindPoseCount = spriteSkinData.bindPoses.Length;
                }
                combinedSkinBatch.verticesIndex.x = combinedSkinBatch.verticesIndex.y;
                combinedSkinBatch.verticesIndex.y = combinedSkinBatch.verticesIndex.x + vertexCount;
                combinedSkinBatch.bindPosesIndex.x = combinedSkinBatch.bindPosesIndex.y;
                combinedSkinBatch.bindPosesIndex.y = combinedSkinBatch.bindPosesIndex.x + bindPoseCount;
                spriteSkinDataArray[index] = spriteSkinData;
                perSkinJobDataArray[index] = combinedSkinBatch;
                combinedSkinBatch.deformVerticesStartPos += vertexBufferSize;
            }
            combinedSkinBatchArray[0] = combinedSkinBatch;
        }
    }
    
    [BurstCompile]
    internal struct CopySpriteRendererBuffersJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<bool> isSpriteSkinValidForDeformArray;
        [ReadOnly]
        public NativeArray<SpriteSkinData> spriteSkinData;
        [ReadOnly, NativeDisableUnsafePtrRestriction]
        public IntPtr ptrVertices;

        [WriteOnly]
        public NativeArray<IntPtr> buffers;
        [WriteOnly]
        public NativeArray<int> bufferSizes;
        
        public void Execute(int i)
        {
            var skinData = spriteSkinData[i];
            var startVertices = default(IntPtr);
            var vertexBufferLength = 0;
            if (isSpriteSkinValidForDeformArray[i])
            {
                startVertices = ptrVertices + skinData.deformVerticesStartPos;
                vertexBufferLength = skinData.spriteVertexCount * skinData.spriteVertexStreamSize;
            }
            buffers[i] = startVertices;
            bufferSizes[i] = vertexBufferLength;
        }
    }    

    internal class SpriteSkinComposite : ScriptableObject
    {
        static class Profiling
        {
            public static readonly ProfilerMarker batchAddSpriteSkin = new ProfilerMarker("SpriteSkinComposite.BatchAddSpriteSkin");
            public static readonly ProfilerMarker batchRemoveSpriteSkin = new ProfilerMarker("SpriteSkinComposite.BatchRemoveSpriteSkin");
            public static readonly ProfilerMarker prepareData = new ProfilerMarker("SpriteSkinComposite.PrepareData");
            public static readonly ProfilerMarker validateSpriteSkinData = new ProfilerMarker("SpriteSkinComposite.ValidateSpriteSkinData");
            public static readonly ProfilerMarker transformAccessJob = new ProfilerMarker("SpriteSkinComposite.TransformAccessJob");
            public static readonly ProfilerMarker getSpriteSkinBatchData = new ProfilerMarker("SpriteSkinComposite.GetSpriteSkinBatchData");
            public static readonly ProfilerMarker resizeBuffers = new ProfilerMarker("SpriteSkinComposite.ResizeBuffers");
            public static readonly ProfilerMarker prepare = new ProfilerMarker("SpriteSkinComposite.Prepare");
            public static readonly ProfilerMarker scheduleJobs = new ProfilerMarker("SpriteSkinComposite.ScheduleJobs");
            public static readonly ProfilerMarker setBatchDeformableBufferAndLocalAABB = new ProfilerMarker("SpriteSkinComposite.SetBatchDeformableBufferAndLocalAABB");
            public static readonly ProfilerMarker deactivateDeformableBuffer = new ProfilerMarker("SpriteSkinComposite.DeactivateDeformableBuffer");
        }

        static SpriteSkinComposite s_Instance;

        public static SpriteSkinComposite instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var composite = Resources.FindObjectsOfTypeAll<SpriteSkinComposite>();
                    if (composite.Length > 0)
                        s_Instance = composite[0];
                    else
                        s_Instance = ScriptableObject.CreateInstance<SpriteSkinComposite>();
                    s_Instance.hideFlags = HideFlags.HideAndDontSave;
                    s_Instance.Init();
                }
                return s_Instance;
            }
        }

        List<SpriteSkin> m_SpriteSkinsToAdd = new List<SpriteSkin>();
        List<SpriteSkin> m_SpriteSkinsToRemove = new List<SpriteSkin>();
        List<int> m_TransformIdsToRemove = new List<int>();
        List<SpriteSkin> m_SpriteSkins = new List<SpriteSkin>();
        SpriteRenderer[] m_SpriteRenderers = new SpriteRenderer[0];

        NativeByteArray m_DeformedVerticesBuffer;
        NativeArray<float4x4> m_FinalBoneTransforms;

        NativeArray<bool> m_IsSpriteSkinActiveForDeform;
        NativeArray<SpriteSkinData> m_SpriteSkinData;
        NativeArray<PerSkinJobData> m_PerSkinJobData;
        NativeArray<Bounds> m_BoundsData;
        NativeArray<IntPtr> m_Buffers;
        NativeArray<int> m_BufferSizes;

        NativeArray<int2> m_BoneLookupData;
        NativeArray<int2> m_VertexLookupData;
        NativeArray<PerSkinJobData> m_SkinBatchArray;
        TransformAccessJob m_LocalToWorldTransformAccessJob;
        TransformAccessJob m_WorldToLocalTransformAccessJob;
        JobHandle m_BoundJobHandle;
        JobHandle m_DeformJobHandle;
        JobHandle m_CopyJobHandle;

        [SerializeField]
        GameObject m_Helper;

        internal GameObject helperGameObject => m_Helper;

        internal void RemoveTransformById(int transformId)
        {
            m_LocalToWorldTransformAccessJob.RemoveTransformById(transformId);
        }

        internal void AddSpriteSkinBoneTransform(SpriteSkin spriteSkin)
        {
            if (spriteSkin == null)
                return;
            if (spriteSkin.boneTransforms != null)
            {
                foreach (var t in spriteSkin.boneTransforms)
                {
                    if(t != null)
                        m_LocalToWorldTransformAccessJob.AddTransform(t);
                }
            }
        }

        internal void AddSpriteSkinRootBoneTransform(SpriteSkin spriteSkin)
        {
            if (spriteSkin == null || spriteSkin.rootBone == null)
                return;
            m_LocalToWorldTransformAccessJob.AddTransform(spriteSkin.rootBone);
        }

        internal void AddSpriteSkin(SpriteSkin spriteSkin)
        {
            if (spriteSkin == null)
                return;
            
            if (!DoesContainSpriteSkin(m_SpriteSkins, spriteSkin) && !DoesContainSpriteSkin(m_SpriteSkinsToAdd, spriteSkin))
                m_SpriteSkinsToAdd.Add(spriteSkin);

            if (DoesContainSpriteSkin(m_SpriteSkinsToRemove, spriteSkin))
            {
                m_SpriteSkinsToRemove.Remove(spriteSkin);
                m_TransformIdsToRemove.Remove(spriteSkin.transform.GetInstanceID());
            }
        }

        internal void CopyToSpriteSkinData(SpriteSkin spriteSkin)
        {
            if (spriteSkin == null)
                return;
            var index = m_SpriteSkins.IndexOf(spriteSkin);
            if (index < 0)
                return;
            CopyToSpriteSkinData(index);
        }

        private void CopyToSpriteSkinData(int index)
        {
            if (index < 0 || index >= m_SpriteSkins.Count || m_SpriteSkins[index] == null)
                return;
            if (!m_SpriteSkinData.IsCreated)
                return;
            
            var spriteSkinData = default(SpriteSkinData);
            var spriteSkin = m_SpriteSkins[index];
            spriteSkin.CopyToSpriteSkinData(ref spriteSkinData, index);
            m_SpriteSkinData[index] = spriteSkinData;
            m_SpriteRenderers[index] = spriteSkin.spriteRenderer;
        }

        internal void RemoveSpriteSkin(SpriteSkin spriteSkin)
        {
            if (spriteSkin == null)
                return;

            if (DoesContainSpriteSkin(m_SpriteSkins, spriteSkin) && !DoesContainSpriteSkin(m_SpriteSkinsToRemove, spriteSkin))
            {
                m_SpriteSkinsToRemove.Add(spriteSkin);
                m_TransformIdsToRemove.Add(spriteSkin.transform.GetInstanceID());
            }

            if (DoesContainSpriteSkin(m_SpriteSkinsToAdd, spriteSkin))
                m_SpriteSkinsToAdd.Remove(spriteSkin);
        }
        
        static bool DoesContainSpriteSkin(in List<SpriteSkin> collection, SpriteSkin skin)
        {
            var index = collection.IndexOf(skin);
            if (index < 0)
                return false;
            return collection[index] != null;
        }

        void Init()
        {
            if (m_LocalToWorldTransformAccessJob == null)
                m_LocalToWorldTransformAccessJob = new TransformAccessJob();
            if (m_WorldToLocalTransformAccessJob == null)
                m_WorldToLocalTransformAccessJob = new TransformAccessJob();

            CreateHelper();
        }

        void CreateHelper()
        {
            if (m_Helper != null)
                return;
            
            m_Helper = new GameObject("SpriteSkinUpdateHelper");
            m_Helper.hideFlags = HideFlags.HideAndDontSave;
            var helperComponent = m_Helper.AddComponent<SpriteSkinUpdateHelper>();
            helperComponent.onDestroyingComponent += OnHelperDestroyed;
                
#if !UNITY_EDITOR
            GameObject.DontDestroyOnLoad(m_Helper);
#endif
        }

        void OnHelperDestroyed(GameObject helperGo)
        {
            if (m_Helper != helperGo)
                return;

            m_Helper = null;
            CreateHelper();
        }

        internal void ResetComposite()
        {
            m_SpriteSkins.Clear();
            m_LocalToWorldTransformAccessJob.Destroy();
            m_WorldToLocalTransformAccessJob.Destroy();
            m_LocalToWorldTransformAccessJob = new TransformAccessJob();
            m_WorldToLocalTransformAccessJob = new TransformAccessJob();
        }

        public void OnEnable()
        {
            s_Instance = this;

            m_FinalBoneTransforms = new NativeArray<float4x4>(1, Allocator.Persistent);
            m_BoneLookupData = new NativeArray<int2>(1, Allocator.Persistent);
            m_VertexLookupData = new NativeArray<int2>(1, Allocator.Persistent);
            m_SkinBatchArray = new NativeArray<PerSkinJobData>(1, Allocator.Persistent);

            Init();
            InitializeArrays();
            BatchRemoveSpriteSkins();
            BatchAddSpriteSkins();

            // Initialise all existing SpriteSkins as execution order is indeterminate
            for (var i = 0; i < m_SpriteSkins.Count; ++i)
                CopyToSpriteSkinData(i);
        }

        void InitializeArrays()
        {
            const int startingCount = 0;
            m_IsSpriteSkinActiveForDeform = new NativeArray<bool>(startingCount, Allocator.Persistent);
            m_PerSkinJobData = new NativeArray<PerSkinJobData>(startingCount, Allocator.Persistent);
            m_SpriteSkinData = new NativeArray<SpriteSkinData>(startingCount, Allocator.Persistent);
            m_BoundsData = new NativeArray<Bounds>(startingCount, Allocator.Persistent);
            m_Buffers = new NativeArray<IntPtr>(startingCount, Allocator.Persistent);
            m_BufferSizes = new NativeArray<int>(startingCount, Allocator.Persistent);            
        }

        void OnDisable()
        {
            m_DeformJobHandle.Complete();
            m_BoundJobHandle.Complete();
            m_CopyJobHandle.Complete();
            m_SpriteSkins.Clear();
            m_SpriteRenderers = new SpriteRenderer[0];
            BufferManager.instance.ReturnBuffer(GetInstanceID());
            m_IsSpriteSkinActiveForDeform.DisposeIfCreated();
            m_PerSkinJobData.DisposeIfCreated();
            m_SpriteSkinData.DisposeIfCreated();
            m_Buffers.DisposeIfCreated();
            m_BufferSizes.DisposeIfCreated();
            m_BoneLookupData.DisposeIfCreated();
            m_VertexLookupData.DisposeIfCreated();
            m_SkinBatchArray.DisposeIfCreated();
            m_FinalBoneTransforms.DisposeIfCreated();
            m_BoundsData.DisposeIfCreated();
            
            if (m_Helper != null)
            {
                m_Helper.GetComponent<SpriteSkinUpdateHelper>().onDestroyingComponent -= OnHelperDestroyed;
                GameObject.DestroyImmediate(m_Helper);
            }
            
            m_LocalToWorldTransformAccessJob.Destroy();
            m_WorldToLocalTransformAccessJob.Destroy();
        }

        internal unsafe void LateUpdate()
        {
            BatchRemoveSpriteSkins();
            BatchAddSpriteSkins();

            var count = m_SpriteSkins.Count;
            if (count == 0)
                return;
            
            Profiling.prepareData.Begin();
            Assert.AreEqual(m_IsSpriteSkinActiveForDeform.Length, count);
            Assert.AreEqual(m_PerSkinJobData.Length, count);
            Assert.AreEqual(m_SpriteSkinData.Length, count);
            Assert.AreEqual(m_BoundsData.Length, count);
            Assert.AreEqual(m_Buffers.Length, count);
            Assert.AreEqual(m_BufferSizes.Length, count);
            Assert.AreEqual(m_SpriteRenderers.Length, count);
            
            using (Profiling.validateSpriteSkinData.Auto())
            {
                for (var i = 0; i < m_SpriteSkins.Count; ++i)
                {
                    var spriteSkin = m_SpriteSkins[i];
                    m_IsSpriteSkinActiveForDeform[i] = spriteSkin.BatchValidate();
                    if (m_IsSpriteSkinActiveForDeform[i] && spriteSkin.NeedUpdateCompositeCache())
                    {
                        CopyToSpriteSkinData(i);
                    }
                }
            }
            
            Profiling.transformAccessJob.Begin();
            var localToWorldJobHandle = m_LocalToWorldTransformAccessJob.StartLocalToWorldJob();
            var worldToLocalJobHandle = m_WorldToLocalTransformAccessJob.StartWorldToLocalJob();
            Profiling.transformAccessJob.End();

            using (Profiling.getSpriteSkinBatchData.Auto())
            {
                NativeArrayHelpers.ResizeIfNeeded(ref m_SkinBatchArray, 1);
                var fillPerSkinJobSingleThread = new FillPerSkinJobSingleThread()
                {
                    isSpriteSkinValidForDeformArray = m_IsSpriteSkinActiveForDeform,
                    combinedSkinBatchArray = m_SkinBatchArray,
                    spriteSkinDataArray = m_SpriteSkinData,
                    perSkinJobDataArray = m_PerSkinJobData,
                };
                fillPerSkinJobSingleThread.Run();
            }
            Profiling.prepareData.End();

            var skinBatch = m_SkinBatchArray[0];
            var batchCount = m_SpriteSkinData.Length;
            var vertexBufferSize = skinBatch.deformVerticesStartPos;
            if (vertexBufferSize <= 0)
            {
                localToWorldJobHandle.Complete();
                worldToLocalJobHandle.Complete();
                DeactivateDeformableBuffers();
                return;
            }

            using (Profiling.resizeBuffers.Auto())
            {
                m_DeformedVerticesBuffer = BufferManager.instance.GetBuffer(GetInstanceID(), vertexBufferSize);
                NativeArrayHelpers.ResizeIfNeeded(ref m_FinalBoneTransforms, skinBatch.bindPosesIndex.y);
                NativeArrayHelpers.ResizeIfNeeded(ref m_BoneLookupData, skinBatch.bindPosesIndex.y);
                NativeArrayHelpers.ResizeIfNeeded(ref m_VertexLookupData, skinBatch.verticesIndex.y);
            }
            
            Profiling.prepare.Begin();
            var prepareJob = new PrepareDeformJob
            {
                batchDataSize = batchCount,
                perSkinJobData = m_PerSkinJobData,
                boneLookupData = m_BoneLookupData,
                vertexLookupData = m_VertexLookupData
            };
            var jobHandle = prepareJob.Schedule();
            Profiling.prepare.End();
            
            Profiling.scheduleJobs.Begin();
            var boneJobBatched = new BoneDeformBatchedJob()
            {
                boneTransform = m_LocalToWorldTransformAccessJob.transformMatrix,
                rootTransform = m_WorldToLocalTransformAccessJob.transformMatrix,
                spriteSkinData = m_SpriteSkinData,
                boneLookupData = m_BoneLookupData,
                finalBoneTransforms = m_FinalBoneTransforms,
                rootTransformIndex = m_WorldToLocalTransformAccessJob.transformData,
                boneTransformIndex = m_LocalToWorldTransformAccessJob.transformData
            };
            jobHandle = JobHandle.CombineDependencies(localToWorldJobHandle, worldToLocalJobHandle, jobHandle);
            jobHandle = boneJobBatched.Schedule(skinBatch.bindPosesIndex.y, 8, jobHandle);

            var skinJobBatched = new SkinDeformBatchedJob()
            {
                vertices = m_DeformedVerticesBuffer.array,
                vertexLookupData = m_VertexLookupData,
                spriteSkinData = m_SpriteSkinData,
                perSkinJobData = m_PerSkinJobData,
                finalBoneTransforms = m_FinalBoneTransforms,
            };
            m_DeformJobHandle = skinJobBatched.Schedule(skinBatch.verticesIndex.y, 16, jobHandle);

            var copySpriteRendererBuffersJob = new CopySpriteRendererBuffersJob()
            {
                isSpriteSkinValidForDeformArray = m_IsSpriteSkinActiveForDeform,
                spriteSkinData = m_SpriteSkinData,
                ptrVertices = (IntPtr) NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(m_DeformedVerticesBuffer.array),
                buffers = m_Buffers,
                bufferSizes = m_BufferSizes,
            };
            m_CopyJobHandle = copySpriteRendererBuffersJob.Schedule(batchCount, 16, jobHandle);

            var updateBoundJob = new CalculateSpriteSkinAABBJob
            {
                vertices = m_DeformedVerticesBuffer.array,
                isSpriteSkinValidForDeformArray = m_IsSpriteSkinActiveForDeform,
                spriteSkinData = m_SpriteSkinData,
                bounds = m_BoundsData,
            };
            m_BoundJobHandle = updateBoundJob.Schedule(batchCount, 4, m_DeformJobHandle);
            Profiling.scheduleJobs.End();

            JobHandle.ScheduleBatchedJobs();
            jobHandle = JobHandle.CombineDependencies(m_BoundJobHandle, m_CopyJobHandle);
            jobHandle.Complete();

            using (Profiling.setBatchDeformableBufferAndLocalAABB.Auto())
            {
                InternalEngineBridge.SetBatchDeformableBufferAndLocalAABBArray(m_SpriteRenderers, m_Buffers, m_BufferSizes, m_BoundsData);
            }

            DeactivateDeformableBuffers();
        }
        
        void BatchRemoveSpriteSkins()
        {
            if (m_SpriteSkinsToRemove.Count == 0)
                return;

            using (Profiling.batchRemoveSpriteSkin.Auto())
            {
                m_WorldToLocalTransformAccessJob.RemoveTransformsByIds(m_TransformIdsToRemove);

                var updatedCount = Mathf.Max(m_SpriteSkins.Count - m_SpriteSkinsToRemove.Count, 0);
                if (updatedCount == 0)
                {
                    m_SpriteSkins.Clear();
                }
                else
                {
                    foreach (var spriteSkin in m_SpriteSkinsToRemove)
                    {
                        var index = m_SpriteSkins.IndexOf(spriteSkin);
                        if (index < 0)
                            continue;

                        // Check if it is not the last SpriteSkin
                        if (index < m_SpriteSkins.Count - 1)
                        {
                            m_SpriteSkins.RemoveAtSwapBack(index);
                            CopyToSpriteSkinData(index);
                        }
                        else
                            m_SpriteSkins.RemoveAt(index);
                    }
                }
                
                Array.Resize(ref m_SpriteRenderers, updatedCount);
                ResizeAndCopyArrays(updatedCount);             

                m_TransformIdsToRemove.Clear();
                m_SpriteSkinsToRemove.Clear();
            }
        }        

        void BatchAddSpriteSkins()
        {
            if (m_SpriteSkinsToAdd.Count == 0)
                return;

            using (Profiling.batchAddSpriteSkin.Auto())
            {
                var updatedCount = m_SpriteSkins.Count + m_SpriteSkinsToAdd.Count;
                Array.Resize(ref m_SpriteRenderers, updatedCount);
                if (m_IsSpriteSkinActiveForDeform.IsCreated)
                    ResizeAndCopyArrays(updatedCount);

                foreach (var spriteSkin in m_SpriteSkinsToAdd)
                {
                    if (DoesContainSpriteSkin(m_SpriteSkins, spriteSkin))
                    {
                        Debug.LogError($"Skin already exists! Name={spriteSkin.name}");
                        continue;
                    }
                    m_SpriteSkins.Add(spriteSkin);
                    var count = m_SpriteSkins.Count;
                    
                    m_SpriteRenderers[count - 1] = spriteSkin.spriteRenderer;
                    m_WorldToLocalTransformAccessJob.AddTransform(spriteSkin.transform);

                    if (m_IsSpriteSkinActiveForDeform.IsCreated)
                        CopyToSpriteSkinData(count - 1);       
                }

                m_SpriteSkinsToAdd.Clear();
            }
        }

        void ResizeAndCopyArrays(int updatedCount)
        {
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_IsSpriteSkinActiveForDeform, updatedCount);
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_PerSkinJobData, updatedCount);
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_SpriteSkinData, updatedCount);
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_BoundsData, updatedCount);
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_Buffers, updatedCount);
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_BufferSizes, updatedCount);            
        }

        void DeactivateDeformableBuffers()
        {
            using (Profiling.deactivateDeformableBuffer.Auto())
            {
                for (var i = 0; i < m_IsSpriteSkinActiveForDeform.Length; ++i)
                {
                    if (m_IsSpriteSkinActiveForDeform[i] || InternalEngineBridge.IsUsingDeformableBuffer(m_SpriteRenderers[i], IntPtr.Zero))
                        continue;
                    m_SpriteRenderers[i].DeactivateDeformableBuffer();
                }
            }
        }

        internal bool HasDeformableBufferForSprite(int dataIndex)
        {
            if (dataIndex < 0 && m_IsSpriteSkinActiveForDeform.Length >= dataIndex)
                throw new InvalidOperationException("Invalid index for deformable buffer");
            return m_IsSpriteSkinActiveForDeform[dataIndex];
        }

        internal unsafe NativeArray<byte> GetDeformableBufferForSprite(int dataIndex)
        {
            if (dataIndex < 0 && m_SpriteSkinData.Length >= dataIndex)
                throw new InvalidOperationException("Invalid index for deformable buffer");
            
            if (!m_DeformJobHandle.IsCompleted)
                m_DeformJobHandle.Complete();

            var skinData = m_SpriteSkinData[dataIndex];
            if (skinData.deformVerticesStartPos < 0)
                throw new InvalidOperationException("There are no currently deformed vertices.");

            var vertexBufferLength = skinData.spriteVertexCount * skinData.spriteVertexStreamSize;
            byte* ptrVertices = (byte*)m_DeformedVerticesBuffer.array.GetUnsafeReadOnlyPtr();
            ptrVertices += skinData.deformVerticesStartPos; 
            var buffer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(ptrVertices, vertexBufferLength, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref buffer, NativeArrayUnsafeUtility.GetAtomicSafetyHandle(m_DeformedVerticesBuffer.array));
#endif
            return buffer;
        }

        // Code for tests
        #region Code For Tests
        internal string GetDebugLog()
        {
            var log = "";
            log += "===SpriteSkinBatch===\n";
            log += "Count: " + m_SpriteSkins.Count +"\n";
            foreach (var ss in m_SpriteSkins)
            {
                log += ss == null ? "null" : ss.name;
                log += "\n";
            }

            log += "===LocalToWorldTransformAccessJob===\n";
            log += m_LocalToWorldTransformAccessJob.GetDebugLog();
            log += "\n";
            log += "===WorldToLocalTransformAccessJob===\n";
            log += "\n";
            log += m_WorldToLocalTransformAccessJob.GetDebugLog();
            return log;
        }

        internal SpriteSkin[] GetSpriteSkins()
        {
            return m_SpriteSkins.ToArray();
        }

        internal TransformAccessJob GetWorldToLocalTransformAccessJob()
        {
            return m_WorldToLocalTransformAccessJob;
        }

        internal TransformAccessJob GetLocalToWorldTransformAccessJob()
        {
            return m_LocalToWorldTransformAccessJob;
        }
        #endregion
    }

#if UNITY_EDITOR

    [UnityEditor.InitializeOnLoad]
    internal class SpriteSkinCompositeStartup
    {
        static SpriteSkinCompositeStartup()
        {
            if (null == SpriteSkinComposite.instance.helperGameObject)
                throw new System.InvalidOperationException("SpriteSkinComposite not initialized properly.");
        }
    }

#endif

}