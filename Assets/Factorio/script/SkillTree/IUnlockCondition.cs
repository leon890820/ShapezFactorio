public interface IUnlockCondition {
    bool IsUnlocked();
    FactorioGameObjectBasePacket[] GetUnlockDescription();
    ProductionUnlockConditionData[] GetUnlockConditionData();
}
