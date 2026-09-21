---
marp: true
paginate: true
dir: rtl
lang: fa
style: |
  section {
    direction: rtl;
    font-family: "B Nazanin", "BNazanin", Tahoma, sans-serif;
    text-align: right;
  }
  code, pre {
    direction: ltr;
    text-align: left;
    font-family: Consolas, "Courier New", monospace;
  }
  table {
    direction: rtl;
    font-size: 0.85em;
  }
  h1, h2, h3 {
    font-family: "B Nazanin", "BNazanin", Tahoma, sans-serif;
  }
---

<!-- _class: lead -->

<div dir="rtl">

# معرفی gRPC در اکوسیستم .NET

<div dir="rtl">

# دستور جلسه

1. مشکل REST در ارتباط سرویس‌به‌سرویس
2. gRPC چیست؟
3. Protocol Buffers و فایل `.proto`
4. Field Number
5. استریم و چهار الگوی RPC
6. دموی زنده (Blazor + Console)
7. Metadata، Deadline، Interceptor
8. gRPC در مقابل REST
9. جمع‌بندی و پرسش و پاسخ

</div>



---

<div dir="rtl">

# انگیزه: وقتی REST کافی نیست



در معماری میکروسرویس / ارتباط داخلی سرویس‌ها:

- JSON برای ترافیک زیاد می‌تواند پرحجم باشد
- قرارداد گاهی شل است (مستند جدا، توافق شفاهی)
- استریم واقعی سخت‌تر است (SSE / WebSocket جدا)

**REST عالی است برای API عمومی.**  
برای ارتباط داخلی گاهی ابزار بهتری داریم.

</div>

---

<div dir="rtl">

# gRPC در یک جمله

> کلاینت یک **متد** را صدا می‌زند؛  
> زیرش **HTTP/2 + Protocol Buffers** است.

برای دات‌نتی‌ها:

> حسش شبیه Interface مشترک است؛  
> پیاده‌سازی روی سرور دیگر است، ولی فراخوانی مثل متد لوکال حس می‌شود.

پروتکل بافرز (Protocol Buffers) که به اختصار به آن Protobuf می‌گویند، روش اختصاصی گوگل برای سریالایز کردن داده‌های ساختاریافته (Serialization) است؛ دقیقاً مثل JSON یا XML، اما به صورت باینری (Binary)، بسیار سبک‌تر، سریع‌تر و کاملاً Strongly-Typed.

اگر بخواهیم در یک جمله بگوییم:

Protobuf برای gRPC همان نقشی را بازی می‌کند که JSON برای REST API بازی می‌کند؛ با این تفاوت که خروجی آن متن خوانا نیست، بلکه بایت‌های فوق‌العاده فشرده است.


</div>

---

<div dir="rtl">

# سه ستون gRPC

| ستون | معنی |
|------|------|
| **Contract** | فایل `.proto` |
| **Transport** | HTTP/2 |
| **Payload** | Protocol Buffers (باینری) |

بدون این سه تا، تصویر gRPC کامل نیست.

</div>

---

<div dir="rtl">

# مقایسه سریع با REST

| | REST | gRPC |
|---|------|------|
| قرارداد | اغلب OpenAPI | `.proto` |
| فرمت | معمولاً JSON | Protobuf |
| پروتکل | HTTP/1.1 یا 2 | عمدتاً HTTP/2 |
| استریم | محدود | بومی و قوی |
| مرورگر | عالی | نیاز به gRPC-Web |

**جملهٔ کلیدی:** جایگزین همه‌چیز نیست؛ برای سرویس‌به‌سرویس اغلب بهتر است.

</div>

---

<div dir="rtl">

# فایل `.proto` = قرارداد مشترک

</div>

<div dir="ltr">

```proto
service AccountService {
  rpc GetBalance (GetBalanceRequest)
      returns (GetBalanceResponse);
}
```
</div>

<div dir="rtl">

- منبع حقیقت بین تیم سرور و کلاینت
- با Build در دات‌نت، کد C# تولید می‌شود
- سرور: `AccountServiceBase`
- کلاینت: `AccountServiceClient`

</div>

---

<div dir="rtl">

# فرمول یک متد RPC
</div>

<div dir="ltr">

```text
rpc نوع ورودی  نام_متد   returns (نوع_خروجی);
```
</div>

<div dir="rtl">

و کلمهٔ جادویی:

```text
stream
```

- بدون `stream` → یک پیام
- با `stream` → چند پیام پشت‌سرهم

</div>

---

<div dir="rtl">

# Field Number چیست؟
</div>

<div dir="ltr">

```proto
message GetBalanceResponse {
  string account_number = 1;
  double balance = 2;
  string currency = 3;
}
```
</div>

<div dir="rtl">

| چیز | نقش |
|-----|-----|
| اسم فیلد | برای برنامه‌نویس / کد C# |
| عدد (`= 1`) | شناسهٔ پایدار روی پیام باینری |

