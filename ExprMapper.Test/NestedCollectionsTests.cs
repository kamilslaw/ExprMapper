using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ExprMapper.Test
{
    public class NestedCollectionsTests
    {
        [Test]
        public void ArrayPropertyMappingTest()
        {
            var mapper = new Mapper().Add<LA, RA>();
            var array = new[] { "44", "hello", null, "world" };
            var arrayObj = new[] { new LA { Id = 1, ValObj = new[] { new LA { Id = 11 }, null } }, new LA { Id = 2, Val = new [] { "some" } } };
            var result = mapper.Map<LA, RA>(new LA { Id = 1, ValObj = arrayObj, Val = array });

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(array, result.Val);
            Assert.AreEqual(1, result.ValObj[0].Id);
            Assert.AreEqual(11, result.ValObj[0].ValObj[0].Id);
            Assert.IsNull(result.ValObj[0].ValObj[1]);
            Assert.AreEqual(2, result.ValObj[1].Id);
            Assert.IsNull(result.ValObj[1].ValObj);
            Assert.AreEqual(new[] { "some" }, result.ValObj[1].Val);
        }

        [Test]
        public void ListPropertyMappingTest()
        {
            var mapper = new Mapper().Add<LL, RL>();
            var array = new List<string> { "44", "hello", null, "world" };
            var arrayObj = new List<LL> { new LL { Id = 1, ValObj = new List<LL> { new LL { Id = 11 }, null } }, new LL { Id = 2, Val = new List<string> { "some" } } };
            var result = mapper.Map<LL, RL>(new LL { Id = 1, ValObj = arrayObj, Val = array });

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(array, result.Val);
            Assert.AreEqual(1, result.ValObj[0].Id);
            Assert.AreEqual(11, result.ValObj[0].ValObj[0].Id);
            Assert.IsNull(result.ValObj[0].ValObj[1]);
            Assert.AreEqual(2, result.ValObj[1].Id);
            Assert.IsNull(result.ValObj[1].ValObj);
            Assert.AreEqual(new[] { "some" }, result.ValObj[1].Val);
        }

        [Test]
        public void IEnumerablePropertyMappingTest()
        {
            var mapper = new Mapper().Add<LE, RE>();
            var array = new List<string> { "44", "hello", null, "woREd" };
            var arrayObj = new List<LE> { new LE { Id = 1, ValObj = new List<LE> { new LE { Id = 11 }, null } }, new LE { Id = 2, Val = new List<string> { "some" } } };
            var result = mapper.Map<LE, RE>(new LE { Id = 1, ValObj = arrayObj, Val = array });

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(array, result.Val);
            Assert.AreEqual(1, result.ValObj.First().Id);
            Assert.AreEqual(11, result.ValObj.First().ValObj.First().Id);
            Assert.IsNull(result.ValObj.First().ValObj.Last());
            Assert.AreEqual(2, result.ValObj.Last().Id);
            Assert.IsNull(result.ValObj.Last().ValObj);
            Assert.AreEqual(new[] { "some" }, result.ValObj.Last().Val);
        }

        public class LA
        {
            public int Id { get; set; }
            public string[] Val { get; set; }
            public LA[] ValObj { get; set; }
        }

        public class RA
        {
            public int Id { get; set; }
            public string[] Val { get; set; }
            public RA[] ValObj { get; set; }
        }

        public class LL
        {
            public int Id { get; set; }
            public List<string> Val { get; set; }
            public List<LL> ValObj { get; set; }
        }

        public class RL
        {
            public int Id { get; set; }
            public List<string> Val { get; set; }
            public List<RL> ValObj { get; set; }
        }

        public class LE
        {
            public int Id { get; set; }
            public IEnumerable<string> Val { get; set; }
            public IEnumerable<LE> ValObj { get; set; }
        }

        public class RE
        {
            public int Id { get; set; }
            public IEnumerable<string> Val { get; set; }
            public IEnumerable<RE> ValObj { get; set; }
        }
    }
}
