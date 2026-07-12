using System.Collections.Generic;

public static class FactorioLibrary {
    public static readonly FactorioId[] BasicResources = {
        FactorioId.IronOre,
        FactorioId.CopperOre
    };

    public static readonly Dictionary<FactorioId, int> AssemblingProducts = new() {
        { FactorioId.Gear, 1 },
        { FactorioId.RedSciencePack, 1 }
    };

    private static readonly HashSet<FactorioId> BasicResourceSet = new(BasicResources);

    public static bool IsBasicResource(FactorioId id) {
        return BasicResourceSet.Contains(id);
    }
}
