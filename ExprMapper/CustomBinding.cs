using System;
using System.Linq.Expressions;

namespace ExprMapper
{
    public class CustomBinding<TSource, TDestination>
    {
        public CustomBinding(Expression<Func<TDestination, object>> memberExpression, Func<TSource, object> bindingFunc)
        {
            if (memberExpression is null)
            {
                throw new ArgumentNullException(nameof(memberExpression));
            }

            var targetExpression = (memberExpression.Body is UnaryExpression)
                ? (memberExpression.Body as UnaryExpression).Operand
                : memberExpression.Body;

            if (!(targetExpression is MemberExpression))
            {
                throw new ArgumentException(nameof(memberExpression));
            }

            MemberName = (targetExpression as MemberExpression).Member.Name;
            BindingFunc = bindingFunc ?? throw new ArgumentNullException(nameof(bindingFunc));
        }

        public string MemberName { get; }
        public Func<TSource, object> BindingFunc { get; }

        public static implicit operator CustomBinding<TSource, TDestination>((Expression<Func<TDestination, object>>, Func<TSource, object>) tuple)
            => new CustomBinding<TSource, TDestination>(tuple.Item1, tuple.Item2);
    }
}
