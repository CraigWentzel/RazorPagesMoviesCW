using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages_Movies
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<Movie> Movie { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

         [BindProperty(SupportsGet = true)]
        public string? GenreSearch { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<Movie> movies = from m in _context.Movie
                                       select m;

            if (!string.IsNullOrEmpty(SearchString))
            {
                movies = movies.Where(m => m.Title!.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(GenreSearch))
            {
                
                movies = movies.Where(g => g.Genre!.Contains(GenreSearch));
            }

            Movie = await movies
            .OrderBy(m => m.Title)
            .ToListAsync();
    }
}
}