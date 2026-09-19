# gRPC Demo (.NET)

نمونهٔ آموزشی برای ارائهٔ gRPC در تیم .NET:

- `src/GrpcDemo.Server` — ASP.NET Core gRPC
- `src/GrpcDemo.Client` — Blazor Server (دموی تصویری UI)
- `src/GrpcDemo.ConsoleClient` — کلاینت کنسول (کد خوانا برای توضیح متدها)
- `protos/account.proto` — قرارداد سرویس
- `presentation/outline.md` — اسکلت ارائهٔ ۴۵ دقیقه‌ای

## اجرا

1. .NET 8 SDK
2. Run پروژهٔ **Server** → `http://localhost:5051`
3. برای UI: Run پروژهٔ **Client** (Blazor) → `http://localhost:5200`
4. برای توضیح کد: Run پروژهٔ **ConsoleClient** و از منو یکی از ۴ الگو را انتخاب کنید

## نقش هر کلاینت

| پروژه | کاربرد در ارائه |
|--------|------------------|
| `GrpcDemo.Client` | نمایش بصری جریان‌ها |
| `GrpcDemo.ConsoleClient` | باز کردن فایل‌های `Demos/*.cs` و توضیح خط‌به‌خط کد |

## الگوها

| تب | الگو |
|----|------|
| Unary / Headers | Unary + Metadata + Deadline |
| Server Stream | Server streaming |
| Client Stream | Client streaming |
| BiDi — چت | Bidirectional streaming |
