# MediaList

An ASP.NET Core MVC app for organizing favorite media into named lists. Users can register, set a public display name, rate entries, and choose whether each list is public or private. Visitors can search media, lists, and users that have public lists.

**Live site:** https://medialist-v8q7.onrender.com/

**Source:** https://github.com/treyz1999-lgtm/MediaList

## Features

- Create and rename lists, then add, edit, and delete anime, movies, TV shows, comics, books, games, music, or other media inside them.
- Store a title, media type, optional genre, 1–10 rating, and optional cover image URL.
- Keep each list private by default, or publish it after setting a display name.
- Search public lists by name or contents, and search public users by display name.
- Open a public user's list from search results. Private lists return 404 to other users.

## Run locally

1. Install the .NET 10 SDK and SQL Server LocalDB (included with Visual Studio's ASP.NET and data development workloads).
2. Open `MediaWebApp.slnx` in Visual Studio, or run `dotnet run` from this directory.
3. The development app applies EF Core migrations automatically on startup to the LocalDB database configured in your local `appsettings.Development.json`. New clones use the generic `MediaList` LocalDB database from `appsettings.json`.
4. Register an account, sign in, and open **My lists**. Create a list, add entries to it, and use the name link in the header to set your display name. Lists start private; open a list and choose **Make public** to share it.

The app uses ASP.NET Core Identity for password storage and sign-in, Entity Framework Core for persistence, SQL Server LocalDB for development, and Bootstrap for the interface.

## How it works

- ASP.NET Core MVC serves the pages and handles list, media, profile, and search requests.
- ASP.NET Core Identity stores accounts and handles cookie-based sign-in. A separate profile holds the public display name.
- Each media entry belongs to a named list and an owner. List visibility controls which lists, entries, and users appear in public search and public pages.
- EF Core uses SQL Server LocalDB on the developer machine and PostgreSQL on Render. Each provider has its own migrations.

## Demo content

Run `dotnet run -- --seed-demo` once to add three public sample accounts (`test1`, `test2`, and `test3`) with lists of movies, shows, books, comics, games, and anime. Repeating the command skips accounts that already exist. Their passwords are randomly generated, so these accounts are for browsing, not signing in. The demo entries are examples of personal lists, not a catalog or licensed cover images.

## Hosting

The live site runs as a Docker web service on Render with a separate Render PostgreSQL database. The included `Dockerfile` and `render.yaml` define the same setup: a free web service and a paid `0.1c-256mb` PostgreSQL plan. Review Render's current pricing before creating another Blueprint. Render supplies `DATABASE_URL` from the database to the web service. On startup, the app applies the PostgreSQL migrations and adds demo content if it is missing. Render checks `/health`.

1. Push the repository to GitHub.
2. In Render, create a new Blueprint from that repository and review its plan and cost before applying it.
3. After deployment, open the service's `onrender.com` URL. The first startup may take longer while migrations and demo content are created. The free web service may also take time to wake after inactivity.

Local development continues to use SQL Server LocalDB when `DATABASE_URL` is absent. To use a local PostgreSQL database, set `DATABASE_URL` to its URL and run the app. Keep connection strings and production settings out of Git.

## Notes

Cover images currently use URLs; file uploads are not implemented. Search returns up to 50 matching public media entries, lists, and users per category. Demo accounts have random unknown passwords and cannot be used for sign-in. The site has no email delivery or account confirmation workflow. A migration moves existing local media into a named list and preserves the former single-list table as `LegacyMediaLists` for rollback. The earlier joke table also remains in the local database so existing data is preserved, but the joke feature is no longer linked or routed in the app. LocalDB data is separate from the hosted PostgreSQL database.
