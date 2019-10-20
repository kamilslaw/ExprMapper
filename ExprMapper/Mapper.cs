using System;
using System.Collections.Generic;
using System.Linq;

namespace ExprMapper
{
    public class Mapper
    {
        private Dictionary<(Type, Type), Delegate> _mappings = new Dictionary<(Type, Type), Delegate>();

        public Mapper Add<TSource, TDestination>(params CustomBinding<TSource, TDestination>[] bindings)
        {
            var mapping = ExpressionGenerator.GetMapper(bindings);
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

        public IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source)
        {
            if (source == default)
            {
                return default;
            }

            var mapping = _mappings[(typeof(TSource), typeof(TDestination))] as Func<TSource, TDestination>;
            return source.Select(mapping);
        }

        public IList<TDestination> Map<TSource, TDestination>(IList<TSource> source)
        {
            if (source == default)
            {
                return default;
            }

            var result = new List<TDestination>(source.Count);
            foreach (var el in source)
            {
                result.Add(Map<TSource, TDestination>(el));
            }

            return result;
        }

        public TDestination[] Map<TSource, TDestination>(TSource[] source)
        {
            if (source == default)
            {
                return default;
            }

            var result = new TDestination[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = Map<TSource, TDestination>(source[i]);
            }

            return result;
        }
    }
}