**عدد ایندکس کلاس نیست؛ ID فیلد روی شبکه است.**

</div>

---

<div dir="rtl">

# چرا Field Number مهم است؟

- JSON اسم می‌فرستد → خوانا ولی پرحجم‌تر
- Protobuf شماره می‌فرستد → کوچک‌تر و سریع‌تر

قوانین طلایی:

1. شماره را عوض نکن
2. فیلد جدید = شمارهٔ جدید
3. کلاینت قدیمی فیلد ناشناس را معمولاً نادیده می‌گیرد

</div>

---

<div dir="rtl">

# استریم یعنی چه؟

**استریم = جریان تدریجی داده**  
نه یک بستهٔ کامل یک‌جا.

| بدون استریم | با استریم |
|-------------|-----------|
| یک پاسخ حجیم | چند پیام پشت‌سرهم |
| صبر تا آماده‌شدن همه | مصرف تدریجی |

تشبیه: نامهٔ پستی در مقابل تماس تلفنی.

</div>

---

<div dir="rtl">

# چهار الگوی RPC

| الگو | شکل | مثال دمو |
|------|------|----------|
| **Unary** | ۱ → ۱ | GetBalance |
| **Server streaming** | ۱ → چند | StreamTransactions |
| **Client streaming** | چند → ۱ | DepositBatch |
| **Bidirectional** | چند ↔ چند | Chat |

</div>

---

<div dir="rtl">

# قانون `stream` در `.proto`
</div>

<div dir="ltr">

```proto
returns (stream Transaction)     // سرور چند پیام می‌فرستد
rpc X (stream DepositRequest)    // کلاینت چند پیام می‌فرستد
rpc Chat (stream M) returns (stream M)  // هر دو طرف
```
</div>

<div dir="rtl">

</div>

---

<div dir="rtl">

# معماری در ASP.NET Core

```text
account.proto
      ↓ Build
Server: AccountServiceBase
Client: AccountServiceClient
      ↓
AddGrpc + MapGrpcService
GrpcChannel + Stub
```

از نظر ASP.NET Core: gRPC یک endpoint دیگر است؛  
پروتکل و فرمت پیام فرق دارد.


۱. مفهوم GrpcChannel (کانال ارتباطی)
GrpcChannel نشان‌دهنده یک ارتباط پایدار و بادوام (Long-lived Connection) به سرور gRPC از طریق پروتکل HTTP/2 است.

وظایف اصلی کانال:

 - مدیریت TCP Connection: باز نگه داشتن کانال و استفاده مجدد از کانکشن‌ها (Connection Pooling).
- مدیریت فریم‌های HTTP/2: هندل کردن Multiplexing (ارسال هم‌زمان چند ریکوئست روی یک کانکشن بدون معطلی).
 - امنیت: مدیریت TLS و SSL Handshake.
 - تنظیمات شبکه: تعیین Timeout، اندازه بافر، فشرده‌سازی (Gzip) و Keep-Alive پکت‌ها.

۲. مفهوم Stub (کلاینت واسط / پروکسی محلی)
کلمه Stub در لغت یعنی «ریشه»، «ته‌مانده» یا «چیز توخالی»، اما در مهندسی نرم‌افزار به معنی پروکسی واسط کلاینت (Client Proxy) است.

وقتی فایل .proto کامپایل می‌شود، ابزار کامپایلر یک کلاس اختصاصی برای کلاینت می‌سازد که به آن Stub می‌گویند.

وظیفه Stub چیست؟
Stub به شما این توهم را می‌دهد که دارید یک متد محلی (Local Method) روی رم سرور خودتان را صدا می‌زنید، در حالی که آن متد فرسنگ‌ها دورتر روی یک سرور دیگر در حال اجراست! (ماهیت واقعی مفهوم RPC یا Remote Procedure Call).

Stub پشت صحنه کارهای زیر را انجام می‌دهد:

- پارامترهای ارسالی C# را می‌گیرد.
 - آن‌ها را با Protobuf تبدیل به بایت می‌کند (Serialization).
 -تحویل GrpcChannel می‌دهد تا به سرور ارسال میشوند.
 - پاسخ باینری برگشتی از سرور را می‌گیرد و دیسریالایز کرده و در قالب یک Object شیک C# به شما تحویل می‌دهد.


</div>

---

<div dir="rtl">

# راه‌اندازی سرور (خلاصه)

</div>

<div dir="ltr">

```csharp
builder.Services.AddGrpc();
app.MapGrpcService<AccountGrpcService>();
```

```csharp
public class AccountGrpcService
    : AccountService.AccountServiceBase
{
    // override متدها
}
```

خطای استاندارد:

```csharp
throw new RpcException(
    new Status(StatusCode.NotFound, "..."));
```

</div>

---

<div dir="rtl">

# کلاینت (خلاصه)
</div>

<div dir="ltr">

