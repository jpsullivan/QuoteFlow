using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteFlow.Api.Infrastructure.Extensions;

namespace QuoteFlow.Api.Util
{
    /// <summary>
    /// This class can be used to "parse" values from a map of parameters.  This is really intended to be used in {@link
    /// webwork.action.Action} code that needs to read input parameters from <see cref="webwork.action.ActionContext#getParameters()"/>
    /// </summary>
    public class ParameterUtils
    {
        /// <summary>
        /// Create a List from the parameter with the specified key.
        /// </summary>
        /// <returns> null if the object is not a String array, or has no elements<br> otherwise it returns a List containing
        ///         all elements of the String array, with the Strings over value ("-1") turned into null's. (Strings with
        ///         value "" are ignored). 
        /// </returns>
        public static IList GetListParam(IDictionary @params, string key)
        {
            object o = @params[key];

            if (!(o is string[])) return null;

            var oArray = (string[]) o;

            if (oArray.Length <= 0) return null;

            IList oList = new ArrayList();
            foreach (string s in oArray.Where(s => !string.IsNullOrEmpty(s)))
            {
                oList.Add(s.Equals("-1", StringComparison.CurrentCultureIgnoreCase) ? null : s);
            }

            return oList;
        }

        public static string GetStringParam(IDictionary<string, object> @params, string key)
        {
            object o = @params[key];

            if (o is string)
            {
                return (string)o;
            }
            
            if (o is string[])
            {
                return ((string[])o)[0];
            }
            
            if (o is ICollection)
            {
                var enumerator = ((ICollection) o).GetEnumerator();
                enumerator.MoveNext();
                return (string) enumerator.Current;
            }
            
            return o == null ? null : o.ToString();
        }

        /// <summary>
        /// Returns the value of the specified parameter name as a String[]
        /// </summary>
        /// <param name="params"> the map of parameters </param>
        /// <param name="paramName"> the name of the parameter </param>
        /// <returns> a String[] of values or null if there are no parameters </returns>
        public static string[] GetStringArrayParam(IDictionary @params, string paramName)
        {
            object o = @params[paramName];

            if (o is string)
            {
                return new[] {(string) o};
            }
            
            if (o is string[])
            {
                return (string[])o;
            }
            
            if (o is ICollection)
            {
                var c = (ICollection) o;
                var sa = new string[c.Count];
                int i = 0;
                foreach (object obj in c)
                {
                    sa[i++] = obj == null ? null : Convert.ToString(obj);
                }
                return sa;
            }
            
            return o == null ? null : new[] { o.ToString() };
        }

        /// <summary>
        /// Searches through the Map (params) for the given targetKey and targetValue extracting the index (i) and uses this
        /// to extract the corresponding index value with the desiredKey. if there is no match, the first value or null is
        /// returned
        /// </summary>
        /// <returns> desiredValue - corresponding value to desiredKey with the same index as the targetKey and targetValue, or
        ///         the first value if there is only one, otherwise null </returns>
        public static string GetStringParam(IDictionary<string, object> @params, string targetKey, string targetValue, string desiredKey)
        {
            object targetO = @params[targetKey];
            object desiredO = @params[desiredKey];

            if (desiredO is string)
            {
                return ((string)desiredO);
            }
            
            if (targetO is string[] && desiredO is string[])
            {
                for (int i = 0; i < ((string[])targetO).Length; i++)
                {
                    if (((string[])targetO)[i].Equals(targetValue))
                    {
                        return ((string[])desiredO)[i];
                    }
                }
                return ((string[])desiredO)[0];
            }
            
            return desiredO == null ? null : desiredO.ToString();
        }

        /// <summary>
        /// Checks if the given key, value pair exists in the given params Map
        /// </summary>
        /// <param name="params"> the map of web parameters </param>
        /// <param name="key"> the name of the parameter to check </param>
        /// <param name="value"> the value to check for </param>
        /// <returns> true of this key/value pair exists </returns>
        public static bool ParamContains(IDictionary<string, object> @params, string key, string value)
        {
            // must check the output is the given value since it may have returned a junk value
            if (GetStringParam(@params, key, value, key) != null)
            {
                return GetStringParam(@params, key, value, key).Equals(value);
            }
            return false;
        }

        /// <summary>
        /// Gets a int value from the map and uses the defaultValue if the value can be converted.
        /// </summary>
        /// <param name="mapOfParameters"> the map of parameters </param>
        /// <param name="paramName"> the parameter name to use </param>
        /// <param name="defaultValue"> the default value in case things cant be converted </param>
        /// <returns> the converted value or the defaultValue if it cant be converted </returns>
        public static int GetIntParam(IDictionary<string, object> mapOfParameters, string paramName, int defaultValue)
        {
            string s = GetStringParam(mapOfParameters, paramName);

            if (string.IsNullOrEmpty(s))
            {
                return defaultValue;
            }

            try
            {
                return Convert.ToInt32(s);
            }
            catch (Exception nfe)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns a boolean value from the map of parameters.  if it does no exist in the map, then false is returned
        /// </summary>
        /// <param name="mapOfParameters"> the map of parameters </param>
        /// <param name="paramName"> the parameter name to use </param>
        /// <returns> true if the value converted to true or false otherwise </returns>
        public static bool GetBooleanParam(IDictionary<string, object> mapOfParameters, string paramName) //<String,String[]>
        {
            string s = GetStringParam(mapOfParameters, paramName);
            return Convert.ToBoolean(s);
        }

        /// <summary>
        /// Convert value to -1 if it is null
        /// </summary>
        private static object SwapNull(object o)
        {
            return o ?? "-1";
        }

        /// <summary>
        /// Convert all null values in a collection into "-1"s.
        /// </summary>
        public static ICollection SwapNulls(ICollection col)
        {
            if (col == null)
            {
                return null;
            }

            var result = new ArrayList(col.Count);

            foreach (object aCol in col)
            {
                result.Add(SwapNull(aCol));
            }

            return result;
        }

        public static IEnumerable<string> GetListFromStringArray(string[] ar)
        {
            if (ar == null || ar.Length == 0)
            {
                return new List<string>();
            }

            var result = new List<string>(ar.Length);
            result.AddRange(ar.Where(anAr => anAr.HasValue()));
            return result;
        }

        public static string MakeCommaSeparated(int?[] longs)
        {
            var result = new StringBuilder();
            for (int i = 0; i < longs.Length; i++)
            {
                result.Append(longs[i]);
                if (i < longs.Length - 1)
                {
                    result.Append(",");
                }
            }
            return result.ToString();
        }
    }
}