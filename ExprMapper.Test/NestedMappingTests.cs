using NUnit.Framework;
using System;

namespace ExprMapper.Test
{
    public class NestedMappingTests
    {
        [Test]
        public void NestedObjectTest()
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
            Assert.AreEqual(inst.Child.Child.Name, result.Child.Child.Name);
            Assert.IsNull(result.Child.ChildNull);
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
                    Child = new LCh.LChCh { Name = "DD" },
                    ChildNull = null
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
                public LChCh Child { get; set; }
                public LChCh ChildNull { get; set; }

                public class LChCh
                {
                    public string Name { get; set; }
                }
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
                public RChCh Child { get; set; }
                public RChCh ChildNull { get; set; }

                public class RChCh
                {
                    public string Name { get; set; }
                }
            }
        }
    }
}
