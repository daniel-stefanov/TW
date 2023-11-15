using TW.Model;

namespace TW.Control
{
    public class RecommendationModule : IRecommendationModule
    {
        public Report ApplyRecommendations(Report report)
        {
            List<string> warnings = new List<string>();
            List<string> recommendations = new List<string>();

            IEnumerable<ElementAnalysis> flatElements = report.Paths.SelectMany(x => x.Path).DistinctBy(x => x.Element.Id);
            IEnumerable<ElementAnalysis> flatPassive = flatElements.Where(x => x.Element is not Generator && x.Element is not Consumer);

            foreach (ElementAnalysis element in flatPassive)
            {
                if (element.LoadPercent > 95)
                {
                    warnings.Add($"Id {element.Element.Id} close to overload.");
                }
                if (element.OptimalityPercentDelta > element.LoadPercent)
                {
                    warnings.Add($"Id {element.Element.Id} very far from optimal load.");
                }
                if (element.Loss * 2 > ((element.LoadPercent / 100) * element.Element.MaxCapacity))
                {
                    warnings.Add($"Id {element.Element.Id} unjustified losses.");
                }
            }

            foreach (PathAnalysis path in report.Paths)
            {
                ElementAnalysis hotspot = path.PassiveElements.MaxBy(x => x.LoadPercent);
                if (hotspot.LoadPercent > (path.AverageLoadPercent * 2))
                {
                    warnings.Add($"Id {hotspot.Element.Id} is hotspot in path {path.Path.Articulate()}");
                }
            }

            IEnumerable<IGrouping<string, PathAnalysis>> parallelPaths = report.Paths.GroupBy(x => $"{x.Path.First().Element.Id}>>>{x.Path.Last().Element.Id}").Where(x => x.Count() > 1);

            foreach (IGrouping<string, PathAnalysis> group in parallelPaths)
            {
                if (group.Average(x => x.AverageLoadPercent) < (100 / group.Count())
                    && group.Average(x => x.AverageOptimalityPercentDelta) > 25)
                {
                    recommendations.Add(
                    $"{group.Count()} parallel paths on {group.Key} with low load and load optimality." +
                    $" Potentially load-balance by disconnecting least optimal path:" +
                    $"{group.MaxBy(x => x.AverageOptimalityPercentDelta).Path.Articulate()}");
                }
            }

            if (report.SupplyDemandRatio < 1.1)
            {
                recommendations.Add($"Potentially insufficient network supply, ratio {report.SupplyDemandRatio}");
            }

            IEnumerable<ElementAnalysis> generators = flatElements.Where(x => x.Element is Generator);
            double averageProduction = generators.Select(x => x.Element).Average(x => (x as Generator).Production);
            IEnumerable<ElementAnalysis> largeConsumers = flatElements.Where(x => x.Element is Consumer && x.Element.Demand > averageProduction / 10);
            if (largeConsumers.Count() * 2 > generators.Count())
            {
                recommendations.Add($"Large consumer to generator ratio greater than 2/1, potential vulnerability in case of production inconsistency. Look into adding generators.");
            }

            report.Warnings = warnings;
            report.Recommendations = recommendations;

            return report;
        }

        //private string ArticulatePath(List<ElementAnalysis> path)
        //{
        //    return string.Join(' ', path.Select(x => x.Element.Id).ToArray());
        //}
    }
}
