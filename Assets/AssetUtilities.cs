// Originally written by Max-H for the StellarModdingToolkit.
// Source: https://github.com/Max-H-0/StellarModdingToolkit

using System;
using System.Collections.Generic;
using System.Reflection;

namespace StellarModdingAPI.Assets
{
    public static class AssetUtilities
    {
        public static string[] ExtractKeysFrom<T>() => ExtractKeysFrom(typeof(T));
        
        public static string[] ExtractKeysFrom(Type type) 
        {
            List<string> keys = [];

            bool keysOnly = type.IsDefined(typeof(AssetKeyCollectionAttribute));

            foreach (var field in type.GetFields())
            {
                bool fieldIsExplicitlyKey = field.IsDefined(typeof(AssetKeyAttribute));

                bool isMarkedAsKey = fieldIsExplicitlyKey || keysOnly;
                bool isConstantString = field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string);
                
                if (isMarkedAsKey && isConstantString)
                {
                    var value = (string)field.GetRawConstantValue();

                    keys.Add(value);
                }
                else if (isMarkedAsKey && !isConstantString)
                {
                    throw new Exception($"The field \"{field.Name}\" was wrongly marked as AssetKey, it's not a constant string.");
                }
            }

            return keys.ToArray();
        }
    }


    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AssetKeyAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
    public class AssetKeyCollectionAttribute : Attribute;
}