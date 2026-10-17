using HomeApplianceRepairCenter.Helpers;

namespace HomeApplianceRepairCenter.Models;

/// <summary>
/// غسالة: التكلفة تتأثر باستبدال المحرك، ونوع التحميل الأمامي (أصعب فكاً وتركيباً)،
/// ورسم مناولة للأجهزة ذات الحمولة الكبيرة.
/// </summary>
public sealed class WashingMachine : Appliance
{
    private const string SerialPrefix = "WSH";
    private const decimal MotorReplacementFee = 90m;
    private const decimal FrontLoadMultiplier = 1.20m;
    private const int HeavyLoadKg = 10;
    private const decimal HeavyUnitHandlingFee = 25m;

    private readonly int _loadCapacityKg;
    private readonly bool _isFrontLoad;
    private readonly bool _needsMotorReplacement;

    public WashingMachine(
        string serialNumber, string brand, string customerName, string faultDescription,
        DateTime receivedDate, DateTime expectedDeliveryDate,
        int loadCapacityKg, bool isFrontLoad, bool needsMotorReplacement)
        : base(serialNumber, SerialPrefix, brand, customerName, faultDescription, receivedDate, expectedDeliveryDate)
    {
        if (!ValidationHelper.IsPositive(loadCapacityKg))
            throw new ArgumentOutOfRangeException(nameof(loadCapacityKg), "Load capacity must be positive.");

        _loadCapacityKg = loadCapacityKg;
        _isFrontLoad = isFrontLoad;
        _needsMotorReplacement = needsMotorReplacement;
    }

    public int LoadCapacityKg => _loadCapacityKg;
    public bool IsFrontLoad => _isFrontLoad;
    public bool NeedsMotorReplacement => _needsMotorReplacement;

    public override decimal CalculateRepairCost()
    {
        decimal cost = CalculateBaseCost();

        if (_needsMotorReplacement) cost += MotorReplacementFee;
        if (_isFrontLoad) cost *= FrontLoadMultiplier;
        if (_loadCapacityKg >= HeavyLoadKg) cost += HeavyUnitHandlingFee;

        return decimal.Round(cost, 2);
    }

    public override string GetStatusSummary() =>
        $"{base.GetStatusSummary()} | {_loadCapacityKg} kg, front-load: {_isFrontLoad}, motor replacement: {_needsMotorReplacement}";
}
