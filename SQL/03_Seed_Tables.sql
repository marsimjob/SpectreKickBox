USE KickBoxingClubDB;
GO

-- Seed Roles
INSERT INTO [Role] (Title) VALUES ('Staff');
INSERT INTO [Role] (Title) VALUES ('Trainer');
INSERT INTO [Role] (Title) VALUES ('Member');

-- Seed Days (1-7)
INSERT INTO [WeekDay] (DayID, DayName) VALUES (1, 'Monday');
INSERT INTO [WeekDay] (DayID, DayName) VALUES (2, 'Tuesday');
INSERT INTO [WeekDay] (DayID, DayName) VALUES (3, 'Wednesday');
INSERT INTO [WeekDay] (DayID, DayName) VALUES (4, 'Thursday');
INSERT INTO [WeekDay] (DayID, DayName) VALUES (5, 'Friday');
INSERT INTO [WeekDay] (DayID, DayName) VALUES (6, 'Saturday');
INSERT INTO [WeekDay] (DayID, DayName) VALUES (7, 'Sunday');

-- Seed Prices
INSERT INTO PriceList (Label, Amount) VALUES ('Monthly – 299 kr', 299.00);
INSERT INTO PriceList (Label, Amount) VALUES ('Quarterly – 699 kr', 699.00);
INSERT INTO PriceList (Label, Amount) VALUES ('Yearly – 999 kr', 999.00);

-- Seed MembershipPlans
INSERT INTO MembershipPlan (RoleID, PriceID, BillingPeriod) VALUES
(3,1,'Monthly'),
(3,2,'Quarterly'),
(3,3,'Yearly');

-- Seed 20 Users (2 Teachers, 2 Staff, 20 Members)
INSERT INTO AppUser (FirstName, LastName, DateOfBirth) VALUES
('Oliver','Smith','1990-01-01'),
('Emma','Johnson','1991-02-02'),
('Liam','Williams','1992-03-03'),
('Ava','Brown','1993-04-04'),
('Noah','Jones','1994-05-05'),
('Sophia','Garcia','1995-06-06'),
('Elijah','Miller','1996-07-07'),
('Isabella','Davis','1997-08-08'),
('Lucas','Rodriguez','1998-09-09'),
('Mia','Martinez','1999-10-10'),
('Mason','Hernandez','2000-11-11'),
('Charlotte','Lopez','2001-12-12'),
('Ethan','Gonzales','2002-01-13'),
('Amelia','Wilson','2003-02-14'),
('Logan','Anderson','2004-03-15'),
('Harper','Thomas','2005-04-16'),
('James','Taylor','2006-05-17'),
('Evelyn','Moore','2007-06-18'),
('Aiden','Jackson','2008-07-19'),
('Abigail','Martin','2009-08-20'),
('Benjamin','Lee','2010-09-21'),
('Emily','Perez','2011-10-22'),
('Sebastian','Thompson','2012-11-23'),
('Ella','White','2013-12-24'),
('Oliver2','Harris','2014-01-25'),
('Avery','Sanchez','2015-02-26'),
('Jacob','Clark','2016-03-27'),
('Scarlett','Ramirez','2017-04-28'),
('Michael','Lewis','2018-05-29'),
('Grace','Robinson','2019-06-30');

-- Seed Accounts
-- Staff
INSERT INTO [Account] (Email, PasswordHash, UserID, RoleID) VALUES
('staff1@club.se','HASH_STAFF1',1,1),
('staff2@club.se','HASH_STAFF2',2,1);

-- Trainers
INSERT INTO [Account] (Email, PasswordHash, UserID, RoleID) VALUES
('trainer1@club.se','HASH_TRAINER1',3,2),
('trainer2@club.se','HASH_TRAINER2',4,2);

-- Members (remaining 26 users)
INSERT INTO [Account] (Email, PasswordHash, UserID, RoleID) VALUES
('member1@club.se','HASH_MEMBER1',5,3),
('member2@club.se','HASH_MEMBER2',6,3),
('member3@club.se','HASH_MEMBER3',7,3),
('member4@club.se','HASH_MEMBER4',8,3),
('member5@club.se','HASH_MEMBER5',9,3),
('member6@club.se','HASH_MEMBER6',10,3),
('member7@club.se','HASH_MEMBER7',11,3),
('member8@club.se','HASH_MEMBER8',12,3),
('member9@club.se','HASH_MEMBER9',13,3),
('member10@club.se','HASH_MEMBER10',14,3),
('member11@club.se','HASH_MEMBER11',15,3),
('member12@club.se','HASH_MEMBER12',16,3),
('member13@club.se','HASH_MEMBER13',17,3),
('member14@club.se','HASH_MEMBER14',18,3),
('member15@club.se','HASH_MEMBER15',19,3),
('member16@club.se','HASH_MEMBER16',20,3),
('member17@club.se','HASH_MEMBER17',21,3),
('member18@club.se','HASH_MEMBER18',22,3),
('member19@club.se','HASH_MEMBER19',23,3),
('member20@club.se','HASH_MEMBER20',24,3),
('member21@club.se','HASH_MEMBER21',25,3),
('member22@club.se','HASH_MEMBER22',26,3),
('member23@club.se','HASH_MEMBER23',27,3),
('member24@club.se','HASH_MEMBER24',28,3),
('member25@club.se','HASH_MEMBER25',29,3),
('member26@club.se','HASH_MEMBER26',30,3);

