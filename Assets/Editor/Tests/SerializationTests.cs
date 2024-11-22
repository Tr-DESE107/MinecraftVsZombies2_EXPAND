using MVZ2Logic;
using NUnit.Framework;
using Tools;
using UnityEngine;

namespace MVZ2.EditorTests
{
    public class SerializationTests
    {
        [Test]
        public static void RNGTest()
        {
            SerializeHelper.init("mvz2");

            var rng = new RandomGenerator(1234);

            var json = rng.ToBson();
            var seri = SerializeHelper.FromBson<RandomGenerator>(json);
            var json2 = seri.ToBson();

            Debug.Log(json);
            Debug.Log(json2);
            Assert.AreEqual(json, json2);

            rng.Next();
            rng.Next();
            rng.Next();
            var json3 = rng.ToBson();
            seri.Next();
            seri.Next();
            seri.Next();
            var json4 = seri.ToBson();
            Debug.Log(json3);
            Debug.Log(json4);
            Assert.AreEqual(json3, json4);
        }
    }
}
