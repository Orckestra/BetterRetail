﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Orckestra.Composer.Cart.Helper
{
    public static class ObjectHelper
    {
        public static T CreateDeepCopy<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Object cannot be null");
            return (T)Process(obj);
        }

        static object Process(object obj)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }
            else if (type.IsArray)
            {
                Type elementType = Type.GetType(
                    type.FullName.Replace("[]", string.Empty));
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(Process(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }
            else if (type.IsClass)
            {
                if(type.FullName == "Orckestra.Overture.ServiceModel.PropertyBag")
                {
                    return new Overture.ServiceModel.PropertyBag((Overture.ServiceModel.PropertyBag)obj);
                }

                object toret = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                                                    BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                        continue;
                    field.SetValue(toret, Process(fieldValue));
                }
                return toret;
            }
            else
                throw new ArgumentException("Unknown type");
        }
    }
}
