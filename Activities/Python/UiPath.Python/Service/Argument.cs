﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace UiPath.Python.Service
{
    [DataContract]
    public class Argument
    {
        [DataMember]
        private object _wrappedValue;

        [DataMember]
        private string _typeName;

        public Argument(object obj)
        {
            Type type = null != obj ? obj.GetType() : typeof(object);
            _wrappedValue = obj;
            _typeName = type.AssemblyQualifiedName;

            // wrap arrays into object[]
            if(type.IsArray && !type.Equals(typeof(object[])))
            {
                Debug.Assert(obj is Array);
                Debug.Assert(null != type.GetElementType());
                _wrappedValue = (obj as Array).Cast<object>().ToArray();
            }
        }

        public object Unwrap()
        {
            // verify the type
            Type type = Type.GetType(_typeName);
            Debug.Assert(null != type);

            // could not load type, return the value as is
            if (null == type)
            {
                return _wrappedValue;
            }

            if (type.IsArray && !type.Equals(typeof(object[])))
            {
                Debug.Assert(_wrappedValue is Array);
                Debug.Assert(null != type.GetElementType());

                Array array = _wrappedValue as Array;
                var typedArray = Array.CreateInstance(type.GetElementType(), array.Length);
                Array.Copy(array, typedArray, array.Length);
                return typedArray;
            }
            return _wrappedValue;
        }
    }
}
