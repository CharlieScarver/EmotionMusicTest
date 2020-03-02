﻿#region Using

using Emotion.Standard.Utility;

#endregion

namespace Emotion.Standard.Text.FontTables
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/typography/opentype/spec/maxp
    /// </summary>
    public class Maxp
    {
        public float Version;
        public ushort NumGlyphs;

        public ushort MaxPoints;
        public ushort MaxCountours;
        public ushort MaxCompositePoints;
        public ushort MaxCompositeCountours;
        public ushort MaxZones;
        public ushort MaxTwilightPoints;
        public ushort MaxStorage;
        public ushort MaxFunctionDefs;
        public ushort MaxInstructionDefs;
        public ushort MaxStackElements;
        public ushort MaxSizeOfInstructions;
        public ushort MaxComponentElements;
        public ushort MaxComponentDepth;

        /// <summary>
        /// Memory requirements for the glyph.
        /// </summary>
        public Maxp(ByteReader reader)
        {
            Version = reader.ReadOpenTypeVersionBE();
            NumGlyphs = reader.ReadUShortBE();

            if (Version != 1.0) return;
            MaxPoints = reader.ReadUShortBE();
            MaxCountours = reader.ReadUShortBE();
            MaxCompositePoints = reader.ReadUShortBE();
            MaxCompositeCountours = reader.ReadUShortBE();
            MaxZones = reader.ReadUShortBE();
            MaxTwilightPoints = reader.ReadUShortBE();
            MaxStorage = reader.ReadUShortBE();
            MaxFunctionDefs = reader.ReadUShortBE();
            MaxInstructionDefs = reader.ReadUShortBE();
            MaxStackElements = reader.ReadUShortBE();
            MaxSizeOfInstructions = reader.ReadUShortBE();
            MaxComponentElements = reader.ReadUShortBE();
            MaxComponentDepth = reader.ReadUShortBE();
        }
    }
}