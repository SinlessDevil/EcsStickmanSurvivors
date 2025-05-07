using System.IO;

namespace UnityEditor.U2D.Aseprite
{
    /// <summary>
    /// Parsed representation of an Aseprite Path chunk.
    /// </summary>
    /// <note>Not supported yet.</note>
    internal class PathChunk : BaseChunk
    {
        public override ChunkTypes chunkType => ChunkTypes.Path;

        internal PathChunk(uint chunkSize) : base(chunkSize) { }
        protected override void InternalRead(BinaryReader reader) { }
    }
}