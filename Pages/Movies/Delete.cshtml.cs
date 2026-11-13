using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages_Movies
{
    public class DeleteModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;
        private readonly IWebHostEnvironment _environment;

        public DeleteModel(RazorPagesMovieContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Movie Movie { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Movie? movie = await _context.Movie.FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            Movie = movie;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Movie? movie = await _context.Movie.FindAsync(id);

            if (movie != null)
            {
                string? posterPath = movie.PosterPath;

                _context.Movie.Remove(movie);
                await _context.SaveChangesAsync();

                // Delete the file only after the database delete succeeded
                if (!string.IsNullOrEmpty(posterPath))
                {
                    string filePath = Path.Combine(_environment.WebRootPath, "posters", posterPath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }

            return RedirectToPage("./Index");
        }
    }
}