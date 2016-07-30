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
        public ActionResult Index(string searchString)
        {
            //var notes =
                //_svc.Value.GetNotes(); //referencing lazy from above

            //return View(notes);  //Webpage view references notes

            var notes = from n in _svc.Value.GetNotes()
                        select n;
                            
            if (!String.IsNullOrEmpty(searchString))
            {
                notes = notes.Where(s => s.Title.Contains(searchString));
            }

            return View(notes);
        
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

        public ActionResult Edit(int id)  
        {
            var note = _svc.Value.GetNoteById(id);  //pulls note info 
            var model =
                new NoteEditModel
                {
                    NoteId = note.NoteId,  //gets note data and equates it to NoteEditModel
                    Title = note.Title,
                    Content = note.Content,
                    IsStarred = note.IsStarred
                };

            return View(model);
           }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NoteEditModel model)  //once user tries to save note
        {
            if (!ModelState.IsValid) return View(model);

            if(!_svc.Value.UpdateNote(model))
            {
                ModelState.AddModelError("", "Unable to update note");
                return View(model);
            }

            TempData["SaveResult"] = "Your note was saved";

            return RedirectToAction("Index");
         
        }

        [ActionName("Delete")]
        public ActionResult DeleteGet(int id)
        {
            var detail = _svc.Value.GetNoteById(id);

            return View(detail);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            _svc.Value.DeleteNote(id);

            TempData["SaveResult"] = "Your note was deleted.";
            
            return RedirectToAction("Index");

        }
    }
}