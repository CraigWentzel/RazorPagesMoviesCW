using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages_Movies
{
    public class EditModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;
        private readonly IWebHostEnvironment _environment;

        public EditModel(RazorPagesMovieContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Movie Movie { get; set; } = default!;

        [BindProperty]
        public IFormFile? PosterFile { get; set; }

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Movie? movieToUpdate = await _context.Movie.FindAsync(Movie.Id);

            if (movieToUpdate == null)
            {
                return NotFound();
            }

            bool updated = await TryUpdateModelAsync<Movie>(
                movieToUpdate,
                "Movie",
                m => m.Title, m => m.ReleaseDate, m => m.Genre, m => m.Price, m => m.Rating);

            if (!updated)
            {
                return Page();
            }

            if (PosterFile != null && PosterFile.Length > 0)
            {
                string[] allowed = { ".jpg", ".jpeg", ".png", ".webp" };
                string extension = Path.GetExtension(PosterFile.FileName).ToLowerInvariant();

                if (!allowed.Contains(extension))
                {
                    ModelState.AddModelError("PosterFile", "Only .jpg, .jpeg, .png or .webp files are allowed.");
                    return Page();
                }

                string postersFolder = Path.Combine(_environment.WebRootPath, "posters");
                Directory.CreateDirectory(postersFolder);

                string newFileName = Guid.NewGuid() + extension;
                string newFilePath = Path.Combine(postersFolder, newFileName);

                using (FileStream stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await PosterFile.CopyToAsync(stream);
                }

                string? oldFileName = movieToUpdate.PosterPath;
                movieToUpdate.PosterPath = newFileName;

                await _context.SaveChangesAsync();

                // Remove the old file only after the save succeeded
                if (!string.IsNullOrEmpty(oldFileName))
                {
                    string oldFilePath = Path.Combine(postersFolder, oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            else
            {
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}