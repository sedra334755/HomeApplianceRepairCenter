using HomeApplianceRepairCenter.Enums;

namespace HomeApplianceRepairCenter.Interfaces;

/// <summary>
/// العمليات الأساسية المشتركة بين جميع الأجهزة المنزلية في المركز.
/// </summary>
public interface IRepairable
{
    string SerialNumber { get; }
    RepairStatus Status { get; }

    /// <summary>حساب تكلفة الإصلاح حسب نوع الجهاز.</summary>
    decimal CalculateRepairCost();

    /// <summary>بدء الإصلاح (Received -> InRepair).</summary>
    void StartRepair();

    /// <summary>إنهاء الإصلاح وتثبيت التكلفة النهائية (InRepair -> Repaired).</summary>
    void CompleteRepair();

    /// <summary>تسليم الجهاز للزبون (Repaired -> Delivered).</summary>
    void Deliver();

    /// <summary>نص مختصر يصف حالة الجهاز.</summary>
    string GetStatusSummary();
}
