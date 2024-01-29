import { gql, useMutation } from "@apollo/client";
import * as React from "react";
import { useMemo, useState } from "react";
import { IMovie } from "./Api/MoviesApi";
import MovieForm from "./MovieForm";
import { Grid, Button } from "@mui/material";
import SaveIcon from "@mui/icons-material/Save";

export default function CreateRoute() {
	const [movie, setMovie] = useState<IMovie>(() => initialState());

	function initialState(): IMovie {
		return {
			key: "",
			name: "",
			description: "",
			rate: 0,
			length: "",
			img: "",
			genres: [],
		};
	}

	const MOVIE_CREATE = useMemo(
		() => gql`
			mutation CreateMovie($movie: MovieCreate!) {
				createMovie(movie: $movie) {
					key
					name
					description
					rate
					genres
					length
					img
				}
			}
		`,
		[]
	);
	const [createMutation] = useMutation<{ createMovie: IMovie }>(MOVIE_CREATE, {
		update(cache, { data }) {
			if (data?.createMovie) {
				// force root query to refresh to include new movie
				// possibly better way by manually adding the created movie to the cached list
				cache.evict({});
			}
		},
	});

	async function create(movie: IMovie) {
		const input = {
			name: movie.name,
			description: movie.description,
			rate: movie.rate,
			length: movie.length,
			img: movie.img,
			genres: movie.genres,
		};
		await createMutation({ variables: { movie: input } });
	}

	return (
		<>
			<Grid container spacing={4} sx={{ marginBottom: 2 }}>
				<Grid
					item
					xs={12}
					sx={{ display: "flex", justifyContent: "right", gap: 3 }}
				>
					<Button
						color="success"
						variant="contained"
						endIcon={<SaveIcon />}
						onClick={() => create(movie)}
					>
						Save
					</Button>
				</Grid>
			</Grid>
			<MovieForm movie={movie} setMovie={(m) => setMovie(m)} disabled={false} />
		</>
	);
}
