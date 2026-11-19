# AI-LOG

Write down, in order, what you asked the AI. It is not a punishment: a missing or false log lowers the level D grade.

| Time (approx.) | What you asked (briefly)                                | What you kept                                                                                                    | What you changed or rejected                |
| -------------- | ------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------- |
| _(fill in)_    | _(fill in)_                                             | _(fill in)_                                                                                                      | _(fill in)_                                 |
| 3 min          | vreau sa incepi acum sa lucrezi la scheletul aplicatiei | the code changes ("Încep cu scheletul comun: Common/CQRS + Domain/Common."):                                     | tested the build + commit to public repo    |
| 4 min          | continua cu partea de rich domain                       | code edit (Continui cu rich domain. Încep cu Room (R1), apoi TimeSlot (R3 + R6), apoi Booking (R2, R4, R5, R8).) | tested the build + commit + bugs identified |
| 7 min | acum vreau sa treci la implementarea Feautures (slice-urile propriu zise) | Trec la Pas 3 — slice-urile. Creez cele 5 features, fiecare cu command/query + handler + endpoint subțire. Întâi helperul comun de mapare Result → ProblemDetails, apoi slice-urile. (code edit) | commit + tests | 

| 7 min | continua cu EF Core + SQLite + repository inainte de rescrierea Program.cs | EF Core + SQLite. Întâi adaug pachetul, apoi DbContext + configurări + implementările de repository. | build + version check for ef core

| 5 | Continua cu rescrierea Program.cs si curatarea proiectului | Rescriu Program.cs cu noul DI și mapări, apoi șterg codul vechi. |

| 7 | vreau sa scrii un set de teste pentru aplicatie @file:roomhub-starter/roomhub.http | Scriu un set complet de teste în roomhub.http, păstrând cele existente (1–7) și adăugând sub secțiunea 8 scenarii pentru fiecare regulă R1–R11, cu codul HTTP așteptat la fiecare. | 