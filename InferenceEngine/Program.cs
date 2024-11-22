/*// See https://aka.ms/new-console-template for more information
Console.WriteLine("P\tQ\tR\tP AND Q\tP AND Q => R");

// Iterate through all combinations of truth values for P, Q, and R
for (int p = 0; p <= 1; p++) // P: 0 (False), 1 (True)
{
    for (int q = 0; q <= 1; q++) // Q: 0 (False), 1 (True)
    {
        for (int r = 0; r <= 1; r++) // R: 0 (False), 1 (True)
        {
            bool P = p == 1;
            bool Q = q == 1;
            bool R = r == 1;

            // Calculate P AND Q
            bool P_AND_Q = P && Q;

            // Calculate P AND Q => R (implication)
            bool P_AND_Q_implies_R = !P_AND_Q || R;

            // Display the results
            Console.WriteLine($"{(P ? "T" : "F")}\t{(Q ? "T" : "F")}\t{(R ? "T" : "F")}\t{(P_AND_Q ? "T" : "F")}\t\t{(P_AND_Q_implies_R ? "T" : "F")}");
        }
    }
}*/

using InferenceEngine;

if (args.Length < 2)
{
    Console.WriteLine("Usage: iengine <filename> <method>");
    return;
}

string fileName = args[0];
string algorithmMethod = args[1].ToUpper();

if (!File.Exists(fileName))
{
    Console.WriteLine($"Error: File '{fileName}' not found.");
    return;
}

try
{
    KnowledgeBase kn = new KnowledgeBase(fileName);
    IEngine algorithm;

    switch (algorithmMethod)
    {
        case "TT":
            algorithm = new TruthTable(kn);
            break;
        case "FC":
            algorithm = new ForwardChaining(kn);
            break;
        case "BC":
            algorithm = new BackwardChaining(kn);
            break;
        default:
            throw new ArgumentException("Method not found.");
    }

    algorithm.Solve();
    algorithm.PrintResult();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
