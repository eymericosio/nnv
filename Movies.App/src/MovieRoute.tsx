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
import {
	ApolloProvider,
	ApolloClient,
	InMemoryCache,
	gql,
	useQuery,
	useMutation,
} from "@apollo/client";
import * as config from "./config.json";
import MenuIcon from "@mui/icons-material/Menu";
import LogoutIcon from "@mui/icons-material/Logout";
import {
	createBrowserRouter,
	Link,
	Outlet,
	RouterProvider,
	useMatch,
	useParams,
	useRouteError,
} from "react-router-dom";
import HomeRoute from "./HomeRoute";
import { key } from "localforage";
import { IMovie } from "./Api/MoviesApi";
import EditIcon from "@mui/icons-material/Edit";
import CancelIcon from "@mui/icons-material/Cancel";
import SaveIcon from "@mui/icons-material/Save";

const MOVIE_QUERY = gql`
	query Movie($key: ID!) {
		movie(key: $key) {
			key
			name
			rate
			genres
			length
			img
			description
		}
	}
`;

export default function MovieRoute() {
	const { movieKey } = useParams();
	const [movie, setMovie] = useState<IMovie>();

	const fetch = useQuery<{ movie: IMovie }>(MOVIE_QUERY, {
		variables: { key: movieKey },
		notifyOnNetworkStatusChange: true,
	});

	if (fetch.loading) return "Loading...";
	if (fetch.error || fetch.data?.movie == null)
		return <pre>{fetch.error?.message}</pre>;

	return (
		<>
			<Form defaultValue={fetch.data.movie} />
		</>
	);
}

const MOVIE_UPDATE = gql`
	mutation UpdateMovie($key: ID!, $movie: MovieUpdate!) {
		updateMovie(key: $key, movie: $movie) {
			key
			name
			rate
			genres
			length
			img
			description
		}
	}
`;

export function Form(props: { defaultValue: IMovie }) {
	const [editing, setEditing] = useState<boolean>(false);
	const [movie, setMovie] = useState<IMovie>({ ...props.defaultValue });

	const [mutate, mutation] = useMutation<{ movie: IMovie }>(MOVIE_UPDATE);

	async function startEdition() {
		setEditing(true);
	}

	async function cancelEdition() {
		setEditing(false);
		setMovie({ ...props.defaultValue });
	}

	async function saveEdition() {
		setEditing(false);
		const input = {
			name: movie.name,
			description: movie.description,
			rate: movie.rate,
			length: movie.length,
			img: movie.img,
			genres: movie.genres,
		};
		await mutate({ variables: { key: props.defaultValue.key, movie: input } });
	}

	return (
		<>
			<Grid container spacing={4}>
				<Grid
					item
					xs={12}
					sx={{ display: "flex", justifyContent: "right", gap: 3 }}
				>
					{!editing && (
						<Button
							color="primary"
							variant="contained"
							endIcon={<EditIcon />}
							onClick={() => startEdition()}
						>
							Edit
						</Button>
					)}
					{editing && (
						<>
							<Button
								color="error"
								variant="contained"
								endIcon={<CancelIcon />}
								onClick={() => cancelEdition()}
							>
								Cancel
							</Button>
							<Button
								color="success"
								variant="contained"
								endIcon={<SaveIcon />}
								onClick={() => saveEdition()}
							>
								Save
							</Button>
						</>
					)}
				</Grid>

				<Grid item xs={6}>
					<TextField
						label="Name"
						value={movie.name}
						onChange={(event) =>
							setMovie({ ...movie, name: event.target.value })
						}
						fullWidth
						disabled={!editing}
					/>
				</Grid>

				<Grid item xs={6}>
					<TextField
						label="Rate"
						type="number"
						inputProps={{ step: ".01" }}
						value={movie.rate}
						onChange={(event) =>
							setMovie({ ...movie, rate: Number(event.target.value) })
						}
						fullWidth
						disabled={!editing}
					/>
				</Grid>

				<Grid item xs={6}>
					<TextField
						label="Duration"
						value={movie.length}
						onChange={(event) =>
							setMovie({ ...movie, length: event.target.value })
						}
						fullWidth
						disabled={!editing}
					/>
				</Grid>

				<Grid item xs={6}>
					<TextField
						label="Image"
						value={movie.img}
						onChange={(event) =>
							setMovie({ ...movie, img: event.target.value })
						}
						fullWidth
						disabled={!editing}
					/>
				</Grid>

				<Grid item xs={12}>
					<Autocomplete
						multiple
						options={[]}
						defaultValue={[]}
						freeSolo
						value={movie.genres}
						onChange={(event, values) =>
							setMovie({ ...movie, genres: [...values] })
						}
						renderTags={(value: readonly string[], getTagProps) =>
							value.map((option: string, index: number) => (
								<Chip label={option} {...getTagProps({ index })} />
							))
						}
						renderInput={(params) => <TextField {...params} label="Genres" />}
						disabled={!editing}
					/>
				</Grid>

				<Grid item xs={12}>
					<TextField
						label="Description"
						value={movie.description}
						onChange={(event) =>
							setMovie({ ...movie, description: event.target.value })
						}
						fullWidth
						multiline
						disabled={!editing}
					/>
				</Grid>
			</Grid>
		</>
	);
}
