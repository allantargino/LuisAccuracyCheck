using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Cognitive.LUIS.Structures.Manager
{
    public class EntityLabel
    {
        public string entityName { get; set; }
        public int startTokenIndex { get; set; }
        public int endTokenIndex { get; set; }
    }

    public class IntentPrediction
    {
        public string name { get; set; }
        public double score { get; set; }
    }

    public class EntityPrediction
    {
        public string entityName { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public string phrase { get; set; }
    }

    public class LabelResponse
    {
        public int id { get; set; }
        public string text { get; set; }
        public List<string> tokenizedText { get; set; }
        public string intentLabel { get; set; }
        public List<EntityLabel> entityLabels { get; set; }
        public List<IntentPrediction> intentPredictions { get; set; }
        public List<EntityPrediction> entityPredictions { get; set; }
    }
}
