﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today,
            };

            SetupActvitiesSelectListItems();

            return View();
        }



        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            // if there are not any "Duration" field validation errors thenmake
            // sure that the duration is greater than "0"
            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                TempData["Message"] = "Your entry was successfully added";

                return RedirectToAction("Index");
            }

            SetupActvitiesSelectListItems();

            return View(entry);
        }



        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the requested entry from the repository
            Entry entry = _entriesRepository.GetEntry((int)id);

            // TReturn a status of not found if entry wasn't found
            if (entry == null)
            {
                return HttpNotFound();
            }

            // TODO Populate the activities select list items viewbag properties.
            SetupActvitiesSelectListItems();
            // Pass the netry into the view.
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            // Validate the entry
            ValidateEntry(entry);

            // If the entry id valid
            // 1) Use the repository to update the entry
            // 2) Redirect the user to the entries list page
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);

                TempData["Message"] = "Your entry was successfully updated";

                return RedirectToAction("Index");
            }

            // Populate the activities select list items viewbag poperty
            SetupActvitiesSelectListItems();

            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO Retieve entry for the provided if parameter value
            Entry entry = _entriesRepository.GetEntry((int)id);


            // TOD Return not found if an entry wasn't found
            if (entry == null)
            {
                return HttpNotFound();
            }
            // TODO Pass the entry to the view

            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            // TODO Delete the entry
            _entriesRepository.DeleteEntry(id);





            // TODo Redirect to the entries index page

            TempData["Message"] = "Your entry was successfully deleted";

            return RedirectToAction("Index");
        }

        private void ValidateEntry(Entry entry)
        {
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field must be greater than '0'.");
            }
        }

        private void SetupActvitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(
                Data.Data.Activities, "Id", "Name");
        }
    }
}