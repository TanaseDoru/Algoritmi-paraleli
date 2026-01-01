# Ex 1
## Output
```c
Parent task has finished!
Main thread exiting...
```
- Dupa decomnetare
```c
Parent task has finished!
Child task has finished!
Main thread exiting...
```
## Explicatie
- TaskCreationOptions.AttachedToParent face ca task-ul copil sa fie atasat de parinte, adica dupa ce se termina copilul atunci se trece mai departe in functia Main
- DenyChildAttach face ca atributul de AttachedToParent al copilului sa fie ignorat
- Task.Run executa munca de tip CPU-Bound pe thread pool, folosit pentru lansarea usoara a task-urilor
- TaskFactory.StarNew este mai versatil si se poate folosi pentru lansarea de task-uri cu optiuni explicite

# Ex 2
- Am paralelizat procesarea prietenilor din lista de prieteni
- Am calulcat nr de parinti pentru fiecare nod
- initializarea unei cozi thread-safe cu nodurile care nua u parinti
- Lansarea in paralel a procesarii nodurilor folosind Task.Run
- La finalizarea unui nod se decrementaza controlul de parinti pentru vecin
- Daca un vecin ajunge la 0 parinti, este adaugat in coada de noduri gata de procesare

# Ex 3
- Am traversat recusriv paralele folosind Paralle.ForEach
- Am utilizat Interlocked si CocnrrentBag pentru concurenta

## Output
```
Files count: 29
Folders count: 9
Total file size: 373838 bytes
Last written file: apphost.exe
Last written file time: 01/01/2026 16:42:39
```

# Ex 4
- Daca schimb bucla:
```cs
for (int i = 0; i < 20; i++)
{
    int nr = await bufferBlock.ReceiveAsync();
}
```
- Programul ruleaza la nesfarsit deoarece BufferBlock contine doar 10 elemente, iar dupa a 10-a apelare, atunci va ramane blocat in asteptare
- Solutie()(tryReceive):
```cs
for (int i = 0; i < 20; i++)
{
    if (bufferBlock.TryReceive(out int nr))
    {
        Console.WriteLine(nr);
    }
    else
    {
        break;
    }
}
```
-
- Daca decomentam linia vom avea:
```
Battery sufficiently charged!
Battery sufficiently charged!
Battery sufficiently charged!
Battery sufficiently charged!
Battery sufficiently charged!
Battery sufficiently charged!
Battery sufficiently charged!
Battery sufficiently charged!
Battery sufficiently charged!
Battery sufficiently charged!
```
- Deoarece se suprascrie valoarea din post
-
- Daca decomentam linia cu WriteOnceBlock rezultatul nu se va schimba, deoarece tot prima valoare ava fii scrisa, iar a doua apelare va esua

# Ex 5
- ActionBlock executa o actiune pentru fiecare element primit
- TransformBlock transforma fiecare element de intrare intr-un element de iesire
-
- Daca decomentam linia `await transformManyBlock.SendAsync(-1234);` Bucla input > 0 nu intra niciodata, digits ramane gol, returneaza IEnumerable gol, iar pentru orice Receive se va face o blocare la infinit
-
- Solutie
```cs
transformManyBlock.Complete();
await transformManyBlock.Completion; 

while (transformManyBlock.TryReceive(out int digit))
{
    Console.Write(digit);
    Console.Write(" ");
}
Console.WriteLine();
```
## Output
```
0
1
2
3
4
5
6
7
8
9
ONE
TWO
THREE
FOUR
FIVE
SIX
SEVEN
EIGHT
NINE
TEN
ELEVEN
TWELVE
THIRTEEN
FOURTEEN
FIFTEEN
SIXTEEN
SEVENTEEN
EIGHTEEN
NINETEEN
TWENTY
1 2 3 4 5 6
0
1 0 0 2 0
1 2 3
```

