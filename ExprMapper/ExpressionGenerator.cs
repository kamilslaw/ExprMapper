using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExprMapper
{
    internal class ExpressionGenerator
    {
        public static Func<TIn, TOut> GetMapper<TIn, TOut>()
        {
            var param = Expression.Parameter(typeof(TIn), "x");
            var bindings = GetBinding(param, typeof(TIn), typeof(TOut));
            var lambda = Expression.Lambda<Func<TIn, TOut>>(bindings, param);
            return lambda.Compile();
        }

        private static Expression GetBinding(Expression prop, Type sourceType, Type targetType)
        {
            var ctor = Expression.New(targetType);
            var bindings = targetType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p =>
                {
                    if (p.PropertyType.IsClass && p.PropertyType != typeof(string))
                    {
                        var nestedProp = Expression.Property(prop, sourceType.GetProperty(p.Name));
                        return Expression.Bind(p, Expression.Condition(
                           test: Expression.Equal(Expression.Default(nestedProp.Type), nestedProp),
                           ifTrue: Expression.Default(p.PropertyType),
                           ifFalse: GetBinding(nestedProp, nestedProp.Type, p.PropertyType)));
                    }

                    var mi = sourceType.GetProperty(p.Name);
                    var sourceP = Expression.Property(prop, mi);
                    return Expression.Bind(p, sourceP);
                })
                .ToList();
            return Expression.MemberInit(ctor, bindings);
        }
    }
}
