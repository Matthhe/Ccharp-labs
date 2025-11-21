using System.Linq.Expressions;
using System.Text;

struct GeneticData
{
    public string protein;    
    public string organism;
    public string amino_acids; 
}

class Program
{
    static void Main()
    {
        string inputSeq = "sequences.txt";
        string inputCmd = "commands.txt";
        string outputFile = "genedata.txt";

        // Count all proteins
        List<GeneticData> proteins = new List<GeneticData>();
        foreach (var line in File.ReadAllLines(inputSeq))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split('\t');
            proteins.Add(new GeneticData
            {
                protein = parts[0],
                organism = parts[1],
                amino_acids = RLDecoding(parts[2])
            });
        }

        // Output file
        using (StreamWriter writer = new StreamWriter(outputFile))
        {
            writer.WriteLine("Матвей");
            writer.WriteLine("Генетический поиск");
            
            int opNum = 1;
            foreach (var line in File.ReadAllLines(inputCmd))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split('\t');
                string cmd = parts[0].ToLower();

                writer.WriteLine($"---");
                writer.WriteLine($"{opNum:D3}: {line}");

                if (cmd == "search")
                    Search(parts[1], proteins, writer);
                else if (cmd == "diff")
                    Diff(parts[1], parts[2], proteins, writer);
                else if (cmd == "mode")
                    Mode(parts[1], proteins, writer);

                opNum++;
            }
        }

        Console.WriteLine("Результат записан в genedata.txt");
    }

    // Search func 
    static void Search(string seq, List<GeneticData> data, StreamWriter writer)
    {
        seq = RLDecoding(seq);
        var found = data.Where(p => p.amino_acids.Contains(seq)).ToList(); //filter by condition => check for each protein => to list
        if (found.Count == 0)
            writer.WriteLine("NOT FOUND");
        else
            foreach (var p in found)
                writer.WriteLine($"{p.organism} — {p.protein}");
    }

    // Diff func 
    static void Diff(string p1, string p2, List<GeneticData> data, StreamWriter writer)
    {
        var prot1 = data.FirstOrDefault(p => p.protein == p1);
        var prot2 = data.FirstOrDefault(p => p.protein == p2);

        if (string.IsNullOrEmpty(prot1.protein) || string.IsNullOrEmpty(prot2.protein))
        {
            writer.Write("MISSING: ");
            if (string.IsNullOrEmpty(prot1.protein)) writer.Write($"{p1} ");
            if (string.IsNullOrEmpty(prot2.protein)) writer.Write($"{p2}");
            writer.WriteLine();
            return;
        }

        int minLen = Math.Min(prot1.amino_acids.Length, prot2.amino_acids.Length);
        int diffCount = 0;
        for (int i = 0; i < minLen; i++)
            if (prot1.amino_acids[i] != prot2.amino_acids[i])
                diffCount++;

        diffCount += Math.Abs(prot1.amino_acids.Length - prot2.amino_acids.Length);

        writer.WriteLine($"amino-acids difference: {diffCount}");
    }

    // Mode func 
    static void Mode(string proteinName, List<GeneticData> data, StreamWriter writer)
    {
        var prot = data.FirstOrDefault(p => p.protein == proteinName);

        if (string.IsNullOrEmpty(prot.protein))
        {
            writer.WriteLine($"MISSING: {proteinName}");
            return;
        }

        var freq = prot.amino_acids.GroupBy(c => c) // group similiar amin-ts ["R", "R"]
                                   .Select(g => new { Amino = g.Key, Count = g.Count() })//create obj with counter
                                   .OrderByDescending(x => x.Count) //sort from frequent to rare
                                   .ThenBy(x => x.Amino)//sort by alfabet(if counter same)
                                   .First();// taking first most frequent amin-ts

        writer.WriteLine($"amino-acid occurs: {freq.Amino} ({freq.Count} times)");
    }

    static string RLDecoding(string input)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsDigit(input[i]))
            {
                int count = input[i] - '0'; // ASCII code
                char next = input[++i];
                sb.Append(new string(next, count));
            }
            else sb.Append(input[i]);
        }
        return sb.ToString();
    }
}
