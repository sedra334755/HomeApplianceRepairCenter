using HomeApplianceRepairCenter.Enums;
using HomeApplianceRepairCenter.Events;
using HomeApplianceRepairCenter.Helpers;
using HomeApplianceRepairCenter.Interfaces;

namespace HomeApplianceRepairCenter.Models;

/// <summary>
/// الفئة المجردة لكل جهاز منزلي يدخل المركز. تطبّق IRepairable وتترك
/// طريقة حساب التكلفة للفئات الفرعية لأنها تختلف من جهاز لآخر.
/// </summary>
public abstract class Appliance : IRepairable
{
    // أجرة ساعة العمل الموحدة لجميع الأجهزة
    private const decimal HourlyLaborRate = 50m;

    // ---- إحصائيات ثابتة مشتركة بين كل الأجهزة ----
    private static readonly int[] _statusCounters = new int[Enum.GetValues<RepairStatus>().Length];
    private static decimal _totalRepairRevenue;

    // ---- حقول الكائن (جميعها خاصة) ----
    private readonly string _serialNumber;
    private readonly string _brand;
    private readonly string _customerName;
    private readonly string _faultDescription;
    private readonly DateTime _receivedDate;
    private readonly DateTime _expectedDeliveryDate;

    private RepairStatus _status;
    private decimal _laborHours;
    private decimal _partsCost;
    private decimal _finalCost;
    private DateTime? _completedDate;

    /// <summary>يُطلق عند أي تغيّر في حالة الجهاز.</summary>
    public event RepairStatusChangedHandler? StatusChanged;

    protected Appliance(
        string serialNumber,
        string serialPrefix,
        string brand,
        string customerName,
        string faultDescription,
        DateTime receivedDate,
        DateTime expectedDeliveryDate)
    {
        if (!ValidationHelper.IsValidSerialNumber(serialNumber, serialPrefix))
            throw new ArgumentException($"Invalid serial number '{serialNumber}'. Expected format: {serialPrefix}-######.", nameof(serialNumber));
        if (!ValidationHelper.IsValidText(brand))
            throw new ArgumentException("Brand is required.", nameof(brand));
        if (!ValidationHelper.IsValidText(customerName))
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        if (!ValidationHelper.IsValidText(faultDescription))
            throw new ArgumentException("Fault description is required.", nameof(faultDescription));
        if (!ValidationHelper.IsValidReceivedDate(receivedDate))
            throw new ArgumentOutOfRangeException(nameof(receivedDate), "Received date cannot be in the future.");
        if (!ValidationHelper.IsValidDeliveryDate(receivedDate, expectedDeliveryDate))
            throw new ArgumentOutOfRangeException(nameof(expectedDeliveryDate), "Expected delivery cannot be before the received date.");

        _serialNumber = serialNumber;
        _brand = brand.Trim();
        _customerName = customerName.Trim();
        _faultDescription = faultDescription.Trim();
        _receivedDate = receivedDate.Date;
        _expectedDeliveryDate = expectedDeliveryDate.Date;
        _status = RepairStatus.Received;

        _statusCounters[(int)_status]++;
    }

    // ================= خصائص للقراءة فقط بعد الإنشاء =================
    public string SerialNumber => _serialNumber;
    public string Brand => _brand;
    public string CustomerName => _customerName;
    public string FaultDescription => _faultDescription;
    public DateTime ReceivedDate => _receivedDate;
    public DateTime ExpectedDeliveryDate => _expectedDeliveryDate;

    // ================= خصائص تتغير ضمن قواعد =================
    public RepairStatus Status => _status;
    public decimal FinalCost => _finalCost;
    public DateTime? CompletedDate => _completedDate;

    /// <summary>عدد ساعات العمل. تُقفل بعد اكتمال الإصلاح.</summary>
    public decimal LaborHours
    {
        get => _laborHours;
        set
        {
            if (!ValidationHelper.IsNonNegative(value))
                throw new ArgumentOutOfRangeException(nameof(value), "Labor hours cannot be negative.");
            EnsureCostDataIsEditable();
            _laborHours = value;
        }
    }

