# نظام إدارة مركز صيانة وإصلاح الأجهزة المنزلية

وظيفة تطبيقية – مقرر البرمجة والتصميم غرضية التوجه (IPG203) – الجامعة الافتراضية السورية

نظام C# (.NET 8) يدير الأجهزة المنزلية (ثلاجة، غسالة، مكيف) من لحظة الاستلام حتى التسليم، مع حساب تكلفة الإصلاح لكل نوع ومتابعة الأجهزة المصلحة وقيد الصيانة.

## التشغيل

```bash
dotnet run --project src/HomeApplianceRepairCenter
```

## هيكل المشروع

```
src/HomeApplianceRepairCenter/
├── Program.cs                 # سيناريو تشغيل تجريبي
├── Enums/RepairStatus.cs      # حالات الصيانة
├── Interfaces/IRepairable.cs  # العمليات المشتركة (التجريد)
├── Models/
│   ├── Appliance.cs           # الفئة المجردة
│   ├── Refrigerator.cs        # فئة فرعية 1
│   ├── WashingMachine.cs      # فئة فرعية 2
│   └── AirConditioner.cs      # فئة فرعية 3
├── Events/RepairDelegates.cs  # التفويضات
├── Helpers/ValidationHelper.cs# فئة ثابتة للتحقق
└── Services/RepairCenter.cs   # الفئة الرئيسية (قائمة + تعدد أشكال + أحداث)
```

## أين نجد كل مبدأ؟

| المبدأ | المكان |
|---|---|
| التجريد | `IRepairable`، `Appliance` (الدالة المجردة `CalculateRepairCost`) |
| الوراثة | `Refrigerator`، `WashingMachine`، `AirConditioner` |
| تعدد الأشكال | `override CalculateRepairCost/GetStatusSummary` + `List<Appliance>` في `RepairCenter` |
| التغليف | حقول خاصة وخصائص في `Appliance` (`SerialNumber` للقراءة فقط، قفل التكلفة بعد الإصلاح) |
| التفويض والأحداث | `RepairStatusChangedHandler`، `DeliveryDelayedHandler`، `StatusChanged`، `DeliveryDelayed` |
| الثابت | `ValidationHelper` (فئة ثابتة) + خصائص ثابتة في `Appliance` (`RepairedCount`, `TotalRepairRevenue`...) |
"# HomeApplianceRepairCenter" 
"# HomeApplianceRepairCenter"  "# HomeApplianceRepairCenter" 
