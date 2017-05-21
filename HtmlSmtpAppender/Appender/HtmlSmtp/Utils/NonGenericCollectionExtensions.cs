/*
 *   (c) Greg Brackley, 2010
 *   
 *   $Id: NonGenericCollectionExtensions.cs 12 2010-10-05 09:54:31Z greg $ 
 */
using System;
using System.Collections;

namespace log4net.Appender.HtmlSmtp.Utils
{
    public static class NonGenericCollectionExtensions
    {
        /// <summary>
        ///   Return the first item from the collection, or the default value if the
        ///   list is empty for null.
        /// </summary>
        public static T GetSingleton<T>(this ICollection collection)
        {
            if (collection != null && collection.Count == 1)
            {
                IEnumerator enumerator = collection.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    return (T) enumerator.Current;
                }
            }
            throw new Exception("The container is not a singleton");
        }


        public static T GetSingleton<T>(this ICollection collection, T defaultValue)
        {
            if (collection != null && collection.Count == 1)
            {
                IEnumerator enumerator = collection.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    return (T) enumerator.Current;
                }
                return defaultValue;
            }
            else if (collection == null || collection.Count == 0)
            {
                return defaultValue;
            }
            else
            {
                throw new Exception(
                    string.Format("The container is not a singleton, but has {0} values", collection.Count));
            }
        }

        public static T GetSingletonOrDefault<T>(this ICollection collection)
        {
            if (collection != null && collection.Count == 1)
            {
                IEnumerator enumerator = collection.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    return (T) enumerator.Current;
                }
                return default(T);
            }
            else if (collection == null || collection.Count == 0)
            {
                return default(T);
            }
            else
            {
                throw new Exception(
                    string.Format("The container is not a singleton, but has {0} values", collection.Count));
            }
        }

       
    }
}