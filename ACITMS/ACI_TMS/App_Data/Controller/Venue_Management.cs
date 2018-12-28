using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using GeneralLayer;
using System.Data;

namespace LogicLayer
{
    public class Venue_Management
    {
        private DB_Venue dbVenue = new DB_Venue();

        //Create new venue
        public Tuple<bool, string> createVenue(string venueId, string venueLocation, string venueDesc, int venueCapacity, DateTime venueEffectDate, int userId)
        {
            if (dbVenue.isVenueIdExist(venueId))
            {
                return new Tuple<bool, string>(false, "Venue ID already exist.");
            }
            else
            {
                if (dbVenue.createVenue(venueId, venueLocation, venueDesc, venueCapacity, venueEffectDate, userId))
                    return new Tuple<bool, string>(true, "Venue created successfully.");
                else
                    return new Tuple<bool, string>(false, "Error creating venue");
            }
        }

        //Retrieve all venue
        public DataTable getAllVenue()
        {
            return dbVenue.getAllVenue();
        }

        //Retrieve venue based on venueID
        public DataTable getVenue(string venueId)
        {
            return dbVenue.getVenue(venueId);
        }

        public DataTable isVenueUsed(string venueID)
        {
            return dbVenue.isVenueUsed(venueID);
        }

        public Tuple<bool, string> updateVenue(string venueId, string venueLocation, string venueDesc, int venueCapacity,
            DateTime venueEffectDate, int userId)
        {
            if (dbVenue.updateVenue(venueId, venueLocation, venueDesc, venueCapacity,
            venueEffectDate, userId))
                return new Tuple<bool, string>(true, "Venue saved successfully.");
            else
                return new Tuple<bool, string>(false, "Error updating venue.");
        }

        public Tuple<bool, string> deleteVenue(string venueId, int userId)
        {
            DataTable dtVenue = isVenueUsed(venueId);

            if (dtVenue.Rows.Count != 0)
            {
                return new Tuple<bool, string>(false, "Error deleting venue.");
            }
            else
            {
                if (dbVenue.deleteVenue(venueId, userId))
                    return new Tuple<bool, string>(true, "Venue deleted successfully.");
                else
                    return new Tuple<bool, string>(false, "Error deleting venue.");
            }
        }

        public DataTable getPeriods()
        {
            return dbVenue.getPeriods();
        }

        public bool checkVenueAvailable(DateTime dt, DayPeriod period, string venueId, int capacity)
        {
            return dbVenue.isVenueAvailable(dt, period, venueId, capacity);
        }

        public bool checkVenueAvailable(DateTime dt, DayPeriod period, string venueId, int capacity, int sessionId)
        {
            return dbVenue.isVenueAvailable(dt, period, venueId, capacity, sessionId);
        }

        public bool checkVenueAvailableForBatch(DateTime dt, DayPeriod period, string venueId, int programmeBatchId)
        {
            return dbVenue.isVenueAvailableForBatch(dt, period, venueId, programmeBatchId);
        }

        public DataTable getVenueBookings(int day, DayPeriod period, DateTime dtStart, DateTime dtEnd, string venueId)
        {
            return dbVenue.getVenueBookings(day, period, dtStart, dtEnd, venueId);
        }

        public int getVenueCapacity(string venueId)
        {
            return dbVenue.getVenueCapacity(venueId);
        }

        public DataTable getVenueCapacity(string[] venueId)
        {
            return dbVenue.getVenueCapacity(venueId);
        }

        public DataTable getRecentVenues(string type)
        {
            return dbVenue.getRecentVenues(type == "SESSION" ? "and b.sessionId is not null " : "and b.sessionId is null ");
        }

        public DataTable getListVenues(string frm, string to)
        {
            bool filterNum = false;
            if (to.Contains("#"))
            {
                filterNum = true;
                to.Replace("#", "");
            }
            return dbVenue.getListVenues(frm, to, filterNum);
        }

        //Retrieve all booking record
        public DataTable getBooking(int bookingId)
        {
            return dbVenue.getBooking(bookingId);
        }

        public DataTable getBooking(string venueId, DateTime bookingStartDate, DateTime bookingEndDate)
        {
            return dbVenue.getBooking(venueId, bookingStartDate, bookingEndDate);
        }

        public DataTable getBooking(DateTime dt, string venueId)
        {
            return dbVenue.getBooking(dt, venueId);
        }

        public Tuple<bool, string> createBooking(string venueId, DateTime bookingDate, DayPeriod[] bookingPeriod, string bookingRemark, int userId)
        {
            if (dbVenue.createBooking(venueId, bookingDate, bookingPeriod, bookingRemark, userId))
                return new Tuple<bool, string>(true, "Booking created successfully.");
            else
                return new Tuple<bool, string>(false, "Error creating booking.");
        }

        public Tuple<bool, string> updateBooking(int bookingId, string bookingRemark, int userId)
        {
            if (dbVenue.updateBooking(bookingId, bookingRemark, userId))
                return new Tuple<bool, string>(true, "Booking updated successfully.");
            else
                return new Tuple<bool, string>(false, "Error updating booking.");
        }

        public Tuple<bool, string> deleteBooking(int bookingId, int userId)
        {
            if (dbVenue.deleteBooking(bookingId, userId))
                return new Tuple<bool, string>(true, "Booking record deleted successfully.");
            else
                return new Tuple<bool, string>(false, "Error deleting booking record.");
        }
    }
}
