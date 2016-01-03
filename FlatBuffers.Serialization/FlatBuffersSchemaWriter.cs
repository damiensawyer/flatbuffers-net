﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlatBuffers.Serialization
{
    public class FlatBuffersSchemaWriter
    {
        private readonly TypeModelRegistry _typeModelRegistry;
        private readonly TextWriter _writer;

        public FlatBuffersSchemaWriter(TextWriter writer)
            : this(TypeModelRegistry.Default, writer)
        {
        }

        public FlatBuffersSchemaWriter(TypeModelRegistry typeModelRegistry, TextWriter writer)
        {
            _typeModelRegistry = typeModelRegistry;
            _writer = writer;
        }

        public void Write<T>()
        {
            var type = typeof(T);
            Write(type);
        }

        public void Write(Type type)
        {
            var typeModel = _typeModelRegistry.GetTypeModel(type);
            if (typeModel == null)
            {
                throw new ArgumentException("Could not determine TypeModel for Type");
            }
            Write(typeModel);
        }

        public void Write(TypeModel typeModel)
        {
            if (typeModel.IsEnum)
            {
                WriteEnum(typeModel);
                return;
            }

            if (typeModel.BaseType == BaseType.Struct)
            {
                WriteStruct(typeModel);
                return;
            }

            throw new NotImplementedException();
        }

        private bool IsNumberEqual(object o, int v)
        {
            var type = o.GetType();
            if (type == typeof (byte))
            {
                return (byte) o == v;
            }
            if (type == typeof(sbyte))
            {
                return (sbyte)o == v;
            }
            if (type == typeof(short))
            {
                return (short)o == v;
            }
            if (type == typeof(ushort))
            {
                return (ushort)o == v;
            }
            if (type == typeof(int))
            {
                return (int)o == v;
            }
            if (type == typeof(uint))
            {
                return (uint)o == v;
            }
            throw new ArgumentException("Unsupported type");
        }

        public void WriteEnum(TypeModel typeModel)
        {
            // Note: .NET will reflect the enum based on the binary order of the VALUE.
            // We cannot reflect it in declared order
            // See: https://msdn.microsoft.com/en-us/library/system.enum.getvalues.aspx

            var values = Enum.GetValues(typeModel.Type);
            var names = Enum.GetNames(typeModel.Type);

            var emitValue = false;

            BeginEnum(typeModel);
            for (var i = 0; i < names.Length; ++i)
            {
                var enumValue = Convert.ChangeType(values.GetValue(i), Enum.GetUnderlyingType(typeModel.Type));
                if (!emitValue && !IsNumberEqual(enumValue, i))
                {
                    emitValue = true;
                }

                if (emitValue)
                {
                    _writer.WriteLine("    {0} = {1}{2}", names[i], enumValue, i == names.Length - 1 ? "" : ","); 
                }
                else
                {
                    _writer.WriteLine("    {0}{1}", names[i], i == names.Length - 1 ? "" : ","); 
                }
            }
            EndEnum();
        }

        public void WriteStruct(TypeModel typeModel)
        {
            var structDef = typeModel.StructDef;

            if (structDef == null)
            {
                throw new ArgumentException("Not a struct/table type", "typeModel");
            }

            BeginStruct(typeModel);
            foreach (var field in structDef.Fields)
            {
                WriteField(field);
            }
            EndObject();
        }

        protected void BeginStruct(TypeModel typeModel)
        {
            var structOrTable = typeModel.StructDef.IsFixed ? "struct" : "table";
            _writer.WriteLine("{0} {1} {{", structOrTable, typeModel.Name);
        }

        private string GetFlatBufferTypeName(TypeModel typeModel)
        {
            var baseType = typeModel.BaseType;
            var typeName = baseType.FlatBufferTypeName();

            if (typeName != null)
            {
                return typeName;
            }

            if (baseType == BaseType.Vector)
            {
                var elementTypeName = typeModel.ElementType.FlatBufferTypeName();
                if (elementTypeName != null)
                {
                    return string.Format("[{0}]", elementTypeName);
                }
            }
            throw new NotImplementedException();
        }

        protected void WriteField(FieldTypeDefinition field)
        {
            var fieldTypeName = GetFlatBufferTypeName(field.TypeModel);
            // TODO: Attributes
            _writer.WriteLine("    {0}:{1};", field.Name, fieldTypeName);
        }

        protected void EndObject()
        {
            // todo: assert nesting
            _writer.WriteLine("}");
        }

        protected void BeginEnum(TypeModel typeModel)
        {
            if (!typeModel.IsEnum)
            {
                throw new ArgumentException();
            }
            _writer.WriteLine("enum {0} : {1} {{", typeModel.Name, typeModel.BaseType.FlatBufferTypeName());
        }

        protected void EndEnum()
        {
            // todo: assert nesting
            _writer.WriteLine("}");
        }

    }
}
