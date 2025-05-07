using System.IO;

namespace UnityEditor.U2D.Aseprite
{
    /// <summary>
    /// Parsed representation of an Aseprite CellExtra chunk.
    /// </summary>
    /// <note>Not supported yet.</note>
    internal class CellExtra : BaseChunk
    {
        public override ChunkTypes chunkType => ChunkTypes.CellExtra;
        internal CellExtra(uint chunkSize) : base(chunkSize) { }
        protected override void InternalRead(BinaryReader reader) { }
    }
}