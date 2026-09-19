# gRPC Demo (.NET)

نمونهٔ آموزشی برای ارائهٔ gRPC در تیم .NET:

- `src/GrpcDemo.Server` — ASP.NET Core gRPC
- `src/GrpcDemo.Client` — Blazor Server (دموی تصویری ۴ الگو)
- `protos/account.proto` — قرارداد سرویس
- `presentation/outline.md` — اسکلت ارائهٔ ۴۵ دقیقه‌ای

## اجرا

1. .NET 8 SDK
2. Run پروژهٔ Server → `http://localhost:5051`
3. Run پروژهٔ Client → `http://localhost:5200`

## الگوها

| تب | الگو |
|----|------|
| Unary / Headers | Unary + Metadata + Deadline |
| Server Stream | Server streaming |
| Client Stream | Client streaming |
| BiDi — چت | Bidirectional streaming |
