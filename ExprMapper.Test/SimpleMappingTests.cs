using NUnit.Framework;
using System;

namespace ExprMapper.Test
{
    public class SimpleMappingTests
    {
        [Test]
        public void NullReferenceTest()
        {
            var mapper = new Mapper().Add<L, R>();
            Assert.IsNull(mapper.Map<L, R>(null));
        }

        [Test]
        public void SimpleObjectTest()
        {
            var mapper = new Mapper().Add<L, R>();
            var inst = L.Instance;

            var result = mapper.Map<L, R>(inst);

            Assert.AreEqual(inst.Id, result.Id);
            Assert.AreEqual(inst.Code, result.Code);
            Assert.AreEqual(inst.Date, result.Date);
            Assert.AreEqual(inst.Name, result.Name);
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