# Ex 6
## Output
```cs
Group 1:
        Emma Johnson
        William Thompson
        Mia Taylor
Group 2:
        Liam Adams
        Charlotte Lewis
        Noah Brooks
Group 3:
        Ethan Mitchell
        Emily Powell
        Lucas Reed
Group 4:
        Lily Simmons
        Daniel Hayes
        Isabella Wright
Group 5:
        Amelia Davis

Toyota Camry: 25000$
Ford Mustang: 35000$
Honda Accord: 27000$
Chevrolet Silverado: 40000$
BMW 3 Series: 45000$
Nissan Rogue: 30000$
Tesla Model 3: 50000$
Volkswagen Golf: 22000$
```
## Explicatie
- 4 groupuri complete de cate 3 persoane
- ultimul grup are doar 1 element
- JoinBlock asteapta cate un element de pe fiecare intrare si cand are o pereche completa elibereaza un tuple
- Avem 8 masini si 8 preturi in ordine => Se face atribuirea in ordine corecta
-
- Am folosit JoinBlock si BatchBlock pentru a extrage informatiile necesare
```cs
Batch 1:
  Cheapest car: Toyota Camry - 25000$
Batch 2:
  Cheapest car: Nissan Rogue - 30000$
Batch 3:
  Cheapest car: Volkswagen Golf - 22000$
```

# Ex 7
- Folosind date prin DataFlow, atunci putem crea mai usor legaturi intre cele 3 clase si putem face combinatii intre acestea mai usor.
- Astfel se pot crea perechi de cate 2 intre toate tipurile de materiale. In plus este mult mai modular, deoarece o adaugare a unei noi resurse se poate face foarte usor
- Daca folosim Greedy = false in oricare dintre cele 2 JoinBlock-uri, acestea vor astepta sa primeasca un wood in momentul in care exista o pereche valida. 
- Greedy practic consuma acel wood in momentul in care il primeste, daca setam pe false asteapta pana are o pereche completa
## Output
```cs
Wood + Iron 1/10
Wood + Stone 1/10
Wood + Iron 2/10
Wood + Stone 2/10
Wood + Iron 3/10
Wood + Stone 3/10
Wood + Iron 4/10
//...
```
## Eficientizare
- Am facut Gredy False
- Apel Complete() pe toate resursele 
- Asteptare finalizare actiuni

# Ex 8
<details>

<summary>Output processare feed-uri</summary>

## Output

