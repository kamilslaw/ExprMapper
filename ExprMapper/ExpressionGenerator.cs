using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExprMapper
{
    internal class ExpressionGenerator
    {
        private static int MAX_REFERENCE_DEPTH = 10; // TODO: move to configuration

        private static readonly MethodInfo ENUMERABLE_SELECT = typeof(Enumerable)
            .GetMethods()
            .First(m => m.Name == nameof(Enumerable.Select) && m.GetParameters()[1].ParameterType.GetGenericArguments().Length == 2);
        private static readonly MethodInfo ENUMERABLE_TO_ARRAY = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray));
        private static readonly MethodInfo ENUMERABLE_TO_LIST = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));

        public static Func<TIn, TOut> GetMapper<TIn, TOut>(CustomBinding<TIn, TOut>[] customBindings)
        {
            var param = Expression.Parameter(typeof(TIn), "x");
            var bindings = GetBinding(param, typeof(TOut), customBindings);
            var lambda = Expression.Lambda<Func<TIn, TOut>>(bindings, param);
            return lambda.Compile();
        }

        private static Expression GetBinding<TIn, TOut>(Expression prop, Type targetType, CustomBinding<TIn, TOut>[] customBindings, int depth = 0)
        {
            var ctor = Expression.New(targetType);
            var bindings = targetType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p =>
                {
                    var customBinding = customBindings.FirstOrDefault(cb => targetType == typeof(TOut) && cb.MemberName == p.Name);
                    if (customBinding is object)
                    {
                        var valueExpr = Expression.Invoke(Expression.Constant(customBinding.BindingFunc), prop);
                        return Expression.Bind(p, Expression.Convert(valueExpr, p.PropertyType));
                    }

                    var sourceMi = prop.Type.GetProperty(p.Name);
                    if (sourceMi is null)
                    {
                        return null;
                    }

                    Expression expr = default;
                    var sourceProp = Expression.Property(prop, sourceMi);
                    if (IsSimpleType(p))
                    {
                        expr = sourceProp;
                    }
                    else if (depth == MAX_REFERENCE_DEPTH)
                    {
                        expr = Expression.Default(p.PropertyType);
                    }
                    else if (IsArray(p.PropertyType))
                    {
                        var baseSourceType = sourceProp.Type.GetElementType();
                        var baseTargetType = p.PropertyType.GetElementType();
                        var childParam = Expression.Parameter(baseSourceType, new string('x', depth + 2));
                        expr = ToNullAwareExpression(p, sourceProp, () =>
                            Expression.Call(
                                ENUMERABLE_TO_ARRAY.MakeGenericMethod(baseTargetType),
                                Expression.Call(
                                    ENUMERABLE_SELECT.MakeGenericMethod(baseSourceType, baseTargetType),
                                    sourceProp,
                                    Expression.Lambda(
                                        baseTargetType == baseSourceType ? childParam : ToNullAwareExpression(baseTargetType, childParam, () => GetBinding(childParam, baseTargetType, customBindings, depth + 1)),
                                        childParam))
                                ));
                    }
                    else if (IsGenericType(p.PropertyType, typeof(List<>)))
                    {
                        var baseSourceType = sourceProp.Type.GetGenericArguments()[0];
                        var baseTargetType = p.PropertyType.GetGenericArguments()[0];
                        var childParam = Expression.Parameter(baseSourceType, new string('x', depth + 2));
                        expr = ToNullAwareExpression(p, sourceProp, () =>
                            Expression.Call(
                                ENUMERABLE_TO_LIST.MakeGenericMethod(baseTargetType),
                                Expression.Call(
                                    ENUMERABLE_SELECT.MakeGenericMethod(baseSourceType, baseTargetType),
                                    sourceProp,
                                    Expression.Lambda(
                                        baseTargetType == baseSourceType ? childParam : ToNullAwareExpression(baseTargetType, childParam, () => GetBinding(childParam, baseTargetType, customBindings, depth + 1)),
                                        childParam))
                                ));
                    }
                    else if (IsGenericType(p.PropertyType, typeof(IEnumerable<>)))
                    {
                        var baseSourceType = sourceProp.Type.GetGenericArguments()[0];
                        var baseTargetType = p.PropertyType.GetGenericArguments()[0];
                        var childParam = Expression.Parameter(baseSourceType, new string('x', depth + 2));
                        expr = ToNullAwareExpression(p, sourceProp, () =>
                            Expression.Call(
                                ENUMERABLE_SELECT.MakeGenericMethod(baseSourceType, baseTargetType),
                                sourceProp,
                                Expression.Lambda(
                                    baseTargetType == baseSourceType ? childParam : ToNullAwareExpression(baseTargetType, childParam, () => GetBinding(childParam, baseTargetType, customBindings, depth + 1)),
                                    childParam))
                            );
                    }
                    else
                    {
                        expr = ToNullAwareExpression(p, sourceProp, () => GetBinding(sourceProp, p.PropertyType, customBindings, depth + 1));
                    }

                    return Expression.Bind(p, expr);
                })
                .Where(b => b is object)
                .ToList();
            return Expression.MemberInit(ctor, bindings);
        }

        private static bool IsSimpleType(PropertyInfo prop)
        {
            return prop.PropertyType == typeof(string) || (!prop.PropertyType.IsClass && !prop.PropertyType.IsInterface);
        }

        private static bool IsArray(Type type)
        {
            return type.IsArray;
        }

        private static bool IsGenericType(Type type, Type genericBase)
        {
            var generics = type.GetGenericArguments();
            if (generics.Length != genericBase.GetGenericArguments().Length)
            {
                return false;
            }

            return genericBase.MakeGenericType(generics).IsAssignableFrom(type);
        }

        private static Expression ToNullAwareExpression(PropertyInfo targetProp, MemberExpression sourcePropExpr, Func<Expression> expressionFunc)
        {
            return Expression.Condition(
                test: Expression.Equal(Expression.Default(sourcePropExpr.Type), sourcePropExpr),
                ifTrue: Expression.Default(targetProp.PropertyType),
                ifFalse: expressionFunc());
        }

        private static Expression ToNullAwareExpression(Type targetType, ParameterExpression sourceParamExpr, Func<Expression> expressionFunc)
        {
            return Expression.Condition(
                test: Expression.Equal(Expression.Default(sourceParamExpr.Type), sourceParamExpr),
                ifTrue: Expression.Default(targetType),
                ifFalse: expressionFunc());
        }
    }
}
