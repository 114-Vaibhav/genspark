-- Ensure UUID extension is available
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Delete existing records for clean state (be careful if this is a production DB, but this is a seed script)
DELETE FROM "SeatLocks";
DELETE FROM "Travelers";
DELETE FROM "Payments";
DELETE FROM "Bookings";
DELETE FROM "Trips";
DELETE FROM "Buses";
DELETE FROM "Seats";
DELETE FROM "Layouts";
DELETE FROM "Routes";
DELETE FROM "Users";
DELETE FROM "Settings";

-- 1. Create Users (Admin, Operator, Standard Users)
INSERT INTO "Users" ("Id", "Name", "Email", "PasswordHash", "MobileNo", "Gender", "Dob", "Role", "Approved", "IsActive", "CreatedAt") VALUES
('11111111-1111-1111-1111-111111111111', 'System Admin', 'admin@bus.com', '$2a$11$f5P3Gg01SgYmB5.XbIfT2.xOq/U3QyV4sA/KqJ4iJ350wA81L5kXG', '1234567890', 'Male', '1990-01-01', 'Admin', true, true, current_timestamp),
('22222222-2222-2222-2222-222222222222', 'Alpha Travels', 'operator1@bus.com', '$2a$11$f5P3Gg01SgYmB5.XbIfT2.xOq/U3QyV4sA/KqJ4iJ350wA81L5kXG', '9876543210', 'Male', '1985-05-15', 'Operator', true, true, current_timestamp),
('33333333-3333-3333-3333-333333333333', 'Beta Express', 'operator2@bus.com', '$2a$11$f5P3Gg01SgYmB5.XbIfT2.xOq/U3QyV4sA/KqJ4iJ350wA81L5kXG', '9876543211', 'Female', '1992-08-20', 'Operator', true, true, current_timestamp),
('44444444-4444-4444-4444-444444444444', 'John Doe', 'john@user.com', '$2a$11$f5P3Gg01SgYmB5.XbIfT2.xOq/U3QyV4sA/KqJ4iJ350wA81L5kXG', '5555555555', 'Male', '1995-12-12', 'User', true, true, current_timestamp),
('55555555-5555-5555-5555-555555555555', 'Jane Smith', 'jane@user.com', '$2a$11$f5P3Gg01SgYmB5.XbIfT2.xOq/U3QyV4sA/KqJ4iJ350wA81L5kXG', '5555555556', 'Female', '1998-03-25', 'User', true, true, current_timestamp),
('66666666-6666-6666-6666-666666666666', 'Alice Johnson', 'alice@user.com', '$2a$11$f5P3Gg01SgYmB5.XbIfT2.xOq/U3QyV4sA/KqJ4iJ350wA81L5kXG', '5555555557', 'Female', '2000-07-10', 'User', true, true, current_timestamp);
-- Note: Password is 'password' for all seeded users.

-- 2. Create Global Settings
INSERT INTO "Settings" ("Key", "Value") VALUES ('PlatformFeePercentage', '12');

-- 3. Create Routes
INSERT INTO "Routes" ("Id", "Source", "Destination", "Distance", "PickupPoints", "DropPoints", "IsActive") VALUES
('77777777-7777-7777-7777-777777777777', 'Mumbai', 'Pune', 150, ARRAY['Dadar', 'Andheri', 'Navi Mumbai'], ARRAY['Wakad', 'Hinjewadi', 'Swargate'], true),
('88888888-8888-8888-8888-888888888888', 'Delhi', 'Jaipur', 280, ARRAY['ISBT', 'Dhaula Kuan'], ARRAY['Sindhi Camp', 'Narayan Singh Circle'], true),
('99999999-9999-9999-9999-999999999999', 'Bangalore', 'Chennai', 350, ARRAY['Madiwala', 'Electronic City'], ARRAY['Koyambedu', 'Guindy'], true);

-- 4. Create Layouts (Automated format)
INSERT INTO "Layouts" ("Id", "OperatorId", "Name", "TotalSeats", "BusType", "Configuration") VALUES
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '22222222-2222-2222-2222-222222222222', 'Volvo 2+2 Seater', 40, 'Seater', '2+2'),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '33333333-3333-3333-3333-333333333333', 'Scania 2+1 Sleeper', 30, 'Sleeper', '1+2');

-- 5. Create Seats for Layout A (40 seats, 10 rows of 4)
DO $$
DECLARE
    i INT; j INT; seat_no INT;
