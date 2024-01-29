import * as React from "react";
import { gql, useMutation, useQuery } from "@apollo/client";
import { useNavigate, useParams } from "react-router-dom";
import { IMovie } from "./Api/MoviesApi";
import { useMemo, useState } from "react";
import { Grid, Button } from "@mui/material";
import MovieForm from "./MovieForm";
import SaveIcon from "@mui/icons-material/Save";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import CancelIcon from "@mui/icons-material/Cancel";

export default function MovieRoute() {
	const { movieKey } = useParams();

	const MOVIE_QUERY = useMemo(
		() => gql`
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
		`,
		[]
	);

	const fetch = useQuery<{ movie: IMovie }>(MOVIE_QUERY, {
		variables: { key: movieKey },
	});

	if (fetch.loading) return "Loading...";
	if (fetch.error || fetch.data?.movie == null)
		return <pre>{fetch.error?.message}</pre>;

	return (
		<>
			<Content movie={fetch.data.movie} />
		</>
	);
}

function Content(props: { movie: Readonly<IMovie> }) {
	const [editing, setEditing] = useState<boolean>(false);
	const [model, setModel] = useState<IMovie>({ ...props.movie });
	const navigate = useNavigate();

	const MOVIE_UPDATE = useMemo(
		() => gql`
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
		`,
		[]
	);
	const [updateMutation] = useMutation<{ movie: IMovie }>(MOVIE_UPDATE);

	const MOVIE_DELETE = useMemo(
		() => gql`
			mutation DeleteMovie($key: ID!) {
				deleteMovie(key: $key) {
					key
					name
					rate
					genres
					length
					img
					description
				}
			}
		`,
		[]
	);
	const [deleteMutation] = useMutation<{ deleteMovie: IMovie }>(MOVIE_DELETE, {
		update(cache, { data }) {
			if (data?.deleteMovie) {
				const cacheKey = `movie:{"key":"${data.deleteMovie.key}"}`;
				cache.evict({ id: cacheKey });
			}
		},
	});

	async function startEdition() {
		setEditing(true);
	}

	async function cancelEdition() {
		setEditing(false);
		setModel({ ...props.movie });
	}

	async function saveMovie() {
		setEditing(false);
		const input = {
			name: model.name,
			description: model.description,
			rate: model.rate,
			length: model.length,
			img: model.img,
			genres: model.genres,
		};
		await updateMutation({ variables: { key: props.movie.key, movie: input } });
	}

	async function deleteMovie() {
		if (confirm("Are you sure to delete this movie?")) {
			setEditing(false);
			await deleteMutation({ variables: { key: props.movie.key } });
			navigate("/");
		}
	}

	return (
		<>
			<Grid container spacing={4} sx={{ marginBottom: 2 }}>
				<Grid
					item
					xs={12}
					sx={{ display: "flex", justifyContent: "right", gap: 3 }}
				>
					{!editing && (
						<>
							<Button
								color="primary"
								variant="contained"
								endIcon={<EditIcon />}
								onClick={() => startEdition()}
							>
								Edit
							</Button>
							<Button
								color="error"
								variant="contained"
								endIcon={<DeleteIcon />}
								onClick={() => deleteMovie()}
							>
								Delete
							</Button>
						</>
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
								onClick={() => saveMovie()}
							>
								Save
							</Button>
						</>
					)}
				</Grid>
			</Grid>
			<MovieForm
				movie={model}
				setMovie={(m) => setModel(m)}
				disabled={!editing}
			/>
		</>
	);
}
