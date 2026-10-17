using HomeApplianceRepairCenter.Enums;
using HomeApplianceRepairCenter.Events;
using HomeApplianceRepairCenter.Models;

namespace HomeApplianceRepairCenter.Services;

/// <summary>
/// الفئة الرئيسية: تحتفظ بقائمة من Appliance وتعالجها بتعدد الأشكال دون معرفة نوعها الفعلي.
/// </summary>
public class RepairCenter
{
    private readonly List<Appliance> _appliances = new();

    /// <summary>يُعيد نشر تغيّر حالة أي جهاز مسجّل في المركز.</summary>
    public event RepairStatusChangedHandler? ApplianceStatusChanged;

    /// <summary>يُطلق عندما يتجاوز جهاز موعد التسليم المتوقع ولم يكتمل إصلاحه.</summary>
    public event DeliveryDelayedHandler? DeliveryDelayed;

    public IReadOnlyList<Appliance> Appliances => _appliances.AsReadOnly();

    public void AddAppliance(Appliance appliance)
    {
        ArgumentNullException.ThrowIfNull(appliance);

        if (FindBySerial(appliance.SerialNumber) is not null)
            throw new InvalidOperationException($"An appliance with serial {appliance.SerialNumber} is already registered.");

        _appliances.Add(appliance);
        appliance.StatusChanged += OnApplianceStatusChanged;
    }

    public Appliance? FindBySerial(string serialNumber) =>
        _appliances.FirstOrDefault(a => a.SerialNumber == serialNumber);

    public IEnumerable<Appliance> GetByStatus(RepairStatus status) =>
        _appliances.Where(a => a.Status == status);

    /// <summary>مجموع التكاليف (تقديرية أو نهائية) لكل الأجهزة، كل جهاز بحسب نوعه.</summary>
    public decimal CalculateTotalCost() =>
        _appliances.Sum(a => a.IsCostFinal ? a.FinalCost : a.CalculateRepairCost());

    /// <summary>تقرير حالة جميع الأجهزة (استدعاء متعدد الأشكال لـ GetStatusSummary).</summary>
    public IEnumerable<string> GetStatusReport() =>
        _appliances.Select(a => a.GetStatusSummary());

    /// <summary>يفحص الأجهزة المتأخرة ويطلق DeliveryDelayed لكل واحد، ويعيد عددها.</summary>
    public int CheckDelayedDeliveries(DateTime today)
    {
        int delayedCount = 0;

        foreach (Appliance appliance in _appliances.Where(a => a.IsOverdue(today)))
        {
            DeliveryDelayed?.Invoke(appliance, appliance.GetDaysLate(today));
            delayedCount++;
        }

        return delayedCount;
    }

    private void OnApplianceStatusChanged(Appliance appliance, RepairStatus oldStatus, RepairStatus newStatus) =>
        ApplianceStatusChanged?.Invoke(appliance, oldStatus, newStatus);
}
