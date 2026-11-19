# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved

Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

R6 (Durata maxima) --> It was absent in the starter and now it lives in Booking.Create for obvious purposes, so it limits the booking duration to 4 hours exatlly after it was created

## 2. Result instead of an exception

Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

Result<T> makes you see the error on compile, it is not like an exception (not hidden)

## 3. Who knows about EF Core?

Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

Classes that know about EF Core: Infrastructure/Persistence/RoomHubDbContext.cs, Configurations/RoomConfiguration.cs + BookingConfiguration.cs, Repository/RoomRepository.cs + BookingRepository.cs (They use DbSet, etc)
Classes that don t know about EF Core: Domain/*, Features/ handlers, IRoomRepository/IBookingRepository/IUnitOfWork interfaces (are in Domain/, no EF references)
What i gain: the domain and use-cases are independent of the db, i can change the persistance without changing the rules
What i pay: more code

## 4. The flow of a request

Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

POST /api/rooms/{roomId}/bookings
-> CreateBookingEndpoint.Handle (Features/Bookings/CreateBooking)
-> ICommandHandler<CreateBookingCommand, Booking>
-> CreateBookingHandler.HandleAsync
-> IRoomRepository.GetByIdAsync (404 if null)
-> TimeSlot.Create (R3)
-> Booking.Create (R2,R4,R5,R6,R10)
-> IBookingRepository.CountActiveForParticipantOnDayAsync (R9)
-> IBookingRepository.ListActiveForRoomAsync + TimeSlot.OverlapsWith (R7)
-> IBookingRepository.AddAsync
-> IUnitOfWork.SaveChangesAsync
-> RoomHubDbContext -> SQLite (roomhub.db)
-> endpoint: Results.Created (201) sau error.ToProblem()

## 5. What you rejected from what the AI gave you

Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

_(your answer here)_

## 6. If you had 30 more minutes

What would you improve and what risk did you leave in the project?

_(your answer here)_
