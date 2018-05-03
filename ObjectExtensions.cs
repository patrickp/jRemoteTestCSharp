using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace jRemoteTest
{
    public static class Constants
    {
        public const string Separator = " || ";
    }

    public static class ObjectExtensions
    {
        public static string ToStringReflection<T>(this T @this)
        {
            return string.Join(Constants.Separator, new List<string>(
                from prop in @this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                where prop.CanRead
                select string.Format("{0}: {1}", prop.Name, prop.GetValue(@this, null))).ToArray());
        }
    }
}
