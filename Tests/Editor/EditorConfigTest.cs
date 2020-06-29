using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using SnowFlakeGamesAssets.PiscesConfigLoader.Structure;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils.Expression;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Tests
{
    public class EditorConfigTest
    {
        [Test]
        public void TestConfigParse()
        {
            GameConfig.ParseAndLoadConfig(testYaml);

            Assert.AreEqual("value", GameConfig.Query(new ConfigPath("a_nested_map", "key")).AsString());
            Assert.AreEqual(11, GameConfig.Query(new ConfigPath("a_nested_map", "2")).AsInt());
            Assert.AreEqual(12.2f, GameConfig.Query(new ConfigPath("a_nested_map", "another_key")).AsFloat());
            Assert.AreEqual(12.2, GameConfig.Query(new ConfigPath("a_nested_map", "another_key")).AsDouble());
            Assert.AreEqual("hello", GameConfig.Query("a_nested_map", "another_nested_map", "hello").AsString());

            var configNode = GameConfig.Query("a_nested_map", "another_nested_map").AsNode();
            Assert.AreEqual("hello", configNode.Query("hello").AsString());
            Assert.AreEqual("hello2", configNode.Query("hello2").AsString());
        }

        [Test]
        public void TestConfigParseMultiple()
        {
            GameConfig.ParseAndLoadConfigs(new List<string> {testYaml, testYaml2});
            Assert.AreEqual("another value", GameConfig.Query(new ConfigPath("a_nested_map", "key")).AsString());
        }


        [Test]
        public void TestConfigParseSequence()
        {
            GameConfig.ParseAndLoadConfig(testYaml2);

            var nodeList = GameConfig.Query("a_sequence").AsNodeList();

            Assert.AreEqual(5, nodeList.Count);
            Assert.AreEqual("Item 0", nodeList[0].AsString());

            Assert.AreEqual("another_value", nodeList[4].AsNode().Query("another_key").AsString());
        }

        [Test]
        public void TestConfigParseExpression()
        {
            GameConfig.ParseAndLoadConfig(testYaml);
            {
                var expression1 = GameConfig.Query("expression1").AsExpression();
                Assert.AreEqual(9, expression1.Evaluate());
            }
            {
                var expression2 = GameConfig.Query("expression2").AsExpression();
                Assert.AreEqual(10.5 * 5, expression2.SetParam("x", 5).Evaluate());
                Assert.AreEqual(10.5 * 6.3, expression2.SetParam("x", 6.3).Evaluate());
            }
            {
                var expression3 = GameConfig.Query("expression3").AsExpression();
                var expression3Data = GameConfig.Query("expression3-data").AsNode();
                Assert.AreEqual(1.2 * 1.44 * 3.5, expression3.WithConfig(expression3Data).Evaluate());
            }
        }

        [Test]
        public void TestConfigRandomFunction()
        {
            ExpressionParser parser = new ExpressionParser();

            parser.AddFunc("rand", (double[] inputs) =>
            {
                float min = (float) inputs[0];
                float max = (float) inputs[1];

                return Random.Range(min, max);
            });

            var expression = parser.EvaluateExpression("rand(10,20)");

            for (int i = 0; i < 100; i++)
            {
                var expressionValue = expression.Value;
                Assert.LessOrEqual(10,expressionValue);
                Assert.GreaterOrEqual(20,expressionValue);
            }
        }


        [Test]
        [Ignore("Takes too long")]
        public void TestConfigExpressionSpeed()
        {
            const int iterationNum = 10000;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < iterationNum; i++)
            {
                builder.Append($"expression{i}: {i}*x^5+3*123.23-23/4*x\n");
            }

            GameConfig.ParseAndLoadConfig(builder.ToString());

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterationNum; i++)
            {
                var expression = GameConfig.Query($"expression{i}").AsExpression();
                var d = expression.SetParam("x", 3).Evaluate();
//                Assert.AreEqual(i * 3 + 1, expression.SetParam("x", 3).Evaluate());
            }

            sw.Stop();
            Debug.Log($"Running {iterationNum} expression took {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (double) iterationNum} on average)");
        }


        private string testYaml =
            @"# Sequences (equivalent to lists or arrays) look like this
# (note that the '-' counts as indentation):
a_nested_map:
  key: value
  2: 11
  another_key: 12.2
  another_nested_map:
    hello: hello
    hello2: hello2

a_nested_map2:
  key: value
  another_key: Another Value
  another_nested_map:
    hello: hello

expression1: (5-2)*3
expression2: 10.5*x
expression3: 1.2*y*z

expression3-data:
    y: 1.44
    z: 3.5
";


        private string testYaml2 =
            @"# Sequences (equivalent to lists or arrays) look like this
# (note that the '-' counts as indentation):
a_sequence:
- Item 0
- Item 3
- 0.5  # sequences can contain disparate types.
- Item 4
- key: value
  another_key: another_value

a_sequence_3:
- Item 1
- Item 2
- 0.5  # sequences can contain disparate types.
- Item 4
- key: value
  another_key: another_value

a_nested_map:
  key: another value
";
    }
}