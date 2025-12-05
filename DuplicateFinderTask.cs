using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

using Jellyfin.Data.Enums;

namespace Jellyfin.Plugin.DuplicateFinder
{
    public class DuplicateFinderTask : IScheduledTask
    {
        private readonly ILibraryManager _libraryManager;
        private readonly ILogger<DuplicateFinderTask> _logger;

        public DuplicateFinderTask(ILibraryManager libraryManager, ILogger<DuplicateFinderTask> logger)
        {
            _libraryManager = libraryManager;
            _logger = logger;
        }

        public string Name => "Find Duplicate Movies";

        public string Key => "DuplicateFinderTask";

        public string Description => "Scans the library for duplicate movies and tags them with 'Duplicate'.";

        public string Category => "Library";

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                new TaskTriggerInfo { Type = TaskTriggerInfo.TriggerWeekly, DayOfWeek = DayOfWeek.Sunday, TimeOfDayTicks = TimeSpan.FromHours(2).Ticks }
            };
        }

        public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Duplicate Finder Task");

            var queryResult = _libraryManager.GetItemsResult(new InternalItemsQuery
            {
                IncludeItemTypes = new[] { BaseItemKind.Movie },
                IsVirtualItem = false,
                Recursive = true
            });
            
            var movies = queryResult.Items.OfType<Movie>().ToList();

            progress.Report(5);

            // First, remove all existing "Duplicate" tags
            _logger.LogInformation("Removing existing 'Duplicate' tags...");
            var moviesWithDuplicateTag = movies.Where(m => m.Tags.Contains("Duplicate", StringComparer.OrdinalIgnoreCase)).ToList();
            
            foreach (var movie in moviesWithDuplicateTag)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var tagsList = movie.Tags.ToList();
                tagsList.RemoveAll(t => t.Equals("Duplicate", StringComparison.OrdinalIgnoreCase));
                movie.Tags = tagsList.ToArray();
                await _libraryManager.UpdateItemAsync(movie, movie, ItemUpdateType.MetadataEdit, cancellationToken);
            }
            
            _logger.LogInformation($"Removed 'Duplicate' tag from {moviesWithDuplicateTag.Count} movies");
            progress.Report(15);

            // Now find current duplicates
            var duplicates = new List<Movie>();
            var groupedByImdb = movies
                .Where(m => m.ProviderIds.ContainsKey("Imdb"))
                .GroupBy(m => m.ProviderIds["Imdb"])
                .Where(g => g.Count() > 1);

            foreach (var group in groupedByImdb)
            {
                duplicates.AddRange(group);
            }

            // Also check by name and year for those without IMDB
            var groupedByName = movies
                .Where(m => !m.ProviderIds.ContainsKey("Imdb"))
                .GroupBy(m => new { m.Name, m.ProductionYear })
                .Where(g => g.Count() > 1);

            foreach (var group in groupedByName)
            {
                duplicates.AddRange(group);
            }

            // Deduplicate the list of duplicates (in case of overlap)
            duplicates = duplicates.Distinct().ToList();

            progress.Report(50);

            _logger.LogInformation($"Found {duplicates.Count} duplicate movies");

            int total = duplicates.Count;
            int current = 0;

            // Tag the current duplicates
            foreach (var movie in duplicates)
            {
                cancellationToken.ThrowIfCancellationRequested();

                movie.AddTag("Duplicate");
                await _libraryManager.UpdateItemAsync(movie, movie, ItemUpdateType.MetadataEdit, cancellationToken);
                _logger.LogInformation($"Tagged duplicate: {movie.Name}");

                current++;
                progress.Report(50 + (50 * current / (double)total));
            }

            _logger.LogInformation("Duplicate Finder Task Completed");
        }
    }
}
