USE KickBoxingClubDB;
GO

/* =========================
   CRUD: AppUser
   ========================= */

-- CREATE (INSERT)
INSERT INTO AppUser (FirstName, LastName, DateOfBirth)
VALUES ('Test', 'Member', '1999-05-20');

-- READ (SELECT)
SELECT TOP 10 *
FROM AppUser
ORDER BY UserID DESC;

-- UPDATE
UPDATE AppUser
SET LastName = 'Member-Updated'
WHERE FirstName = 'Test' AND LastName = 'Member';

-- DELETE (OBS: kräver att inga FK pekar på användaren)
-- Alternativ: radera först Account/Membership som refererar UserID
DELETE FROM AppUser
WHERE FirstName = 'Test' AND LastName = 'Member-Updated';
GO


/* =========================
   CRUD: Account
   ========================= */

-- CREATE
-- Välj ett befintligt UserID och RoleID från seed, eller hämta dem först:
-- SELECT TOP 1 UserID FROM AppUser ORDER BY UserID DESC;
-- SELECT * FROM [Role];

INSERT INTO Account (Email, PasswordHash, UserID, RoleID)
VALUES ('test.member@kickbox.se', 'HASHED_PASSWORD', 1, 3); -- ex: RoleID=3 Member

-- READ
SELECT a.AccountID, a.Email, a.CreationDate, u.FirstName, u.LastName, r.Title AS RoleTitle
FROM Account a
JOIN AppUser u ON a.UserID = u.UserID
JOIN [Role] r ON a.RoleID = r.RoleID
ORDER BY a.AccountID DESC;

-- UPDATE (t.ex. byta email)
UPDATE Account
SET Email = 'test.member+updated@kickbox.se'
WHERE Email = 'test.member@kickbox.se';

-- DELETE (OBS: kräver att inga FK pekar på AccountID, t.ex. Invoice/Newsletter/Session TrainerID)
DELETE FROM Account
WHERE Email = 'test.member+updated@kickbox.se';
GO


/* =========================
   CRUD: Membership (relation AppUser <-> MembershipPlan)
   ========================= */

-- CREATE (nytt medlemskap)
-- Hämta ID:n:
-- SELECT TOP 5 UserID, FirstName, LastName FROM AppUser;
-- SELECT TOP 5 MembershipPlanID, BillingPeriod FROM MembershipPlan;

INSERT INTO Membership (UserID, EndDate, MembershipPlanID)
VALUES (1, DATEADD(MONTH, 1, GETDATE()), 1);

-- READ (aktiva medlemskap)
SELECT m.MembershipID, u.FirstName, u.LastName, m.IsActive, m.StartDate, m.EndDate, mp.BillingPeriod
FROM Membership m
JOIN AppUser u ON m.UserID = u.UserID
JOIN MembershipPlan mp ON m.MembershipPlanID = mp.MembershipPlanID
WHERE m.IsActive = 1
ORDER BY m.EndDate ASC;

-- UPDATE (förläng medlemskap 1 månad)
UPDATE Membership
SET EndDate = DATEADD(MONTH, 1, EndDate)
WHERE MembershipID = 1;

-- DELETE (om ni vill visa delete, annars “soft delete” via IsActive)
DELETE FROM Membership
WHERE MembershipID = 1;
GO


/* =========================
   CRUD: Session (schema)
   ========================= */

-- CREATE (nytt pass)
-- OBS TrainerID pekar på Account.AccountID (ni har FK mot Account)
-- DayID från WeekDay, SessionTypeID från SessionType
INSERT INTO [Session]
(TrainerID, SessionTypeID, StartTime, Duration, Capacity, Focus, DayID, SessionWeek)
VALUES
(1, 1, '18:00', 60, 20, 'Basics + Conditioning', 1, 5);

-- READ (schema med dag + typ + tränare)
SELECT s.SessionID, wd.DayName, s.SessionWeek, s.StartTime, s.Duration, s.Capacity,
       st.TypeTitle, s.Focus,
       u.FirstName + ' ' + u.LastName AS TrainerName
FROM [Session] s
JOIN WeekDay wd ON s.DayID = wd.DayID
JOIN SessionType st ON s.SessionTypeID = st.SessionTypeID
JOIN Account a ON s.TrainerID = a.AccountID
JOIN AppUser u ON a.UserID = u.UserID
ORDER BY s.SessionWeek DESC, s.DayID ASC, s.StartTime ASC;

-- UPDATE (ändra kapacitet)
UPDATE [Session]
SET Capacity = 25
WHERE SessionID = 1;

-- DELETE
DELETE FROM [Session]
WHERE SessionID = 1;
GO


/* =========================
   CRUD: Invoice
   ========================= */

-- CREATE (skapa faktura för konto + plan)
INSERT INTO Invoice (AccountID, MembershipPlanID)
VALUES (1, 1);

-- READ (fakturor med belopp)
SELECT i.InvoiceID, i.InvoiceDate,
       a.Email,
       mp.BillingPeriod,
       pl.Label, pl.Amount
FROM Invoice i
JOIN Account a ON i.AccountID = a.AccountID
JOIN MembershipPlan mp ON i.MembershipPlanID = mp.MembershipPlanID
JOIN PriceList pl ON mp.PriceID = pl.PriceID
ORDER BY i.InvoiceDate DESC;

-- UPDATE (flytta fakturadatum – mest för att visa UPDATE)
UPDATE Invoice
SET InvoiceDate = DATEADD(DAY, -1, InvoiceDate)
WHERE InvoiceID = 1;

-- DELETE
DELETE FROM Invoice
WHERE InvoiceID = 1;
GO
