using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace trackrequests
{
    [TestFixture]
    class TrackTests
    {
        [Test]
        public void TestBasicTrack()
        {
            Track opsLast5seconds = new Track(
                130,
                5,
                (one, two) =>
                {
                    return one.Year == two.Year
                        && one.Month == two.Month
                        && one.Day == two.Day
                        && one.Hour == two.Hour
                        && one.Minute == two.Minute
                        && one.Second == two.Second;
                },
                (one, two) =>
                {
                    return (int) (two - one).TotalSeconds;
                });

            DateTime start = new DateTime(2020, 04, 24, 16, 00, 00);

            opsLast5seconds.Operation(start.AddSeconds(1), 120, 1);
            opsLast5seconds.Operation(start.AddSeconds(1), 120, 1);
            opsLast5seconds.Operation(start.AddSeconds(1), 121, 1);
            opsLast5seconds.Operation(start.AddSeconds(1), 122, 1);
            opsLast5seconds.Operation(start.AddSeconds(1), 122, 1);

            opsLast5seconds.Operation(start.AddSeconds(2), 120, 1);
            opsLast5seconds.Operation(start.AddSeconds(2), 121, 1);
            opsLast5seconds.Operation(start.AddSeconds(2), 122, 1);
            opsLast5seconds.Operation(start.AddSeconds(2), 122, 1);

            opsLast5seconds.Operation(start.AddSeconds(3), 120, 1);

            opsLast5seconds.Operation(start.AddSeconds(4), 120, 1);
            opsLast5seconds.Operation(start.AddSeconds(4), 121, 1);
            opsLast5seconds.Operation(start.AddSeconds(4), 122, 1);

            opsLast5seconds.Operation(start.AddSeconds(5), 121, 1);
            opsLast5seconds.Operation(start.AddSeconds(5), 122, 1);

            Assert.AreEqual(15, opsLast5seconds.GetTotal(start.AddSeconds(5)));

            opsLast5seconds.Operation(start.AddSeconds(6), 121, 1);

            Assert.AreEqual(11, opsLast5seconds.GetTotal(start.AddSeconds(6)));

            Assert.AreEqual(6, opsLast5seconds.GetTotal(start.AddSeconds(8)));

            Assert.AreEqual(0, opsLast5seconds.GetTotal(start.AddSeconds(120)));

            opsLast5seconds.Operation(start.AddSeconds(200), 120, 1);
            opsLast5seconds.Operation(start.AddSeconds(200), 120, 1);

            Assert.AreEqual(2, opsLast5seconds.GetTotal(start.AddSeconds(201)));
        }
    }
}
