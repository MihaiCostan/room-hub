# BUGS

The starter contains behaviors that contradict `ASSIGNMENT.md`. Write here every discrepancy you found.
Fill it in yourself, in your own words. There is no required number: what counts is that they are real.

| Which rule from ASSIGNMENT.md is violated (R1–R8)                             | Where the problem was in the starter (file, approximate line)                                                                            | How you discovered it (test, reading, AI) | What you changed                             |
| ----------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------- | -------------------------------------------- |
| _(fill in)_                                                                   | _(fill in)_                                                                                                                              | _(fill in)_                               | _(fill in)_                                  |
| R6 (The maximum duration of a booking is 4 hours (exactly 4 hours is allowed) | In started, BookingService.Create nu verifica nicaieri durata bookingului => se poate face booking de mai multe ore decat limita propusa | Ai + reading                              | In TimeSlot.Create se adauga regula "durata" |

| R5 (A booking cannot start in the past. “Now” comes from TimeProvider, not from DateTime.UtcNow / DateTime.Now.) | starterul nu verifica deloc faptul ca bookingul nu incepe in trecut si folosea "DateTime" | Ai + reading code | in Booking.Create primesc nowUtc ca parametru (de la TimeProvider) si resping start < now

| R8 (Cancelling moves a booking from Active to Cancelled. Cancelling an already cancelled booking gives 409. Cancelling a missing one gives 404. A cancelled booking frees its interval.) | starterul Cancel reseta statusul si intorcea 204 chiar daca era de "Cancelled"| Ai | In Booking.Cancel() intorc conflict in acest caz

| R7 | intervalele care se ating sunt tratate ca suprapuse | AI | overlap-ul folosește TimeSlot.OverlapsWith (strict <), nu <= ca starter-ul.
| R7/R8 | overlapul nu tine cont de status (bookingurile anulate nu elibereaza intervalul) foreach (var existing in bookings.ListForRoom(roomId)) | Ai | overlap-ul se face doar pe bookings Active (ListActiveForRoomAsync), deci cele anulate elibereaza intervalul. Starter-ul le numara pe toate.
