using JackpotOracle.Global;

namespace JackpotOracle.Pages
{
    public partial class Home
    {
        private RowGenerationType RowGenerationType { get; set; } = RowGenerationType.System;
        private int NumberOfSets { get; set; } = 10;
        private HashSet<int> SelectedMainNumbers { get; set; } = new();
        private HashSet<int> SelectedStarNumbers { get; set; } = new();
        private Dictionary<int, List<int>> GeneratedCombinations { get; set; } = new();
        private bool _expanded = false;

        private bool CanGenerate => RowGenerationType == RowGenerationType.Random ||
        (SelectedMainNumbers.Count >= 5 && SelectedStarNumbers.Count >= 2);

        private int CombinationCount => CalculateCombinations();
        protected override void OnInitialized()
        {
            _expanded = RowGenerationType == RowGenerationType.System;
        }

        private void ToggleMainNumber(int number)
        {
            if (SelectedMainNumbers.Contains(number))
                SelectedMainNumbers.Remove(number);
            else
                SelectedMainNumbers.Add(number);
        }

        private void ToggleStarNumber(int number)
        {
            if (SelectedStarNumbers.Contains(number))
                SelectedStarNumbers.Remove(number);
            else
                SelectedStarNumbers.Add(number);
        }

        private void ValidateNumber()
        {
            if (NumberOfSets < 0)
                NumberOfSets = 0;
            else if (NumberOfSets > 999)
                NumberOfSets = 999;
        }

        private int CalculateCombinations()
        {
            int mainCount = SelectedMainNumbers.Count;
            int starCount = SelectedStarNumbers.Count;

            if (mainCount < 5 || starCount < 2)
                return 0;

            return BinomialCoefficient(mainCount, 5) * BinomialCoefficient(starCount, 2);
        }

        private int BinomialCoefficient(int n, int r)
        {
            if (r > n) return 0;
            if (r == 0 || r == n) return 1;
            r = Math.Min(r, n - r);
            int c = 1;
            for (int i = 0; i < r; i++)
            {
                c = c * (n - i) / (i + 1);
            }
            return c;
        }

        private void GenerateCombinations()
        {
            var newCombinations = new Dictionary<int, List<int>>();

            if (RowGenerationType == RowGenerationType.Random)
            {
                newCombinations = GetRandomCombinations();
            }
            else if (RowGenerationType == RowGenerationType.System)
            {
                newCombinations = GenerateSystemCombinations();
            }

            GeneratedCombinations = newCombinations;
        }

        private Dictionary<int, List<int>> GenerateSystemCombinations()
        {
            var newCombinations = new Dictionary<int, List<int>>();
            int setNo = 1;

            var mainCombinations = GetCombinations(SelectedMainNumbers.ToList(), 5);
            var starCombinations = GetCombinations(SelectedStarNumbers.ToList(), 2);

            foreach (var mainCombo in mainCombinations)
            {
                foreach (var starCombo in starCombinations)
                {
                    var sortedMainCombo = mainCombo.OrderBy(x => x).ToList();
                    var sortedStarCombo = starCombo.OrderBy(x => x).ToList();
                    var fullCombo = sortedMainCombo.Concat(sortedStarCombo).ToList();

                    if (!newCombinations.Values.Any(existingCombo => existingCombo.SequenceEqual(fullCombo)))
                    {
                        newCombinations[setNo++] = fullCombo;
                    }

                    if (newCombinations.Count >= NumberOfSets)
                    {
                        return newCombinations;
                    }
                }
            }

            return newCombinations;
        }

        private Dictionary<int, List<int>> GetRandomCombinations()
        {
            var newCombinations = new Dictionary<int, List<int>>();
            var random = new Random();

            for (int setNumber = 1; setNumber <= NumberOfSets; setNumber++)
            {
                var randomMainNumbers = Enumerable.Range(1, 50).OrderBy(_ => random.Next()).Take(5).OrderBy(x => x).ToList();
                var randomStarNumbers = Enumerable.Range(1, 12).OrderBy(_ => random.Next()).Take(2).OrderBy(x => x).ToList();

                var fullCombo = randomMainNumbers.Concat(randomStarNumbers).ToList();
                newCombinations[setNumber] = fullCombo;
            }
            return newCombinations;
        }

        private IEnumerable<List<int>> GetCombinations(List<int> list, int length)
        {
            if (length == 0) return new List<List<int>> { new List<int>() };
            if (!list.Any()) return new List<List<int>>();

            var head = list.First();
            var tail = list.Skip(1).ToList();

            var withoutHead = GetCombinations(tail, length);
            var withHead = GetCombinations(tail, length - 1).Select(c => new List<int> { head }.Concat(c).ToList());

            return withHead.Concat(withoutHead);
        }

        private void OnExpandCollapseClick()
        {
            _expanded = !_expanded;
        }

        private void ResetSelection()
        {
            SelectedMainNumbers.Clear();
            SelectedStarNumbers.Clear();
            GeneratedCombinations.Clear();
            NumberOfSets = 10;
        }
    }
}
