using System.Text.RegularExpressions;

namespace HomeApplianceRepairCenter.Helpers;

/// <summary>
/// فئة ثابتة للتحقق من صحة البيانات المدخلة (رقم تسلسلي، تواريخ، نصوص، قيم رقمية).
/// </summary>
public static class ValidationHelper
{
    // الصيغة المعتمدة للرقم التسلسلي: ثلاثة أحرف كبيرة - ستة أرقام (مثال: REF-100001)
    private static readonly Regex SerialNumberPattern = new(@"^[A-Z]{3}-\d{6}$", RegexOptions.Compiled);

    private static readonly DateTime EarliestAllowedDate = new(2000, 1, 1);

    public static bool IsValidSerialNumber(string? serialNumber) =>
        !string.IsNullOrWhiteSpace(serialNumber) && SerialNumberPattern.IsMatch(serialNumber);

    /// <summary>يتحقق من الصيغة ومن أن الرقم يبدأ ببادئة نوع الجهاز (REF / WSH / ACN).</summary>
    public static bool IsValidSerialNumber(string? serialNumber, string expectedPrefix) =>
        IsValidSerialNumber(serialNumber) &&
        serialNumber!.StartsWith(expectedPrefix + "-", StringComparison.Ordinal);

    /// <summary>تاريخ الاستلام يجب ألا يكون في المستقبل ولا قبل عام 2000.</summary>
    public static bool IsValidReceivedDate(DateTime date) =>
        date >= EarliestAllowedDate && date.Date <= DateTime.Today;

    /// <summary>موعد التسليم المتوقع يجب ألا يسبق تاريخ الاستلام.</summary>
    public static bool IsValidDeliveryDate(DateTime receivedDate, DateTime expectedDeliveryDate) =>
        expectedDeliveryDate.Date >= receivedDate.Date;

    public static bool IsValidText(string? text, int minLength = 2) =>
        !string.IsNullOrWhiteSpace(text) && text.Trim().Length >= minLength;

    public static bool IsPositive(decimal value) => value > 0;

    public static bool IsNonNegative(decimal value) => value >= 0;
}
