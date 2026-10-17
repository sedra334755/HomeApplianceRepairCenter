using HomeApplianceRepairCenter.Enums;
using HomeApplianceRepairCenter.Models;
using HomeApplianceRepairCenter.Services;

// ---------- إنشاء المركز والاشتراك في الأحداث ----------
var center = new RepairCenter();

center.ApplianceStatusChanged += (appliance, oldStatus, newStatus) =>
{
    Console.WriteLine($"  >> [Notification] {appliance.SerialNumber}: {oldStatus} -> {newStatus}");

    if (newStatus == RepairStatus.Repaired)
        Console.WriteLine($"  >> [Customer SMS] Dear {appliance.CustomerName}, your {appliance.Brand} is ready. Total: {appliance.FinalCost:N2}");
};

center.DeliveryDelayed += (appliance, daysLate) =>
    Console.WriteLine($"  !! [Delay Alert] {appliance.SerialNumber} ({appliance.CustomerName}) is {daysLate} day(s) late.");

// ---------- تسجيل أجهزة من أنواع مختلفة ----------
DateTime today = DateTime.Today;

var fridge = new Refrigerator("REF-100001", "Samsung", "Ahmad Khaled", "Not cooling",
    today.AddDays(-5), today.AddDays(2), capacityLiters: 600, hasCompressorFault: true, needsGasRefill: true);

var washer = new WashingMachine("WSH-200001", "LG", "Sara Mahmoud", "Drum does not spin",
    today.AddDays(-8), today.AddDays(-3), loadCapacityKg: 9, isFrontLoad: true, needsMotorReplacement: true);

var airConditioner = new AirConditioner("ACN-300001", "Gree", "Omar Nasser", "Low cooling, gas leak",
    today.AddDays(-2), today.AddDays(4), coolingCapacityBtu: 24000, isInverter: true, requiresOnSiteVisit: true);

var secondWasher = new WashingMachine("WSH-200002", "Bosch", "Lina Hassan", "Water leak",
    today.AddDays(-1), today.AddDays(3), loadCapacityKg: 7, isFrontLoad: false, needsMotorReplacement: false);

fridge.LaborHours = 3; fridge.PartsCost = 80;
washer.LaborHours = 2.5m; washer.PartsCost = 45;
airConditioner.LaborHours = 2; airConditioner.PartsCost = 60;
secondWasher.LaborHours = 1; secondWasher.PartsCost = 15;

foreach (Appliance appliance in new Appliance[] { fridge, washer, airConditioner, secondWasher })
    center.AddAppliance(appliance);

// ---------- التحقق من البيانات (فئة ثابتة) ----------
Console.WriteLine("=== Validation demo ===");
try
{
    _ = new Refrigerator("XYZ-12", "Bosch", "Test", "Test fault", today, today.AddDays(1), 300, false, false);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  Rejected: {ex.Message}");
}

// ---------- معالجة الأجهزة بتعدد الأشكال ----------
Console.WriteLine("\n=== All appliances (polymorphic summary) ===");
foreach (string line in center.GetStatusReport()) Console.WriteLine(line);

Console.WriteLine($"\nTotal estimated cost of all appliances: {center.CalculateTotalCost():N2}");

// ---------- دورة الصيانة وإطلاق الأحداث ----------
Console.WriteLine("\n=== Repair workflow ===");
fridge.StartRepair();
washer.StartRepair();
airConditioner.StartRepair();
fridge.CompleteRepair();
airConditioner.CompleteRepair();
airConditioner.Deliver();

// ---------- القيم المقفلة بعد اكتمال الإصلاح ----------
Console.WriteLine("\n=== Locked data after repair ===");
try
{
    fridge.PartsCost = 5;
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  Rejected: {ex.Message}");
}

// ---------- فحص التأخير ----------
Console.WriteLine("\n=== Delayed deliveries check ===");
int delayed = center.CheckDelayedDeliveries(today);
Console.WriteLine($"  Delayed appliances: {delayed}");

// ---------- الإحصائيات الثابتة ----------
Console.WriteLine("\n=== Center statistics (static members) ===");
Console.WriteLine($"  Total appliances : {Appliance.TotalAppliancesCount}");
Console.WriteLine($"  Received         : {Appliance.ReceivedCount}");
Console.WriteLine($"  Under repair     : {Appliance.UnderRepairCount}");
Console.WriteLine($"  Repaired         : {Appliance.RepairedCount}");
Console.WriteLine($"  Delivered        : {Appliance.DeliveredCount}");
Console.WriteLine($"  Total revenue    : {Appliance.TotalRepairRevenue:N2}");

Console.WriteLine("\n=== Final status report ===");
foreach (string line in center.GetStatusReport()) Console.WriteLine(line);