```ruby

[2026-01-01 06:20] Newegg Promo Code: 10% Off in January 2026
   Categories: Gear

[2026-01-01 06:10] 20% Off LG Promo Code & Coupons | January 2026
   Categories: Gear

[2026-01-01 06:00] NordVPN Coupons and Deals: 77% Off in January 2026
   Categories: Gear

[2025-12-31 12:31] Factor Meal Delivery Promo: Free $200 Withings Body-Scan Scale
   Categories: Gear, Gear / Deals, Gear / Products / Home, Gear / Products / Online Services

[2025-12-31 12:08] The Best Over-the-Counter Sleep Aids (2025), Tested and Reviewed
   Categories: Gear, Gear / Buying Guides, Gear / Products / Sleep

[2025-12-31 12:00] Poor Sleep Quality Accelerates Brain Aging
   Categories: Science, Science / Health

[2025-12-31 11:00] AI-Powered Dating Is All Hype. IRL Cruising Is the Future
   Categories: Culture, Culture / Digital Culture

[2025-12-31 11:00] Fears Mount That US Federal Cybersecurity Is Stagnating-or Worse
   Categories: Security, Security / National Security, Security / Privacy

[2025-12-31 10:00] Discovering the Dimensions of a New Cold War
   Categories: Security, Security / National Security

[2025-12-31 06:10] Walmart Promo Codes and Coupons: Up to 65% Off
   Categories: Gear

[2025-12-31 06:00] Google Workspace Promo Code: Up to 14% Off in 2026
   Categories: Gear, Gear / Deals

[2025-12-30 12:30] Commodore 64 Ultimate Review: An Astonishing Remake
   Categories: Gear, Gear / Reviews, Gear / Products / Gaming

[2025-12-30 12:00] What Is a Preamp, and Do I Really Need One?
   Categories: Gear, Gear / How To and Advice, Gear / Products, Gear / Products / Audio

[2025-12-30 11:38] 7 Best Digital Notebooks (2026): reMarkable, Kobo, Kindle
   Categories: Gear, Gear / Buying Guides, Gear / Products / Accessories

[2025-12-30 11:00] Tips for Keeping a Digital Diary and Why You Should
   Categories: Gear, Gear / How To and Advice

[2025-12-30 11:00] The Great Big Power Play
   Categories: Science, Science / Environment

[2025-12-30 10:30] China Will Tax Contraceptives in a Bid to Improve Birth Rates
   Categories: Politics / Policy

[2025-12-29 13:00] iMP Tech Mini Arcade Pro Review: A Nintendo Switch Arcade Cabinet
   Categories: Gear, Gear / Reviews, Gear / Products / Gaming

[2025-12-29 12:02] The Best Vacuum for Pet Hair-We Tested Many to Find Which Ones Work Best (2026)
   Categories: Gear, Gear / Buying Guides, Gear / Products / Home

[2025-12-29 12:00] The Worst Hacks of 2025
   Categories: Security, Security / Cyberattacks and Hacks

[2025-12-29 12:00] The Best Body Pillow, Tested and Reviewed (2025)
   Categories: Gear, Gear / Buying Guides, Gear / Products / Home, Gear / Products / Sleep

[2025-12-29 11:00] 3 New Tricks to Try With Google Gemini Live After Its Latest Major Upgrade
   Categories: Gear, Gear / How To and Advice

[2025-12-29 11:00] The New Surveillance State Is You
   Categories: Security, Security / National Security, Security / Privacy

[2025-12-29 10:30] The Most Dangerous People on the Internet in 2025
   Categories: Security, Security / Security News

[2025-12-29 10:00] The Earth Is Nearing an Environmental Tipping Point
   Categories: Science, Science / Environment

[2025-12-29 07:00] Home Chef Promo Code: 50% Off
   Categories: Gear

[2025-12-28 13:09] How Much Melatonin Should You Be Taking? (2026)
   Categories: Gear, Gear / How To and Advice, Gear / Products / Sleep

[2025-12-28 12:30] 10 Best Drones (2025): Flight-Tested and Reviewed
   Categories: Gear, Gear / Buying Guides, Gear / Products / Cameras

[2025-12-28 11:32] 8 Best Plant-Based Meal Delivery Services and Kits (2025), Tested, Tasted, and Reviewed
   Categories: Gear, Gear / Buying Guides, Gear / Products / Kitchen, Gear / Products / Online Services

[2025-12-28 11:30] People Who Drink Bottled Water on a Daily Basis Ingest 90,000 More Microplastic Particles Each Year
   Categories: Science, Science / Health

[2025-12-28 11:00] Billion-Dollar Data Centers Are Taking Over the World
   Categories: Business, Business / Energy, Business / Computers and Software, Business / Big Tech, Business / Artificial Intelligence

[2025-12-28 10:00] The Dollar Is Facing an End to Its Dominance
   Categories: Business, Business / Blockchain and Cryptocurrency

[2025-12-28 07:00] Behold the Manifold, the Concept that Changed How Mathematicians View Space
   Categories: Science, Science / Physics and Math

[2025-12-28 06:30] Therabody Promo Codes and Deals: Get 30% Off This Month
   Categories: Gear

[2025-12-28 06:00] Hungryroot Coupon Codes: 30% Off This Month
   Categories: Gear, Gear / Deals

[2025-12-27 16:42] The Best After-Christmas Deals on Gear We ve Tested (2025)
   Categories: Gear, Gear / Deals

[2025-12-27 12:30] Hyperkin The Competitor Controller Review: A DualSense Copycat
   Categories: Gear, Gear / Reviews, Gear / Products / Gaming

[2025-12-27 12:00] The Environmental and Human Rights Costs of China's Clean Energy Investments Abroad
   Categories: Science, Science / Environment

[2025-12-27 12:00] The 48 Best Shows on Netflix, WIRED's Picks (December 2025)
   Categories: Culture, Culture / TV

[2025-12-27 11:32] Tuft & Needle Original Hybrid Mattress Review: A Soft Landing
   Categories: Gear, Gear / Reviews, Gear / Products / Sleep

[2025-12-31 18:05] Top .NET Videos & Live Streams of 2025
   Categories: .NET, 2025 wrapped

[2025-12-30 18:05] Top .NET Blog Posts of 2025
   Categories: .NET, .NET Aspire, AI, C#, Cloud Native, Performance, Visual Studio, Visual Studio Code, .NET 10, 2025 wrapped, aspire, community, performance, tooling

[2025-12-16 18:05] Microsoft.Testing.Platform Now Fully Supported in Azure DevOps
   Categories: .NET, C#, F#, Visual Basic, Announcements, azdo, Azure DevOPs, testing

[2025-12-15 18:05] How to Build iOS Widgets with .NET MAUI
   Categories: .NET, .NET for iOS, .NET MAUI, .net maui, ios, mobile development, Swift, widgets, Xcode

[2025-12-09 18:48] .NET and .NET Framework December 2025 servicing releases updates
   Categories: .NET, .NET Framework, Maintenance & Updates, .net framework

[2025-12-09 18:05] Implementing Cross-Platform In-App Billing in .NET MAUI Applications
   Categories: .NET, .NET MAUI, billing, Google Play Billing, in-app purchases, Microsoft Store, StoreKit

[2025-12-08 20:00] Microsoft Learn MCP Server Elevates Development
   Categories: .NET, AI, C#, Visual Studio, Visual Studio Code, copilot, docs, learn, MCP Server

[2025-12-08 18:05] .NET 10 Networking Improvements
   Categories: .NET, Networking, .NET 10, http, net-security, web-sockets

[2025-12-04 18:05] .NET Conf 2025 Recap - Celebrating .NET 10, Visual Studio 2026, AI, Community, & More
   Categories: .NET, .NET MAUI, AI, ASP.NET Core, Blazor, C#, NuGet, Visual Studio, .NET 10, conference, Featured

[2025-12-03 18:05] Introducing Data Ingestion Building Blocks (Preview)
   Categories: .NET, AI, Data, dataingestion, rag

[2025-12-11 18:00] Available today: GPT-5.2 in Microsoft 365 Copilot

[2025-12-04 16:00] Advancing Microsoft 365: New capabilities and pricing update

[2025-12-02 17:00] Microsoft 365 Copilot Business: The future of work for small businesses

[2025-11-18 16:00] Why Microsoft Copilot Studio is the foundation for agentic business transformation
   Categories: Agents, Copilot Studio

[2025-11-18 16:00] Microsoft Agent 365: The control plane for AI agents
   Categories: AI

[2025-11-18 16:00] Microsoft Ignite 2025: Copilot and agents built to power the Frontier Firm
   Categories: Agents

[2025-11-10 18:00] Whats new in Copilot Studio: October 2025

[2025-11-05 07:00] Microsoft offers in-country data processing to 15 countries to strengthen sovereign controls for Microsoft 365 Copilot

[2025-10-28 13:30] Microsoft 365 Copilot now enables you to build apps and workflows
   Categories: Agents, Copilot for Work

[2025-10-15 15:00] Whats new in Copilot Studio: September 2025
   Categories: AI, Copilot Studio

[2025-12-04 05:37] How I Get Free Traffic from ChatGPT in 2025 (AIO vs SEO)

[2025-01-02 09:26] Top 10 AI Tools That Will Transform Your Content Creation in 2025

[2023-12-12 16:10] LimeWire AI Studio Review 2023: Details, Pricing & Features
   Categories: Ai

[2023-01-25 19:52] Top 10 AI Tools in 2023 That Will Make Your Life Easier
   Categories: Ai

[2022-11-15 08:58] Top 10 AI Content Generator & Writer Tools in 2022
   Categories: Ai, Content Writing

[2022-09-10 08:57] Beginner Guide to CJ Affiliate (Commission Junction) in 2022
   Categories: Affiliate marketing

[2022-07-13 15:25] TOP 11 AI MARKETING TOOLS YOU SHOULD USE (Updated 2022)
   Categories: Ai, Blockchain, Tool

[2022-06-01 13:03] Most Frequently Asked Questions About Affiliate Marketing
   Categories: Affiliate marketing, FAQ

[2022-04-18 05:49] What is Blockchain: Everything You Need to Know (2022)
   Categories: Blockchain

[2022-03-13 15:04] ProWritingAid VS Grammarly: Which Grammar Checker is Better in (2022) ?
   Categories: Comparison

[2022-03-12 11:54] Sellfy Review 2022: How Good Is This Ecommerce Platform?

[2022-03-01 12:16] Ahrefs vs SEMrush: Which SEO Tool Should You Use?
   Categories: Comparison

[2022-02-26 13:36] Top 10 Best PLR(Private Label Rights)  Websites | Which One You Should Join in 2022?
   Categories: PLR

[2022-02-20 12:02] Canva Review 2022: Details, Pricing & Features
   Categories: review

[2022-02-11 09:00] Top 7 Best Wordpress Plugin Of All Time
   Categories: Wordpress

[2022-02-10 10:28] Ginger VS Grammarly: Which Grammar Checker is Better in (2022) ?
   Categories: Comparison

[2022-02-06 10:04] Most Frequently Asked Questions About NFTs(Non-Fungible Tokens)
   Categories: FAQ, NFTs

[2022-01-31 10:56] 10 Best Chrome Extensions That Are Perfect for Everyone
   Categories: Chrome Extension

[2022-01-29 12:45] Most Frequently Asked Questions About  Email Marketing
   Categories: Email Marketing, FAQ

[2022-01-27 15:19] 7 Free Websites Every Content Creator Needs to Know
   Categories: Tool

[2022-01-24 12:07] Top 9 Free AI Tools That Make Your Life Easier
   Categories: Ai

Toate articolele au fost procesate.
```


