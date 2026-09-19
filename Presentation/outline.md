# gRPC — اسکلت ارائه ۴۵ دقیقه‌ای (داتین / .NET)

## زمان‌بندی

| دقیقه | بخش |
|------:|------|
| ۰–۲ | معرفی |
| ۲–۷ | محدودیت REST در سرویس‌به‌سرویس |
| ۷–۱۴ | gRPC چیست؟ (Contract / HTTP/2 / Protobuf) |
| ۱۴–۲۰ | فایل `.proto` و field number |
| ۲۰–۲۳ | چهار نوع RPC + استریم |
| ۲۳–۳۵ | دموی زنده (Blazor) |
| ۳۵–۴۱ | Metadata، Deadline، Interceptor |
| ۴۱–۴۵ | کی استفاده کنیم + Q&A |

## ترتیب دمو

1. اول `GrpcDemo.Server` را Run کنید (`http://localhost:5051`)
2. بعد `GrpcDemo.Client` را Run کنید (`http://localhost:5200`)
3. تب‌ها به ترتیب:
   - **Unary / Headers** — correlation-id، Deadline، Trailers
   - **Server Stream** — تراکنش‌ها با Delay حدود ۱.۵ث
   - **Client Stream** — چند واریز → یک خلاصه
   - **BiDi — چت** — کانال باز، رفت‌وبرگشت هم‌زمان

## جملات کلیدی

- gRPC = RPC روی HTTP/2 با Protobuf
- `.proto` قرارداد است؛ کد C# از روی آن تولید می‌شود
- عدد کنار فیلد ایندکس نیست؛ شناسهٔ پایدار باینری است
- Metadata شبیه Header است؛ جزو body پروتوباف نیست
- Deadline سقف زمان تماس است
- Interceptor شبیه Middleware برای RPC است
- بیرون REST، داخل سازمان اغلب gRPC

## سوالات پرتکرار

1. REST را دور بریزیم؟ — نه؛ مکمل هم هستند.
2. از مرورگر مستقیم؟ — معمولاً gRPC-Web یا Gateway.
3. Field number عوض شود؟ — خطر شکستن سازگاری.
4. کجا در بانک؟ — ارتباط داخلی سرویس‌ها.
