# Solution projects
- Movies.Api: contains the REST and GraphQL APIs using NET 8 and GraphQL 7
- Movies.App: Contains the front end app using React 18
- Movies.Silo: Contains the Silo using Orleans 8
- Movies.Test: Contains the unit tests using XUnit

# How to run
## Visual Studio Code
- Open the root folder of the git repository
- Open a first terminal, open the Silo project `cd ./Movies.Silo` and run `dotnet run`.
- Open a second terminal, open the Api project `cd ./Movies.Api` and run `dotnet run`.
- Open a third terminal, open the App project `cd ./Movies.App` and run `npm ci && npm start`.
## Visual Studio
- Open the `nnv.sln` solution
- Start the Silo project followed by the Api project by using the Startup Projects selection in the navbar
- Open a terminal in the root folder of the git repository, open the App project `cd ./Movies.App` and run `npm ci && npm start`.

# Use the app
When the front end app is run it should automatically open `http://localhost:1234/`.
The main components of the app:
- the Home page displaying all movies and an option to search by keywords and/or genres
- the Top Rated displaying the 5 highest ranked movies
- the creation page accessible via the `+` button at the bottom of the Home page
- the details and edition page accessible via the `See more` links

> To note: the app requires an authenticated user via an OpenIdConnect flow ; you can use users `bob/bob` or `alice/alice` to login

# Helpers
The Api project includes a `/swagger` page for the REST endpoints, as well as a GQL UI page at `/ui/altair`. With the default configuration of the project the full URLs would be:
- https://localhost:7236/swagger
- https://localhost:7236/ui/altair

> To note: the Api requires an authenticated user via a bearer token scheme ; it is retrieved by the app during the OpenIdConnect flow mentioned above