</details>

# Ex 9
## Output
```js
Categorii unice gasite in feed:
BUSINESS
BUSINESS / ARTIFICIAL INTELLIGENCE
BUSINESS / BIG TECH
BUSINESS / BLOCKCHAIN AND CRYPTOCURRENCY
BUSINESS / COMPUTERS AND SOFTWARE
BUSINESS / ENERGY
CULTURE
CULTURE / DIGITAL CULTURE
CULTURE / TV
GEAR
GEAR / BUYING GUIDES
GEAR / DEALS
GEAR / HOW TO AND ADVICE
GEAR / PRODUCTS
GEAR / PRODUCTS / ACCESSORIES
GEAR / PRODUCTS / AUDIO
GEAR / PRODUCTS / CAMERAS
GEAR / PRODUCTS / GAMING
GEAR / PRODUCTS / HEALTH AND FITNESS
GEAR / PRODUCTS / HOME
GEAR / PRODUCTS / KITCHEN
GEAR / PRODUCTS / ONLINE SERVICES
GEAR / PRODUCTS / OUTDOOR
GEAR / PRODUCTS / SLEEP
GEAR / REVIEWS
POLITICS / POLICY
SCIENCE
SCIENCE / ENVIRONMENT
SCIENCE / HEALTH
SCIENCE / PHYSICS AND MATH
SECURITY
SECURITY / CYBERATTACKS AND HACKS
SECURITY / NATIONAL SECURITY
SECURITY / PRIVACY
SECURITY / SECURITY NEWS

Total categorii unice: 35
```

## Explicatie
- BufferBlock primeste articolele din feed-ul RSS
- TransformManyBlock extrage toate numerele de categorii din fiecare articol
- ActionBlock Adauga fiecare categorie scrisa cu majuscule
- Al doilea ActionBlock afiseaza lista unica de categorii