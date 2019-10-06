using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ExprMapper.Test
{
    public class SimpleMappingTests
    {
        [Test]
        public void NoMappingTest()
        {
            var mapper = new Mapper().Add<L, R>();
            Assert.Throws<KeyNotFoundException>(() => mapper.Map<L, N>(new L()));
        }

        [Test]
        public void NullReferenceTest()
        {
            var mapper = new Mapper().Add<L, R>();
            Assert.IsNull(mapper.Map<L, R>(null));
        }

        [Test]
        public void SimpleObjectTest()
        {
            var mapper = new Mapper()
                .Add<L, N>()
                .Add<L, R>()
                .Add<R, L>();
            var inst = L.Instance;

            var result = mapper.Map<L, R>(inst);

            Assert.AreEqual(inst.Id, result.Id);
            Assert.AreEqual(inst.Code, result.Code);
            Assert.AreEqual(inst.Date, result.Date);
            Assert.AreEqual(inst.Name, result.Name);
        }

        public class N
        {
        }

        public class L
        {
            public static L Instance => new L
            {
                Id = 11,
                Code = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Name = "Some user"
            };

            public int Id { get; set; }
            public Guid Code { get; set; }
            public DateTime Date { get; set; }
            public string Name { get; set; }
        }

        public class R
        {
            public int Id { get; set; }
            public Guid Code { get; set; }
            public DateTime Date { get; set; }
            public string Name { get; set; }
        }
    }
}