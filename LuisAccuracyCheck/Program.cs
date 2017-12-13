using Microsoft.Cognitive.LUIS;
using Microsoft.Cognitive.LUIS.Manager;
using Microsoft.Cognitive.LUIS.Structures.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuisAccuracyCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            string subscriptionKey = "<YOUR PROGRAMATIC SUBSCRIPTION KEY>";
            string appName = "<YOUR APP NAME>";
            string appVersion = "<YOUR APP VERSION>";

            MainAsync(subscriptionKey, appName, appVersion).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string subscriptionKey, string appName, string appVersion)
        {
            var manager = new LuisManager(subscriptionKey);
            var application = await manager.Apps.GetApplicationAsync(appName);

            var intents = await application.Intent.GetIntentsAsync(versionId: appVersion);
            var labels = await application.Intent.GetAllLabelsAsync(versionId: appVersion);

            AnalyzeData(intents, labels, out Dictionary<string, double> accuracyErrors, out Dictionary<string, double> accuracyAvg);

            Report(nameof(accuracyErrors), accuracyErrors);
            Report(nameof(accuracyAvg), accuracyAvg);

            Console.ReadLine();
        }

        private static void AnalyzeData(IEnumerable<LuisIntent> intents, IEnumerable<LabelResponse> labels, out Dictionary<string, double> accuracyErrors, out Dictionary<string, double> accuraryAvg)
        {
            accuracyErrors = new Dictionary<string, double>();
            accuraryAvg = new Dictionary<string, double>();
            foreach (var intent in intents)
            {
                var name = intent.Name;
                var specificLabels = labels.Where(l => l.intentLabel == name);

                var correct = specificLabels.Where(l => l.intentLabel == l.intentPredictions.First().name);
                var incorrect = specificLabels.Where(l => l.intentLabel != l.intentPredictions.First().name);

                accuracyErrors.Add(name, ((double)correct.Count()) / specificLabels.Count());
                accuraryAvg.Add(name, correct.Select(l => l.intentPredictions.First().score).Average());
            }
        }

        private static void Report(string title, Dictionary<string, double> dictionary)
        {
            Console.WriteLine(title);
            foreach (var item in dictionary)
                Console.WriteLine($"{item.Key}\t{item.Value}");
            Console.WriteLine();
        }
    }
}