-- Seed Membership Plans (RoleID 3 = Member)
INSERT INTO Membership (UserID, EndDate, MembershipPlanID) VALUES
(5, DATEADD(MONTH,1,GETDATE()), 1),
(6, DATEADD(MONTH,3,GETDATE()), 2),
(7, DATEADD(YEAR,1,GETDATE()), 3),
(8, DATEADD(MONTH,1,GETDATE()), 1),
(9, DATEADD(MONTH,3,GETDATE()), 2),
(10, DATEADD(YEAR,1,GETDATE()), 3),
(11, DATEADD(MONTH,1,GETDATE()), 1),
(12, DATEADD(MONTH,3,GETDATE()), 2),
(13, DATEADD(YEAR,1,GETDATE()), 3),
(14, DATEADD(MONTH,1,GETDATE()), 1),
(15, DATEADD(MONTH,3,GETDATE()), 2),
(16, DATEADD(YEAR,1,GETDATE()), 3),
(17, DATEADD(MONTH,1,GETDATE()), 1),
(18, DATEADD(MONTH,3,GETDATE()), 2),
(19, DATEADD(YEAR,1,GETDATE()), 3),
(20, DATEADD(MONTH,1,GETDATE()), 1),
(21, DATEADD(MONTH,3,GETDATE()), 2),
(22, DATEADD(YEAR,1,GETDATE()), 3),
(23, DATEADD(MONTH,1,GETDATE()), 1),
(24, DATEADD(MONTH,3,GETDATE()), 2),
(25, DATEADD(YEAR,1,GETDATE()), 3),
(26, DATEADD(MONTH,1,GETDATE()), 1),
(27, DATEADD(MONTH,3,GETDATE()), 2),
(28, DATEADD(YEAR,1,GETDATE()), 3),
(29, DATEADD(MONTH,1,GETDATE()), 1),
(30, DATEADD(MONTH,3,GETDATE()), 2);

-- Session types
INSERT INTO SessionType (TypeTitle)
VALUES
('Newbies'),
('Experienced'),
('Advanced');

-- Seed the 3 Graded Sessions each with respective Teachers
-- Newbies
INSERT INTO [Session] (TrainerID,SessionTypeID,StartTime,Duration,Capacity,Focus,DayID, SessionWeek) VALUES
(3,1,'08:00',60,15,'Basics',1,DATEPART(ISO_WEEK, GETDATE())),
(3,1,'09:30',60,15,'Defense Techniques',3,DATEPART(ISO_WEEK, GETDATE())),
(3,1,'11:00',60,15,'Combine Moves',5,DATEPART(ISO_WEEK, GETDATE()));

-- Experienced
INSERT INTO [Session] (TrainerID,SessionTypeID,StartTime,Duration,Capacity, Focus,DayID,SessionWeek) VALUES
(4,2,'10:00',75,20,'Sparring Drills',2,DATEPART(ISO_WEEK, GETDATE())),
(4,2,'12:00',75,20,'Footwork Patterns',4,DATEPART(ISO_WEEK, GETDATE())),
(4,2,'14:00',75,20,'Agility & Power',6,DATEPART(ISO_WEEK, GETDATE()));

-- Advanced
INSERT INTO [Session] (TrainerID,SessionTypeID,StartTime,Duration,Capacity,Focus,DayID,SessionWeek) VALUES
(3,3,'15:00',90,25,'Combos & Strategy',2,DATEPART(ISO_WEEK, GETDATE())),
(4,3,'17:00',90,25,'Sparring Pressure',4,DATEPART(ISO_WEEK, GETDATE())),
(3,3,'19:00',90,25,'Conditioning',6,DATEPART(ISO_WEEK, GETDATE()));

-- Seed MembershipPlans
INSERT INTO MembershipPlan (RoleID, PriceID, BillingPeriod) VALUES
(3,1,'Monthly'),
(3,2,'Quarterly'),
(3,3,'Yearly');

-- Seed Invoice
INSERT INTO Invoice (AccountID, MembershipPlanID) VALUES
(5,1),(6,2),(7,3),(8,1),(9,2),(10,3),
(11,1),(12,2),(13,3),(14,1),(15,2),(16,3),
(17,1),(18,2),(19,3),(20,1),(21,2),(22,3),
(23,1),(24,2),(25,3),(26,1),(27,2),(28,3),
(29,1),(30,2);

-- Sample Newsletter inserts
INSERT INTO Newsletter (PostedByAccountID, NewsTitle, NewsContent, PostWeek, PostYear, NewsType, IsActive)
VALUES
(1, 'New Kickboxing Classes Added', 'We are excited to announce new evening kickboxing classes starting next month.', 4, 2026, 'Announcement', 1),
(2, 'Holiday Schedule', 'Please note our holiday schedule: the gym will be closed Dec 24-26.', 50, 2025, 'Info', 1),
(1, 'Trainer Spotlight: John Doe', 'Meet John, our expert trainer with 10 years of experience!', 5, 2026, 'Highlight', 1),
(3, 'Membership Discounts', 'Sign up before the end of the month to get 10% off your next membership.', 3, 2026, 'Promotion', 1),
(2, 'Equipment Maintenance', 'Some equipment will be temporarily unavailable due to maintenance.', 6, 2026, 'Notice', 1),
(1, 'Weekly Challenge', 'This week, try our endurance challenge! See if you can complete it!', 5, 2026, 'Challenge', 1),
(3, 'Newsletter Test Inactive', 'This newsletter is inactive for testing.', 1, 2026, 'Test', 0);