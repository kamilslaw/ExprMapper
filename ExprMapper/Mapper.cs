using System;
using System.Collections.Generic;

namespace ExprMapper
{
    public class Mapper
    {
        private Dictionary<(Type, Type), Delegate> _mappings = new Dictionary<(Type, Type), Delegate>();

        public Mapper Add<TSource, TDestination>()
        {
            var mapping = ExpressionGenerator.GetMapper<TSource, TDestination>();
            _mappings.Add((typeof(TSource), typeof(TDestination)), mapping);
            return this;
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            if (source == default)
            {
                return default;
            }

            var mapping = _mappings[(typeof(TSource), typeof(TDestination))] as Func<TSource, TDestination>;
            return mapping(source);
        }
    }
}
