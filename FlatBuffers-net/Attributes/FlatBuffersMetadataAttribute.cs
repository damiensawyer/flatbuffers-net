﻿using System;

namespace FlatBuffers.Attributes
{
    /// <summary>
    /// Attribute to annotate Flatbuffers objects with custom metadata.
    /// This metadata is declared in the schema.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class FlatBuffersMetadataAttribute : Attribute
    {
        public string Name { get; private set; }

        public object Value { get; set; }
        public bool HasValue { get; private set; }

        public FlatBuffersMetadataAttribute(string name)
        {
            Name = name;
        }

        public FlatBuffersMetadataAttribute(string name, int value)
            : this(name)
        {
            Value = value;
            HasValue = true;
        }

        public FlatBuffersMetadataAttribute(string name, bool value)
            : this(name)
        {
            Value = value;
            HasValue = true;
        }

        public FlatBuffersMetadataAttribute(string name, string value)
            : this(name)
        {
            Value = value;
            HasValue = true;
        }
    }
}
