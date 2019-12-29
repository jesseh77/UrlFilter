using System.Collections.Generic;

namespace UrlFilter.Tests
{
    class TestDocument
    {
        public int Value { get; set; }
        public int? NullableValue { get; set; }
        public double AnotherValue { get; set; }
        public string Text { get; set; }
        public string MoreText { get; set; }
        public TestDocument SubDocument { get; set; }
        public List<TestDocument> DocumentCollection { get; set; }
        public State State { get; set; }
    }

    enum State
    {
        StateOne,
        StateTwo,
        StateThree
    }
}
