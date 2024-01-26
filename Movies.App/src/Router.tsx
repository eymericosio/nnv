import {
	Box,
	Grid,
	Chip,
	TextField,
	Autocomplete,
	Container,
	AppBar,
	Button,
	IconButton,
	Toolbar,
	Typography,
} from "@mui/material";
import * as React from "react";
import { useEffect, useMemo, useState } from "react";
import { useAuth } from "oidc-react";
import { MoviesList } from "./MoviesList";
import { ApolloProvider, ApolloClient, InMemoryCache } from "@apollo/client";
import * as config from "./config.json";
import MenuIcon from "@mui/icons-material/Menu";
import LogoutIcon from "@mui/icons-material/Logout";
import {
	createBrowserRouter,
	Link,
	Outlet,
	RouterProvider,
	useRouteError,
} from "react-router-dom";
import HomeRoute from "./HomeRoute";
import MovieRoute from "./MovieRoute";
import TopRatedRoute from "./TopRatedRoute";

const router = createBrowserRouter([
	{
		path: "/",
		element: <Root />,
		errorElement: <RouterError />,
		children: [
			{
				path: "/",
				element: <HomeRoute />,
			},
			{
				path: "top-rated",
				element: <TopRatedRoute />,
			},
			{
				path: "movies/:movieKey",
				element: <MovieRoute />,
			},
		],
	},
]);

export default function Router() {
	return <RouterProvider router={router} />;
}

function Root() {
	const auth = useAuth();

	return (
		<>
			<AppBar position="static">
				<Toolbar sx={{ columnGap: 3 }}>
					<Container maxWidth="lg" sx={{ display: "flex", gap: 5, alignItems: "center" }}>
						<Box>
							<Link to={`/`}>
								<Typography variant="h6" component="div">
									Movies
								</Typography>
							</Link>
						</Box>
						<Box sx={{ flexGrow: 1 }}>
							<Link to={`top-rated`}>Top Rated</Link>
						</Box>
						<Box></Box>
						<Typography variant="subtitle1" component="div">
							Hey {auth.userData?.profile.name}!
						</Typography>
						<Button
							color="secondary"
							variant="contained"
							endIcon={<LogoutIcon />}
							onClick={() => auth.signOutRedirect()}
						>
							Logout
						</Button>
					</Container>
				</Toolbar>
			</AppBar>
			<Container maxWidth="lg" sx={{ marginY: 5 }}>
				<Outlet />
			</Container>
		</>
	);
}

function RouterError() {
	const error = useRouteError();
	console.error(error);

	return (
		<div id="error-page">
			<h1>Oops!</h1>
			<p>Sorry, an unexpected error has occurred.</p>
		</div>
	);
}
