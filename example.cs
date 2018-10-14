using Bogus;
using Bogus.Extensions;
using Newtonsoft.Json;
using NFluent;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Check = NFluent.Check;

namespace PseudoPbt
{
    [TestFixture]
    public class GetDomainCommandShould
    {
        private static IEnumerable<MyMessageType> Generate()
        {
            string RandomString(Faker f) => (string) f.Lorem.Text().OrNull(f);
            Guid RandomGuid(Faker f) => f.Random.Uuid();
            bool RandomBool(Faker f) => f.Random.Bool();
            int RandomInt(Faker f) => f.Random.Int();
            ushort RandomUShort(Faker f) => f.Random.UShort();
            DateTime RandomDate(Faker f) => f.Date.Recent(60);

            var fakeJournal = new Faker<Journal>().CustomInstantiator(f => new Journal(
                RandomGuid(f),
                RandomInt(f),
                RandomDate(f)));
            var fakeIntervention = new Faker<Intervention>().CustomInstantiator(f => new Intervention(
                RandomGuid(f),
                RandomGuid(f),
                RandomString(f),
                fakeJournal.Generate()));
            var fakeInformation = new Faker<Information>().CustomInstantiator(f => new Information(
                RandomString(f),
                RandomBool(f),
                RandomString(f)));
            var fakeMyMessageType = new Faker<MyMessageType>().CustomInstantiator(f => new MyMessageType(
                RandomGuid(f),
                RandomGuid(f),
                null,
                (IList<Intervention>)fakeIntervention.Generate(RandomUShort(f)).OrNull(f),
                (Information)fakeInformation.Generate().OrNull(f),
                (Journal)fakeJournal.Generate().OrNull(f)));

            return fakeMyMessageType.Generate(10);
        }

        private static IEnumerable<MyMessageType> MyMessageTypeRandom => Generate();

        [Test]
        [TestCaseSource(nameof(MyMessageTypeRandom))]
        public void DoesNotThrowIfRandomMessage(MyMessageType myMessageType)
        {
            Check.ThatCode(MyMessageType.GetDomainCommand).DoesNotThrow();
        }
    }
}
