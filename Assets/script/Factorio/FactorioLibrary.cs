using System.Collections.Generic;

public static class FactorioLibrary {
    public static readonly string[] BasicResources = {
        "IronOre",
        "CopperOre"
    };

    public static readonly Dictionary<string, int> AssemblingProducts = new() {
        { "Gear", 1 },
        { "RedSciencePack", 1 }
    };

    private static readonly HashSet<string> BasicResourceSet = new(BasicResources);

    public static bool IsBasicResource(string id) {
        return BasicResourceSet.Contains(id);
    }
}
