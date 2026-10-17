using HomeApplianceRepairCenter.Helpers;

namespace HomeApplianceRepairCenter.Models;

/// <summary>
/// ثلاجة: التكلفة ترتفع عند تلف الضاغط أو الحاجة لتعبئة غاز،
/// وتزيد نسبة إضافية للثلاجات كبيرة السعة.
/// </summary>
public sealed class Refrigerator : Appliance
{
    private const string SerialPrefix = "REF";
    private const decimal CompressorReplacementFee = 120m;
    private const decimal GasRefillFee = 40m;
    private const int LargeCapacityLiters = 500;
    private const decimal LargeCapacityMultiplier = 1.10m;

    private readonly int _capacityLiters;
    private readonly bool _hasCompressorFault;
    private readonly bool _needsGasRefill;

    public Refrigerator(
        string serialNumber, string brand, string customerName, string faultDescription,
        DateTime receivedDate, DateTime expectedDeliveryDate,
        int capacityLiters, bool hasCompressorFault, bool needsGasRefill)
        : base(serialNumber, SerialPrefix, brand, customerName, faultDescription, receivedDate, expectedDeliveryDate)
    {
        if (!ValidationHelper.IsPositive(capacityLiters))
            throw new ArgumentOutOfRangeException(nameof(capacityLiters), "Capacity must be positive.");

        _capacityLiters = capacityLiters;
        _hasCompressorFault = hasCompressorFault;
        _needsGasRefill = needsGasRefill;
    }

    public int CapacityLiters => _capacityLiters;
    public bool HasCompressorFault => _hasCompressorFault;
    public bool NeedsGasRefill => _needsGasRefill;

    public override decimal CalculateRepairCost()
    {
        decimal cost = CalculateBaseCost();

        if (_hasCompressorFault) cost += CompressorReplacementFee;
        if (_needsGasRefill) cost += GasRefillFee;
        if (_capacityLiters >= LargeCapacityLiters) cost *= LargeCapacityMultiplier;

        return decimal.Round(cost, 2);
    }

    public override string GetStatusSummary() =>
        $"{base.GetStatusSummary()} | {_capacityLiters} L, compressor fault: {_hasCompressorFault}, gas refill: {_needsGasRefill}";
}
