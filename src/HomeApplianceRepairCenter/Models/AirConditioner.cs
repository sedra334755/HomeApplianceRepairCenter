using HomeApplianceRepairCenter.Helpers;

namespace HomeApplianceRepairCenter.Models;

/// <summary>
/// مكيّف: تعبئة الغاز تتناسب مع القدرة التبريدية (BTU)، مع رسم إضافي
/// لوحدات Inverter ولزيارة الموقع (فك الوحدة الخارجية).
/// </summary>
public sealed class AirConditioner : Appliance
{
    private const string SerialPrefix = "ACN";
    private const int BtuPerGasUnit = 12000;
    private const decimal GasFeePerUnit = 35m;
    private const decimal InverterBoardSurcharge = 80m;
    private const decimal OnSiteVisitFee = 30m;

    private readonly int _coolingCapacityBtu;
    private readonly bool _isInverter;
    private readonly bool _requiresOnSiteVisit;

    public AirConditioner(
        string serialNumber, string brand, string customerName, string faultDescription,
        DateTime receivedDate, DateTime expectedDeliveryDate,
        int coolingCapacityBtu, bool isInverter, bool requiresOnSiteVisit)
        : base(serialNumber, SerialPrefix, brand, customerName, faultDescription, receivedDate, expectedDeliveryDate)
    {
        if (!ValidationHelper.IsPositive(coolingCapacityBtu))
            throw new ArgumentOutOfRangeException(nameof(coolingCapacityBtu), "Cooling capacity must be positive.");

        _coolingCapacityBtu = coolingCapacityBtu;
        _isInverter = isInverter;
        _requiresOnSiteVisit = requiresOnSiteVisit;
    }

    public int CoolingCapacityBtu => _coolingCapacityBtu;
    public bool IsInverter => _isInverter;
    public bool RequiresOnSiteVisit => _requiresOnSiteVisit;

    public override decimal CalculateRepairCost()
    {
        decimal cost = CalculateBaseCost();

        cost += (decimal)_coolingCapacityBtu / BtuPerGasUnit * GasFeePerUnit;
        if (_isInverter) cost += InverterBoardSurcharge;
        if (_requiresOnSiteVisit) cost += OnSiteVisitFee;

        return decimal.Round(cost, 2);
    }

    public override string GetStatusSummary() =>
        $"{base.GetStatusSummary()} | {_coolingCapacityBtu} BTU, inverter: {_isInverter}, on-site visit: {_requiresOnSiteVisit}";
}