```csharp
var channel = GrpcChannel.ForAddress("http://localhost:5051");
var client = new AccountService.AccountServiceClient(channel);

var balance = await client.GetBalanceAsync(
    new GetBalanceRequest { AccountNumber = "1001" });
```
</div>

<div dir="rtl">

برای Client Streaming بعد از آخرین `Write`:
</div>

<div dir="ltr">

```csharp
await call.RequestStream.CompleteAsync();
```

</div>




---

<div dir="rtl">

# Metadata (Headers)

بدنهٔ پیام داخل `.proto` است.  
**Metadata** اطلاعات همراه تماس است.

| REST | gRPC |
|------|------|
| HTTP Header | Metadata |
| Body = JSON | Body = Protobuf |

نمونه‌ها:

- `x-correlation-id`
- توکن / Authorization
- کد شعبه، کانال، trace

</div>

---

<div dir="rtl">

# Headers در مقابل Trailers

| نوع | کی می‌رسد | کاربرد |
|-----|-----------|--------|
| Request Headers | اول تماس | هویت، correlation |
| Response Trailers | آخر تماس | processed-by، جزئیات |

در دموی ما Interceptor این را برمی‌گرداند:

- `x-correlation-id`
- `x-processed-by = account-service`

</div>

---

<div dir="rtl">

# Deadline

سقف زمان تماس:

```csharp
deadline: DateTime.UtcNow.AddSeconds(5)
```

اگر سرور دیر کند:

```text
StatusCode.DeadlineExceeded
```

تفاوت با Cancel:

- **Deadline:** از اول مشخص است («تا ساعت X»)
- **Cancel:** وسط کار قطع می‌کنیم

هر دو به `CancellationToken` سرور می‌رسند.

</div>

---

<div dir="rtl">

# Interceptor

شبیه Middleware در ASP.NET Core، ولی روی RPC:

```text
کلاینت → Client Interceptor → شبکه
       → Server Interceptor → متد سرویس
```

کاربردها:

- لاگ و correlation-id
- احراز هویت از Metadata
- متریک مدت‌زمان
- تبدیل خطا

`continuation` = همان `next()` در Middleware.

</div>

---

<div dir="rtl">

# gRPC در مقابل REST — عمیق‌تر

| موضوع | REST | gRPC |
|--------|------|------|
| واحد کار | منبع + فعل | متد RPC |
| قرارداد | URL + JSON schema | `.proto` |
| هویت فیلد | اسم JSON | field number |
| همراه تماس | Header | Metadata |
| سقف زمان | Timeout کلاینت | Deadline تماس |
| خطا | HTTP 404/... | `StatusCode` |

</div>

---

<div dir="rtl">

# نقشهٔ تقریبی خطاها

| HTTP | gRPC |
|------|------|
| 400 | InvalidArgument |
| 401 | Unauthenticated |
| 403 | PermissionDenied |
| 404 | NotFound |
| 429 | ResourceExhausted |
| 500 | Internal |
| 504 | DeadlineExceeded |

دقیق یک‌به‌یک نیستند؛ برای کار روزمره کافی است.

</div>

---

<div dir="rtl">

# الگوی معماری پیشنهادی

```text
کانال خارجی (موبایل، وب، شریک)
        REST / JSON
              ↓
         API Gateway
              ↓
   سرویس‌های داخلی با gRPC
```

بیرون: سازگار و آشنا  
داخل: قرارداد سخت + عملکرد بهتر

</div>

---

<div dir="rtl">

# کی استفاده کنیم؟

**بله:**

- ارتباط داخلی میکروسرویس‌ها
- نیاز به قرارداد سخت بین تیم‌ها
- نیاز به استریم
- ترافیک زیاد سرویس‌به‌سرویس

**احتیاط / خیر:**

- API عمومی مرورگر بدون gRPC-Web
- وقتی مصرف‌کننده فقط REST می‌خواهد
- وقتی اولویت اول دیباگ انسانی با JSON است

</div>



---

<div dir="rtl">

# سوالات پرتکرار 

**REST را دور بریزیم؟**  
نه. مکمل‌اند.

**از مرورگر مستقیم؟**  
معمولاً gRPC-Web یا Gateway.

*«gRPC استاندارد برای دنیای بک‌اند به بک‌اند (سرویس‌های داخلی با HTTP/2 خالص) پادشاهی می‌کند.

اما وقتی پای مرورگر وب وسط می‌آید، به دلیل محدودیت‌های امنیتی مرورگرها در دسترسی به Trailers و فریم‌های HTTP/2، از gRPC-Web به عنوان یک پل ارتباطی (Bridge) استفاده می‌کنیم.»*

**Field number عوض شود؟**  
خطر شکستن سازگاری.

**کجا در سازمان بانکی؟**  
ارتباط داخلی سرویس‌ها، نه لزوماً درگاه عمومی مشتری.

</div>

---

<!-- _class: lead -->

<div dir="rtl">

# پرسش و پاسخ

### ممنون از توجه شما

</div>
