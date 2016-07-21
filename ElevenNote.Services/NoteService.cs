using ElevenNote.Data;
using ElevenNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services
{
    public class NoteService
    {   //making service aware of user
        private readonly Guid _userId;  //Identity uses guid to set user
 
        public NoteService(Guid userId)  //constructor, takes userId and sets it as such
        {
            _userId = userId;
        }
        /// <summary>
        /// Get all notes for current user.
        /// </summary>
        /// <returns>The current user's notes.</returns>

        public IEnumerable<NoteListItemModel> GetNotes()
        {
            using (var ctx = new ElevenNoteDbContext())
            { //ctx = context
                var query =
                    ctx
                        .Notes  //returning notes where ownerid is equal to userid, onwerid is the property of the note entity property
                        .Where(e => e.OwnerId == _userId)
                        .Select(
                                e =>
                                    new NoteListItemModel
                                    {
                                        NoteId = e.NoteId,
                                        Title = e.Title,
                                        IsStarred = e.IsStarred,
                                        CreatedUtc = e.CreatedUtc
                                    }
                             );

                return query.ToArray();

            }
        }  //gets note
           /// <summary>
           /// create a new note for the current user.
           /// </summary>
           /// <param name="model">The model base the new note upon.</param>
           /// <returns>a boolean indicating whether creating note was successful.</returns>

        public bool CreateNote(NoteCreateModel model)
        {
            var entity =  //converts model to database
                new NoteEntity
                {
                    OwnerId = _userId,
                    Title = model.Title, 
                    Content = model.Content,
                    CreatedUtc = DateTimeOffset.UtcNow
                };
            using (var ctx = new ElevenNoteDbContext())
            {
                ctx.Notes.Add(entity);
                return ctx.SaveChanges() == 1;

            }
        }  //creates a note

        /// <summary>
        /// Gets a particular note for a current user.
        /// </summary>
        /// <param name="noteId">The id of the note to retrieve</param>
        /// <returns>The specified note</returns>

        public NoteDetailModel GetNoteById(int noteId)  //looking for a specific note by id
        {
            using (var ctx = new ElevenNoteDbContext()) //pulling data (entity) from database and converting it into NoteDetailModel
            {
                var entity = 
                    ctx
                            .Notes
                            .Single(e => e.NoteId == noteId && e.OwnerId == _userId); //get note and authorize note
                return
                    new NoteDetailModel
                    {
                        NoteId = entity.NoteId,
                        Title = entity.Title,
                        Content = entity.Content,
                        CreatedUtc = entity.CreatedUtc,
                        ModifiedUtc = entity.ModifiedUtc
                    };
            }

        }

        public bool UpdateNote(NoteEditModel model) //retrieve note from database, convert entity into model, save changes and returning 1 changed row.
        {
            using (var ctx = new ElevenNoteDbContext())
            {
                var entity =
                    ctx
                        .Notes
                        .Single(e => e.NoteId == model.NoteId && e.OwnerId == _userId);

                entity.Title = model.Title;
                entity.Content = model.Content;
                entity.ModifiedUtc = DateTimeOffset.UtcNow;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteNote(int noteId)  //retrieves and removes note from database
        {
            using (var ctx = new ElevenNoteDbContext())
            {
                var entity =
                    ctx
                        .Notes
                        .Single(e => e.NoteId == noteId && e.OwnerId == _userId);

                ctx.Notes.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }
    }
}
