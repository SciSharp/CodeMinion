using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Regen.Helpers {
    public static class AttributeExtensions {
        #region Methods

        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>(this Enum value) where T : Attribute {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0
                ? (T) attributes[0]
                : null;
        }

        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        public static List<T> GetAttributes<T>(this Enum value) where T : Attribute {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0
                ? attributes.Cast<T>().ToList()
                : new List<T>(0);
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        public static string ToName(this Enum value) {
            var attribute = GetAttribute<DescriptionAttribute>(value);
            return attribute == null ? value.ToString() : attribute.Description;
        }

        #endregion
    }
}