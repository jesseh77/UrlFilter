using System;
using System.Collections.Generic;

namespace UrlFilter.Tests
{
    class TestDocument
    {
        public TestDocument(){}

        public TestDocument(int seed, bool includeSubdocument = false)
        {
            Value = seed;
            AnotherValue = Math.Pow(-1, seed);
            NullableValue = seed % 2 == 0 ? null : (int?)seed;
            Text = $"Item {seed}";
            MoreText = $"Item{seed}";
            if (includeSubdocument)
            {
                SubDocument = new TestDocument(seed *100);
            }            
        }
        public int Value { get; set; }
        public int? NullableValue { get; set; }
        public double AnotherValue { get; set; }
        public DateTime ADate { get; set; }
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
