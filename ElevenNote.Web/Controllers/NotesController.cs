using ElevenNote.Models;
using ElevenNote.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElevenNote.Web.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly Lazy<NoteService> _svc;

        public NotesController() //constructor + default
        {
            _svc = 
                new Lazy<NoteService>(  //creates service when we actually need it, delays
                    () =>
                    {
                        var userId = Guid.Parse(User.Identity.GetUserId());
                        return new NoteService(userId);
                    }
              );
        }

        // GET: Notes
        public ActionResult Index()
        {
            var notes =
                _svc.Value.GetNotes(); //referencing lazy from above

            return View(notes);  //Webpage view references notes
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]  //user is sending info to us via the controller
        [ValidateAntiForgeryToken]  //from the create.cshtml
        public ActionResult Create(NoteCreateModel model)
        {
            if (!ModelState.IsValid) return View(model); //return view with data so that it isn't needing to be retyped, just corrected

            if (!_svc.Value.CreateNote(model)) //if note is not created, generate string below and return to view
                {
                    ModelState.AddModelError("", "Unable to create note.");
                    return View(model);
                }

            TempData["SaveResult"] = "Your note was created.";

            return RedirectToAction("Index");  //returns to list page
        }

        public ActionResult Details(int id)  //controller will look at notescontroller, method of details, id is id from the route (note ID number)
        {
            var note = _svc.Value.GetNoteById(id);

            return View(note);
        }
    }
}