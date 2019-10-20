using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExprMapper.Bench
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(BenchmarkRunner.Run<Test>());
            Console.ReadLine();
        }
    }

    [RankColumn, MinColumn, MaxColumn]
    public class Test
    {
        private Mapper _mapper;
        private Mapper _mapperWithCustomBinding;
        private List<L> _list;
        private AutoMapper.Mapper _autoMapper;
        private AutoMapper.Mapper _autoMapperWithCustomBinding;

        [GlobalSetup]
        public void Setup()
        {
            _mapper = new Mapper().Add<L, R>();
            _mapperWithCustomBinding = new Mapper().Add<L, R>(
                (r => r.Id, l => l.Id),
                (r => r.Name, l => l.Name),
                (r => r.Year, l => l.Year));

            var config = new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<L, R>());
            _autoMapper = new AutoMapper.Mapper(config);
            var withCustomBindingConfig = new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<L, R>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom((l, r, m) => r.Id = m))
                .ForMember(dest => dest.Name, opt => opt.MapFrom((l, r, m) => r.Name = m))
                .ForMember(dest => dest.Year, opt => opt.MapFrom((l, r, m) => r.Year = m)));
            _autoMapperWithCustomBinding = new AutoMapper.Mapper(withCustomBindingConfig);

            _list = Enumerable.Range(0, 10_000).Select(_ => L.Instance).ToList();
        }

        [Benchmark]
        public R Mapper() => _mapper.Map<L, R>(L.Instance);

        [Benchmark]
        public R MapperWithCustomBindings() => _mapperWithCustomBinding.Map<L, R>(L.Instance);

        [Benchmark]
        public R Native() => Map(L.Instance);

        [Benchmark]
        public R AutoMapper() => _autoMapper.Map<R>(L.Instance);

        [Benchmark]
        public R AutoMapperWithCustomBindings() => _autoMapperWithCustomBinding.Map<R>(L.Instance);

        [Benchmark]
        public List<R> Mapper_List() => _mapper.Map<L, R>(_list);

        [Benchmark]
        public List<R> MapperWithCustomBindings_List() => _mapperWithCustomBinding.Map<L, R>(_list);

        [Benchmark]
        public List<R> Native_List() => _list.Select(Map).ToList();

        [Benchmark]
        public List<R> AutoMapper_List() => _autoMapper.Map<List<R>>(_list);

        [Benchmark]
        public List<R> AutoMapperWithCustomBindings_List() => _autoMapperWithCustomBinding.Map<List<R>>(_list);

        private static R Map(L l) => new R
        {
            Id = l.Id,
            Name = l.Name,
            Year = l.Year,
            Child = l.Child is null ? null : Map(l.Child)
        };
    }
}
