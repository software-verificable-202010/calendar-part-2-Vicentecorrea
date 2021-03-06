﻿using CalendarApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalendarApp.Controllers
{
    public static class AppointmentController
    {
        public static (bool, string) CreateAppointment(Appointment appointment)
        {
            bool couldTheAppointmentBeCreated = Constants.TheAppointmentCouldBeCreated;
            bool appointmentHasTitle = !String.IsNullOrWhiteSpace(appointment.Title);
            bool appointmentHasDescription = !String.IsNullOrWhiteSpace(appointment.Description);
            bool appointmentEndDateIsLaterThanStartDate = appointment.StartDate < appointment.EndDate;
            string feedbackText = "Successfully created appointment";
            if (!appointmentHasTitle || !appointmentHasDescription || !appointmentEndDateIsLaterThanStartDate)
            {
                feedbackText = "Error" + Environment.NewLine;
                couldTheAppointmentBeCreated = !Constants.TheAppointmentCouldBeCreated;
                if (!appointmentHasTitle)
                {
                    feedbackText += "The appointment must have a title" + Environment.NewLine;

                }
                if (!appointmentHasDescription)
                {
                    feedbackText += "The appointment must have a description" + Environment.NewLine;

                }
                if (!appointmentEndDateIsLaterThanStartDate)
                {
                    feedbackText += "The end date must be later than the start date";
                }
            }
            else
            {
                try
                {
                    DatabaseHelper.SaveAppointment(appointment);
                }
                catch
                {
                    couldTheAppointmentBeCreated = !Constants.TheAppointmentCouldBeCreated;
                    feedbackText = "Error, the appointment could not be created";
                }
            }
            return (couldTheAppointmentBeCreated, feedbackText);
        }

        public static List<Appointment> GetAppointmentsInMonth(DateTime date)
        {
            return DatabaseHelper.GetAppointmentsInMonth(date);
        }

        public static bool IsAppointmentInThisTimePeriod(Appointment appointment, DateTime timePeriod, string selectedCalendarView)
        {
            bool IsAppointmentInThisTimePeriod;
            if (selectedCalendarView == Constants.MonthOption)
            {
                IsAppointmentInThisTimePeriod = appointment.StartDate.Date <= timePeriod && timePeriod <= appointment.EndDate.Date;
            }
            else
            {
                DateTime previousHour = appointment.StartDate.AddHours(Constants.PreviousTimeInterval);
                IsAppointmentInThisTimePeriod = previousHour < timePeriod && timePeriod < appointment.EndDate;
            }
            return IsAppointmentInThisTimePeriod;
        }


        public static List<Appointment> GetAppointmentsInThisTimePeriod(List<Appointment> appointments, DateTime timePeriod, string selectedCalendarView)
        {
            IEnumerable<Appointment> appointmentsObtained = from appointment in appointments
                                                where IsAppointmentInThisTimePeriod(appointment, timePeriod, selectedCalendarView)
                                                     orderby appointment.StartDate
                                                     select appointment;
            List<Appointment> appointmentsInTimePeriod = new List<Appointment>(appointmentsObtained);
            return appointmentsInTimePeriod;
        }
    }
}
