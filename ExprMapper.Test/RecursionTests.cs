using NUnit.Framework;
using System;

namespace ExprMapper.Test
{
    class RecursionTests
    {
        [Test]
        public void RecursionTest()
        {
            var mapper = new Mapper().Add<L, R>();
            var inst = L.Instance;

            var result = mapper.Map<L, R>(inst);

            Assert.AreEqual(inst.Id, result.Id);
            Assert.AreEqual(inst.Code, result.Code);
            Assert.AreEqual(inst.Date, result.Date);
            Assert.AreEqual(inst.Name, result.Name);

            Assert.AreEqual(inst.Child.Id, result.Child.Id);
            Assert.AreEqual(inst.Child.City, result.Child.City);
            Assert.IsNull(inst.Child.Parent);
        }

        [Test]
        public void ReferenceUniquenessTest()
        {
            var mapper = new Mapper().Add<L, R>();
            var inst = new L { Id = 44, Child = new L.LCh { Id = 88 } };
            inst.Child.Parent = inst;

            var result = mapper.Map<L, R>(inst);

            Assert.AreEqual(inst.Id, result.Id);
            Assert.AreEqual(inst.Child.Id, result.Child.Id);

            Assert.AreEqual(result.Id, result.Child.Parent.Id);
            Assert.AreNotEqual(result, result.Child.Parent);
        }

        public class L
        {
            public static L Instance => new L
            {
                Id = 11,
                Code = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Name = "Some user",
                Child = new LCh
                {
                    Id = 44,
                    City = "Kraków",
                    Parent = null
                }
            };

            public int Id { get; set; }
            public Guid Code { get; set; }
            public DateTime Date { get; set; }
            public string Name { get; set; }
            public LCh Child { get; set; }

            public class LCh
            {
                public int Id { get; set; }
                public string City { get; set; }
                public L Parent { get; set; }
            }
        }

        public class R
        {
            public int Id { get; set; }
            public Guid Code { get; set; }
            public DateTime Date { get; set; }
            public string Name { get; set; }
            public RCh Child { get; set; }

            public class RCh
            {
                public int Id { get; set; }
                public string City { get; set; }
                public R Parent { get; set; }
            }
        }
    }
}
