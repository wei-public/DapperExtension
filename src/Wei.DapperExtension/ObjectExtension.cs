using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Wei.DapperExtension
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 拷贝对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="t">被拷贝的对象</param>
        /// <returns>拷贝后的新对象</returns>
        public static T Copy<T>(this T t) where T : class => ObjectMap<T, T>.Copy(t);

        /// <summary>
        /// 拷贝对象(集合)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="t">被拷贝的对象</param>
        /// <returns>拷贝后的新对象集合</returns>
        public static List<T> Copy<T>(this List<T> t) where T : class
        {
            if (t == null) return t;
            var res = new List<T>();
            t.ForEach(x => res.Add(ObjectMap<T, T>.Copy(x)));
            return res;
        }

        /// <summary>
        /// 对象映射
        /// </summary>
        /// <typeparam name="Source">源类型</typeparam>
        /// <typeparam name="Target">目标类型</typeparam>
        /// <param name="source">源实体对象</param>
        /// <returns>映射后的对象</returns>
        public static Target MapTo<Source, Target>(this Source source) where Target : class => ObjectMap<Source, Target>.MapTo(source);

        /// <summary>
        /// 对象映射
        /// </summary>
        /// <typeparam name="Source">源类型</typeparam>
        /// <typeparam name="Target">目标类型</typeparam>
        /// <param name="source">源实体对象</param>
        /// <returns>映射后的对象</returns>
        public static List<Target> MapTo<Source, Target>(this List<Source> source) where Target : class
        {
            if (source == null) return null;
            var res = new List<Target>();
            source.ForEach(x => res.Add(ObjectMap<Source, Target>.MapTo(x)));
            return res;
        }
    }

    public static class ObjectMap<TIn, TOut>
    {
        private static readonly Func<TIn, TOut> cache = CopyFunc();
        private static Func<TIn, TOut> CopyFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();
            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite)
                    continue;

                var prop = typeof(TIn).GetProperty(item.Name);
                var isExist = prop != null;

                if (!isExist)
                    continue;

                MemberExpression property = Expression.Property(parameterExpression, prop);
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        public static TOut MapTo(TIn tIn)
        {
            if (tIn == null) return default;
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();
            var attrDic = new Dictionary<PropertyInfo, object>();
            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite)
                    continue;

                var prop = typeof(TIn).GetProperty(item.Name);
                var isExist = prop != null;

                if (!isExist)
                    continue;

                if (item.PropertyType != prop.PropertyType)
                {
                    var s = prop.GetValue(tIn);
                    attrDic.Add(item, s);
                    continue;
                }

                MemberExpression property = Expression.Property(parameterExpression, prop);
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });
            var func = lambda.Compile();
            var tOut = func(tIn);
            if (attrDic.Count > 0)
            {
                foreach (var item in attrDic)
                {
                    if (item.Value == null) continue;
                    item.Key.SetValue(tOut, ChangeType(item.Value, item.Key.PropertyType));
                }
            }
            return tOut;
        }

        public static TOut Copy(TIn tIn) => tIn == null ? default : cache(tIn);


        private static object ChangeType(object value, Type targetType)
        {
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                var nullableConverter = new NullableConverter(targetType);
                targetType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(value, targetType);
        }

    }
}
