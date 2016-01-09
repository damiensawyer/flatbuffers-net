﻿using System;

namespace FlatBuffers.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FlatBuffersFieldAttribute : Attribute
    {
        private bool _hasOrderSet;
        private int _order = -1;
        private Type _unionType;

        /// <summary>
        /// Gets and sets the order of serialization and deserialization of the member
        /// </summary>
        public int Order
        {
            get { return _order; }
            set { _hasOrderSet = true;
                _order = value;
            }
        }

        /// <summary>
        /// Gets if the Order value has been set explicitly
        /// </summary>
        public bool IsOrderSetExplicitly { get { return _hasOrderSet; } }

        /// <summary>
        /// Gets and sets if the field is required to be set during serialization
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets and sets if this field is deprecated. It will still exist in the schema but will
        /// be skipped by serialization
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Gets and sets the Union type this field will serialize as.  Provided type must have a FlatBuffersUnionAttribute set.
        /// The field/property type must be object.
        /// </summary>
        public Type UnionType
        {
            get { return _unionType; }
            set
            {
                if (!value.Defined<FlatBuffersUnionAttribute>())
                {
                    throw new ArgumentException();
                }
                _unionType = value;
            }
        }

        /// <summary>
        /// Gets if this field holds a union type.
        /// </summary>
        public bool IsUnionField { get { return _unionType != null; } }

    }
}