BEGIN
    seat_no := 1;
    FOR i IN 1..10 LOOP
        FOR j IN 1..4 LOOP
            INSERT INTO "Seats" ("Id", "LayoutId", "SeatNumber", "Row", "Column")
            VALUES (uuid_generate_v4(), 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'S' || seat_no, i, j);
            seat_no := seat_no + 1;
        END LOOP;
    END LOOP;
END $$;

-- 6. Create Seats for Layout B (30 seats, 10 rows of 3)
DO $$
DECLARE
    i INT; j INT; seat_no INT;
BEGIN
    seat_no := 1;
    FOR i IN 1..10 LOOP
        FOR j IN 1..3 LOOP
            INSERT INTO "Seats" ("Id", "LayoutId", "SeatNumber", "Row", "Column")
            VALUES (uuid_generate_v4(), 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'L' || seat_no, i, j);
            seat_no := seat_no + 1;
        END LOOP;
    END LOOP;
END $$;

-- 7. Create Buses
INSERT INTO "Buses" ("Id", "OperatorId", "LayoutId", "BusNumber", "TotalSeats", "IsActive") VALUES
('cccccccc-cccc-cccc-cccc-cccccccccccc', '22222222-2222-2222-2222-222222222222', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'MH-01-AB-1234', 40, true),
('dddddddd-dddd-dddd-dddd-dddddddddddd', '33333333-3333-3333-3333-333333333333', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'KA-05-XY-9876', 30, true);

-- 8. Create Trips
INSERT INTO "Trips" ("Id", "BusId", "RouteId", "JourneyDate", "DepartureTime", "ArrivalTime", "Price", "PickupAddress", "DropAddress") VALUES
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'cccccccc-cccc-cccc-cccc-cccccccccccc', '77777777-7777-7777-7777-777777777777', current_date + interval '2 days', '08:00:00', '12:00:00', 800, 'Dadar Circle Main', 'Wakad Bridge'),
('ffffffff-ffff-ffff-ffff-ffffffffffff', 'dddddddd-dddd-dddd-dddd-dddddddddddd', '99999999-9999-9999-9999-999999999999', current_date + interval '5 days', '21:00:00', '05:00:00', 1500, 'Madiwala Boarding Point', 'Koyambedu Main Bus Stand');

-- 9. Create Bookings & Travelers (One confirmed booking for John, one for Jane)
-- John books Seat S1 and S2 on Trip E
INSERT INTO "Bookings" ("Id", "UserId", "TripId", "TotalAmount", "PlatformFee", "Status", "CreatedAt") VALUES
('00000000-0000-0000-0001-000000000000', '44444444-4444-4444-4444-444444444444', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 1792.00, 192.00, 'Confirmed', current_timestamp);

INSERT INTO "Travelers" ("Id", "BookingId", "Name", "Age", "Gender", "SeatNumber") VALUES
(uuid_generate_v4(), '00000000-0000-0000-0001-000000000000', 'John Doe', 28, 'Male', 'S1'),
(uuid_generate_v4(), '00000000-0000-0000-0001-000000000000', 'Jimmy Doe', 25, 'Male', 'S2');

-- Jane books Seat L5 on Trip F (Single Lady logic test)
INSERT INTO "Bookings" ("Id", "UserId", "TripId", "TotalAmount", "PlatformFee", "Status", "CreatedAt") VALUES
('00000000-0000-0000-0002-000000000000', '55555555-5555-5555-5555-555555555555', 'ffffffff-ffff-ffff-ffff-ffffffffffff', 1680.00, 180.00, 'Confirmed', current_timestamp);

INSERT INTO "Travelers" ("Id", "BookingId", "Name", "Age", "Gender", "SeatNumber") VALUES
(uuid_generate_v4(), '00000000-0000-0000-0002-000000000000', 'Jane Smith', 26, 'Female', 'L5');

-- Add payments for these bookings
INSERT INTO "Payments" ("Id", "BookingId", "Amount", "Status", "PaymentMethod", "CreatedAt") VALUES
(uuid_generate_v4(), '00000000-0000-0000-0001-000000000000', 1792.00, 'Success', 'Dummy', current_timestamp),
(uuid_generate_v4(), '00000000-0000-0000-0002-000000000000', 1680.00, 'Success', 'Dummy', current_timestamp);

SELECT 'Seeding Complete!' AS Status;
