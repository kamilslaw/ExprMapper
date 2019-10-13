using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ExprMapper.Test
{
    public class CollectionsTests
    {
        private const int N = 100;

        [Test]
        public void EmptyCollectionMappingTest()
        {
            var mapper = new Mapper().Add<L, R>();
            var result = mapper.Map<L, R>(Enumerable.Range(0, 0).Select(_ => new L()));
            Assert.IsEmpty(result);
        }


        [Test]
        public void NullReferenceCollectionTest()
        {
            var mapper = new Mapper().Add<L, R>();
            var result = mapper.Map<L, R>(default(IEnumerable<L>));
            Assert.IsNull(result);
        }

        [Test]
        public void IEnumerableMappingTest()
        {
            var mapper = new Mapper().Add<L, R>();
            var result = mapper.Map<L, R>(Enumerable.Range(0, N).Select(i => new L { Id = i, Val = $"{i}" }));
            CollectionAssertions(result.ToList());
        }

        [Test]
        public void ListMappingTest()
        {
            var mapper = new Mapper().Add<L, R>();
            var result = mapper.Map<L, R>(Enumerable.Range(0, N).Select(i => new L { Id = i, Val = $"{i}" }).ToList());
            CollectionAssertions(result);
        }

        [Test]
        public void ArrayMappingTest()
        {
            var mapper = new Mapper().Add<L, R>();
            var result = mapper.Map<L, R>(Enumerable.Range(0, N).Select(i => new L { Id = i, Val = $"{i}" }).ToArray());
            CollectionAssertions(result);
        }

        private static void CollectionAssertions(IList<R> result)
        {
            Assert.AreEqual(N, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(i, result[i].Id);
                Assert.AreEqual(i.ToString(), result[i].Val);
            }
        }

        public class L
        {
            public int Id { get; set; }
            public string Val { get; set; }
        }

        public class R
        {
            public int Id { get; set; }
            public string Val { get; set; }
        }
    }
}
