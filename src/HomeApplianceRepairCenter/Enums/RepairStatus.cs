namespace HomeApplianceRepairCenter.Enums;

/// <summary>
/// مراحل دورة حياة الجهاز داخل مركز الصيانة.
/// </summary>
public enum RepairStatus
{
    Received,   // تم استلام الجهاز من الزبون
    InRepair,   // قيد الإصلاح
    Repaired,   // اكتمل الإصلاح وبانتظار التسليم
    Delivered   // تم تسليم الجهاز للزبون
}
