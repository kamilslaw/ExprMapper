using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace ExprMapper.Test
{
    public class CustomMappingTests
    {
        [Test]
        public void ExpressionClosureTest()
        {
            var lambdas = new Func<int>[10];
            var expressions = new Expression<Func<int>>[10];
            for (int i = 0; i < 10; i++)
            {
                lambdas[i] = () => i;
                expressions[i] = () => i;
            }

            foreach (var l in lambdas) Assert.AreEqual(10, l());
            foreach (var e in expressions) Assert.AreEqual(10, e.Compile()());
        }

        [Test]
        public void CustomAssignTest()
        {
            Expression<Func<L, object>> aa = r => r.Name;
            var mapper = new Mapper().Add<L, R>(
                (r => r.Value, l => l.Year * 2L),
                (r => r.LastName, l => l.Name));
            var inst = L.Instance;

            var result = mapper.Map<L, R>(inst);

            Assert.AreEqual(inst.Id, result.Id);
            Assert.AreEqual(inst.Name, result.LastName);
            Assert.AreEqual(inst.Year * 2, result.Value);
            Assert.AreEqual(inst.Child.Id, result.Child.Id);
            Assert.AreEqual(inst.Child.Name, result.Child.LastName);
            Assert.AreEqual(inst.Child.Year * 2, result.Child.Value);
            Assert.IsNull(result.Child.Child);

        }

        public class L
        {
            public static L Instance => new L
            {
                Id = 1,
                Name = "Smith",
                Year = 2019,
                Child = new L
                {
                    Id = 2,
                    Name = "John",
                    Year = 44
                }
            };

            public int Id { get; set; }
            public string Name { get; set; }
            public int Year { get; set; }
            public L Child { get; set; }
        }

        public class R
        {
            public int Id { get; set; }
            public string LastName { get; set; }
            public long Value { get; set; }
            public R Child { get; set; }
        }
    }
}