    /// <summary>كلفة القطع المستبدلة. تُقفل بعد اكتمال الإصلاح.</summary>
    public decimal PartsCost
    {
        get => _partsCost;
        set
        {
            if (!ValidationHelper.IsNonNegative(value))
                throw new ArgumentOutOfRangeException(nameof(value), "Parts cost cannot be negative.");
            EnsureCostDataIsEditable();
            _partsCost = value;
        }
    }

    public bool IsCostFinal => _status is RepairStatus.Repaired or RepairStatus.Delivered;

    // ================= الأعضاء الثابتة (إحصائيات المركز) =================
    public static int TotalAppliancesCount => _statusCounters.Sum();
    public static int ReceivedCount => _statusCounters[(int)RepairStatus.Received];
    public static int UnderRepairCount => _statusCounters[(int)RepairStatus.InRepair];
    public static int RepairedCount => _statusCounters[(int)RepairStatus.Repaired];
    public static int DeliveredCount => _statusCounters[(int)RepairStatus.Delivered];

    /// <summary>مجموع التكاليف النهائية لكل الإصلاحات المكتملة.</summary>
    public static decimal TotalRepairRevenue => _totalRepairRevenue;

    // ================= الدالة المجردة =================
    /// <summary>تكلفة الإصلاح: تعيد كل فئة فرعية تعريفها حسب طبيعة الجهاز.</summary>
    public abstract decimal CalculateRepairCost();

    /// <summary>الجزء المشترك من التكلفة: أجرة العمل + القطع.</summary>
    protected decimal CalculateBaseCost() => (_laborHours * HourlyLaborRate) + _partsCost;

    // ================= دورة حياة الصيانة =================
    public void StartRepair()
    {
        RequireStatus(RepairStatus.Received, nameof(StartRepair));
        ChangeStatus(RepairStatus.InRepair);
    }

    public void CompleteRepair()
    {
        RequireStatus(RepairStatus.InRepair, nameof(CompleteRepair));
        _finalCost = CalculateRepairCost();
        _completedDate = DateTime.Today;
        _totalRepairRevenue += _finalCost;
        ChangeStatus(RepairStatus.Repaired);
    }

    public void Deliver()
    {
        RequireStatus(RepairStatus.Repaired, nameof(Deliver));
        ChangeStatus(RepairStatus.Delivered);
    }

    /// <summary>هل تأخر الجهاز عن الموعد المتوقع ولم يكتمل إصلاحه بعد؟</summary>
    public bool IsOverdue(DateTime today) =>
        _status is RepairStatus.Received or RepairStatus.InRepair &&
        today.Date > _expectedDeliveryDate;

    public int GetDaysLate(DateTime today) =>
        Math.Max(0, (today.Date - _expectedDeliveryDate).Days);

    // ================= العرض =================
    public virtual string GetStatusSummary()
    {
        string costLabel = IsCostFinal ? "Final cost" : "Estimated cost";
        decimal cost = IsCostFinal ? _finalCost : CalculateRepairCost();
        return $"[{GetType().Name}] {_serialNumber} | {_brand} | Customer: {_customerName} | " +
               $"Status: {_status} | {costLabel}: {cost:N2}";
    }

    public override string ToString() => GetStatusSummary();

    // ================= دوال داخلية =================
    private void ChangeStatus(RepairStatus newStatus)
    {
        RepairStatus oldStatus = _status;
        _status = newStatus;
        _statusCounters[(int)oldStatus]--;
        _statusCounters[(int)newStatus]++;
        OnStatusChanged(oldStatus, newStatus);
    }

    protected virtual void OnStatusChanged(RepairStatus oldStatus, RepairStatus newStatus) =>
        StatusChanged?.Invoke(this, oldStatus, newStatus);

    private void RequireStatus(RepairStatus expected, string operation)
    {
        if (_status != expected)
            throw new InvalidOperationException($"Cannot {operation} for {_serialNumber}: status is {_status}, expected {expected}.");
    }

    private void EnsureCostDataIsEditable()
    {
        if (IsCostFinal)
            throw new InvalidOperationException($"Cost data for {_serialNumber} is locked after the repair is completed.");
    }
}
