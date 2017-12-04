using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunctionsHandOnLab.Pages
{

    public class IndexModel : PageModel
    {
        private readonly StorageAccess _storage;

        public IndexModel(StorageAccess storage)
        {
            _storage = storage;
        }

        [BindProperty]
        public string Comment { get; set; }

        public IEnumerable<CommentModel> Comments { get; set; }

        public int Count { get; set; }

        public async Task OnGet()
        {
            Comments = await _storage.GetCommentsAsync();
            Count = await _storage.GetQueueCount();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var comment = new CommentModel
            {
                Text = Comment,
                Status = "New comment"
            };

            await _storage.AddCommentAsync(comment);

            return RedirectToPage("/Index");
        }
    }
}
