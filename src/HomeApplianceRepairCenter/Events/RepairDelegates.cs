using HomeApplianceRepairCenter.Enums;
using HomeApplianceRepairCenter.Models;

namespace HomeApplianceRepairCenter.Events;

/// <summary>تفويض للإشعار بتغيّر حالة الجهاز.</summary>
public delegate void RepairStatusChangedHandler(Appliance appliance, RepairStatus oldStatus, RepairStatus newStatus);

/// <summary>تفويض للإشعار بتأخر تسليم جهاز عن الموعد المتوقع.</summary>
public delegate void DeliveryDelayedHandler(Appliance appliance, int daysLate);
